using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConcordV3
{
    public static class Helper
    {
        public static Random Rand = new Random();
        public static bool RandomBoolean(double chanceTrue = 0.5)
        {
            return Rand.NextDouble() < chanceTrue;
        }
    }
}
