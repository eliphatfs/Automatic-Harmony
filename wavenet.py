'''
A Enhanced Version of Wavenet Implemention in https://github.com/usernaamee/keras-wavenet.
Used LeakyReLU instead of ReLU and Added BatchNormalization.
Input to model is now not quantized. (Output kept quantized)
Both training on a sine wave or on a music dataset is faster and accuracy improves some 30%.
Generating is much faster than the original repo.
'''
import os
import sys
import time
import numpy as np
from keras import losses
from keras.callbacks import Callback
from scipy.io.wavfile import read, write
from keras.models import Model
from keras.layers import Conv1D, Dense, RepeatVector,\
    Input, Activation, Reshape, BatchNormalization, TimeDistributed
from keras.layers.merge import Multiply, Add
from keras.layers.advanced_activations import LeakyReLU
from keras.layers.recurrent import GRU


def wavenetBlock(n_atrous_filters, atrous_filter_size, atrous_rate):
    def f(input_):
        residual = input_
        tanh_out = Conv1D(n_atrous_filters, atrous_filter_size,
                          dilation_rate=atrous_rate,
                          padding='causal',
                          activation='tanh')(input_)
        sigmoid_out = Conv1D(n_atrous_filters, atrous_filter_size,
                             dilation_rate=atrous_rate,
                             padding='causal',
                             activation='sigmoid')(input_)
        merged = Multiply()([tanh_out, sigmoid_out])
        merged = BatchNormalization()(merged)
        skip_out = Conv1D(32, 1)(merged)
        skip_out = LeakyReLU()(skip_out)
        skip_out = BatchNormalization()(skip_out)
        out = Add()([skip_out, residual])
        return out, skip_out
    return f


def cut_loss(y_true, y_pred):
    print(y_true.shape)
    y_true = y_true[:, 3085:, :]
    y_pred = y_pred[:, 3085:, :]
    return losses.categorical_crossentropy(y_true, y_pred)


def get_basic_generative_model(input_size):
    input_ = Input(shape=(input_size, 1))
    first = Conv1D(32, 32, padding='causal', activation='tanh')(input_)
    A, B = wavenetBlock(32, 2, 1)(first)
    skip_connections = [B]
    for i in range(1, 30):
        A, B = wavenetBlock(32, 3, 2 ** (i % 10))(A)
        skip_connections.append(B)
    net = Add()(skip_connections)
    net = LeakyReLU()(net)
    net = Conv1D(512, 1)(net)
    net = LeakyReLU()(net)
    '''net = Reshape([1, input_size, 256])(first)
    primary_caps = capsulelayers.PrimaryCap(net,
                                            dim_capsule=16,
                                            n_channels=32,
                                            kernel_size=(1, 11),
                                            strides=(1, 9),
                                            padding='valid')
    digit_caps = capsulelayers.CapsuleLayer(num_capsule=16,
                                            dim_capsule=16,
                                            routings=1,
                                            name='digitcaps')(primary_caps)
    out_caps = capsulelayers.Length()(digit_caps)
    masked = capsulelayers.Mask()(digit_caps)
    # net = Flatten()(masked)
    net = Dense(256)(masked)
    net = LeakyReLU()(net)
    net = Dense(256)(net)
    net = LeakyReLU()(net)
    net = Dense(256)(net)
    net = RepeatVector(512)(net)
    net = Conv1D(256, 3, dilation_rate=1, padding='causal')(net)
    net = LeakyReLU()(net)
    net = BatchNormalization()(net)
    net = Conv1D(256, 3, dilation_rate=2, padding='causal')(net)
    net = LeakyReLU()(net)
    net = BatchNormalization()(net)
    net = Conv1D(256, 3, dilation_rate=4, padding='causal')(net)
    net = LeakyReLU()(net)
    net = BatchNormalization()(net)
    net = Conv1D(256, 3, dilation_rate=8, padding='causal')(net)
    net = LeakyReLU()(net)
    net = BatchNormalization()(net)
    net = Conv1D(256, 3, dilation_rate=16, padding='causal')(net)
    net = LeakyReLU()(net)
    net = BatchNormalization()(net)
    net = Conv1D(256, 3, dilation_rate=32, padding='causal')(net)
    net = LeakyReLU()(net)
    net = BatchNormalization()(net)
    net = Conv1D(256, 3, dilation_rate=64, padding='causal')(net)
    net = LeakyReLU()(net)
    net = BatchNormalization()(net)
    net = Conv1D(256, 3, dilation_rate=128, padding='causal')(net)
    net = LeakyReLU()(net)
    net = BatchNormalization()(net)
    net = Conv1D(256, 3, dilation_rate=256, padding='causal')(net)
    net = LeakyReLU()(net)
    net = BatchNormalization()(net)
    net = Conv1D(256, 1, padding='causal')(net)'''
    net = Conv1D(256, 1)(net)
    net = Activation('softmax')(net)
    model = Model(input=input_, output=[net])
    model.compile(loss=cut_loss, optimizer='adam',
                  metrics=['acc'])
    model.summary()
    return model


def get_audio(filename):
    sr, audio = read(filename)
    audio = audio.astype(float)
    audio = audio - audio.min()
    audio = audio / (audio.max() - audio.min())
    audio = (audio - 0.5) * 2
    return sr, audio


