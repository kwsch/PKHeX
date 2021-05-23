using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Specialized Egg Generator for Gen2
    /// </summary>
    internal static class EncounterEggGenerator2
    {
        public static IEnumerable<EncounterEgg> GenerateEggs(PKM pkm, bool all = false)
        {
            var table = EvolutionTree.GetEvolutionTree(pkm, 2);
            int maxSpeciesOrigin = Legal.GetMaxSpeciesOrigin(2);
            var evos = table.GetValidPreEvolutions(pkm, maxLevel: 100, maxSpeciesOrigin: maxSpeciesOrigin, skipChecks: true);
            return GenerateEggs(pkm, evos, all);
        }

        public static IEnumerable<EncounterEgg> GenerateEggs(PKM pkm, IReadOnlyList<EvoCriteria> chain, bool all = false)
        {
            int species = pkm.Species;
            if (!Breeding.CanHatchAsEgg(species))
                yield break;

            var canBeEgg = all || GetCanBeEgg(pkm);
            if (!canBeEgg)
                yield break;

            // Gen2 was before split-breed species existed; try to ensure that the egg we try and match to can actually originate in the game.
            // Species must be < 251
            // Form must be 0 (Unown cannot breed).
            var baseID = chain[^1];
            if ((baseID.Species >= Legal.MaxSpeciesID_2 || baseID.Form != 0) && chain.Count != 1)
                baseID = chain[^2];
            if (baseID.Form != 0)
                yield break; // Forms don't exist in Gen2, besides Unown (which can't breed). Nothing can form-change.

            species = baseID.Species;
            if (species > Legal.MaxSpeciesID_2)
                yield break;
            if (GetCanBeCrystalEgg(pkm, species, all))
                yield return new EncounterEgg(species, 0, 5, 2, GameVersion.C); // gen2 egg
            yield return new EncounterEgg(species, 0, 5, 2, GameVersion.GS); // gen2 egg
        }

        private static bool GetCanBeCrystalEgg(PKM pkm, int species, bool all)
        {
            if (!ParseSettings.AllowGen2Crystal(pkm))
                return false;

            if (all)
                return true;

            // Check if the met data is present or could have been erased.
            if (pkm.Format > 2)
                return true; // doesn't have original met location
            if (pkm.Met_Location != 0)
                return true; // has original met location
            if (species < Legal.MaxSpeciesID_1)
                return true; // can trade RBY to wipe location
            if (pkm.Species < Legal.MaxSpeciesID_1)
                return true; // can trade RBY to wipe location

            return false;
        }

        private static bool GetCanBeEgg(PKM pkm)
        {
            bool canBeEgg = !pkm.Gen1_NotTradeback && GetCanBeEgg2(pkm);
            if (!canBeEgg)
                return false;

            if (!IsEvolutionValid(pkm))
                return false;

            return true;
        }

        private static bool GetCanBeEgg2(PKM pkm)
        {
            if (pkm.IsEgg)
                return pkm.Format == 2;

            if (pkm.Format > 2)
            {
                if (pkm.Met_Level < 5)
                    return false;
            }
            else
            {
                if (pkm.Met_Location != 0 && pkm.Met_Level != 1) // 2->1->2 clears met info
                    return false;
            }

            return pkm.CurrentLevel >= 5;
        }

        private static bool IsEvolutionValid(PKM pkm)
        {
            var curr = EvolutionChain.GetValidPreEvolutions(pkm, minLevel: 5);
            var poss = EvolutionChain.GetValidPreEvolutions(pkm, maxLevel: 100, minLevel: 5, skipChecks: true);
            return curr.Count >= poss.Count;
        }
    }
}
