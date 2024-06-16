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

    private const uint Mult2  = unchecked(Mult * Mult);           // 0xA9FC6809
    private const uint rMult2 = unchecked(rMult * rMult);         // 0xE05FA639
    private const uint Add2   = unchecked(Add * (Mult + 1));      // 0x1E278E7A
    private const uint rAdd2  = unchecked(rAdd * (rMult + 1));    // 0x03882AD6

    private const uint  Mult3 = unchecked(Mult2 * Mult);          // 0x45C82BE5
    private const uint rMult3 = unchecked(rMult2 * rMult);        // 0x396E19ED
    private const uint  Add3  = unchecked((Add2 * Mult) + Add);   // 0xD2F65B55
    private const uint rAdd3  = unchecked((rAdd2 * rMult) + rAdd);// 0x777C254F

    private const uint  Mult4 = unchecked(Mult3 * Mult);          // 0xDDFF5051
    private const uint rMult4 = unchecked(rMult3 * rMult);        // 0x8A3BF8B1
    private const uint  Add4  = unchecked((Add3 * Mult) + Add);   // 0x098520C4
    private const uint rAdd4  = unchecked((rAdd3 * rMult) + rAdd);// 0x3E0A787C

    private const uint  Mult5 = unchecked(Mult4 * Mult);          // 0x284A930D
    private const uint rMult5 = unchecked(rMult4 * rMult);        // 0x2D4673C5
    private const uint  Add5  = unchecked((Add4 * Mult) + Add);   // 0xA2974C77
    private const uint rAdd5  = unchecked((rAdd4 * rMult) + rAdd);// 0x16AEB36D

    private const uint  Mult6 = unchecked(Mult5 * Mult);          // 0x0F56BAD9
    private const uint rMult6 = unchecked(rMult5 * rMult);        // 0xD44C2569
    private const uint  Add6  = unchecked((Add5 * Mult) + Add);   // 0x2E15555E
    private const uint rAdd6  = unchecked((rAdd5 * rMult) + rAdd);// 0xD4016672

    private const uint  Mult7 = unchecked(Mult6 * Mult);          // 0x0C287375
    private const uint rMult7 = unchecked(rMult6 * rMult);        // 0x19DC84DD
    private const uint  Add7  = unchecked((Add6 * Mult) + Add);   // 0x20AD96A9
    private const uint rAdd7  = unchecked((rAdd6 * rMult) + rAdd);// 0x4E39CC1B

    private const uint  Mult8 = unchecked(Mult7 * Mult);          // 0xF490B9A1
    private const uint rMult8 = unchecked(rMult7 * rMult);        // 0x672D6A61
    private const uint  Add8  = unchecked((Add7 * Mult) + Add);   // 0x7E1DBEC8
    private const uint rAdd8  = unchecked((rAdd7 * rMult) + rAdd);// 0xE493E638

    private const uint  Mult9 = unchecked(Mult8 * Mult);          // 0xC07F971D
    private const uint rMult9 = unchecked(rMult8 * rMult);        // 0x6E43E335
    private const uint  Add9  = unchecked((Add8 * Mult) + Add);   // 0xA8D2826B
    private const uint rAdd9  = unchecked((rAdd8 * rMult) + rAdd);// 0x46C51ED9

    private const uint rMult10 = unchecked(rMult9 * rMult);        // 0xC6169599
    private const uint rAdd10  = unchecked((rAdd9 * rMult) + rAdd);// 0x3E86BD4E

    private const uint rMult11 = unchecked(rMult10 * rMult);
    private const uint rAdd11  = unchecked((rAdd10 * rMult) + rAdd);

    private const uint rMult12 = unchecked(rMult11 * rMult);
    private const uint rAdd12  = unchecked((rAdd11 * rMult) + rAdd);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static uint Next (uint seed) => (seed * Mult ) + Add ;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static uint Next2(uint seed) => (seed * Mult2) + Add2;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static uint Next3(uint seed) => (seed * Mult3) + Add3;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static uint Next4(uint seed) => (seed * Mult4) + Add4;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static uint Next5(uint seed) => (seed * Mult5) + Add5;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static uint Next6(uint seed) => (seed * Mult6) + Add6;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static uint Next7(uint seed) => (seed * Mult7) + Add7;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static uint Next8(uint seed) => (seed * Mult8) + Add8;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static uint Next9(uint seed) => (seed * Mult9) + Add9;

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static uint Prev (uint seed) => (seed * rMult ) + rAdd ;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static uint Prev2(uint seed) => (seed * rMult2) + rAdd2;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static uint Prev3(uint seed) => (seed * rMult3) + rAdd3;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static uint Prev4(uint seed) => (seed * rMult4) + rAdd4;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static uint Prev5(uint seed) => (seed * rMult5) + rAdd5;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static uint Prev6(uint seed) => (seed * rMult6) + rAdd6;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static uint Prev7(uint seed) => (seed * rMult7) + rAdd7;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static uint Prev8(uint seed) => (seed * rMult8) + rAdd8;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static uint Prev9(uint seed) => (seed * rMult9) + rAdd9;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static uint Prev10(uint seed) => (seed * rMult10) + rAdd10;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static uint Prev11(uint seed) => (seed * rMult11) + rAdd11;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static uint Prev12(uint seed) => (seed * rMult12) + rAdd12;

    /// <summary>
    /// Gets the next 16 bits of the next RNG seed.
    /// </summary>
    /// <param name="seed">Seed to advance one step.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Next16(ref uint seed)
    {
        seed = Next(seed);
        return seed >> 16;
    }

    /// <summary>
    /// Gets the next 16 bits of the next RNG seed.
    /// </summary>
    /// <param name="seed">Seed to advance one step.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Next15(ref uint seed)
    {
        seed = Next(seed);
        return (seed >> 16) & 0x7FFF;
    }

    /// <summary>
    /// Gets the previous 16 bits of the previous RNG seed.
    /// </summary>
    /// <param name="seed">Seed to advance one step.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Prev16(ref uint seed)
    {
        seed = Prev(seed);
        return seed >> 16;
    }

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

    /// <summary>
    /// Generates an IV for each RNG call using the top 5 bits of frame seeds.
    /// </summary>
    /// <param name="seed">RNG seed</param>
    /// <returns>Combined IVs as <see cref="uint"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static uint GetSequentialIVsInt32(uint seed)
    {
        var result = 0u;
        for (int i = 0; i < 6; i++)
        {
            seed = Next(seed);
            var shift = 27 - (i * 5);
            result |= (seed >> shift);
        }
        return result;
    }

    // By abusing the innate properties of a LCG, we can calculate the seed from a known result.
    // https://crypto.stackexchange.com/questions/10608/how-to-attack-a-fixed-lcg-with-partial-output/10629#10629
    // Unlike our LCRNG implementation, `k` is small enough (max = 7).
    // Instead of using yield and iterators, we calculate all results in a tight loop and return the count found.
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

    /// <summary>
    /// Multiplication constants for jumping 2^(index) frames forward.
    /// </summary>
    private static ReadOnlySpan<uint> JumpMult =>
	[
        0x000343FD, 0xA9FC6809, 0xDDFF5051, 0xF490B9A1, 0x43BA1741, 0xD290BE81, 0x82E3BD01, 0xBF507A01,
        0xF8C4F401, 0x7A19E801, 0x1673D001, 0xB5E7A001, 0x8FCF4001, 0xAF9E8001, 0x9F3D0001, 0x3E7A0001,
        0x7CF40001, 0xF9E80001, 0xF3D00001, 0xE7A00001, 0xCF400001, 0x9E800001, 0x3D000001, 0x7A000001,
        0xF4000001, 0xE8000001, 0xD0000001, 0xA0000001, 0x40000001, 0x80000001, 0x00000001, 0x00000001,
    ];

    /// <summary>
    /// Addition constants for jumping 2^(index) frames forward.
    /// </summary>
    private static ReadOnlySpan<uint> JumpAdd =>
	[
        0x00269EC3, 0x1E278E7A, 0x098520C4, 0x7E1DBEC8, 0x3E314290, 0x824E1920, 0x844E8240, 0xFD864480,
        0xDFB18900, 0xD9F71200, 0x5E3E2400, 0x65BC4800, 0x70789000, 0x74F12000, 0x39E24000, 0xB3C48000,
        0x67890000, 0xCF120000, 0x9E240000, 0x3C480000, 0x78900000, 0xF1200000, 0xE2400000, 0xC4800000,
        0x89000000, 0x12000000, 0x24000000, 0x48000000, 0x90000000, 0x20000000, 0x40000000, 0x80000000,
    ];

    /// <summary>
    /// Computes the amount of advances (distance) between two seeds.
    /// </summary>
    /// <param name="start">Initial seed</param>
    /// <param name="end">Final seed</param>
    /// <returns>Count of advances from <see cref="start"/> to arrive at <see cref="end"/>.</returns>
    /// <remarks>
    /// To compute the distance, we abuse the fact that a given state bit at index `i` has a periodicity of `2^i`.
    /// If the bit is present in the state, we must include that bit in our distance result.
    /// The algorithmic complexity is O(log(n)) for finding n advancements.
    /// We store a precomputed table of multiply &amp; addition constants (skip 2^n) to avoid computing them on the fly.
    /// </remarks>
    public static uint GetDistance(in uint start, in uint end)
    {
        int i = 0;
        uint bit = 1u;

        uint distance = 0u;
        uint seed = start;

        // Instead of doing a for loop which always does 32 iterations, check to see if we end up at the end seed.
        // If we do, we can return after [0..31] jumps.
        // Due to the inputs, we normally have low distance, so normally this won't take more than a few loops.
        while (seed != end)
        {
            // 50:50 odds of this being true.
            if (((seed ^ end) & bit) != 0)
            {
                seed = (seed * JumpMult[i]) + JumpAdd[i];
                distance |= bit;
            }
            i++;
            bit <<= 1;
        }
        return distance;
    }
}
