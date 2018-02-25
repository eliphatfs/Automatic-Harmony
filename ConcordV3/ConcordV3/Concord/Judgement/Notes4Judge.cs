using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConcordV3.Concord.Judgement
{
    public class Notes4Judge : IChordJudge
    {
        public string JudgeChord(bool[] noteExists, int count)
        {
            if (count == 2)
            {
                if (noteExists[10]) return "X7";
                if (noteExists[11]) return "Xmaj7";
            }
            else if (count == 3)
            {
                if (noteExists[6] && noteExists[9]) return "X-";
                if (noteExists[4] && noteExists[9]) return "X6";
                if (noteExists[3] && noteExists[9]) return "Xm6";
                if (noteExists[10] && noteExists[4]) return "X7";
                if (noteExists[11] && noteExists[4]) return "Xmaj7";
                if (noteExists[10] && noteExists[7]) return "X7omit3";
                if (noteExists[11] && noteExists[7]) return "Xmaj7omit3";
                if (noteExists[10] && noteExists[3]) return "Xm7";
            }
            else if (count == 4)
            {
                if (noteExists[3] && noteExists[6] && noteExists[9]) return "X-";
                if (noteExists[4] && noteExists[7] && noteExists[9]) return "X6";
                if (noteExists[3] && noteExists[7] && noteExists[9]) return "Xm6";
                if (noteExists[10] && noteExists[4] && noteExists[7]) return "X7";
                if (noteExists[11] && noteExists[4] && noteExists[7]) return "Xmaj7";
                if (noteExists[10] && noteExists[3] && noteExists[7]) return "Xm7";
                if (noteExists[10] && noteExists[4] && noteExists[6]) return "X7-5";
                if (noteExists[10] && noteExists[4] && noteExists[8]) return "X7+5";
                if (noteExists[10] && noteExists[3] && noteExists[6]) return "Xm7-5";
            }
            return null;
        }
    }
}
