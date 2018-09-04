using System.Collections.Generic;

using static PKHeX.Core.Legal;

namespace PKHeX.Core
{
    public static class EncounterEggGenerator
    {
        // EncounterEgg
        public static IEnumerable<EncounterEgg> GenerateEggs(PKM pkm, bool all = false)
        {
            var table = EvolutionTree.GetEvolutionTree(pkm.Format);
            int maxSpeciesOrigin = GetMaxSpeciesOrigin(pkm.GenNumber);
            var evos = table.GetValidPreEvolutions(pkm, maxLevel: 100, maxSpeciesOrigin: maxSpeciesOrigin, skipChecks: true);
            return GenerateEggs(pkm, evos, all);
        }

        public static IEnumerable<EncounterEgg> GenerateEggs(PKM pkm, IReadOnlyList<DexLevel> vs, bool all = false)
        {
            if (NoHatchFromEgg.Contains(pkm.Species))
                yield break;
            if (FormConverter.IsTotemForm(pkm.Species, pkm.AltForm, pkm.GenNumber))
                yield break; // no totem eggs

            int gen = pkm.GenNumber;
            if (gen <= 1)
                yield break; // can't get eggs
            // version is a true indicator for all generation 3-5 origins
            var ver = (GameVersion)pkm.Version;
            int max = GetMaxSpeciesOrigin(gen);

            var baseSpecies = GetBaseSpecies(pkm, vs, 0);
            int lvl = GetEggHatchLevel(gen);
            if (baseSpecies <= max)
            {
                yield return new EncounterEgg { Version = ver, Level = lvl, Species = baseSpecies };
                if (gen > 5 && (pkm.WasTradedEgg || all))
                    yield return new EncounterEgg { Version = GetOtherTradePair(ver), Level = lvl, Species = baseSpecies };
            }

            if (!GetSplitBreedGeneration(pkm).Contains(pkm.Species))
                yield break; // no other possible species

            var other = GetBaseSpecies(pkm, vs, 1);
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
