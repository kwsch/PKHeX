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

    public const int MaxCountSeedsPID = 3;
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
}
