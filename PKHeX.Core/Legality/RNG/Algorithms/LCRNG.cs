using System;
using System.Runtime.CompilerServices;

namespace PKHeX.Core;

/// <summary>
/// 32 Bit Linear Congruential Random Number Generator
/// </summary>
/// <remarks>Frame advancement for forward and reverse.
/// <br>
/// https://en.wikipedia.org/wiki/Linear_congruential_generator
/// </br>
/// <br>
/// seed_n+1 = seed_n * <see cref="Mult"/> + <see cref="Add"/>
/// </br>
/// </remarks>
public static class LCRNG
{
    // Forward and reverse constants
    public const uint Mult  = 0x41C64E6D;
    public const uint Add   = 0x00006073;
    public const uint rMult = 0xEEB9EB65;
    public const uint rAdd  = 0x0A3561A1;
    private const uint Mult2 = unchecked(Mult * Mult);
    private const uint Add2 = unchecked(Add * (Mult + 1));

    /// <summary>
    /// Advances the RNG seed to the next state value.
    /// </summary>
    /// <param name="seed">Current seed</param>
    /// <returns>Seed advanced a single time.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Next(uint seed) => (seed * Mult) + Add;

    /// <summary>
    /// Advances the RNG seed forward 2 steps.
    /// </summary>
    /// <param name="seed">Current seed</param>
    /// <returns>Seed advanced twice.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Next2(uint seed) => (seed * Mult2) + Add2;

    /// <summary>
    /// Reverses the RNG seed to the previous state value.
    /// </summary>
    /// <param name="seed">Current seed</param>
    /// <returns>Seed reversed a single time.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Prev(uint seed) => (seed * rMult) + rAdd;

    /// <summary>
    /// Advances the RNG seed to the next state value a specified amount of times.
    /// </summary>
    /// <param name="seed">Current seed</param>
    /// <param name="frames">Amount of times to advance.</param>
    /// <returns>Seed advanced the specified amount of times.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Advance(uint seed, int frames)
    {
        for (int i = 0; i < frames; i++)
            seed = Next(seed);
        return seed;
    }

    /// <summary>
    /// Reverses the RNG seed to the previous state value a specified amount of times.
    /// </summary>
    /// <param name="seed">Current seed</param>
    /// <param name="frames">Amount of times to reverse.</param>
    /// <returns>Seed reversed the specified amount of times.</returns>
    public static uint Reverse(uint seed, int frames)
    {
        for (int i = 0; i < frames; i++)
            seed = Prev(seed);
        return seed;
    }

    public const int MaxCountSeedsPID = 3;
    public const int MaxCountSeedsIV = 6;

    static LCRNG()
    {
        // Populate Meet Middle Arrays
        for (uint i = 0; i <= byte.MaxValue; i++)
        {
            SetFlagData(i, Mult, Add, flags, low8); // 1,2
            SetFlagData(i, Mult2, Add2, g_flags, g_low8); // 1,3
        }
    }

    // Bruteforce cache for searching seeds
    private const int cacheSize = 1 << 16;
    private static readonly byte[] low8 = new byte[cacheSize];
    private static readonly bool[] flags = new bool[cacheSize];
    private static readonly byte[] g_low8 = new byte[cacheSize];
    private static readonly bool[] g_flags = new bool[cacheSize];

    private const uint k2 = Mult << 8;
    private const uint k2s = Mult2 << 8;

    #region Initialization

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SetFlagData(uint i, uint mult, uint add, bool[] f, byte[] v)
    {
        // the second rand() also has 16 bits that aren't known. It is a 16 bit value added to either side.
        // to consider these bits and their impact, they can at most increment/decrement the result by 1.
        // with the current calc setup, the search loop's calculated value may be -1 (loop does subtraction)
        // since LCGs are linear (hence the name), there's no values in adjacent cells. (no collisions)
        // if we mark the prior adjacent cell, we eliminate the need to check flags twice on each loop.
        uint right = (mult * i) + add;
        ushort val = (ushort)(right >> 16);

        f[val] = true; v[val] = (byte)i;
        --val;
        f[val] = true; v[val] = (byte)i;
        // now the search only has to access the flags array once per loop.
    }

    #endregion

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
    /// Finds all seeds that can generate the <see cref="pid"/> with a discarded rand() between the two halves.
    /// </summary>
    /// <param name="result">Result storage array, to be populated starting at index 0.</param>
    /// <param name="pid">PID to be reversed into seeds that generate it.</param>
    /// <returns>Count of results added to <see cref="result"/></returns>
    public static int GetSeedsSkip(Span<uint> result, uint pid)
    {
        uint first = pid << 16;
        uint second = pid & 0xFFFF_0000;
        return GetSeedsSkip(result, first, second);
    }

