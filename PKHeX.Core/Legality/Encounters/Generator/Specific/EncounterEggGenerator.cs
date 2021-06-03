using System.Collections.Generic;

using static PKHeX.Core.Legal;

namespace PKHeX.Core
{
    public static class EncounterEggGenerator
    {
        public static IEnumerable<EncounterEgg> GenerateEggs(PKM pkm, int generation, bool all = false)
        {
            var table = EvolutionTree.GetEvolutionTree(pkm, pkm.Format);
            int maxSpeciesOrigin = GetMaxSpeciesOrigin(generation);
            var evos = table.GetValidPreEvolutions(pkm, maxLevel: 100, maxSpeciesOrigin: maxSpeciesOrigin, skipChecks: true);
            return GenerateEggs(pkm, evos, generation, all);
        }

        public static IEnumerable<EncounterEgg> GenerateEggs(PKM pkm, IReadOnlyList<EvoCriteria> chain, int generation, bool all = false)
        {
            System.Diagnostics.Debug.Assert(generation >= 3); // if generating Gen2 eggs, use the other generator.
            int species = pkm.Species;
            if (!Breeding.CanHatchAsEgg(species))
                yield break;
            if (!Breeding.CanHatchAsEgg(species, pkm.Form, generation))
                yield break; // can't originate from eggs

            // version is a true indicator for all generation 3-5 origins
            var ver = (GameVersion)pkm.Version;
            if (!Breeding.CanGameGenerateEggs(ver))
                yield break;

            int lvl = generation <= 3 ? 5 : 1;
            int max = GetMaxSpeciesOrigin(generation);

            var e = EvoBase.GetBaseSpecies(chain, 0);
            if (e.Species <= max && Breeding.CanHatchAsEgg(e.Species, e.Form, ver))
            {
                yield return new EncounterEgg(e.Species, e.Form, lvl, generation, ver);
                if (generation > 5 && (pkm.WasTradedEgg || all) && HasOtherGamePair(ver))
                    yield return new EncounterEgg(e.Species, e.Form, lvl, generation, GetOtherTradePair(ver));
            }

            if (!Breeding.GetSplitBreedGeneration(generation).Contains(species))
                yield break; // no other possible species

            var o = EvoBase.GetBaseSpecies(chain, 1);
            if (o.Species == e.Species)
                yield break;

            if (o.Species <= max && Breeding.CanHatchAsEgg(o.Species, o.Form, ver))
            {
                yield return new EncounterEgg(o.Species, o.Form, lvl, generation, ver);
                if (generation > 5 && (pkm.WasTradedEgg || all) && HasOtherGamePair(ver))
                    yield return new EncounterEgg(o.Species, o.Form, lvl, generation, GetOtherTradePair(ver));
            }
        }

        // Gen6+ update the origin game when hatched. Quick manip for X.Y<->A.O | S.M<->US.UM, ie X->A
        private static GameVersion GetOtherTradePair(GameVersion ver) => ver switch
        {
            <= GameVersion.OR => (GameVersion) ((int) ver ^ 2), // gen6
            <= GameVersion.MN => ver + 2, // gen7
            _ => ver - 2
        };

        private static bool HasOtherGamePair(GameVersion ver)
        {
            return ver < GameVersion.GP; // lgpe and sw/sh don't have a sister pair
        }
    }
}
