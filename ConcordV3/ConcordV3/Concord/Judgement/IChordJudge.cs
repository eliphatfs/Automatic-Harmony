using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConcordV3.Concord.Judgement
{
    public interface IChordJudge
    {
        /// <summary>
        /// Judge whether a chord matches the notion inside the implemention.
        /// </summary>
        /// <param name="noteExists">12 bools representing notes of an octave</param>
        /// <param name="count">count of note existing</param>
        /// <returns>null if not matched; notion of chord if matched.</returns>
        string JudgeChord(bool[] noteExists, int count);
    }
}
