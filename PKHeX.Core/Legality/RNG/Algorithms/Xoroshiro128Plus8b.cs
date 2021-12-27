using System.Runtime.CompilerServices;

namespace PKHeX.Core
{
    /// <summary>
    /// Self-modifying RNG structure that implements xoroshiro128+ which split-mixes the initial seed to populate all 128-bits of the initial state, rather than using a fixed 64-bit half.
    /// </summary>
    /// <remarks>https://en.wikipedia.org/wiki/Xoroshiro128%2B</remarks>
    /// <seealso cref="Xoroshiro128Plus"/>
    public ref struct Xoroshiro128Plus8b
    {
        private ulong s0, s1;

        public Xoroshiro128Plus8b(ulong seed)
        {
            var _s0 = seed - 0x61C8864680B583EB;
            var _s1 = seed + 0x3C6EF372FE94F82A;

            _s0 = 0xBF58476D1CE4E5B9 * (_s0 ^ (_s0 >> 30));
            _s1 = 0xBF58476D1CE4E5B9 * (_s1 ^ (_s1 >> 30));

            _s0 = 0x94D049BB133111EB * (_s0 ^ (_s0 >> 27));
            _s1 = 0x94D049BB133111EB * (_s1 ^ (_s1 >> 27));

            s0 = _s0 ^ (_s0 >> 31);
            s1 = _s1 ^ (_s1 >> 31);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong RotateLeft(ulong x, int k)
        {
            return (x << k) | (x >> (64 - k));
        }

        /// <summary>
        /// Gets the next random <see cref="ulong"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong Next()
        {
            var _s0 = s0;
            var _s1 = s1;
            ulong result = _s0 + _s1;

            _s1 ^= _s0;
            // Final calculations and store back to fields
            s0 = RotateLeft(_s0, 24) ^ _s1 ^ (_s1 << 16);
            s1 = RotateLeft(_s1, 37);

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool NextBool() => (Next() >> 63) != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte NextByte() => (byte)(Next() >> 56);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort UShort() => (ushort)(Next() >> 48);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint NextUInt() => (uint)(Next() >> 32);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint NextUInt(uint max)
        {
            var rnd = NextUInt();
            return rnd - ((rnd / max) * max);
        }
    }
}
