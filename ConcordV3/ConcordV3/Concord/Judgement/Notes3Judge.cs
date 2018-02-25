using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConcordV3.Concord.Judgement
{
    public class Notes3Judge : IChordJudge
    {
        public string JudgeChord(bool[] noteExists, int count)
        {
            if (count == 2)
            {
                if (noteExists[4]) return "X";
                if (noteExists[3]) return "Xm";
                if (noteExists[6]) return "X-5";
                if (noteExists[8]) return "X+5";
            }
            else if (count == 3)
            {
                if (noteExists[4] && noteExists[7]) return "X";
                if (noteExists[3] && noteExists[7]) return "Xm";
                if (noteExists[4] && noteExists[6]) return "X-5";
                if (noteExists[4] && noteExists[8]) return "X+5";
                if (noteExists[5] && noteExists[7]) return "Xsus";
            }
            return null;
        }
    }
}
