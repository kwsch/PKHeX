using System.Runtime.CompilerServices;

namespace PKHeX.Core
{
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
    public class LCRNG
    {
        // Forward
        protected readonly uint Mult;
        private readonly uint Add;

        // Reverse
        private readonly uint rMult;
        private readonly uint rAdd;

        public LCRNG(uint f_mult, uint f_add, uint r_mult, uint r_add)
        {
            Mult = f_mult;
            Add = f_add;
            rMult = r_mult;
            rAdd = r_add;
        }

        /// <summary>
        /// Advances the RNG seed to the next state value.
        /// </summary>
        /// <param name="seed">Current seed</param>
        /// <returns>Seed advanced a single time.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Next(uint seed) => (seed * Mult) + Add;

        /// <summary>
        /// Reverses the RNG seed to the previous state value.
        /// </summary>
        /// <param name="seed">Current seed</param>
        /// <returns>Seed reversed a single time.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Prev(uint seed) => (seed * rMult) + rAdd;

        /// <summary>
        /// Advances the RNG seed to the next state value a specified amount of times.
        /// </summary>
        /// <param name="seed">Current seed</param>
        /// <param name="frames">Amount of times to advance.</param>
        /// <returns>Seed advanced the specified amount of times.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Advance(uint seed, int frames)
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
        public uint Reverse(uint seed, int frames)
        {
            for (int i = 0; i < frames; i++)
                seed = Prev(seed);
            return seed;
        }
    }
}
