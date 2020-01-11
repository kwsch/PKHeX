using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic for applying ribbons.
    /// </summary>
    public static class RibbonApplicator
    {
        private static List<string> GetAllRibbonNames(PKM pkm) => RibbonInfo.GetRibbonInfo(pkm).Select(z => z.Name).ToList();

        /// <summary>
        /// Gets a list of valid ribbons for the <see cref="pkm"/>.
        /// </summary>
        /// <param name="pkm">Entity to fetch the list for.</param>
        /// <param name="allRibbons">All ribbon names.</param>
        /// <returns>List of all valid ribbon names.</returns>
        public static IReadOnlyList<string> GetValidRibbons(PKM pkm, IList<string> allRibbons)
        {
            var pk = pkm.Clone();
            return SetAllValidRibbons(allRibbons, pk);
        }

        /// <summary>
        /// Gets a list of valid ribbons for the <see cref="pkm"/>.
        /// </summary>
        /// <param name="pkm">Entity to fetch the list for.</param>
        /// <returns>List of all valid ribbon names.</returns>
        public static IReadOnlyList<string> GetValidRibbons(PKM pkm)
        {
            var names = GetAllRibbonNames(pkm);
            return GetValidRibbons(pkm, names);
        }

        /// <summary>
        /// Gets a list of valid ribbons for the <see cref="pkm"/> that can be removed.
        /// </summary>
        /// <param name="pkm">Entity to fetch the list for.</param>
        /// <param name="allRibbons">All ribbon names.</param>
        /// <returns>List of all removable ribbon names.</returns>
        public static IReadOnlyList<string> GetRemovableRibbons(PKM pkm, IList<string> allRibbons)
        {
            var pk = pkm.Clone();
            return RemoveAllValidRibbons(allRibbons, pk);
        }

        /// <summary>
        /// Gets a list of valid ribbons for the <see cref="pkm"/> that can be removed.
        /// </summary>
        /// <param name="pkm">Entity to fetch the list for.</param>
        /// <returns>List of all removable ribbon names.</returns>
        public static IReadOnlyList<string> GetRemovableRibbons(PKM pkm)
        {
            var names = GetAllRibbonNames(pkm);
            return GetRemovableRibbons(pkm, names);
        }

        /// <summary>
        /// Sets all valid ribbons to the <see cref="pkm"/>.
        /// </summary>
        /// <param name="pkm">Entity to set ribbons for.</param>
        /// <returns>True if any ribbons were applied.</returns>
        public static bool SetAllValidRibbons(PKM pkm)
        {
            var ribNames = GetAllRibbonNames(pkm);
            ribNames.RemoveAll(z => z.StartsWith("RibbonMark")); // until marking legality is handled
            return SetAllValidRibbons(pkm, ribNames);
        }

        /// <summary>
        /// Sets all valid ribbons to the <see cref="pkm"/>.
        /// </summary>
        /// <param name="pkm">Entity to set ribbons for.</param>
        /// <param name="ribNames">Ribbon names to try setting.</param>
        /// <returns>True if any ribbons were applied.</returns>
        public static bool SetAllValidRibbons(PKM pkm, List<string> ribNames)
        {
            var list = SetAllValidRibbons(ribNames, pkm);
            return list.Count != 0;
        }

        private static IReadOnlyList<string> SetAllValidRibbons(IList<string> allRibbons, PKM pk)
        {
            var la = new LegalityAnalysis(pk);
            var valid = new List<string>();

            while (TryApplyAllRibbons(pk, la, allRibbons, valid) != 0)
            {
                // Repeat the operation until no more ribbons are set.
            }

            return valid;
        }

        /// <summary>
        /// Sets all valid ribbons to the <see cref="pkm"/>.
        /// </summary>
        /// <param name="pkm">Entity to set ribbons for.</param>
        /// <returns>True if any ribbons were removed.</returns>
        public static bool RemoveAllValidRibbons(PKM pkm)
        {
            var ribNames = GetAllRibbonNames(pkm);
            return RemoveAllValidRibbons(pkm, ribNames);
        }

        /// <summary>
        /// Sets all valid ribbons to the <see cref="pkm"/>.
        /// </summary>
        /// <param name="pkm">Entity to set ribbons for.</param>
        /// <param name="ribNames">Ribbon names to try setting.</param>
        /// <returns>True if any ribbons were removed.</returns>
        public static bool RemoveAllValidRibbons(PKM pkm, List<string> ribNames)
        {
            var list = RemoveAllValidRibbons(ribNames, pkm);
            return list.Count != 0;
        }

        private static IReadOnlyList<string> RemoveAllValidRibbons(IList<string> allRibbons, PKM pk)
        {
            var la = new LegalityAnalysis(pk);
            var valid = new List<string>();

            while (TryRemoveAllRibbons(pk, la, allRibbons, valid) != 0)
            {
                // Repeat the operation until no more ribbons are set.
            }

            return valid;
        }

        private static int TryApplyAllRibbons(PKM pk, LegalityAnalysis la, IList<string> allRibbons, ICollection<string> valid)
        {
            int applied = 0;
            for (int i = 0; i < allRibbons.Count;)
            {
                la.ResetParse();
                var rib = allRibbons[i];
                var success = TryApplyRibbon(pk, la, rib);
                if (success)
                {
                    ++applied;
                    allRibbons.RemoveAt(i);
                    valid.Add(rib);
                }
                else
                {
                    RemoveRibbon(pk, rib);
                    ++i;
                }
            }

            return applied;
        }

        private static int TryRemoveAllRibbons(PKM pk, LegalityAnalysis la, IList<string> allRibbons, ICollection<string> valid)
        {
            int removed = 0;
            for (int i = 0; i < allRibbons.Count;)
            {
                la.ResetParse();
                var rib = allRibbons[i];
                var success = TryRemoveRibbon(pk, la, rib);
                if (success)
                {
                    ++removed;
                    allRibbons.RemoveAt(i);
                    valid.Add(rib);
                }
                else
                {
                    SetRibbonValue(pk, rib, 1);
                    ++i;
                }
            }

            return removed;
        }

        private static void RemoveRibbon(PKM pk, string rib) => SetRibbonValue(pk, rib, 0);

        private static bool TryRemoveRibbon(PKM pk, LegalityAnalysis la, string rib)
        {
            RemoveRibbon(pk, rib);
            LegalityAnalysis.Ribbon.Verify(la);
            return la.Results.All(z => z.Valid);
        }

        private static bool TryApplyRibbon(PKM pk, LegalityAnalysis la, string rib)
        {
            SetRibbonValue(pk, rib, 1);
            LegalityAnalysis.Ribbon.Verify(la);
            return la.Results.All(z => z.Valid);
        }

        private static void SetRibbonValue(PKM pk, string rib, int value)
        {
            switch (rib)
            {
                case nameof(PK7.RibbonCountMemoryBattle):
                    ReflectUtil.SetValue(pk, rib, value * (pk.Gen4 ? 6 : 8));
                    break;
                case nameof(PK7.RibbonCountMemoryContest):
                    ReflectUtil.SetValue(pk, rib, value * (pk.Gen4 ? 20 : 40));
                    break;
                default:
                    if (rib.StartsWith("RibbonCountG3"))
                        ReflectUtil.SetValue(pk, rib, value * 4);
                    else
                        ReflectUtil.SetValue(pk, rib, value != 0);
                    break;
            }
        }
    }
}