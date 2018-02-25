using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConcordNew
{
    public static class Rand
    {
        public static Random random = new Random();
        /// <summary>
        /// 返回[l,r]间的随机整数
        /// </summary>
        /// <param name="l">下界</param>
        /// <param name="r">上界</param>
        /// <returns></returns>
        public static int rint(int l, int r)
        {
            return random.Next(l, r + 1);
        }
        public static T pick<T>(T[] array)
        {
            return array[random.Next(0, array.Length)];
        }
    }
}
