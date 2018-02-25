using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConcordV3.Concord.Judgement
{
    public static class ChordJudge
    {
        public static IChordJudge[] Judgers = { new Notes3Judge(), new Notes4Judge() };
        public static string JudgeChord(bool[] noteOctaveExistsRaw)
        {
            List<Tuple<string, int>> result = new List<Tuple<string, int>>();
            bool[] noteExists = new bool[12];
            int count = 0;
            for (int i = 0; i < 12; i++) if (noteOctaveExistsRaw[i]) count++;
            for (int start = 0; start < 12; start++)
            {
                if (!noteOctaveExistsRaw[start]) continue;
                for (int i = 0; i < 12; i++) noteExists[i] = false;
                for (int i = start; i < start + 12; i++) noteExists[i - start] = noteOctaveExistsRaw[i % 12];
                foreach (IChordJudge cj in Judgers)
                {
                    string current = cj.JudgeChord(noteExists, count);
                    if (current != null)
                        result.Add(Tuple.Create(current, start));
                }
            }
            result.Sort(new ChordJudgeSort());
            return result[0].Item1.Replace("X", NoteNotions.Octave[result[0].Item2]);
        }
    }
}
