using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public static class HiddenPower
    {
        public static int GetType(IReadOnlyList<int> IVs, int format)
        {
            if (format <= 2)
                return GetTypeGB(IVs);
            return GetType(IVs);
        }

        public static int GetType(IReadOnlyList<int> IVs)
        {
            int hp = 0;
            for (int i = 0; i < 6; i++)
                hp |= (IVs[i] & 1) << i;
            hp *= 0xF;
            hp /= 0x3F;
            return hp;
        }

        public static int GetTypeGB(IReadOnlyList<int> IVs)
        {
            var IV_ATK = IVs[1];
            var IV_DEF = IVs[2];
            return ((IV_ATK & 3) << 2) | (IV_DEF & 3);
        }

        public static bool SetIVsForType(int hpVal, int[] IVs, int format)
        {
            if (format <= 2)
            {
                IVs[1] = (IVs[1] & ~3) | (hpVal >> 2);
                IVs[2] = (IVs[2] & ~3) | (hpVal  & 3);
                return true;
            }
            return SetIVsForType(hpVal, IVs);
        }

        public static bool SetIVsForType(int hpVal, int[] IVs)
        {
            if (IVs.All(z => z == 31))
            {
                PKX.SetHPIVs(hpVal, IVs); // Get IVs
                return true;
            }

            int current = GetType(IVs);
            if (current == hpVal)
                return true; // no mods necessary

            // Required HP type doesn't match IVs. Make currently-flawless IVs flawed.
            int[] best = GetSuggestedHiddenPowerIVs(hpVal, IVs);
            if (best == null)
                return false; // can't force hidden power?

            // set IVs back to array
            for (int i = 0; i < IVs.Length; i++)
                IVs[i] = best[i];
            return true;
        }

        private static int[] GetSuggestedHiddenPowerIVs(int hpVal, int[] IVs)
        {
            var flawless = IVs.Select((v, i) => v == 31 ? i : -1).Where(v => v != -1).ToArray();
            var permutations = GetPermutations(flawless, flawless.Length);
            int flawedCount = 0;
            int[] best = null;
            foreach (var permute in permutations)
            {
                var ivs = (int[])IVs.Clone();
                foreach (var item in permute)
                {
                    ivs[item] ^= 1;
                    if (hpVal != GetType(ivs))
                        continue;

                    int ct = ivs.Count(z => z == 31);
                    if (ct <= flawedCount)
                        break; // any further flaws are always worse

                    flawedCount = ct;
                    best = ivs;
                    break; // any further flaws are always worse
                }
            }
            return best;
        }

        private static IEnumerable<IEnumerable<T>> GetPermutations<T>(ICollection<T> list, int length)
        {
            // https://stackoverflow.com/a/10630026
            if (length == 1)
                return list.Select(t => new[] { t });

            return GetPermutations(list, length - 1)
                .SelectMany(list.Except, (t1, t2) => t1.Concat(new[] { t2 }));
        }
    }
}
