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
    public const uint Mult = 0x41C64E6D;
    public const uint Add = 0x00006073;
    public const uint rMult = 0xEEB9EB65;
    public const uint rAdd = 0x0A3561A1;

    /// <summary>
    /// Advances the RNG seed to the next state value.
    /// </summary>
    /// <param name="seed">Current seed</param>
    /// <returns>Seed advanced a single time.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Next(uint seed) => (seed * Mult) + Add;

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

    /// <summary>
    /// Generates an IV for each RNG call using the top 5 bits of frame seeds.
    /// </summary>
    /// <param name="seed">RNG seed</param>
    /// <param name="IVs">Expected IVs</param>
    /// <returns>True if all match.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool GetSequentialIVsUInt32(uint seed, ReadOnlySpan<uint> IVs)
    {
        foreach (var iv in IVs)
        {
            seed = Next(seed);
            var IV = seed >> 27;
            if (IV != iv)
                return false;
        }
        return true;
    }

    /// <summary>
    /// Generates an IV for each RNG call using the top 5 bits of frame seeds.
    /// </summary>
    /// <param name="seed">RNG seed</param>
    /// <param name="ivs">Buffer to store generated values</param>
    /// <returns>Array of 6 IVs as <see cref="int"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void GetSequentialIVsInt32(uint seed, Span<int> ivs)
    {
        for (int i = 0; i < ivs.Length; i++)
        {
            seed = Next(seed);
            ivs[i] = (int)(seed >> 27);
        }
    }

    private const uint LCRNG_MUL = Mult;
    private const uint LCRNG_SUB = unchecked(Add - 0xFFFF); // 0xFFFF6074
    private const ulong LCRNG_BASE = (Mult + 1ul) * 0xFFFF; // 0x41C60CA7B192
    private const uint LCRNG_PRIME = 0x25;
    private const uint LCRNG_RMAX = 0x250000; // prime * 0x10000
    private const uint LCRNG_SKIP1 = 0x73E2B0;
    private const uint LCRNG_SKIP2 = 0x39F158;

    // LCRNGR gives better skips than LCRNG for Method 2
    private const uint LCRNGR_MUL_2 = unchecked(rMult * rMult); // 0xDC6C95D9
    private const uint LCRNGR_SUB_2 = unchecked(rAdd * (1 + rMult)) - 0xFFFF; // 0x4D3BB127
    private const ulong LCRNGR_BASE_2 = (LCRNGR_MUL_2 + 1ul) * 0xFFFF; // 0xDC6BB96D6A26
    private const uint LCRNGR_PRIME_2 = 0x1F;
    private const uint LCRNGR_RMAX_2 = 0x1F0000; // prime * 0x10000
    private const uint LCRNGR_SKIP1_2 = 0xBAED7C;
    private const uint LCRNGR_SKIP2_2 = 0x5D76BE;

    public static int GetSeeds(Span<uint> result, uint pid)
    {
        uint first = pid << 16;
        uint second = pid & 0xffff0000;

        return GetSeeds(result, first, second);
    }

    public static int GetSeedsIVs(Span<uint> result, uint hp, uint atk, uint dfs, uint spa, uint spd, uint spe)
    {
        uint first = (hp | (atk << 5) | (dfs << 10)) << 16;
        uint second = (spe | (spa << 5) | (spd << 10)) << 16;
        return GetSeedsIVs(result, first, second);
    }

    public static int GetSeedsSkip(Span<uint> result, uint pid)
    {
        uint first = pid & 0xffff0000;
        uint second = pid << 16;
        return GetSeedsSkip(result, second, first);
    }

    public static int GetSeedsIVsSkip(Span<uint> result, uint hp, uint atk, uint def, uint spa, uint spd, uint spe)
    {
        uint first = (spe | (spa << 5) | (spd << 10)) << 16;
        uint second = (hp | (atk << 5) | (def << 10)) << 16;
        return GetSeedsIVsSkip(result, first, second);
    }

    public static int GetSeeds(Span<uint> result, uint first, uint second)
    {
        uint t = second - (first * LCRNG_MUL) - LCRNG_SUB;
        uint x = (t * LCRNG_PRIME) % LCRNG_MUL;
        ulong kmax = (LCRNG_BASE - t) >> 32;

        int ctr = 0;
        for (ulong k = 0; k <= kmax;) // at most 117 iterations
        {
            ulong r = (x + (k * LCRNG_SKIP1)) % LCRNG_MUL;
            if (r % LCRNG_PRIME == 0 && r < LCRNG_RMAX)
            {
                ulong tmp = t + (k * 0x100000000);
                ushort low = (ushort)(tmp / LCRNG_MUL);
                result[ctr++] = Prev(first | low);
            }
            k += ((LCRNG_MUL - r) + LCRNG_SKIP1 - 1) / LCRNG_SKIP1;
        }
        return ctr;
    }

    public static int GetSeedsIVs(Span<uint> result, uint first, uint second)
    {
        uint t = (second - (first * LCRNG_MUL) - LCRNG_SUB) & 0x7fffffff;
        uint x = (t * LCRNG_PRIME) % LCRNG_MUL;
        ulong kmax = (LCRNG_BASE - t) >> 31;

        int ctr = 0;
        for (ulong k = 0; k <= kmax;) // at most 117 iterations
        {
            ulong r = (x + (k * LCRNG_SKIP2)) % LCRNG_MUL;
            if (r % LCRNG_PRIME == 0 && r < LCRNG_RMAX)
            {
                ulong tmp = t + (k * 0x80000000);
                ushort low = (ushort)(tmp / LCRNG_MUL);
                uint s = Prev(first | low);
                result[ctr++] = s;
                result[ctr++] = s ^ 0x80000000;
            }
            k += ((LCRNG_MUL - r) + LCRNG_SKIP2 - 1) / LCRNG_SKIP2;
        }
        return ctr;
    }

    public static int GetSeedsSkip(Span<uint> result, uint second, uint first)
    {
        uint t = second - (first * LCRNGR_MUL_2) - LCRNGR_SUB_2;
        uint x = (t * LCRNGR_PRIME_2) % LCRNGR_MUL_2;
        ulong kmax = (LCRNGR_BASE_2 - t) >> 32;

        int ctr = 0;
        for (ulong k = 0; k <= kmax;) // at most 188 iterations
        {
            ulong r = (x + (LCRNGR_SKIP1_2 * k)) % LCRNGR_MUL_2;
            if (r % LCRNGR_PRIME_2 == 0 && r < LCRNGR_RMAX_2)
            {
                ulong tmp = t + (k * 0x100000000);
                // backward of 2 states to get the correct 16bits low for LCRNG
                ushort low = (ushort)(((tmp / LCRNGR_MUL_2) * 0x95D9) + 0xB126);
                result[ctr++] = Prev(second | low);
            }
            k += ((LCRNGR_MUL_2 - r) + LCRNGR_SKIP1_2 - 1) / LCRNGR_SKIP1_2;
        }
        return ctr;
    }

    public static int GetSeedsIVsSkip(Span<uint> result, uint first, uint second)
    {
        uint t = (second - (first * LCRNGR_MUL_2) - LCRNGR_SUB_2) & 0x7fffffff;
        uint x = ((t * LCRNGR_PRIME_2) % LCRNGR_MUL_2);
        ulong kmax = (LCRNGR_BASE_2 - t) >> 31;

        int ctr = 0;
        for (ulong k = 0; k <= kmax;) // at most 188 iterations
        {
            ulong r = (x + (k * LCRNGR_SKIP2_2)) % LCRNGR_MUL_2;
            if (r % LCRNGR_PRIME_2 == 0 && r < LCRNGR_RMAX_2)
            {
                ulong tmp = t + (k * 0x80000000);
                ushort low = (ushort)(((tmp / LCRNGR_MUL_2) * 0x95D9) + 0xB126); // backward of 2 states to get the correct 16bits low for LCRNG
                uint s = Prev(second | low);
                result[ctr++] = s;
                result[ctr++] = s ^ 0x80000000;
            }
            k += ((LCRNGR_MUL_2 - r) + LCRNGR_SKIP2_2 - 1) / LCRNGR_SKIP2_2;
        }
        return ctr;
    }
}
