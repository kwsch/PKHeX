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
    /// <param name="items">Item collection</param>
    public static void Shuffle<T>(Span<T> items) => Shuffle(items, 0, items.Length, Rand);

    /// <summary>
    /// Shuffles the order of items within a collection of items.
    /// </summary>
    /// <typeparam name="T">Item type</typeparam>
    /// <param name="items">Item collection</param>
    /// <param name="start">Starting position</param>
    /// <param name="end">Ending position</param>
    /// <param name="rnd">RNG object to use</param>
    public static void Shuffle<T>(Span<T> items, int start, int end, Random rnd)
    {
        for (int i = start; i < end; i++)
        {
            int index = i + rnd.Next(end - i);
            (items[index], items[i]) = (items[i], items[index]);
        }
    }
}