def frame_generator(sr, audio, frame_size, frame_shift, minibatch_size=1):
    audio_len = len(audio)
    X = []
    y = []
    ymod = []
    while 1:
        for i in range(0, audio_len - frame_size - 1, frame_shift):
            frame = audio[i:i+frame_size]
            if len(frame) < frame_size:
                break
            if i + frame_size * 3 >= audio_len:
                break
            temp = audio[i + frame_size]
            target_val = int((np.sign(temp) * (np.log(1 + 256*abs(temp)) / (
                np.log(1+256))) + 1)/2.0 * 255)
            X.append(frame.reshape(frame_size, 1))
            temp = audio[i + frame_size: i + frame_size + frame_size]
            target = np.zeros([frame_size, 256])
            ymod.append((np.eye(16)[target_val % 16]))
            for l in range(frame_size):
                target_val = int((np.sign(temp[l]) *
                                  (np.log(1 + 256*abs(temp[l])) / (
                                          np.log(1+256))) + 1)/2.0 * 255)
                target[l, target_val] = 1
            y.append(target)
            if len(X) == minibatch_size:
                yield np.array(X), [np.array(y)]
                X = []
                y = []
                ymod = []


def get_audio_from_model(model, sr, duration, seed_audio):
    new_audio = np.zeros((sr * duration))
    curr_sample_idx = 0
    while curr_sample_idx < new_audio.shape[0]:
        distribution = np.array(model.predict(
                seed_audio.reshape(1, frame_size, 1))[0], dtype=float)\
                .reshape(frame_size, 256)
        for i in range(511):
            if curr_sample_idx >= new_audio.shape[0]:
                break
            predicted_val = np.argmax(distribution[i])
            ampl_val_8 = ((((predicted_val) / 255.0) - 0.5) * 2.0)
            ampl_val_16 = (np.sign(ampl_val_8) * (1/256.0) * ((1 + 256.0)**abs(
                ampl_val_8) - 1)) * 2**15
            new_audio[curr_sample_idx] = ampl_val_16
            seed_audio[-1] = ampl_val_16 / 32768.0
            seed_audio[:-1] = seed_audio[1:]
            pc_str = str(round(100 * curr_sample_idx / new_audio.shape[0], 2))
            sys.stdout.write('Percent complete: ' + pc_str + '\r')
            sys.stdout.flush()
            curr_sample_idx += 1
    return new_audio.astype(np.int16)


class SaveAudioCallback(Callback):
    def __init__(self, ckpt_freq, sr, seed_audio):
        super(SaveAudioCallback, self).__init__()
        self.ckpt_freq = ckpt_freq
        self.sr = sr
        self.seed_audio = seed_audio

    def on_epoch_end(self, epoch, logs={}):
        if (epoch + 1) % self.ckpt_freq == 0:
            ts = str(int(time.time()))
            filepath = os.path.join('output/', 'ckpt_'+ts+'.wav')
            audio = get_audio_from_model(self.model,
                                         self.sr,
                                         0.5,
                                         self.seed_audio)
            write(filepath, self.sr, audio)


frame_size = 100000
frame_shift = 511
model = get_basic_generative_model(frame_size)


def train_on(file_path, n_epochs):
    sr_training, training_audio = get_audio(file_path)
    # training_audio = training_audio[:sr_training*1200]
    sr_valid, valid_audio = get_audio(file_path)
    # valid_audio = valid_audio[:sr_valid*60]
    assert sr_training == sr_valid, "Training, validation samplerate mismatch"
    n_training_examples = int((len(training_audio)-frame_size-1) / float(
        frame_shift))
    n_validation_examples = int((len(valid_audio)-frame_size-1) / float(
        frame_shift))
    print('Total training examples:', n_training_examples)
    print('Total validation examples:', n_validation_examples)
    '''audio_context = valid_audio[:frame_size]
    save_audio_clbk = SaveAudioCallback(100, sr_training, audio_context)
    validation_data_gen = frame_generator(sr_valid,
                                          valid_audio,
                                          frame_size,
                                          frame_shift)'''
    training_data_gen = frame_generator(sr_training,
                                        training_audio,
                                        frame_size,
                                        frame_shift)
    for k in range(100 * n_epochs):
        xx, yy = next(training_data_gen)
        hist = model.train_on_batch(xx, yy)
        print("step:", k,
              "loss:", hist[0],
              "acc:", str(round(hist[1] * 100, 2)) + "%")
    '''model.fit_generator(training_data_gen,
                        3000,
                        epochs=n_epochs,
                        validation_data=validation_data_gen,
                        validation_steps=50,
                        verbose=1,
                        callbacks=[save_audio_clbk])'''
    print('Saving model...')
    str_timestamp = str(int(time.time()))
    model.save_weights('model_'+str_timestamp+'_'+str(n_epochs)+'.h5')
    generate(file_path)


def load_model_from_file(filename):
    global model
    model = get_basic_generative_model(frame_size)
    model.load_weights(filename)


def generate(seed):
    print('Generating audio...')
    _, seeda = get_audio(seed)
    new_audio = get_audio_from_model(model, 16000, 5, seeda[:frame_size])
    str_timestamp = str(int(time.time()))
    outfilepath = 'generated_'+str_timestamp+'.wav'
    print('Writing generated audio to:', outfilepath)
    write(outfilepath, 16000, new_audio)
    print('Done!')


if __name__ == '__main__':
    '''train_on('sine.wav', 1)
    for u in range(400):
        train_on('../../touhou_original/%d.wav' % u, 1)'''
