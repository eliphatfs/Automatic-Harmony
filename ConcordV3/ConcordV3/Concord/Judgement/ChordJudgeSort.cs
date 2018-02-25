using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConcordV3.Concord.Judgement
{
    public class ChordJudgeSort : IComparer<Tuple<string, int>>
    {
        private static readonly string[] CHORD_SORT = {
                                                          "X",
                                                          "Xm",
                                                          "X7",
                                                          "X7omit3",
                                                          "Xmaj7",
                                                          "Xmaj7omit3",
                                                          "Xsus",
                                                          "Xm7",
                                                          "X-5",
                                                          "X+5",
                                                          "X-",
                                                          "X6",
                                                          "Xm6",
                                                          "X7-5",
                                                          "X7+5",
                                                          "Xm7-5"
                                                      };
        public static readonly Dictionary<string, int> DICTIONARY_CHORD;
        static ChordJudgeSort() {
            DICTIONARY_CHORD = new Dictionary<string, int>();
            for (int i = 0; i < CHORD_SORT.Length; i++)
                DICTIONARY_CHORD.Add(CHORD_SORT[i], i);
        }
        public int Compare(Tuple<string, int> x, Tuple<string, int> y)
        {
            if (x == null && y == null)
                return 0;
            if (x == null)
                return int.MaxValue.CompareTo(DICTIONARY_CHORD[y.Item1]);
            else if (y == null)
                return DICTIONARY_CHORD[x.Item1].CompareTo(int.MaxValue);
            else
                return DICTIONARY_CHORD[x.Item1].CompareTo(DICTIONARY_CHORD[y.Item1]);
        }
    }
}
