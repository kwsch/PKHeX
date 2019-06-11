using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    public static partial class Util
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
        public static void Shuffle<T>(IList<T> items) => Shuffle(items, 0, items.Count);

        /// <summary>
        /// Shuffles the order of items within a collection of items.
        /// </summary>
        /// <typeparam name="T">Item type</typeparam>
        /// <param name="items">Item collection</param>
        /// <param name="start">Starting position</param>
        /// <param name="end">Ending position</param>
        public static void Shuffle<T>(IList<T> items, int start, int end)
        {
            for (int i = start; i < end; i++)
            {
                int index = i + Rand.Next(end - i);
                T t = items[index];
                items[index] = items[i];
                items[i] = t;
            }
        }
    }
}
