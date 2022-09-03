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

    // By abusing the innate properties of a LCG, we can calculate the seed from a known result.
    // https://crypto.stackexchange.com/questions/10608/how-to-attack-a-fixed-lcg-with-partial-output/10629#10629
    // https://github.com/StarfBerry/poke-scripts/blob/ebd5db0e8a48eb9fceaaebc4a8d907e88839ddcb/RNG/Euclid.py
    // Using a Prime and Skip constant, we can avoid most values of `k` that do not produce a valid seed.
    // Other ways to attack include a 2^8 time complexity meet-in-the-middle approach...
    // However, the euclidean division approach here (forward/reverse + prime skips) is better performing.
    // Instead of using yield and iterators, we calculate all results in a tight loop and return the count found.
    // More discussion in the PokemonRNG Discord server: https://discord.com/channels/285269328469950464/551605684815265824/1015422870299611226
    public const int MaxCountSeedsPID = 3;
    public const int MaxCountSeedsIV = 6;

    // Euclidean division constants
    private const uint LCRNG_MUL = Mult;
    private const uint LCRNG_SUB = unchecked(Add - 0xFFFF); // 0xFFFF6074
    private const ulong LCRNG_BASE = (Mult + 1ul) * 0xFFFF; // 0x41C60CA7B192
    private const uint LCRNG_PRIME = 0x25; // ideal prime skip
    private const uint LCRNG_RMAX = 0x250000; // prime * 0x10000
    private const uint LCRNG_SKIP1 = 0x73E2B0; // ??
    private const uint LCRNG_SKIP2 = 0x39F158; // ??

    // LCRNGR gives better skips than LCRNG when the second rand result is skipped (ex/ ACDE and ABCE).
    private const uint LCRNGR_MUL_2 = unchecked(rMult * rMult); // 0xDC6C95D9
    private const uint LCRNGR_SUB_2 = unchecked(rAdd * (1 + rMult)) - 0xFFFF; // 0x4D3BB127
    private const ulong LCRNGR_BASE_2 = (LCRNGR_MUL_2 + 1ul) * 0xFFFF; // 0xDC6BB96D6A26
    private const uint LCRNGR_PRIME_2 = 0x1F; // ideal prime skip
    private const uint LCRNGR_RMAX_2 = 0x1F0000; // prime * 0x10000
    private const uint LCRNGR_SKIP1_2 = 0xBAED7C; // ??
    private const uint LCRNGR_SKIP2_2 = 0x5D76BE; // ??
    private const ushort LCRNG_MUL_2_16 = unchecked((ushort)LCRNGR_MUL_2);
    private const ushort LCRNG_ADD_2_16 = 0xB126; // ??

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
        ulong t = second - (first * LCRNG_MUL) - LCRNG_SUB;
        ulong x = (t * LCRNG_PRIME) % LCRNG_MUL; // 32bit, but keep as 64bit
        ulong kmax = (LCRNG_BASE - t) >> 32;

        int ctr = 0;
        for (ulong k = 0; k <= kmax;) // at most 117 iterations
        {
            ulong r = (x + (k * LCRNG_SKIP1)) % LCRNG_MUL;
            if (r % LCRNG_PRIME == 0 && r < LCRNG_RMAX)
            {
                ulong tmp = t + (k * 0x1_0000_0000);
                ushort low = (ushort)(tmp / LCRNG_MUL);
                result[ctr++] = Prev(first | low);
            }
            k += ((LCRNG_MUL - r) + LCRNG_SKIP1 - 1) / LCRNG_SKIP1;
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
        ulong t = (second - (first * LCRNG_MUL) - LCRNG_SUB) & 0x7FFF_FFFF;
        ulong x = (t * LCRNG_PRIME) % LCRNG_MUL; // 32bit, but keep as 64bit
        ulong kmax = (LCRNG_BASE - t) >> 31;

        int ctr = 0;
        for (ulong k = 0; k <= kmax;) // at most 117 iterations
        {
            ulong r = (x + (k * LCRNG_SKIP2)) % LCRNG_MUL;
            if (r % LCRNG_PRIME == 0 && r < LCRNG_RMAX)
            {
                ulong tmp = t + (k * 0x8000_0000);
                ushort low = (ushort)(tmp / LCRNG_MUL);
                uint s = Prev(first | low);
                result[ctr++] = s;
                result[ctr++] = s ^ 0x8000_0000;
            }
            k += ((LCRNG_MUL - r) + LCRNG_SKIP2 - 1) / LCRNG_SKIP2;
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
        ulong t = first - (third * LCRNGR_MUL_2) - LCRNGR_SUB_2;
        ulong x = (t * LCRNGR_PRIME_2) % LCRNGR_MUL_2; // 32bit, but keep as 64bit
        ulong kmax = (LCRNGR_BASE_2 - t) >> 32;

        int ctr = 0;
        for (ulong k = 0; k <= kmax;) // at most 188 iterations
        {
            ulong r = (x + (LCRNGR_SKIP1_2 * k)) % LCRNGR_MUL_2;
            if (r % LCRNGR_PRIME_2 == 0 && r < LCRNGR_RMAX_2)
            {
                ulong tmp = t + (k * 0x1_0000_0000);
                // backward of 2 states to get the correct 16bits low for LCRNG
                ushort low = (ushort)(((tmp / LCRNGR_MUL_2) * LCRNG_MUL_2_16) + LCRNG_ADD_2_16);
                result[ctr++] = Prev(first | low);
            }
            k += ((LCRNGR_MUL_2 - r) + LCRNGR_SKIP1_2 - 1) / LCRNGR_SKIP1_2;
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
        ulong t = (first - (third * LCRNGR_MUL_2) - LCRNGR_SUB_2) & 0x7FFF_FFFF;
        ulong x = ((t * LCRNGR_PRIME_2) % LCRNGR_MUL_2); // 32bit, but keep as 64bit
        ulong kmax = (LCRNGR_BASE_2 - t) >> 31;

        int ctr = 0;
        for (ulong k = 0; k <= kmax;) // at most 188 iterations
        {
            ulong r = (x + (k * LCRNGR_SKIP2_2)) % LCRNGR_MUL_2;
            if (r % LCRNGR_PRIME_2 == 0 && r < LCRNGR_RMAX_2)
            {
                ulong tmp = t + (k * 0x8000_0000);
                // backward of 2 states to get the correct 16bits low for LCRNG
                ushort low = (ushort)(((tmp / LCRNGR_MUL_2) * LCRNG_MUL_2_16) + LCRNG_ADD_2_16);
                uint s = Prev(first | low);
                result[ctr++] = s;
                result[ctr++] = s ^ 0x8000_0000;
            }
            k += ((LCRNGR_MUL_2 - r) + LCRNGR_SKIP2_2 - 1) / LCRNGR_SKIP2_2;
        }
        return ctr;
    }
}
