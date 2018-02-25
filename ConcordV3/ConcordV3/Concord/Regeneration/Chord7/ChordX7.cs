using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConcordV3.Concord.Regeneration.Chord7
{
    public class ChordX7 : IChordRegen
    {
        public bool[] RegenChord(bool[] lastpass, string name, ref int root, ref int lowlimit, ref int highlimit, int count = 4)
        {
            if (name == "X7")
            {
                Expansion.BroadcastTo128(lastpass, root, highlimit);
                Expansion.BroadcastTo128(lastpass, root + 4, highlimit);
                Expansion.BroadcastTo128(lastpass, root + 7, highlimit);
                Expansion.BroadcastTo128(lastpass, root + 10, highlimit);
            }
            if (name == "Xm7")
            {
                Expansion.BroadcastTo128(lastpass, root, highlimit);
                Expansion.BroadcastTo128(lastpass, root + 3, highlimit);
                Expansion.BroadcastTo128(lastpass, root + 7, highlimit);
                Expansion.BroadcastTo128(lastpass, root + 10, highlimit);
            }
            return lastpass;
        }
    }
}
