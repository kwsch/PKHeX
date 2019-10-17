using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PKHeX.Core
{
    public sealed class RNG
    {
        public static readonly RNG LCRNG = new RNG(0x41C64E6D, 0x00006073, 0xEEB9EB65, 0x0A3561A1);
        public static readonly RNG XDRNG = new RNG(0x000343FD, 0x00269EC3, 0xB9B33155, 0xA170F641);
        public static readonly RNG ARNG  = new RNG(0x6C078965, 0x00000001, 0x9638806D, 0x69C77F93);

        private readonly uint Mult, Add, rMult, rAdd;

        // Bruteforce cache for searching seeds
        private const int cacheSize = 1 << 16;
        // 1,2 (no gap)
        private readonly uint k2; // Mult<<8
        private readonly byte[] low8 = new byte[cacheSize];
        private readonly bool[] flags = new bool[cacheSize];
        // 1,3 (single gap)
        private readonly uint k0g; // Mult*Mult
        private readonly uint k2s; // Mult*Mult<<8
        private readonly byte[] g_low8 = new byte[cacheSize];
        private readonly bool[] g_flags = new bool[cacheSize];
        // Euclidean division approach
        private readonly long t0; // Add - 0xFFFF
        private readonly long t1; // 0xFFFF * ((long)Mult + 1)

        private RNG(uint f_mult, uint f_add, uint r_mult, uint r_add)
        {
            Mult = f_mult;
            Add = f_add;
            rMult = r_mult;
            rAdd = r_add;

            // Set up bruteforce utility
            k2 = Mult << 8;
            k0g = Mult * Mult;
            k2s = k0g << 8;
            PopulateMeetMiddleArrays();
            t0 = Add - 0xFFFF;
            t1 = 0xFFFF * ((long) Mult + 1);
        }

        private void PopulateMeetMiddleArrays()
        {
            uint k4g = Add * (Mult + 1); // 1,3's multiplier
            for (uint i = 0; i <= byte.MaxValue; i++)
            {
                SetFlagData(i, Mult, Add, flags, low8); // 1,2
                SetFlagData(i, k0g,  k4g, g_flags, g_low8); // 1,3
            }
        }

        private static void SetFlagData(uint i, uint mult, uint add, bool[] f, byte[] v)
        {
            // the second rand() also has 16 bits that aren't known. It is a 16 bit value added to either side.
            // to consider these bits and their impact, they can at most increment/decrement the result by 1.
            // with the current calc setup, the search loop's calculated value may be -1 (loop does subtraction)
            // since LCGs are linear (hence the name), there's no values in adjacent cells. (no collisions)
            // if we mark the prior adjacent cell, we eliminate the need to check flags twice on each loop.
            uint right = (mult * i) + add;
            ushort val = (ushort) (right >> 16);

            f[val] = true; v[val] = (byte)i;
            --val;
            f[val] = true; v[val] = (byte)i;
            // now the search only has to access the flags array once per loop.
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Next(uint seed) => (seed * Mult) + Add;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Prev(uint seed) => (seed * rMult) + rAdd;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Advance(uint seed, int frames)
        {
            for (int i = 0; i < frames; i++)
                seed = Next(seed);
            return seed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        /// Gets the origin seeds for two successive 16 bit rand() calls using a meet-in-the-middle approach.
        /// </summary>
        /// <param name="first">First rand() call, 16 bits, already shifted left 16 bits.</param>
        /// <param name="second">Second rand() call, 16 bits, already shifted left 16 bits.</param>
        /// <remarks>
        /// Use a meet-in-the-middle attack to reduce the search space to 2^8 instead of 2^16
        /// flag/2^8 tables are precomputed and constant (unrelated to rand pairs)
        /// https://crypto.stackexchange.com/a/10609
        /// </remarks>
        /// <returns>Possible origin seeds that generate the 2 random numbers</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal IEnumerable<uint> RecoverLower16Bits(uint first, uint second)
        {
            uint k1 = second - (first * Mult);
            for (uint i = 0, k3 = k1; i <= 255; ++i, k3 -= k2)
            {
                ushort val = (ushort)(k3 >> 16);
                if (flags[val])
                    yield return Prev(first | i << 8 | low8[val]);
            }
        }

        /// <summary>
        /// Gets the origin seeds for two 16 bit rand() calls (ignoring a rand() in between) using a meet-in-the-middle approach.
        /// </summary>
        /// <param name="first">First rand() call, 16 bits, already shifted left 16 bits.</param>
        /// <param name="third">Third rand() call, 16 bits, already shifted left 16 bits.</param>
        /// <remarks>
        /// Use a meet-in-the-middle attack to reduce the search space to 2^8 instead of 2^16
        /// flag/2^8 tables are precomputed and constant (unrelated to rand pairs)
        /// https://crypto.stackexchange.com/a/10609
        /// </remarks>
        /// <returns>Possible origin seeds that generate the 2 random numbers</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal IEnumerable<uint> RecoverLower16BitsGap(uint first, uint third)
        {
            uint k1 = third - (first * k0g);
            for (uint i = 0, k3 = k1; i <= 255; ++i, k3 -= k2s)
            {
                ushort val = (ushort)(k3 >> 16);
                if (g_flags[val])
                    yield return Prev(first | i << 8 | g_low8[val]);
            }
        }

        /// <summary>
        /// Gets the origin seeds for two successive 16 bit rand() calls using a Euclidean division approach.
        /// </summary>
        /// <param name="first">First rand() call, 16 bits, already shifted left 16 bits.</param>
        /// <param name="second">Second rand() call, 16 bits, already shifted left 16 bits.</param>
        /// <remarks>
        /// For favorable multiplier values, this k_max gives a search space less than 2^8 (meet-in-the-middle)
        /// For the programmed methods in this program, it is only advantageous to use this with <see cref="XDRNG"/>.
        /// https://crypto.stackexchange.com/a/10629
        /// </remarks>
        /// <returns>Possible origin seeds that generate the 2 random numbers</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal IEnumerable<uint> RecoverLower16BitsEuclid16(uint first, uint second)
        {
            const int bitshift = 32;
            const long inc = 1L << bitshift;
            return GetPossibleSeedsEuclid(first, second, bitshift, inc);
        }

        /// <summary>
        /// Gets the origin seeds for two successive 15 bit rand() calls using a Euclidean division approach.
        /// </summary>
        /// <param name="first">First rand() call, 15 bits, already shifted left 16 bits.</param>
        /// <param name="second">Second rand() call, 15 bits, already shifted left 16 bits.</param>
        /// <remarks>
        /// Calculate the quotient of the Euclidean division (k_max) attack to reduce the search space.
        /// For favorable multiplier values, this k_max gives a search space less than 2^8 (meet-in-the-middle)
        /// For the programmed methods in this program, it is only advantageous to use this with <see cref="XDRNG"/>.
        /// https://crypto.stackexchange.com/a/10629
        /// </remarks>
        /// <returns>Possible origin seeds that generate the 2 random numbers</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal IEnumerable<uint> RecoverLower16BitsEuclid15(uint first, uint second)
        {
            const int bitshift = 31;
            const long inc = 1L << bitshift;
            return GetPossibleSeedsEuclid(first, second, bitshift, inc);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IEnumerable<uint> GetPossibleSeedsEuclid(uint first, uint second, int bitshift, long inc)
        {
            long t = second - (Mult * first) - t0;
            long kmax = (((t1 - t) >> bitshift) << bitshift) + t;
            for (long k = t; k <= kmax; k += inc)
            {
                // compute modulo in steps for reuse in yielded value (x % Mult)
                long fix = k / Mult;
                long remainder = k - (Mult * fix);
                if (remainder >> 16 == 0)
                    yield return Prev(first | (uint) fix);
            }
        }
    }

    public enum RNGType
    {
        None,
        LCRNG,
        XDRNG,
        ARNG,
    }

    public static class RNGTypeUtil
    {
        public static RNG GetRNG(this RNGType type)
        {
            return type switch
            {
                RNGType.LCRNG => RNG.LCRNG,
                RNGType.XDRNG => RNG.XDRNG,
                RNGType.ARNG => RNG.ARNG,
                _ => throw new ArgumentException(nameof(type))
            };
        }
    }
}
