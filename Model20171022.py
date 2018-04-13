# -*- coding: utf-8 -*-
"""
Created on Mon Oct  2 08:58:04 2017

"""

import keras
import numpy as np
import random
import tensorflow as tf
import keras.initializers as init
from keras.models import Sequential, Model
from keras.layers import RepeatVector, TimeDistributed, Activation, Flatten
from keras.optimizers import SGD
from keras.utils.generic_utils import Progbar
import keras.backend as K
import json
import sklearn.preprocessing
import Model20171021

LOCAL = False
TRAIN = True
DATA_PATH = "D:/Data/" if LOCAL else "../data_binary/"
DATA_SIZE = 151819 if TRAIN else 328393
HIDDEN_SIZE = 128
BATCH_SIZE = 32

Data_X = []
Data_Y = []
VAL_X = []
VAL_Y = []

RandomInit = init.RandomNormal(mean=0, stddev=1, seed=None)
RNN = keras.layers.recurrent.GRU
DNN = keras.layers.core.Dense
CNN = keras.layers.Convolution1D
MGE = keras.layers.Concatenate
LReLU = keras.layers.LeakyReLU
PReLU = keras.layers.PReLU
MPN = keras.layers.MaxPooling1D
LCN = keras.layers.LocallyConnected1D


def wasserstein(y_true, y_pred):
    return K.mean(y_true * y_pred)


def generator_model(transfered):
    inputs = keras.layers.Input(shape=(64, 16))
    model = Sequential()
    model.add(RNN(128, input_shape=(64, 16)))
    model.add(RepeatVector(64))
    for _ in range(1):
        model.add(RNN(HIDDEN_SIZE, return_sequences=True,
                      recurrent_dropout=0.382))
    model.add(CNN(512, 5, padding='causal', strides=3))
    model.add(LReLU())
    model.add(CNN(256, 5, padding='causal'))
    model.add(LReLU())
    model.add(CNN(128, 5, padding='causal'))
    model.add(LReLU())
    model.add(LCN(128, 5))
    model.add(LReLU())
    model.add(RNN(HIDDEN_SIZE))
    model.add(RepeatVector(64))
    for _ in range(2):
        model.add(RNN(HIDDEN_SIZE, return_sequences=True,
                      recurrent_dropout=0.45))
    model.add(TimeDistributed(DNN(38)))
    model.add(Activation('linear'))
    modelt = transfered(inputs)
    models = model(inputs)
    return Model(inputs=inputs,
                 outputs=keras.layers.Average()([modelt, models]))


def discriminator_model(transfered):
    inputs = keras.layers.Input(shape=(64, 38))
    model = Sequential()
    model.add(RNN(128, input_shape=(64, 38)))
    model.add(RepeatVector(64))
    model.add(RNN(128, return_sequences=True,
                  recurrent_dropout=0.382))
    model.add(CNN(128, 5, padding='causal'))
    model.add(PReLU())
    model.add(CNN(128, 5, padding='causal'))
    model.add(PReLU())
    model.add(MPN())
    model.add(Flatten())
    model.add(DNN(256))
    model.add(Activation('tanh'))
    model.add(DNN(1))
    model.add(Activation('sigmoid'))
    modelt = transfered(inputs)
    models = model(inputs)
    return Model(inputs=inputs,
                 outputs=keras.layers.Average()([modelt, models]))


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
        for i in range(10):
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
    dt = Model20171021.discriminator_model()
    dt.load_weights("discriminator-22-Copy.dat")
    d = discriminator_model(dt)
    print("Building Generator...")
    gt = Model20171021.generator_model()
    gt.load_weights("generator-22-Copy.dat")
    g = generator_model(gt)
    print("Compiling Models...")
    d_on_g = generator_containing_discriminator(g, d)
    d_optim = SGD(lr=0.0001)
    g_optim = SGD(lr=0.0002)
    g.compile(loss=wasserstein, optimizer=SGD())
    d_on_g.compile(loss=wasserstein,
                   optimizer=g_optim,
                   metrics=['binary_accuracy'])
    d.trainable = True
    d.compile(loss=wasserstein,
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
            if index % 40 == 39:
                g.save_weights('generator.dat', True)
                d.save_weights('discriminator.dat', True)
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
    gt = Model20171021.generator_model()
    gt.load_weights("E:/generator-22-Copy.dat")
    gt.trainable = False
    g = Sequential(layers=[gt, g])
    g.compile(loss='binary_crossentropy', optimizer="SGD")
    g.load_weights('E:/generator-1.dat')
    generated_seq = g.predict(np.reshape(input_seq, (1, 64, 16)), verbose=1)[0]
    sklearn.preprocessing.minmax_scale(generated_seq, copy=False)
    np.set_printoptions(threshold=65535)
    print(generated_seq)
    return generated_seq


if __name__ == '__main__':
    VAL_X, VAL_Y = load_data(9)
    if TRAIN:
        train()
    else:
        for i in range(1):
            fidx = open(DATA_PATH + "XP/" + str(i) + ".txt")
            np.save("PR19", predict(eval(str(fidx.read(-1)))))
            fidx.close()
