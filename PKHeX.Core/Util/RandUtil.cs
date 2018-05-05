using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    public partial class Util
    {
        public static readonly Random Rand = new Random();
        public static uint Rand32()
        {
            return (uint)Rand.Next(1 << 30) << 2 | (uint)Rand.Next(1 << 2);
        }

        /// <summary>
        /// Shuffles the order of items within a collection of items.
        /// </summary>
        /// <typeparam name="T">Item type</typeparam>
        /// <param name="items">Item collection</param>
        public static void Shuffle<T>(IList<T> items)
        {
            int n = items.Count;
            for (int i = 0; i < n; i++)
            {
                int r = i + Rand.Next(n-i);
                T t = items[r];
                items[r] = items[i];
                items[i] = t;
            }
        }
    }
}
