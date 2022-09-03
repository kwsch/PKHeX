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
public static class XDRNG
{
    // Forward and reverse constants
    public const uint Mult  = 0x000343FD;
    public const uint Add   = 0x00269EC3;
    public const uint rMult = 0xB9B33155;
    public const uint rAdd  = 0xA170F641;

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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            var expect = seed >> 27;
            if (iv != expect)
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

    // By abusing the innate properties of a LCG, we can calculate the seed from a known result.
    // https://crypto.stackexchange.com/questions/10608/how-to-attack-a-fixed-lcg-with-partial-output/10629#10629
    // https://github.com/StarfBerry/poke-scripts/blob/ebd5db0e8a48eb9fceaaebc4a8d907e88839ddcb/RNG/Euclid.py
    // Unlike our LCRNG implementation, there's no need to utilize a Prime and Skip constant, as `k` is small enough (max = 7).
    // Instead of using yield and iterators, we calculate all results in a tight loop and return the count found.
    // More discussion in the PokemonRNG Discord server: https://discord.com/channels/285269328469950464/551605684815265824/1015422870299611226
    public const int MaxCountSeedsPID = 2;
    public const int MaxCountSeedsIV = 6;

    // Euclidean division constants
    private const uint Sub = Add - 0xFFFF;
    private const ulong Base = (Mult + 1ul) * 0xFFFF;

    /// <summary>
    /// Finds all seeds that can generate the <see cref="pid"/> by two successive rand() calls.
    /// </summary>
    /// <param name="result">Result storage array, to be populated starting at index 0.</param>
    /// <param name="pid">PID to be reversed into seeds that generate it.</param>
    /// <returns>Count of results added to <see cref="result"/></returns>
    public static int GetSeeds(Span<uint> result, uint pid)
    {
        uint first = pid & 0xFFFF_0000;
        uint second = pid << 16;
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
    public static int GetSeeds(Span<uint> result, uint hp, uint atk, uint def, uint spa, uint spd, uint spe)
    {
        var first = (hp | (atk << 5) | (def << 10)) << 16;
        var second = (spe | (spa << 5) | (spd << 10)) << 16;
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
        ulong t = second - (first * Mult) - Sub;
        ulong kmax = (Base - t) >> 32;

        int ctr = 0;
        for (ulong k = 0; k <= kmax; k++, t += 0x1_0000_0000) // at most 4 iterations
        {
            if (t % Mult < 0x1_0000)
                result[ctr++] = Prev(first | (ushort)(t / Mult));
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
        ulong t = (second - (first * Mult) - Sub) & 0x7FFF_FFFF;
        ulong kmax = (Base - t) >> 31;

        int ctr = 0;
        for (ulong k = 0; k <= kmax; k++, t += 0x8000_0000) // at most 7 iterations
        {
            if (t % Mult < 0x1_0000)
            {
                var s = Prev(first | (ushort)(t / Mult));
                result[ctr++] = s;
                result[ctr++] = s ^ 0x8000_0000; // top bit flip
            }
        }
        return ctr;
    }
}
