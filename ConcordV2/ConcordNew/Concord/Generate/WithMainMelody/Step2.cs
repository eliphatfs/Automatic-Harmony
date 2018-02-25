using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConcordNew.Concord.DataStructure;

namespace ConcordNew.Concord.Generate.WithMainMelody
{
    public static class Step2
    {
        public static Chord[] result;
        public static int[] result_type;
        public static readonly int[] 三和弦 = { 2, 4, 7, 9, 11, 14, 16, 18, 21 };
        public static readonly int[] 七和弦 = { 2, 4, 6, 7, 9, 11, 13, 14, 16, 18, 20, 21 };
        public static void 和弦()
        {
            result = new Chord[Step1.result.Length];
            result_type = new int[Step1.result.Length];
            for (int i = 0; i < Step1.result.Length; i++)
            {
                result[i] = new Chord();
                result[i].Add(Step1.result[i]);
                result[i].Add(WithMainMelody.MainMelody[i]);
            }
            _step_2_1();
            _step_2_2();
        }
        private static void _step_2_1()
        {
            for (int i = 0; i < Step1.result.Length; i++)
            {
                if (WithMainMelody.MainMelody[i] <= -100)
                    result_type[i] = -1;
                else if (三和弦.Contains(WithMainMelody.MainMelody[i] - Step1.result[i]))
                    result_type[i] = 3;
                else if (七和弦.Contains(WithMainMelody.MainMelody[i] - Step1.result[i]))
                    result_type[i] = 7;
                else
                    result_type[i] = -1;
            }
        }
        private static void _step_2_2()
        {
            int[] 三音 = new int[] { 2, 9, 16 };
            int[] 七音 = new int[] { 6, 13, 20 };
            int MAXLOOP = 200;
            int loop = MAXLOOP; //防止死循环
            for (int i = 0; i < Step1.result.Length; i++)
            {
                if (result_type[i] == 3)
                {
                    int n;
                    do
                    {
                        n = Rand.pick(三音);
                        loop--;
                        if (loop < 0) break;
                    }
                    while (n + Step1.result[i] > WithMainMelody.MainMelody[i] || n < 4 || result[i].Contains(n + Step1.result[i]));
                    loop = MAXLOOP;
                    result[i].Add(n + Step1.result[i]);
                    int m;
                    do
                    {
                        m = Rand.pick(三和弦);
                        loop--;
                        if (loop < 0) break;
                    }
                    while (三音.Contains(m) || m <= 4 || result[i].Contains(m + Step1.result[i]) || Step1.result[i] + m - WithMainMelody.MainMelody[i] > -4);
                    loop = MAXLOOP;
                    result[i].Add(Step1.result[i] + m);
                    for (int j = 1; j < 三和弦.Length; j++)
                    {
                        if (Step1.result[i] + 三和弦[j] - WithMainMelody.MainMelody[i] < 0
                            && Step1.result[i] + 三和弦[j] - WithMainMelody.MainMelody[i] > -4)
                        {
                            result[i].Add(Step1.result[i] + 三和弦[j]);
                            break;
                        }
                    }
                }
                else if (result_type[i] == 7)
                {
                    int n;
                    do
                    {
                        n = Rand.pick(七音);
                        loop--;
                        if (loop < 0) break;
                    }
                    while (n + Step1.result[i] > WithMainMelody.MainMelody[i] || n < 4 || result[i].Contains(n + Step1.result[i]));
                    loop = MAXLOOP;
                    result[i].Add(n + Step1.result[i]);
                    int m;
                    do
                    {
                        m = Rand.pick(七和弦);
                        loop--;
                        if (loop < 0) break;
                    }
                    while (七音.Contains(m) || m <= 4 || result[i].Contains(m + Step1.result[i]) || Step1.result[i] + m - WithMainMelody.MainMelody[i] > -4);
                    loop = MAXLOOP;
                    result[i].Add(Step1.result[i] + m);
                    for (int j = 1; j < 七和弦.Length; j++)
                    {
                        if (Step1.result[i] + 七和弦[j] - WithMainMelody.MainMelody[i] < 0
                            && Step1.result[i] + 七和弦[j] - WithMainMelody.MainMelody[i] > -4)
                        {
                            result[i].Add(Step1.result[i] + 七和弦[j]);
                            break;
                        }
                    }
                }
                else for (int m=0; m<5; m++)
                    result[i].Add(-100);
            }
        }
    }
}
