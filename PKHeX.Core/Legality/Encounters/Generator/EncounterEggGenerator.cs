using System.Collections.Generic;

using static PKHeX.Core.Legal;

namespace PKHeX.Core
{
    public static class EncounterEggGenerator
    {
        // EncounterEgg
        public static IEnumerable<EncounterEgg> GenerateEggs(PKM pkm, bool all = false)
        {
            var table = EvolutionTree.GetEvolutionTree(pkm, pkm.Format);
            int maxSpeciesOrigin = GetMaxSpeciesOrigin(pkm.GenNumber);
            var evos = table.GetValidPreEvolutions(pkm, maxLevel: 100, maxSpeciesOrigin: maxSpeciesOrigin, skipChecks: true);
            return GenerateEggs(pkm, evos, all);
        }

        public static IEnumerable<EncounterEgg> GenerateEggs(PKM pkm, IReadOnlyList<DexLevel> vs, bool all = false)
        {
            int species = pkm.Species;
            if (NoHatchFromEgg.Contains(species))
                yield break;

            int gen = pkm.GenNumber;
            if (gen <= 1)
                yield break; // can't get eggs
            if (FormConverter.IsTotemForm(species, pkm.AltForm, gen))
                yield break; // no totem eggs

            // version is a true indicator for all generation 3-5 origins
            var ver = (GameVersion)pkm.Version;
            int lvl = GetEggHatchLevel(gen);
            int max = GetMaxSpeciesOrigin(gen);

            var baseSpecies = GetBaseSpecies(species, vs, 0);
            if (baseSpecies <= max)
            {
                yield return new EncounterEgg { Version = ver, Level = lvl, Species = baseSpecies };
                if (gen > 5 && (pkm.WasTradedEgg || all))
                    yield return new EncounterEgg { Version = GetOtherTradePair(ver), Level = lvl, Species = baseSpecies };
            }

            if (!GetSplitBreedGeneration(gen).Contains(species))
                yield break; // no other possible species

            var other = GetBaseSpecies(species, vs, 1);
            if (other <= max)
            {
                yield return new EncounterEggSplit { Version = ver, Level = lvl, Species = other, OtherSpecies = baseSpecies };
                if (gen > 5 && (pkm.WasTradedEgg || all))
                    yield return new EncounterEggSplit { Version = GetOtherTradePair(ver), Level = lvl, Species = other, OtherSpecies = baseSpecies };
            }
        }

        // Gen6+ update the origin game when hatched. Quick manip for X.Y<->A.O | S.M<->US.UM, ie X->A
        private static GameVersion GetOtherTradePair(GameVersion ver)
        {
            if (ver <= GameVersion.OR) // gen6
                return (GameVersion)((int)ver ^ 2);
            if (ver <= GameVersion.MN) // gen7
                return ver + 2;
            return ver - 2;
        }
    }
}