    /// <summary>
    /// Finds all seeds that can generate the IVs, with a vblank skip between the two IV rand() calls.
    /// </summary>
    /// <param name="result">Result storage array, to be populated starting at index 0.</param>
    /// <param name="hp" >Entity IV for HP</param>
    /// <param name="atk">Entity IV for Attack</param>
    /// <param name="def">Entity IV for Defense</param>
    /// <param name="spa">Entity IV for Special Attack</param>
    /// <param name="spd">Entity IV for Special Defense</param>
    /// <param name="spe">Entity IV for Speed</param>
    /// <returns>Count of results added to <see cref="result"/></returns>
    public static int GetSeedsIVsSkip(Span<uint> result, uint hp, uint atk, uint def, uint spa, uint spd, uint spe)
    {
        uint first = (hp | (atk << 5) | (def << 10)) << 16;
        uint second = (spe | (spa << 5) | (spd << 10)) << 16;
        return GetSeedsIVsSkip(result, first, second);
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
            var next = Next(seed);
            if ((next & 0xFFFF0000) == second)
                result[ctr++] = Prev(seed);
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
            uint val = (ushort)(search1 >> 16);
            if (flags[val])
            {
                // Verify PID calls line up
                var seed = first | (i << 8) | low8[val];
                var next = Next(seed);
                if ((next & 0x7FFF0000) == second)
                {
                    var origin = Prev(seed);
                    result[ctr++] = origin;
                    result[ctr++] = origin ^ 0x80000000;
                }
            }

            val = (ushort)(search2 >> 16);
            if (flags[val])
            {
                // Verify PID calls line up
                var seed = first | (i << 8) | low8[val];
                var next = Next(seed);
                if ((next & 0x7FFF0000) == second)
                {
                    var origin = Prev(seed);
                    result[ctr++] = origin;
                    result[ctr++] = origin ^ 0x80000000;
                }
            }
        }
        return ctr;
    }

    /// <summary>
    /// Finds all the origin seeds for two 16 bit rand() calls (ignoring a rand() in between)
    /// </summary>
    /// <param name="result">Result storage array, to be populated starting at index 0.</param>
    /// <param name="first">First rand() call, 16 bits, already shifted left 16 bits.</param>
    /// <param name="third">Third rand() call, 16 bits, already shifted left 16 bits.</param>
    /// <returns>Count of results added to <see cref="result"/></returns>
    public static int GetSeedsSkip(Span<uint> result, uint first, uint third)
    {
        int ctr = 0;
        uint search = third - (first * Mult2);
        for (uint i = 0; i <= 255; ++i, search -= k2s)
        {
            ushort val = (ushort)(search >> 16);
            if (g_flags[val])
            {
                // Verify PID calls line up
                var seed = first | (i << 8) | g_low8[val];
                var next = Next2(seed);
                if ((next & 0xFFFF0000) == third)
                    result[ctr++] = Prev(seed);
            }
        }
        return ctr;
    }

    /// <summary>
    /// Finds all the origin seeds for two 15 bit rand() calls (ignoring a rand() in between)
    /// </summary>
    /// <param name="result">Result storage array, to be populated starting at index 0.</param>
    /// <param name="first">First rand() call, 15 bits, already shifted left 16 bits.</param>
    /// <param name="third">Third rand() call, 15 bits, already shifted left 16 bits.</param>
    /// <returns>Count of results added to <see cref="result"/></returns>
    public static int GetSeedsIVsSkip(Span<uint> result, uint first, uint third)
    {
        int ctr = 0;
        uint search1 = third - (first * Mult2);
        uint search3 = third - ((first ^ 0x80000000) * Mult2);

        for (uint i = 0; i <= 255; i++, search1 -= k2s, search3 -= k2s)
        {
            uint val = (ushort)(search1 >> 16);
            if (g_flags[val])
            {
                // Verify PID calls line up
                var seed = first | (i << 8) | g_low8[val];
                var next = Next2(seed);
                if ((next & 0x7FFF0000) == third)
                {
                    var origin = Prev(seed);
                    result[ctr++] = origin;
                    result[ctr++] = origin ^ 0x80000000;
                }
            }

            val = (ushort)(search3 >> 16);
            if (g_flags[val])
            {
                // Verify PID calls line up
                var seed = first | (i << 8) | g_low8[val];
                var next = Next2(seed);
                if ((next & 0x7FFF0000) == third)
                {
                    var origin = Prev(seed);
                    result[ctr++] = origin;
                    result[ctr++] = origin ^ 0x80000000;
                }
            }
        }
        return ctr;
    }
}
