using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConcordV3.Concord.Regeneration.Chord3;
using ConcordV3.Concord.Regeneration.Chord7;

namespace ConcordV3.Concord.Regeneration
{
    public class ChordRegen
    {
        readonly IChordRegen firstPass = new RegenFirstPass();
        readonly IChordRegen[] secondPass = {
                                                new ChordX(),
                                                new ChordXm(),
                                                new ChordX7(),
                                                new ChordXsus(),
                                                new ChordXdim5(),
                                                new ChordXaug5()
                                            };
        public bool[] Regen(string name, int lowlimit = 20, int highlimit = 90)
        {
            string originalName = name; // Keep this for error reporting.
            int root = -1; // Root of Chord
            // Parse Root Note:
            for (int i = 0; i < 12; i++)
                if (name.StartsWith(NoteNotions.Octave[i]))
                {
                    name = "X" + name.Substring(NoteNotions.Octave[i].Length);
                    root = i;
                }
            if (name[0] != 'X') throw new ArgumentException("Unaccepted Chord Notion, Caused By Wrong Root Note: " + originalName);

            // Make First-Pass Preparations:
            bool[] gen = firstPass.RegenChord(null, name, ref root, ref lowlimit, ref highlimit);

            // Generate the Whole Chord with Every Possible Note in the Chord:
            foreach (IChordRegen icr in secondPass)
                icr.RegenChord(gen, name, ref root, ref lowlimit, ref highlimit);
            return gen;
        }
    }
}
