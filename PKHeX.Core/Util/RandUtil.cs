using System;

namespace PKHeX.Core;

public static partial class Util
{
    /// <inheritdoc cref="Random.Shared"/>
    public static Random Rand => Random.Shared;

    /// <inheritdoc cref="Rand32(Random)"/>
    /// <remarks>Uses <see cref="Random.Shared"/> to generate the random number.</remarks>
    public static uint Rand32() => Rand32(Rand);

    /// <summary>
    /// Generates a random 32-bit unsigned integer.
    /// </summary>
    /// <param name="rnd">The <see cref="Random"/> instance used to generate the random number.</param>
    /// <returns>A random 32-bit unsigned integer.</returns>
    public static uint Rand32(this Random rnd) => (uint)rnd.NextInt64();

    /// <summary>
    /// Generates a 64-bit unsigned random number by combining two 32-bit random values.
    /// </summary>
    /// <remarks>
    /// This method extends the <see cref="Random"/> class to provide a 64-bit random number by  combining the results of two 32-bit random number generations.
    /// The lower 32 bits are  derived from one call to <c>Rand32</c>, and the upper 32 bits are derived from another call.
    /// </remarks>
    /// <param name="rnd">The <see cref="Random"/> instance used to generate the random values.</param>
    /// <returns>A 64-bit unsigned integer representing the combined random value.</returns>
    public static ulong Rand64(this Random rnd) => rnd.Rand32() | ((ulong)rnd.Rand32() << 32);
}
