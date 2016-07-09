using System;

namespace PKHeX
{
    public partial class Util
    {
        internal static readonly Random rand = new Random();
        internal static uint rnd32()
        {
            return (uint)rand.Next(1 << 30) << 2 | (uint)rand.Next(1 << 2);
        }
        internal static void Shuffle<T>(T[] array)
        {
            int n = array.Length;
            for (int i = 0; i < n; i++)
            {
                int r = i + (int)(rand.NextDouble() * (n - i));
                T t = array[r];
                array[r] = array[i];
                array[i] = t;
            }
        }
    }
}
