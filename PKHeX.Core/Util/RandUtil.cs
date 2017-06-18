using System;

namespace PKHeX.Core
{
    public partial class Util
    {
        public static readonly Random Rand = new Random();
        public static uint Rand32()
        {
            return (uint)Rand.Next(1 << 30) << 2 | (uint)Rand.Next(1 << 2);
        }
        public static void Shuffle<T>(T[] array)
        {
            int n = array.Length;
            for (int i = 0; i < n; i++)
            {
                int r = i + (int)(Rand.NextDouble() * (n - i));
                T t = array[r];
                array[r] = array[i];
                array[i] = t;
            }
        }
    }
}
