using System.Runtime.CompilerServices;

namespace PKHeX.Core;

/// <summary>
/// 31-bit linear congruential random number generator.
/// </summary>
/// <remarks>
/// Standard C/C++ standard library implementation RNG (add/multiply constants). Used in Gen3 wild encounter triggers, Gen4 Lottery, Rumble, My Pokémon Ranch.
/// <br>
/// https://en.wikipedia.org/wiki/Linear_congruential_generator
/// </br>
/// <br>
/// seed_n+1 = seed_n * <see cref="Mult"/> + <see cref="Add"/>
/// </br>
/// </remarks>
public static class MRNG
{
    public const uint Mult  = 0x41C64E6D; // Standard ISO C / POSIX implementation multiplier constant
    public const uint Add   = 0x00003039; // 12345
    public const uint rMult = 0xEEB9EB65;
    public const uint rAdd  = 0xFC77A683;

    private const uint Mult2  = unchecked(Mult * Mult);           // 0xC2A29A69
    private const uint rMult2 = unchecked(rMult * rMult);         // 0xDC6C95D9
    private const uint Add2   = unchecked(Add * (Mult + 1));      // 0xD3DC167E
    private const uint rAdd2  = unchecked(rAdd * (rMult + 1));    // 0x8C319932

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static uint Next(uint seed) => (seed * Mult) + Add;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static uint Next2(uint seed) => (seed * Mult2) + Add2;

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static uint Prev(uint seed) => (seed * rMult) + rAdd;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static uint Prev2(uint seed) => (seed * rMult2) + rAdd2;

    /// <summary>
    /// Generates IVs for a given seed.
    /// </summary>
    /// <param name="seed">Seed to use for the RNG.</param>
    /// <returns>32-bit value containing the IVs (HABSCD, low->high).</returns>
    public static uint GetSequentialIVs(uint seed)
    {
        var rand1 = Next15(ref seed);
        var rand2 = Next15(ref seed);
        var rand3 = Next15(ref seed);

        // rand2 effectively unused, but included for completeness
        return ((rand2 << 30) | (rand1 << 15) | rand3) & 0x3FFF_FFFF;
    }

    /// <summary>
    /// Gets the upper 0x7FFF bits of the next RNG seed.
    /// </summary>
    /// <param name="seed">Seed to advance one step.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Next15(ref uint seed)
    {
        seed = Next(seed);
        return (seed >> 16) & 0x7FFF;
    }
}
