using System.Runtime.CompilerServices;

namespace PKHeX.Core
{
    /// <summary>
    /// Self-mutating value that returns a crypto value to be xor-ed with another (unaligned) byte stream.
    /// <para>
    /// This implementation allows for yielding crypto bytes on demand.
    /// </para>
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "Unused")]
    public ref struct SCXorShift32
    {
        private int Counter;
        private uint Seed;

        public SCXorShift32(uint seed)
        {
#if NET5
            var pop_count = System.Numerics.BitOperations.PopCount(seed);
#else
            var pop_count = PopCount(seed);
#endif
            for (var i = 0; i < pop_count; i++)
                seed = XorshiftAdvance(seed);

            Counter = 0;
            Seed = seed;
        }

        /// <summary>
        /// Gets a <see cref="byte"/> from the current state.
        /// </summary>
        public uint Next()
        {
            var c = Counter;
            var val = (Seed >> (c << 3)) & 0xFF;
            if (c == 3)
            {
                Seed = XorshiftAdvance(Seed);
                Counter = 0;
            }
            else
            {
                ++Counter;
            }
            return val;
        }

        /// <summary>
        /// Gets a <see cref="uint"/> from the current state.
        /// </summary>
        public uint Next32()
        {
            return Next() | (Next() << 8) | (Next() << 16) | (Next() << 24);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint XorshiftAdvance(uint key)
        {
            key ^= key << 2;
            key ^= key >> 15;
            key ^= key << 13;
            return key;
        }

#if !NET5
        /// <summary>
        /// Count of bits set in value
        /// </summary>
        private static uint PopCount(uint x)
        {
            x -= ((x >> 1) & 0x55555555u);
            x = (x & 0x33333333u) + ((x >> 2) & 0x33333333u);
            x = (x + (x >> 4)) & 0x0F0F0F0Fu;
            x += (x >> 8);
            x += (x >> 16);
            return x & 0x0000003Fu;
        }
#endif
    }
}
