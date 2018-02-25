using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConcordNew.Concord.Generate.WithMainMelody
{
    public static class Step1
    {
        public static int[] result;
        /*static readonly int[] T = { 1, 3, 6 };
        static readonly int[] S = { 2, 4 };
        static readonly int[] D = { 5, 7 };*/
        static readonly int[] T = { 1, 3, 6 };
        static readonly int[] S = { 2, 4 };
        static readonly int[] D = { 5 };
        public static void 和弦式()
        {
            result = new int[WithMainMelody.MainMelody.Length];
            int stat = 0;
            for (int i = 0; i < WithMainMelody.MainMelody.Length; i+=1)
            {
                switch (stat % 3)
                {
                    case 0: //T主
                        result[i] = Rand.pick(T);
                        if (i > 0 && result[i - 1] == 5 && result[i] == 6)
                            result[i] = 3;
                        break;
                    case 1: //S下属
                        result[i] = Rand.pick(S);
                        break;
                    case 2: //D属
                        result[i] = Rand.pick(D);
                        break;
                }
                stat++;
            }
        }
    }
}
