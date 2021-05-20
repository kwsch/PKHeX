using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PKHeX.Core
{
    /// <summary>
    /// <inheritdoc cref="Core.LCRNG"/>
    /// </summary>
    /// <remarks>
    /// <inheritdoc cref="Core.LCRNG"/>
    /// <br>
    /// Provides common RNG algorithms used by Generation 3 &amp; 4.
    /// This class has extra logic (tuned for performance) that can be used to find the original state(s) based on a limited amount of observed results.
    /// Refer to the documentation for those methods.
    /// </br>
    /// </remarks>
    public sealed class RNG : LCRNG
    {
        /// <summary> LCRNG used for Encryption and mainline game RNG calls. </summary>
        public static readonly RNG LCRNG = new(0x41C64E6D, 0x00006073, 0xEEB9EB65, 0x0A3561A1);

        /// <summary> LCRNG used by Colosseum & XD for game RNG calls. </summary>
        public static readonly RNG XDRNG = new(0x000343FD, 0x00269EC3, 0xB9B33155, 0xA170F641);

        /// <summary> Alternate LCRNG used by mainline game RNG calls to disassociate the seed from the <see cref="LCRNG"/>, for anti-shiny and other purposes. </summary>
        public static readonly LCRNG ARNG  = new(0x6C078965, 0x00000001, 0x9638806D, 0x69C77F93);

        #region Seed Reversal Logic

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

        #endregion

        private RNG(uint f_mult, uint f_add, uint r_mult, uint r_add) : base(f_mult, f_add, r_mult, r_add)
        {
            // Set up bruteforce utility
            k2 = f_mult << 8;
            k0g = f_mult * f_mult;
            k2s = k0g << 8;

            // Populate Meet Middle Arrays
            uint k4g = f_add * (f_mult + 1); // 1,3's multiplier
            for (uint i = 0; i <= byte.MaxValue; i++)
            {
                SetFlagData(i, f_mult, f_add, flags, low8); // 1,2
                SetFlagData(i, k0g, k4g, g_flags, g_low8); // 1,3
            }

            t0 = f_add - 0xFFFFU;
            t1 = 0xFFFFL * ((long) f_mult + 1);
        }

        #region Initialization

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        #endregion

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
}
