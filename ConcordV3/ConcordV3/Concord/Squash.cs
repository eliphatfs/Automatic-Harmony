using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConcordV3.Concord
{
    public static class Squash
    {
        public static bool[] ToOctaveRaw(bool[] note128)
        {
            if (note128.Length != 128)
                throw new InvalidOperationException("Squash.ToOcataveRaw can only squash Array Length of 128!");
            bool[] result = new bool[12];
            for (int i = 0; i < 12; i++) result[i] = false;
            for (int i = 0; i < 128; i++)
                result[i % 12] |= note128[i];
            return result;
        }
    }
}
