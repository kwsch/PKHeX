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
public static class ARNG
{
    // Forward and reverse constants
    public const uint Mult  = 0x6C078965;
    public const uint Add   = 0x00000001;
    public const uint rMult = 0x9638806D;
    public const uint rAdd  = 0x69C77F93;

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
}
