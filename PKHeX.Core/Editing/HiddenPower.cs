using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic for calculating a Hidden Power Type based on IVs and generation-format.
    /// </summary>
    public static class HiddenPower
    {
        /// <summary>
        /// Gets the current Hidden Power Type of the input <see cref="IVs"/> for the requested format generation.
        /// </summary>
        /// <param name="IVs">Current IVs</param>
        /// <returns>Hidden Power Type of the <see cref="IVs"/></returns>
        /// <param name="format">Generation format</param>
        public static int GetType(IReadOnlyList<int> IVs, int format)
        {
            if (format <= 2)
                return GetTypeGB(IVs);
            return GetType(IVs);
        }

        /// <summary>
        /// Gets the current Hidden Power Type of the input <see cref="IVs"/> for Generations 3+
        /// </summary>
        /// <param name="IVs">Current IVs</param>
        /// <returns>Hidden Power Type of the <see cref="IVs"/></returns>
        public static int GetType(IReadOnlyList<int> IVs)
        {
            int hp = 0;
            for (int i = 0; i < 6; i++)
                hp |= (IVs[i] & 1) << i;
            hp *= 0xF;
            hp /= 0x3F;
            return hp;
        }

        /// <summary>
        /// Gets the current Hidden Power Type of the input <see cref="IVs"/> for Generations 1 &amp; 2
        /// </summary>
        /// <param name="IVs">Current IVs</param>
        /// <returns>Hidden Power Type of the <see cref="IVs"/></returns>
        public static int GetTypeGB(IReadOnlyList<int> IVs)
        {
            var atk = IVs[1];
            var def = IVs[2];
            return ((atk & 3) << 2) | (def & 3);
        }

        /// <summary>
        /// Modifies the provided <see cref="IVs"/> to have the requested <see cref="hiddenPowerType"/> for Generations 1 &amp; 2
        /// </summary>
        /// <param name="hiddenPowerType">Hidden Power Type</param>
        /// <param name="IVs">Current IVs</param>
        /// <returns>True if the Hidden Power of the <see cref="IVs"/> is obtained, with or without modifications</returns>
        public static bool SetTypeGB(int hiddenPowerType, int[] IVs)
        {
            IVs[1] = (IVs[1] & ~3) | (hiddenPowerType >> 2);
            IVs[2] = (IVs[2] & ~3) | (hiddenPowerType & 3);
            return true;
        }

        /// <summary>
        /// Modifies the provided <see cref="IVs"/> to have the requested <see cref="hiddenPowerType"/>.
        /// </summary>
        /// <param name="hiddenPowerType">Hidden Power Type</param>
        /// <param name="IVs">Current IVs (6 total)</param>
        /// <param name="format">Generation format</param>
        /// <returns>True if the Hidden Power of the <see cref="IVs"/> is obtained, with or without modifications</returns>
        public static bool SetIVsForType(int hiddenPowerType, int[] IVs, int format)
        {
            if (format <= 2)
                return SetTypeGB(hiddenPowerType, IVs);
            return SetIVsForType(hiddenPowerType, IVs);
        }

        /// <summary>
        /// Sets the <see cref="IVs"/> to the requested <see cref="hpVal"/> for Generation 3+ game formats.
        /// </summary>
        /// <param name="hpVal">Hidden Power Type</param>
        /// <param name="IVs">Current IVs (6 total)</param>
        /// <returns>True if the Hidden Power of the <see cref="IVs"/> is obtained, with or without modifications</returns>
        public static bool SetIVsForType(int hpVal, int[] IVs)
        {
            if (Array.TrueForAll(IVs, z => z == 31))
            {
                SetIVs(hpVal, IVs); // Get IVs
                return true;
            }

            int current = GetType(IVs);
            if (current == hpVal)
                return true; // no mods necessary

            // Required HP type doesn't match IVs. Make currently-flawless IVs flawed.
            int[]? best = GetSuggestedHiddenPowerIVs(hpVal, IVs);
            if (best == null)
                return false; // can't force hidden power?

            // set IVs back to array
            for (int i = 0; i < IVs.Length; i++)
                IVs[i] = best[i];
            return true;
        }

        private static int[]? GetSuggestedHiddenPowerIVs(int hpVal, int[] IVs)
        {
            const int max = 31;
            var flawless = new int[IVs.Length]; // future: stackalloc
            int flawlessCount = 0;
            for (int i = 0; i < IVs.Length; i++)
            {
                if (IVs[i] == max)
                    flawless[++flawlessCount] = i;
            }

            var permutations = GetPermutations(flawless, flawlessCount);
            int flawedCount = 0; // result tracking
            int[]? best = null; // result tracking
            int[] ivs = (int[])IVs.Clone();
            foreach (var permute in permutations)
            {
                foreach (var index in permute)
                {
                    ivs[index] ^= 1;
                    if (hpVal != GetType(ivs))
                        continue;

                    int ct = ivs.Count(z => z == 31);
                    if (ct <= flawedCount)
                        break; // any further flaws are always worse

                    flawedCount = ct;
                    best = (int[])ivs.Clone();
                    break; // any further flaws are always worse
                }
                // Restore IVs for another iteration
                Buffer.BlockCopy(IVs, 0, ivs, 0, ivs.Length);
            }
            return best;
        }

        private static IEnumerable<IEnumerable<T>> GetPermutations<T>(ICollection<T> list, int length)
        {
            // https://stackoverflow.com/a/10630026
            if ((uint)length <= 1)
                return list.Select(t => new[] { t });

            return GetPermutations(list, length - 1)
                .SelectMany(list.Except, (t1, t2) => t1.Concat(new[] { t2 }));
        }

        /// <summary>Calculate the Hidden Power Type of the entered IVs.</summary>
        /// <param name="type">Hidden Power Type</param>
        /// <param name="ivs">Individual Values (H/A/B/S/C/D)</param>
        /// <param name="format">Generation specific format</param>
        /// <returns>Hidden Power Type</returns>
        public static int[] SetIVs(int type, int[] ivs, int format = PKX.Generation)
        {
            if (format <= 2)
            {
                ivs[1] = (ivs[1] & ~3) | (type >> 2);
                ivs[2] = (ivs[2] & ~3) | (type & 3);
                return ivs;
            }

            var bits = DefaultLowBits[type];
            for (int i = 0; i < 6; i++)
                ivs[i] = (ivs[i] & 0x1E) + ((bits >> i) & 1);
            return ivs;
        }

        /// <summary>
        /// Hidden Power IV values (even or odd) to achieve a specified Hidden Power Type
        /// </summary>
        /// <remarks>
        /// There are other IV combinations to achieve the same Hidden Power Type.
        /// These are just precomputed for fast modification.
        /// Individual Values (H/A/B/S/C/D)
        /// </remarks>
        public static readonly byte[] DefaultLowBits =
        {
            0b000011, // Fighting
            0b001000, // Flying
            0b001011, // Poison
            0b001111, // Ground
            0b010011, // Rock
            0b011001, // Bug
            0b011101, // Ghost
            0b011111, // Steel
            0b100101, // Fire
            0b101001, // Water
            0b101101, // Grass
            0b101111, // Electric
            0b110101, // Psychic
            0b111001, // Ice
            0b111101, // Dragon
            0b111111, // Dark
        };
    }
}
