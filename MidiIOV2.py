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
MyMIDI.addTempo(track, time, 72)
MyMIDI.addProgramChange(track, 0, 0, 50) # 40 50
while True:
    sequence = input("Enter Sequence, Q To Quit: \n")
    if sequence == 'Q':
        break
    subseqs = sequence.split("\n")
    for subseq in subseqs:
        notes = subseq.split(",")
        time = 0
        # Add a note. addNote expects the following information:
        channel = 0
        volume = 100

        # Now add the note.
        i = -1
        while True:
            if i >= len(notes):
                break
            if notes[i] == '':
                i += 1
                continue
            pitch = int(notes[i])
            length = 0
            while int(notes[i]) == pitch:
                i += 1
                length += 0.5
                if notes[i] == '' or i > len(notes):
                    break
            # length = random.randint(1, 1)
            # findNextNote - i
            if pitch > -111:
                MyMIDI.addNote(track, channel, pitch + 24, time, length, volume)
            time += length

# And write it to disk.
binfile = open(name + ".mid", 'wb')
MyMIDI.writeFile(binfile)
binfile.close()
