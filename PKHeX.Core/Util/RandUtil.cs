using System;

namespace PKHeX.Core;

public static partial class Util
{
    public static Random Rand => Random.Shared;

    public static uint Rand32() => Rand32(Rand);
    public static uint Rand32(this Random rnd) => ((uint)rnd.Next(1 << 30) << 2) | (uint)rnd.Next(1 << 2);
    public static ulong Rand64(this Random rnd) => rnd.Rand32() | ((ulong)rnd.Rand32() << 32);

    /// <summary>
    /// Shuffles the order of items within a collection of items.
    /// </summary>
    /// <typeparam name="T">Item type</typeparam>
    /// <param name="rnd">RNG object to use</param>
    /// <param name="items">Item collection</param>
    public static void Shuffle<T>(this Random rnd, Span<T> items)
    {
        int n = items.Length;
        for (int i = 0; i < n - 1; i++)
        {
            int j = rnd.Next(i, n);
            if (j != i)
                (items[i], items[j]) = (items[j], items[i]);
        }
    }
}
