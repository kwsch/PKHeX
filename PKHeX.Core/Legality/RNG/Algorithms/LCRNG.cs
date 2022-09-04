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
    internal const uint Mult2 = unchecked(Mult * Mult);
    internal const uint Add2 = unchecked(Add * (Mult + 1));
    internal const uint rMult2 = unchecked(rMult * rMult);
    internal const uint rAdd2 = unchecked(rAdd * (rMult + 1));

    public const int MaxCountSeedsPID = 3;
    public const int MaxCountSeedsIV = 6;

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
    /// Reverses the RNG seed backward 2 steps.
    /// </summary>
    /// <param name="seed">Current seed</param>
    /// <returns>Seed reversed twice.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Prev2(uint seed) => (seed * rMult2) + rAdd2;

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
