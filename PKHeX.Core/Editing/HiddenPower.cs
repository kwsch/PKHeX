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
            var flawless = IVs.Select((v, i) => v == 31 ? i : -1).Where(v => v != -1).ToArray();
            var permutations = GetPermutations(flawless, flawless.Length);
            int flawedCount = 0;
            int[]? best = null;
            int[] ivs = (int[])IVs.Clone();
            foreach (var permute in permutations)
            {
                foreach (var item in permute)
                {
                    ivs[item] ^= 1;
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
                Array.Copy(IVs, 0, ivs, 0, ivs.Length);
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
            for (int i = 0; i < 6; i++)
                ivs[i] = (ivs[i] & 0x1E) + DefaultLowBits[type, i];
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
        public static readonly byte[,] DefaultLowBits =
        {
            { 1, 1, 0, 0, 0, 0 }, // Fighting
            { 0, 0, 0, 1, 0, 0 }, // Flying
            { 1, 1, 0, 1, 0, 0 }, // Poison
            { 1, 1, 1, 1, 0, 0 }, // Ground
            { 1, 1, 0, 0, 1, 0 }, // Rock
            { 1, 0, 0, 1, 1, 0 }, // Bug
            { 1, 0, 1, 1, 1, 0 }, // Ghost
            { 1, 1, 1, 1, 1, 0 }, // Steel
            { 1, 0, 1, 0, 0, 1 }, // Fire
            { 1, 0, 0, 1, 0, 1 }, // Water
            { 1, 0, 1, 1, 0, 1 }, // Grass
            { 1, 1, 1, 1, 0, 1 }, // Electric
            { 1, 0, 1, 0, 1, 1 }, // Psychic
            { 1, 0, 0, 1, 1, 1 }, // Ice
            { 1, 0, 1, 1, 1, 1 }, // Dragon
            { 1, 1, 1, 1, 1, 1 }, // Dark
        };
    }
}
