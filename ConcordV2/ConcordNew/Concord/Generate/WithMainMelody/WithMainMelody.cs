using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConcordNew.Concord.Generate.WithMainMelody
{
    public static class WithMainMelody
    {
        public static int[] 蝶梦之门 = new int[] { 1, 2, 3, 3, 3, 3, 3, 2, 3, 5, 5, 5, 5, 2, 2, -114, -114, -114, 1, 1, 1, 1, 1, 0, 1, 3, 3, 3, 3, 3, 0, 0, -114, -114, -114, -1, -1, -1, -1, -1, 0, 1, 2, 2, 2, 2, 2, -2, 3, 3, 3, 3, 3, 4, 4, 4, 3, 4, 5, 5, 5, 5, 3, 2, 2, 2, 2, 1, -114, -114, -114 };
        public static int[] Random01 = new int[] { 6, 6, 5, -114, 4, 1, 7, 7, 6, 1, 3, 5, 3, 2, -114, 5, 4, 1, 7, 3, 5, 1, 4, -114, 3, 6, 3, 9, -114, 3 };
        public static int[] MainMelody = new int[] { 1, 1, 5, 5, 6, 6, 5, -114, 4, 4, 3, 3, 2, 2, 1, -114, 5, 5, 4, 4, 3, 3, 2, -114, 5, 5, 4, 4, 3, 3, 2, -114, 1, 1, 5, 5, 6, 6, 5, -114, 4, 4, 3, 3, 2, 2, 1, 1, 1, 1 };
        static WithMainMelody()
        {
            MainMelody = new int[1000];
            for (int i = 0; i < MainMelody.Length; i++)
            {
                MainMelody[i] = Rand.rint(0, 9);
            }
            for (int i = 0; i < MainMelody.Length; i++)
            {
                MainMelody[i] += 14;
            }
        }
        public static string Run()
        {
            Step1.和弦式();
            Step2.和弦();
            
            //System.Windows.Forms.MessageBox.Show(IO.ScoreIO.ToStringSequences(new DataStructure.Score().SetData(Step2.result)));
            return IO.ScoreIO.ToStringSequences(new DataStructure.Score().SetData(Step2.result));
        }
    }
}
