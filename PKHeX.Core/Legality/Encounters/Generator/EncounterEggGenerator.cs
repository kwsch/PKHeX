using System.Collections.Generic;

using static PKHeX.Core.Legal;

namespace PKHeX.Core
{
    public static class EncounterEggGenerator
    {
        // EncounterEgg
        public static IEnumerable<EncounterEgg> GenerateEggs(PKM pkm)
        {
            if (NoHatchFromEgg.Contains(pkm.Species))
                yield break;
            if (FormConverter.IsTotemForm(pkm.Species, pkm.AltForm, pkm.GenNumber))
                yield break; // no totem eggs

            int gen = pkm.GenNumber;
            // version is a true indicator for all generation 3-5 origins
            var ver = (GameVersion)pkm.Version;
            int max = GetMaxSpeciesOrigin(gen);

            var baseSpecies = GetBaseSpecies(pkm, 0);
            int lvl = gen < 4 ? 5 : 1;
            if (baseSpecies <= max)
            {
                yield return new EncounterEgg { Game = ver, Level = lvl, Species = baseSpecies };
                if (gen > 5 && pkm.WasTradedEgg)
                    yield return new EncounterEgg { Game = GetOtherTradePair(ver), Level = lvl, Species = baseSpecies };
            }

            if (!GetSplitBreedGeneration(pkm).Contains(pkm.Species))
                yield break; // no other possible species

            baseSpecies = GetBaseSpecies(pkm, 1);
            if (baseSpecies <= max)
            {
                yield return new EncounterEgg { Game = ver, Level = lvl, Species = baseSpecies, SplitBreed = true };
                if (gen > 5 && pkm.WasTradedEgg)
                    yield return new EncounterEgg { Game = GetOtherTradePair(ver), Level = lvl, Species = baseSpecies, SplitBreed = true };
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
