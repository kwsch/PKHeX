using System;

namespace PKHeX.Core;

/// <summary>
/// Seed reversal logic for the <see cref="LCRNG"/> algorithm.
/// </summary>
/// <remarks>
/// Uses lattice-reduction bounds from https://github.com/StarfBerry/PokeRNG/blob/main/Recovery/LCG_Recovery.py.
/// </remarks>
public static class LCRNGReversal
{
    // https://github.com/StarfBerry/PokeRNG/blob/main/Recovery/LCG_Recovery.py
    private const uint RMult = LCRNG.rMult; // reverse multiplier constant
    private const uint RLag0 = 0x7ED7; // 32471
    private const uint RLag1 = 0x71A4; // -68321 mod 32471
    private const uint RLower = 0x79C8BF4A; // ((-0x50F40B53C37 + 0xffff_ffff) >> 16) + (32471 << 16)
    private const uint RUpper = 0x79C8A5F4; // (-0x50E5A0B3C37 >> 16) + (32471 << 16)

    private const uint Lag0 = 0x6134; // -26579 mod 51463
    private const uint Lag1 = 0xC907; // 51463
    private const uint Lower = 0x64833CB0; // ((-0xC34F11DB + 0x7fff_ffff) >> 16) + (51463 << 15)
    private const uint Upper = 0x6483CBBC; // (0x4BBCEE25 >> 16) + (51463 << 15)

    /// <summary>
    /// Finds all seeds that can generate the IVs.
    /// </summary>
    /// <param name="result">Result storage array, to be populated starting at index 0.</param>
    /// <param name="hp" >Entity IV for HP</param>
    /// <param name="atk">Entity IV for Attack</param>
    /// <param name="def">Entity IV for Defense</param>
    /// <param name="spa">Entity IV for Special Attack</param>
    /// <param name="spd">Entity IV for Special Defense</param>
    /// <param name="spe">Entity IV for Speed</param>
    /// <returns>Count of results added to <see cref="result"/></returns>
    public static int GetSeedsIVs(Span<uint> result, uint hp, uint atk, uint def, uint spa, uint spd, uint spe)
    {
        uint first = (hp | (atk << 5) | (def << 10)) << 16;
        uint second = (spe | (spa << 5) | (spd << 10)) << 16;
        return GetSeedsIVs(result, first, second);
    }

    /// <summary>
    /// Finds all the origin seeds for two 16 bit rand() calls.
    /// </summary>
    /// <param name="result">Result storage array, to be populated starting at index 0.</param>
    /// <param name="first">First rand() call, 15 bits, already shifted left 16 bits.</param>
    /// <param name="second">Second rand() call, 15 bits, already shifted left 16 bits.</param>
    /// <returns>Count of results added to <see cref="result"/></returns>
    public static int GetSeeds(Span<uint> result, uint first, uint second)
    {
        ulong tmp = (ulong)(((first - (second * RMult)) >> 16) & 0xFFFF) * RLag0;
        var lo = (uint)((tmp + RLower) >> 16);
        var up = (uint)((tmp + RUpper) >> 16);
        if (lo != up) // true in around 10% of cases
            return 0;

        int ctr = 0;
        // at most 3 iterations (around 2.02 in average)
        uint low = (lo * RLag1) % RLag0;
        do
        {
            var seed = LCRNG.Prev(second | low);
            if ((seed & 0xffff0000) == first)
                result[ctr++] = LCRNG.Prev(seed);
        } while ((low += RLag0) < 0x1_0000);
        return ctr;
    }

    /// <summary>
    /// Finds all the origin seeds for two 15 bit rand() calls.
    /// </summary>
    /// <param name="result">Result storage array, to be populated starting at index 0.</param>
    /// <param name="first">First rand() call, 15 bits, already shifted left 16 bits.</param>
    /// <param name="second">Second rand() call, 15 bits, already shifted left 16 bits.</param>
    /// <returns>Count of results added to <see cref="result"/></returns>
    public static int GetSeedsIVs(Span<uint> result, uint first, uint second)
    {
        long diff = ((long)LCRNG.Mult * first) - second;
        ulong tmp = (ulong)((diff >> 16) & 0xFFFF) * Lag1;
        var lo = (uint)(((tmp + Lower) >> 15) * Lag0);
        var mi = lo + Lag0;
        var up = (uint)(((tmp + Upper) >> 15) * Lag0);

        int ctr = 0;
        // around 2.70 iterations in average
        AddSeeds(result, lo % Lag1, first, second, ref ctr);
        AddSeeds(result, mi % Lag1, first, second, ref ctr);
        if (mi != up) // true in around 12% of cases
            AddSeeds(result, up % Lag1, first, second, ref ctr);
        return ctr;
    }

    private static void AddSeeds(Span<uint> result, uint low, uint first, uint second, ref int ctr)
    {
        // at most 3 iterations
        do
        {
            var seed = first | low;
            if ((LCRNG.Next(seed) & 0x7fff0000) != second)
                continue;
            seed = LCRNG.Prev(seed);
            result[ctr++] = seed;
            result[ctr++] = seed ^ 0x80000000;
        } while ((low += Lag1) < 0x1_0000);
    }
}
