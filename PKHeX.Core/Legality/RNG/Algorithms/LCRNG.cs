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

    private const uint Mult2  = unchecked(Mult * Mult);           // 0xC2A29A69
    private const uint rMult2 = unchecked(rMult * rMult);         // 0xDC6C95D9
    private const uint Add2   = unchecked(Add * (Mult + 1));      // 0xE97E7B6A
    private const uint rAdd2  = unchecked(rAdd * (rMult + 1));    // 0x4D3CB126

    private const uint  Mult3 = unchecked(Mult2 * Mult);          // 0x807DBCB5
    private const uint rMult3 = unchecked(rMult2 * rMult);        // 0xAC36519D
    private const uint  Add3  = unchecked((Add2 * Mult) + Add);   // 0x52713895
    private const uint rAdd3  = unchecked((rAdd2 * rMult) + rAdd);// 0x923B279F

    private const uint  Mult4 = unchecked(Mult3 * Mult);          // 0xEE067F11
    private const uint rMult4 = unchecked(rMult3 * rMult);        // 0xBECE51F1
    private const uint  Add4  = unchecked((Add3 * Mult) + Add);   // 0x31B0DDE4
    private const uint rAdd4  = unchecked((rAdd3 * rMult) + rAdd);// 0x7CD1F85C

    private const uint  Mult5 = unchecked(Mult4 * Mult);          // 0xEBA1483D
    private const uint rMult5 = unchecked(rMult4 * rMult);        // 0xF1C78F15
    private const uint  Add5  = unchecked((Add4 * Mult) + Add);   // 0x8E425287
    private const uint rAdd5  = unchecked((rAdd4 * rMult) + rAdd);// 0x0A84D1ED

    private const uint  Mult6 = unchecked(Mult5 * Mult);          // 0xD3DC57F9
    private const uint rMult6 = unchecked(rMult5 * rMult);        // 0x8040BA49
    private const uint  Add6  = unchecked((Add5 * Mult) + Add);   // 0xE2CCA5EE
    private const uint rAdd6  = unchecked((rAdd5 * rMult) + rAdd);// 0x2795C322

    private const uint  Mult7 = unchecked(Mult6 * Mult);          // 0x9B355305
    private const uint rMult7 = unchecked(rMult6 * rMult);        // 0x814B81CD
    private const uint  Add7  = unchecked((Add6 * Mult) + Add);   // 0xAFC58AC9
    private const uint rAdd7  = unchecked((rAdd6 * rMult) + rAdd);// 0xC1FD940B

    private const uint  Mult8 = unchecked(Mult7 * Mult);          // 0xCFDDDF21
    private const uint rMult8 = unchecked(rMult7 * rMult);        // 0xB61664E1
    private const uint  Add8  = unchecked((Add7 * Mult) + Add);   // 0x67DBB608
    private const uint rAdd8  = unchecked((rAdd7 * rMult) + rAdd);// 0x9019E2F8

    private const uint  Mult9 = unchecked(Mult8 * Mult);          // 0xFFA0F0DU
    private const uint rMult9 = unchecked(rMult8 * rMult);        // 0x7A0957C5
    private const uint  Add9  = unchecked((Add8 * Mult) + Add);   // 0xFC3351DB
    private const uint rAdd9  = unchecked((rAdd8 * rMult) + rAdd);// 0x3CFD9579

    /// <summary> Used to pre-size a result array for the maximum number of seeds that may be returned when searching for seeds via PID. </summary>
    public const int MaxCountSeedsPID = 3;
    /// <summary> Used to pre-size a result array for the maximum number of seeds that may be returned when searching for seeds via IVs. </summary>
    public const int MaxCountSeedsIV = 6;

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static uint Next (uint seed) => (seed * Mult ) + Add ;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static uint Next2(uint seed) => (seed * Mult2) + Add2;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static uint Next3(uint seed) => (seed * Mult3) + Add3;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static uint Next4(uint seed) => (seed * Mult4) + Add5;
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
    public static uint Reverse(uint seed, int frames)
    {
        for (int i = 0; i < frames; i++)
            seed = Prev(seed);
        return seed;
    }

    /// <summary>
    /// Multiplication constants for jumping 2^(index) frames forward.
    /// </summary>
    private static ReadOnlySpan<uint> JumpMult =>
	[
        0x41C64E6D, 0xC2A29A69, 0xEE067F11, 0xCFDDDF21, 0x5F748241, 0x8B2E1481, 0x76006901, 0x1711D201,
        0xBE67A401, 0xDDDF4801, 0x3FFE9001, 0x90FD2001, 0x65FA4001, 0xDBF48001, 0xF7E90001, 0xEFD20001,
        0xDFA40001, 0xBF480001, 0x7E900001, 0xFD200001, 0xFA400001, 0xF4800001, 0xE9000001, 0xD2000001,
        0xA4000001, 0x48000001, 0x90000001, 0x20000001, 0x40000001, 0x80000001, 0x00000001, 0x00000001,
    ];

    /// <summary>
    /// Addition constants for jumping 2^(index) frames forward.
    /// </summary>
    private static ReadOnlySpan<uint> JumpAdd =>
	[
        0x00006073, 0xE97E7B6A, 0x31B0DDE4, 0x67DBB608, 0xCBA72510, 0x1D29AE20, 0xBA84EC40, 0x79F01880,
        0x08793100, 0x6B566200, 0x803CC400, 0xA6B98800, 0xE6731000, 0x30E62000, 0xF1CC4000, 0x23988000,
        0x47310000, 0x8E620000, 0x1CC40000, 0x39880000, 0x73100000, 0xE6200000, 0xCC400000, 0x98800000,
        0x31000000, 0x62000000, 0xC4000000, 0x88000000, 0x10000000, 0x20000000, 0x40000000, 0x80000000,
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
