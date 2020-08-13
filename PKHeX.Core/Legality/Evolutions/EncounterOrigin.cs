using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    public static class EncounterOrigin
    {
        public static IReadOnlyList<EvoCriteria> GetOriginChain(PKM pkm, GameVersion gameSource)
        {
            var max = GetMaxSpecies(gameSource);
            var maxLevel = pkm.CurrentLevel;
            var minLevel = Math.Max(1, GetMaxLevelEncounter(pkm));
            return GetOriginChain(pkm, maxspeciesorigin: max, maxLevel, minLevel);
        }

        public static IReadOnlyList<EvoCriteria> GetOriginChain(PKM pkm)
        {
            var maxLevel = pkm.CurrentLevel;
            var minLevel = Math.Max(1, GetMaxLevelEncounter(pkm));
            return GetOriginChain(pkm, maxspeciesorigin: -1, maxLevel, minLevel);
        }

        private static int GetMaxSpecies(GameVersion gameSource)
        {
            if (gameSource == GameVersion.RBY)
                return Legal.MaxSpeciesID_1;
            if (GameVersion.GSC.Contains(gameSource))
                return Legal.MaxSpeciesID_2;
            return -1;
        }

        private static List<EvoCriteria> GetOriginChain(PKM pkm, int maxspeciesorigin, int maxLevel, int minLevel, bool skipChecks = false)
        {
            var chain = EvolutionChain.GetValidPreEvolutions(pkm, maxspeciesorigin, maxLevel, minLevel, skipChecks);
            if (pkm.HasOriginalMetLocation)
                return chain;

            var maxEncounter = GetMaxLevelEncounter(pkm);
            if (maxEncounter < 0)
            {
                chain.Clear();
                return chain;
            }
            foreach (var c in chain)
                c.Level = Math.Min(maxEncounter, c.Level);
            return chain;
        }

        private static int GetMaxLevelGeneration(PKM pkm)
        {
            return GetMaxLevelGeneration(pkm, pkm.GenNumber);
        }

        private static int GetMaxLevelGeneration(PKM pkm, int generation)
        {
            if (!pkm.InhabitedGeneration(generation))
                return pkm.Met_Level;

            if (pkm.Format <= 2)
            {
                if (generation == 1 && Legal.FutureEvolutionsGen1_Gen2LevelUp.Contains(pkm.Species))
                    return pkm.CurrentLevel - 1;
                return pkm.CurrentLevel;
            }

            if ((int)Species.Sylveon == pkm.Species && generation == 5)
                return pkm.CurrentLevel - 1;

            if (generation == 3 && pkm.Format > 4 && pkm.Met_Level == pkm.CurrentLevel && Legal.FutureEvolutionsGen3_LevelUpGen4.Contains(pkm.Species))
                return pkm.Met_Level - 1;

            if (!pkm.HasOriginalMetLocation)
                return pkm.Met_Level;

            return pkm.CurrentLevel;
        }

        private static int GetMaxLevelEncounter(PKM pkm)
        {
            // Only for gen 3 pokemon in format 3, after transfer to gen 4 it should return transfer level
            if (pkm.Format == 3 && pkm.WasEgg)
                return 5;

            // Only for gen 4 pokemon in format 4, after transfer to gen 5 it should return transfer level
            if (pkm.Format == 4 && pkm.Gen4 && pkm.WasEgg)
                return 1;

            return pkm.HasOriginalMetLocation ? pkm.Met_Level : GetMaxLevelGeneration(pkm);
        }
    }
}