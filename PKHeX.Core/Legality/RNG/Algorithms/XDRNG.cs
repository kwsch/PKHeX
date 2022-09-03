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
    public const uint Mult  = 0x000343FD;
    public const uint Add   = 0x00269EC3;
    public const uint rMult = 0xB9B33155;
    public const uint rAdd  = 0xA170F641;
    public const uint Sub   = Add - 0xFFFF;
    public const ulong Base = (Mult + 1ul) * 0xFFFF;

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

    public const int MaxCountSeedsPID = 2;
    public const int MaxCountSeedsIV = 6;

    public static int GetSeeds(Span<uint> result, uint pid)
    {
        uint first = pid & 0xffff0000;
        uint second = pid << 16;

        return GetSeeds(result, first, second);
    }

    public static int GetSeeds(Span<uint> result, uint hp, uint atk, uint def, uint spa, uint spd, uint spe)
    {
        var first = (hp | (atk << 5) | (def << 10)) << 16;
        var second = (spe | (spa << 5) | (spd << 10)) << 16;

        return GetSeedsIVs(result, first, second);
    }

    public static int GetSeeds(Span<uint> result, uint first, uint second)
    {
        ulong t = second - (first * Mult) - Sub;
        ulong kmax = (Base - t) >> 32;

        int ctr = 0;
        for (ulong k = 0; k <= kmax; k++, t += 0x100000000) // at most 4 iterations
        {
            if (t % Mult < 0x10000)
                result[ctr++] = Prev(first | (ushort)(t / Mult));
        }
        return ctr;
    }

    public static int GetSeedsIVs(Span<uint> result, uint first, uint second)
    {
        ulong t = (second - (first * Mult) - Sub) & 0x7fffffff;
        ulong kmax = (Base - t) >> 31;

        int ctr = 0;
        for (ulong k = 0; k <= kmax; k++, t += 0x80000000) //at most 7 iterations
        {
            if (t % Mult < 0x10000)
            {
                var s = Prev(first | (ushort)(t / Mult));
                result[ctr++] = s;
                result[ctr++] = s ^ 0x8000_0000; // top bit flip
            }
        }
        return ctr;
    }
}
