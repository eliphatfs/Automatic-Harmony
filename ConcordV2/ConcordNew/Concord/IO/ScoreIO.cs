using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConcordNew.Concord.DataStructure;

namespace ConcordNew.Concord.IO
{
    static class ScoreIO
    {
        public static string ToStringSequences(Score score)
        {
            List<StringBuilder> builders = new List<StringBuilder>();
            for (int i = 0; i < Score.MAXN; i++)
            {
                if (score[i].Count == 0) continue;
                int j;
                for (j = builders.Count; j < score[i].Count; j++)
                    builders.Add(new StringBuilder());
                for (j = 0; j < score[i].Count; j++)
                {
                    builders[j].Append(NoteMidiConverter.NoteToMidi(score[i][j].Value))
                               .Append(",");
                }
            }
            StringBuilder finalBuilder = new StringBuilder();
            foreach (var builder in builders)
                finalBuilder.AppendLine(builder.ToString());
            return finalBuilder.ToString();
        }
    }
}
