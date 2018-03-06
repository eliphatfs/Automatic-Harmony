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

        public static void BreakChordsTimely(List<List<int>> toModify, int point, int expandLength, int keep = 0)
        {
            List<int> chordAtPoint = toModify[point];
            chordAtPoint.Sort();
            int step = expandLength / (chordAtPoint.Count - 1 - keep);
            int breakcount = chordAtPoint.Count - 1 - keep;
            for (int i = point, j = 0; j < breakcount; i += step, j++)
            {
                int note = chordAtPoint[keep];
                chordAtPoint.RemoveAt(keep);
                for (int k = i; k < i + step; k++)
                {
                    List<int> current = toModify[k];
                    current.Add(note);
                }
            }
        }
    }
}
