using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConcordNew.Concord.IO
{
    static class NoteMidiConverter
    {
        static readonly int[] note2midi = new int[] { -1, 0, 2, 4, 5, 7, 9 };
        public static int NoteToMidi(int note)
        {
            int octave = note / 7 + 2;
            while (note < 0)
                note += 7;
            int pitch = note % 7;
            int nPitch = note2midi[pitch];
            return nPitch + octave * 12;
        }
    }
}
