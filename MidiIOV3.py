# Test Midi File Output
############################################################################
# A sample program to create a single-track MIDI file, add a note,
# and write to disk.
############################################################################

from midiutil import MIDIFile
# import random

# Create the MIDIFile Object
MyMIDI = MIDIFile(1)
# Add track name and tempo. The first argument to addTrackName and
# addTempo is the time to write the event.
track = 0
time = 0
name = input("Enter Name:")
MyMIDI.addTrackName(track, time, name)
MyMIDI.addTempo(track, time, 212)
MyMIDI.addProgramChange(track, 0, 0, 48)  # 40 48 50
while True:
    time = 0
    sequence = input("Enter Sequence, Q To Quit: \n")
    if sequence == 'Q':
        break
    chords_r = sequence.split("\n")
    chords = [chord.split(",") for chord in chords_r]
    for i in range(len(chords)):
        # Add a note. addNote expects the following information:
        channel = 0
        volume = 100
        for note in chords[i]:
            length = 1
            if note == "":
                continue
            try:
                fn = i + 1
                while fn < len(chords):
                    chords[fn][chords[fn].index(note)] = ""
                    length += 1
                    fn += 1
                raise ValueError
            except ValueError:
                MyMIDI.addNote(track, channel, int(note), time, length, volume)
        time += 1

# And write it to disk.
binfile = open(name + ".mid", 'wb')
MyMIDI.writeFile(binfile)
binfile.close()
