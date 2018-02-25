using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConcordV3.Concord
{
    public static class NoteNotions
    {
        public static readonly string[] Octave = { "C", "C#", "D", "Eb", "E", "F", "F#", "G", "G#", "A", "Bb", "B" };
        public static int GetPlaceInOctave(string str)
        {
            for (int i = 0; i < 12; i++)
                if (str.StartsWith(Octave[i]))
                    return i;
            return -1;
        }
    }
}
