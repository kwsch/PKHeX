using System;
using System.Runtime.CompilerServices;

namespace PKHeX.Core;

public static class RNGUtil
{
    /// <summary>
    /// Generates an IV for each RNG call using the top 5 bits of frame seeds.
    /// </summary>
    /// <param name="rng">RNG to use</param>
    /// <param name="seed">RNG seed</param>
    /// <param name="IVs">Expected IVs</param>
    /// <returns>True if all match.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool GetSequentialIVsUInt32(this LCRNG rng, uint seed, ReadOnlySpan<uint> IVs)
    {
        foreach (var iv in IVs)
        {
            seed = rng.Next(seed);
            var IV = seed >> 27;
            if (IV != iv)
                return false;
        }
        return true;
    }

    /// <summary>
    /// Generates an IV for each RNG call using the top 5 bits of frame seeds.
    /// </summary>
    /// <param name="rng">RNG to use</param>
    /// <param name="seed">RNG seed</param>
    /// <param name="ivs">Buffer to store generated values</param>
    /// <returns>Array of 6 IVs as <see cref="int"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void GetSequentialIVsInt32(this LCRNG rng, uint seed, Span<int> ivs)
    {
        for (int i = 0; i < ivs.Length; i++)
        {
            seed = rng.Next(seed);
            ivs[i] = (int)(seed >> 27);
        }
    }
}
