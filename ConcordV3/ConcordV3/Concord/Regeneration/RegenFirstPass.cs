using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConcordV3.Concord.Regeneration
{
    public class RegenFirstPass : IChordRegen
    {
        public bool[] RegenChord(bool[] lastpass, string name, ref int root, ref int lowlimit, ref int highlimit, int count = 4)
        {
            bool[] gen = new bool[128];
            for (int i = 0; i < 128; i++)
                gen[i] = false;
            while (root < lowlimit)
                root += 12;
            gen[root] = true;
            return gen;
        }
    }
}
