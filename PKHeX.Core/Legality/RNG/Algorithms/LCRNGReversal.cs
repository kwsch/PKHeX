using System;

namespace PKHeX.Core;

/// <summary>
/// Seed reversal logic for the <see cref="LCRNG"/> algorithm.
/// </summary>
/// <remarks>
/// Use a meet-in-the-middle attack to reduce the search space to 2^8 instead of 2^16
/// flag/2^8 tables are precomputed and constant (unrelated to rand pairs)
/// https://crypto.stackexchange.com/a/10609
/// </remarks>
public static class LCRNGReversal
{
    // Bruteforce cache for searching seeds
    private const int cacheSize = 1 << 16;
    private static readonly byte[] low8 = new byte[cacheSize];
    private static readonly bool[] flags = new bool[cacheSize];
    private const uint Mult = LCRNG.Mult;
    private const uint Add = LCRNG.Add;
    private const uint k2 = Mult << 8;

    static LCRNGReversal()
    {
        // Populate Meet Middle Arrays
        var f = flags;
        var b = low8;
        for (uint i = 0; i <= byte.MaxValue; i++)
        {
            // the second rand() also has 16 bits that aren't known. It is a 16 bit value added to either side.
            // to consider these bits and their impact, they can at most increment/decrement the result by 1.
            // with the current calc setup, the search loop's calculated value may be -1 (loop does subtraction)
            // since LCGs are linear (hence the name), there's no values in adjacent cells. (no collisions)
            // if we mark the prior adjacent cell, we eliminate the need to check flags twice on each loop.
            uint right = (Mult * i) + Add;
            ushort val = (ushort)(right >> 16);

            f[val] = true; b[val] = (byte)i;
            --val;
            f[val] = true; b[val] = (byte)i;
            // now the search only has to access the flags array once per loop.
        }
    }

    /// <summary>
    /// Finds all seeds that can generate the <see cref="pid"/> by two successive rand() calls.
    /// </summary>
    /// <param name="result">Result storage array, to be populated starting at index 0.</param>
    /// <param name="pid">PID to be reversed into seeds that generate it.</param>
    /// <returns>Count of results added to <see cref="result"/></returns>
    public static int GetSeeds(Span<uint> result, uint pid)
    {
        uint first = pid << 16;
        uint second = pid & 0xFFFF_0000;
        return GetSeeds(result, first, second);
    }

    /// <summary>
    /// Finds all seeds that can generate the IVs by two successive rand() calls.
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
    /// Finds all the origin seeds for two 16 bit rand() calls
    /// </summary>
    /// <param name="result">Result storage array, to be populated starting at index 0.</param>
    /// <param name="first">First rand() call, 16 bits, already shifted left 16 bits.</param>
    /// <param name="second">Second rand() call, 16 bits, already shifted left 16 bits.</param>
    /// <returns>Count of results added to <see cref="result"/></returns>
    public static int GetSeeds(Span<uint> result, uint first, uint second)
    {
        int ctr = 0;
        uint k1 = second - (first * Mult);
        for (uint i = 0, k3 = k1; i <= 255; ++i, k3 -= k2)
        {
            ushort val = (ushort)(k3 >> 16);
            if (!flags[val])
                continue;
            // Verify PID calls line up
            var seed = first | (i << 8) | low8[val];
            var next = LCRNG.Next(seed);
            if ((next & 0xFFFF0000) == second)
                result[ctr++] = LCRNG.Prev(seed);
        }
        return ctr;
    }

    /// <summary>
    /// Finds all the origin seeds for two 15 bit rand() calls
    /// </summary>
    /// <param name="result">Result storage array, to be populated starting at index 0.</param>
    /// <param name="first">First rand() call, 15 bits, already shifted left 16 bits.</param>
    /// <param name="second">Second rand() call, 15 bits, already shifted left 16 bits.</param>
    /// <returns>Count of results added to <see cref="result"/></returns>
    public static int GetSeedsIVs(Span<uint> result, uint first, uint second)
    {
        int ctr = 0;
        // Check with the top bit of the first call both
        // flipped and unflipped to account for only knowing 15 bits
        uint search1 = second - (first * Mult);
        uint search2 = second - ((first ^ 0x80000000) * Mult);

        for (uint i = 0; i <= 255; i++, search1 -= k2, search2 -= k2)
        {
            ushort val = (ushort)(search1 >> 16);
            if (flags[val])
            {
                // Verify PID calls line up
                var seed = first | (i << 8) | low8[val];
                var next = LCRNG.Next(seed);
                if ((next & 0x7FFF0000) == second)
                {
                    var origin = LCRNG.Prev(seed);
                    result[ctr++] = origin;
                    result[ctr++] = origin ^ 0x80000000;
                }
            }

            val = (ushort)(search2 >> 16);
            if (flags[val])
            {
                // Verify PID calls line up
                var seed = first | (i << 8) | low8[val];
                var next = LCRNG.Next(seed);
                if ((next & 0x7FFF0000) == second)
                {
                    var origin = LCRNG.Prev(seed);
                    result[ctr++] = origin;
                    result[ctr++] = origin ^ 0x80000000;
                }
            }
        }
        return ctr;
    }
}
