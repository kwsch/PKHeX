using System;

namespace PKHeX.Core;

public static partial class Util
{
    /// <inheritdoc cref="Random.Shared"/>
    public static Random Rand => Random.Shared;

    /// <inheritdoc cref="Rand32(Random)"/>
    /// <remarks>Uses <see cref="Random.Shared"/> to generate the random number.</remarks>
    public static uint Rand32() => Rand.Rand32();

    extension(Random rnd)
    {
        /// <summary>
        /// Generates a random 32-bit unsigned integer.
        /// </summary>
        /// <returns>A random 32-bit unsigned integer.</returns>
        public uint Rand32() => (uint)rnd.NextInt64();

        /// <summary>
        /// Generates a 64-bit unsigned random number by combining two 32-bit random values.
        /// </summary>
        /// <remarks>
        /// This method extends the <see cref="Random"/> class to provide a 64-bit random number by  combining the results of two 32-bit random number generations.
        /// The lower 32 bits are  derived from one call to <c>Rand32</c>, and the upper 32 bits are derived from another call.
        /// </remarks>
        /// <returns>A 64-bit unsigned integer representing the combined random value.</returns>
        public ulong Rand64() => rnd.Rand32() | ((ulong)rnd.Rand32() << 32);
    }
}
