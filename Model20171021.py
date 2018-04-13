# -*- coding: utf-8 -*-
"""
Created on Mon Oct  2 08:58:04 2017

"""

import keras
import numpy as np
import random
import tensorflow as tf
import keras.initializers as init
from keras.models import Sequential
from keras.layers import RepeatVector, TimeDistributed, Activation, Flatten
from keras.layers import BatchNormalization
from keras.optimizers import RMSprop
from keras.utils.generic_utils import Progbar
from keras.engine.topology import Layer
import keras.backend as K
import json
import sklearn.preprocessing

LOCAL = False
TRAIN = True
DATA_PATH = "D:/Data/" if LOCAL else "../data_binary/"
DATA_SIZE = 457632 if TRAIN else 328393
HIDDEN_SIZE = 128
BATCH_SIZE = 32

Data_X = []
Data_Y = []
VAL_X = []
VAL_Y = []


class MyLayer(Layer):

    def __init__(self, output_dim, **kwargs):
        self.output_dim = output_dim
        super(MyLayer, self).__init__(**kwargs)

    def build(self, input_shape):
        # Create a trainable weight variable for this layer.
        self.kernel = []
        # self.binput_shape = input_shape
        for i in range(1, input_shape[-2] + 1):
            k = self.add_weight(name='kernel%d' % i,
                                shape=(input_shape[-1], self.output_dim),
                                initializer='he_normal',
                                trainable=True)
            self.kernel.append(k)
        super(MyLayer, self).build(input_shape)

    def call(self, x):
        res = []
        for i in range(1, 1 + len(self.kernel)):
            res.append(K.mean(x=K.dot(x[:, 0:i, :], self.kernel[i-1]),
                              axis=-2, keepdims=True))
        return K.concatenate(res, axis=-2)

    def compute_output_shape(self, input_shape):
        return (input_shape[0], input_shape[-2], self.output_dim)


RandomInit = init.RandomNormal(mean=0, stddev=1, seed=None)
RNN = keras.layers.recurrent.GRU
DNN = keras.layers.core.Dense
CNN = keras.layers.Convolution1D
MGE = keras.layers.Concatenate
LReLU = keras.layers.LeakyReLU
MPN = keras.layers.MaxPooling1D
LCN = keras.layers.LocallyConnected1D


def generator_model():
    model = Sequential()
    model.add(RNN(128, input_shape=(32, 16)))
    model.add(RepeatVector(64))
    for _ in range(1):
        model.add(RNN(HIDDEN_SIZE, return_sequences=True,
                      recurrent_dropout=0.382))
    model.add(BatchNormalization())
    model.add(CNN(512, 5, padding='causal', strides=3))
    model.add(LReLU())
    model.add(CNN(256, 4, padding='causal'))
    model.add(LReLU())
    model.add(CNN(128, 6, padding='causal'))
    model.add(LReLU())
    model.add(LCN(128, 5))
    model.add(LReLU())
    model.add(keras.layers.GaussianDropout(0.2))
    model.add(RNN(HIDDEN_SIZE))
    model.add(RepeatVector(32))
    for _ in range(2):
        model.add(RNN(HIDDEN_SIZE, return_sequences=True,
                      recurrent_dropout=0.382))
    model.add(TimeDistributed(DNN(38)))
    model.add(Activation('linear'))
    return model


def discriminator_model():
    model = Sequential()
    model.add(RNN(HIDDEN_SIZE, input_shape=(32, 38)))
    model.add(RepeatVector(64))
    model.add(RNN(128, return_sequences=True,
                  recurrent_dropout=0.382))
    model.add(CNN(128, 5, padding='causal'))
    model.add(LReLU())
    model.add(CNN(128, 4, padding='causal'))
    model.add(LReLU())
    model.add(CNN(128, 6, padding='causal'))
    model.add(LReLU())
    model.add(MPN())
    model.add(Flatten())
    model.add(DNN(256))
    model.add(Activation('tanh'))
    model.add(DNN(1))
    model.add(Activation('sigmoid'))
    return model


def generator_containing_discriminator(g, d):
    model = Sequential()
    model.add(g)
    d.trainable = False
    model.add(d)
    return model


def load_data(part):
    global Data_X
    global Data_Y
    Data_X = np.load(DATA_PATH + "x" + str(part) + ".npy",
                     allow_pickle=False)
    Data_Y = np.load(DATA_PATH + "y" + str(part) + ".npy",
                     allow_pickle=False)
    return Data_X, Data_Y


