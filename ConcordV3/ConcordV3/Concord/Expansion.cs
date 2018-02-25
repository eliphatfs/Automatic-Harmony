using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConcordV3.Concord
{
    public static class Expansion
    {
        public static void BroadcastTo128(bool[] note128, int root, int highlimit)
        {
            if (note128.Length != 128)
                throw new InvalidOperationException("Expansion.BroadcastTo128 can only broadcast a note into Array of Length 128!");
            while (root < highlimit)
            {
                note128[root] = true;
                root += 12;
            }
        }
    }
}
