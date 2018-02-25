using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConcordV3.Concord.Regeneration.Chord3
{
    public class ChordXm : IChordRegen
    {
        public bool[] RegenChord(bool[] lastpass, string name, ref int root, ref int lowlimit, ref int highlimit, int count = 4)
        {
            if (name == "Xm")
            {
                Expansion.BroadcastTo128(lastpass, root, highlimit);
                Expansion.BroadcastTo128(lastpass, root + 3, highlimit);
                Expansion.BroadcastTo128(lastpass, root + 7, highlimit);
            }
            return lastpass;
        }
    }
}
