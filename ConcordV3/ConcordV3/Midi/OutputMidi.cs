using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sanford.Multimedia.Midi;

namespace ConcordV3.Midi
{
    public static class OutputMidi
    {
        public static string Score2String(List<int>[] score)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < score.Length; i++)
            {
                for (int j = 0; j < score[i].Count; j++)
                {
                    builder.Append(score[i][j]);
                    if (j != score[i].Count - 1)
                        builder.Append(',');
                }
                builder.AppendLine();
            }
            return builder.ToString();
        }
    }
}
