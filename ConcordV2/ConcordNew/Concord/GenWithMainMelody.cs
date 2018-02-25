using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConcordNew.Concord.DataStructure;

namespace ConcordNew.Concord
{
    static class GenWithMainMelody
    {
        //static int stat = 0;
        [Obsolete]
        public static Score Run(Score score)
        {
            Score s = new Score();
            int?[] melody = new int?[1024];
            for (int i = 0; i < Score.MAXN; i++)
            {
                melody[i] = score[i].Count > 0 ? score[i][0] : null;
            }
            int last1 = 0;
            int last2 = 0;
            for (int i = 0; i < Score.MAXN; i++)
            {
                if (melody[i] == null) break;
                int lowest = 0;

                if (last1 > last2)
                {
                    s[i].Add(melody[i]);
                    s[i].Add(melody[i] - 2);
                    s[i].Add(melody[i] - 4);
                    s[i].Add(lowest = melody[i].Value - 6);
                }
                else
                {
                    if (melody[i].Value - 6 > last1)
                    {
                        s[i].Add(melody[i]);
                        s[i].Add(melody[i] - 2);
                        s[i].Add(melody[i] - 4);
                        s[i].Add(lowest = melody[i].Value - Rand.pick(new int[] { 8, 10, 11 }));
                    }
                    else if (melody[i].Value - 4 > last1)
                    {
                        s[i].Add(melody[i] + 2);
                        s[i].Add(melody[i]);
                        s[i].Add(melody[i] - 2);
                        s[i].Add(lowest = melody[i].Value - 6);
                    }
                    else
                    {
                        s[i].Add(melody[i] + 4);
                        s[i].Add(melody[i] + 2);
                        s[i].Add(melody[i]);
                        s[i].Add(lowest = melody[i].Value - 4);
                    }
                }
                last2 = last1;
                last1 = lowest;
            }
            return s;
        }
    }
}
