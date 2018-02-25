using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConcordV3.Concord.Regeneration
{
    public interface IChordRegen
    {
        bool[] RegenChord(bool[] lastpass, string name, ref int root, ref int lowlimit, ref int highlimit, int count = 4);
    }
}