def fit_datagen():
    global Data_X, Data_Y
    while True:
        for i in range(15):
            load_data(i)
            Data_X = (Data_X - 0.5) / 0.5
            Data_Y = (Data_Y - 0.5) / 0.5
            shuf = [k for k in range(0,
                                     (len(Data_X) // BATCH_SIZE) * BATCH_SIZE,
                                     BATCH_SIZE)]
            random.shuffle(shuf)
            for j in shuf:
                yield (Data_X[j:j+BATCH_SIZE],
                       Data_Y[j:j+BATCH_SIZE])


def loss_softmax_cross_entropy_with_logits(y_true, y_pred):
    return tf.nn.softmax_cross_entropy_with_logits(labels=y_true,
                                                   logits=y_pred)


def train():
    # X_train = X_train.reshape((X_train.shape, 1) + X_train.shape[1:])
    print("Building Discriminator...")
    d = discriminator_model()
    print("Building Generator...")
    g = generator_model()
    print("Compiling Models...")
    d_on_g = generator_containing_discriminator(g, d)
    d_optim = RMSprop(lr=0.00005)
    g_optim = RMSprop(lr=0.000108)
    g.compile(loss='binary_crossentropy', optimizer="adam")
    d_on_g.compile(loss='binary_crossentropy',
                   optimizer=g_optim,
                   metrics=['binary_accuracy'])
    d.trainable = True
    d.compile(loss='binary_crossentropy',
              optimizer=d_optim,
              metrics=['binary_accuracy'])
    d.summary()
    g.summary()
    print("Training...")
    datagen = fit_datagen()
    d_metrics = None
    g_metrics = None
    for epoch in range(1, 101):
        print("\nEpoch #%d" % epoch)
        num_batches = DATA_SIZE // BATCH_SIZE
        progress_bar = Progbar(target=num_batches)
        y = [1] * BATCH_SIZE + [0] * BATCH_SIZE
        y1 = [1] * BATCH_SIZE
        for index in range(num_batches):
            current_data = next(datagen)
            seq_batch = current_data[1]
            generated_seqs = g.predict(current_data[0], verbose=0)
            X = np.concatenate((seq_batch, generated_seqs))
            d_metrics = d.train_on_batch(X, y)
            d.trainable = False
            g_metrics = d_on_g.train_on_batch(current_data[0], y1)
            d.trainable = True
            progress_bar.update(index + 1,
                                values=[("d_loss", d_metrics[0]),
                                        ("g_loss", g_metrics[0]),
                                        ("d_acc", d_metrics[1]),
                                        ("g_acc", g_metrics[1])])
            if index % 100 == 99:
                metrics = [np.mean(progress_bar.sum_values[j][0] /
                                   max(1, progress_bar.sum_values[j][1]))
                           for j in ["d_loss", "g_loss", "d_acc", "g_acc"]
                           ]
                json_str = json.dumps({"epoch": epoch,
                                       "step": index + 1,
                                       "d_loss": float(metrics[0]),
                                       "g_loss": float(metrics[1]),
                                       "d_acc": float(metrics[2]),
                                       "g_acc": float(metrics[3])})
                fp = open("train_log.json", "a")
                fp.write(json_str)
                fp.write("\n")
                fp.close()
            if index % 400 == 399:
                g.save_weights('generator-{}-{}.dat'.format(epoch, index))
                fidx = open(DATA_PATH + "XP/" + str(0) + ".txt")
                prd = eval(str(fidx.read(-1)))
                generated_seq = g.predict(np.reshape(prd, (1, 32, 16)))[0]
                sklearn.preprocessing.minmax_scale(generated_seq, copy=False)
                np.save("pr-{}-{}.npy".format(epoch, index), generated_seq)
                fidx.close()
        g.save_weights('generator-{}.dat'.format(epoch), True)
        d.save_weights('discriminator-{}.dat'.format(epoch), True)
        metrics = [np.mean(progress_bar.sum_values[j][0] /
                           max(1, progress_bar.sum_values[j][1]))
                   for j in ["d_loss", "g_loss", "d_acc", "g_acc"]
                   ]
        json_str = json.dumps({"epoch": epoch,
                               "step": -1,
                               "d_loss": float(metrics[0]),
                               "g_loss": float(metrics[1]),
                               "d_acc": float(metrics[2]),
                               "g_acc": float(metrics[3])})
        fp = open("train_log.json", "a")
        fp.write(json_str)
        fp.write("\n")
        fp.close()


def predict(input_seq):
    g = generator_model()
    g.compile(loss='binary_crossentropy', optimizer="SGD")
    g.load_weights('G:/KT/AI/generator-25(1).dat')
    generated_seq = g.predict(np.reshape(input_seq, (1, 32, 16)), verbose=1)[0]
    sklearn.preprocessing.minmax_scale(generated_seq, copy=False)
    return generated_seq


if __name__ == '__main__':
    VAL_X, VAL_Y = load_data(14)
    if TRAIN:
        train()
    else:
        for i in range(0, 1):
            fidx = open(DATA_PATH + "XP/" + str(i) + ".txt")
            np.save("PR18-T2", predict(eval(str(fidx.read(-1)))))
            fidx.close()
