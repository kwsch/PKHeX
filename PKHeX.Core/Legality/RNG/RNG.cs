using System.Collections.Generic;

namespace PKHeX.Core
{
    public class RNG
    {
        public static readonly RNG LCRNG = new RNG(0x41C64E6D, 0x00006073, 0xEEB9EB65, 0x0A3561A1);
        public static readonly RNG XDRNG = new RNG(0x000343FD, 0x00269EC3, 0xB9B33155, 0xA170F641);
        public static readonly RNG ARNG  = new RNG(0x6C078965, 0x00000001, 0x9638806D, 0x69C77F93);

        private readonly uint Mult, Add, rMult, rAdd;

        // Bruteforce cache for searching seeds
        private const int cacheSize = 1 << 16;
        private readonly uint k2;
        private readonly byte[] low8 = new byte[cacheSize];
        private readonly bool[] flags = new bool[cacheSize];

        private RNG(uint f_mult, uint f_add, uint r_mult, uint r_add)
        {
            Mult = f_mult;
            Add = f_add;
            rMult = r_mult;
            rAdd = r_add;

            // Set up bruteforce utility
            k2 = Mult << 8;
            PopulateMeetMiddleArrays();
        }

        private void PopulateMeetMiddleArrays()
        {
            for (uint i = 0; i <= byte.MaxValue; i++)
            {
                uint right = Mult * i;
                ushort val = (ushort)(right >> 16);
                flags[val] = true;
                low8[val] = (byte)i;

                // when calculating the left side, sometimes the low bits might not carry (not considered in calc)
                // since LCGs are linear, there are no collisions if we mark the next adjacent with the prior val
                // we do this now so that the search only has to access the array once per loop.
                ++val;
                flags[val] = true;
                low8[val] = (byte)i;
            }
        }

        public uint Next(uint seed) => seed * Mult + Add;
        public uint Prev(uint seed) => seed * rMult + rAdd;

        public uint Advance(uint seed, int frames)
        {
            for (int i = 0; i < frames; i++)
                seed = Next(seed);
            return seed;
        }
        public uint Reverse(uint seed, int frames)
        {
            for (int i = 0; i < frames; i++)
                seed = Prev(seed);
            return seed;
        }

        /// <summary>
        /// Generates an IV for each RNG call using the top 5 bits of frame seeds.
        /// </summary>
        /// <param name="seed">RNG seed</param>
        /// <returns>Array of 6 IVs</returns>
        internal uint[] GetSequentialIVsUInt32(uint seed)
        {
            uint[] ivs = new uint[6];
            for (int i = 0; i < 6; i++)
            {
                seed = Next(seed);
                ivs[i] = seed >> 27;
            }
            return ivs;
        }
        internal int[] GetSequentialIVsInt32(uint seed)
        {
            int[] ivs = new int[6];
            for (int i = 0; i < 6; i++)
            {
                seed = Next(seed);
                ivs[i] = (int)(seed >> 27);
            }
            return ivs;
        }

        /// <summary>
        /// Recovers sets of lower 16 bit seeds, then returns the origin seed.
        /// </summary>
        /// <param name="first">First rand() call, 16 bits, already shifted left 16 bits.</param>
        /// <param name="second">Second rand() call, 16 bits, already shifted left 16 bits.</param>
        /// <remarks>
        /// Use a meet-in-the-middle attack to reduce the search space to 2^8 instead of 2^16
        /// flag/2^8 tables are precomputed and constant (unrelated to rand pairs)
        /// </remarks>
        /// <returns>Possible origin seeds that generate the 2 random numbers</returns>
        internal IEnumerable<uint> RecoverLower16Bits(uint first, uint second)
        {
            uint k1 = second - (first * Mult + Add);
            for (uint i = 0, k3 = k1; i <= 255; ++i, k3 -= k2)
            {
                ushort val = (ushort)(k3 >> 16);
                if (flags[val])
                    yield return Prev(first | i << 8 | low8[val]);
            }
        }
    }
}
