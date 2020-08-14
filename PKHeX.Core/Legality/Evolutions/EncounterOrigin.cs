using System;
using System.Collections.Generic;
using static PKHeX.Core.Species;

namespace PKHeX.Core
{
    public static class EncounterOrigin
    {
        public static IReadOnlyList<EvoCriteria> GetOriginChain(PKM pkm)
        {
            bool hasOriginMet = pkm.HasOriginalMetLocation;
            var maxLevel = GetLevelOriginMax(pkm, hasOriginMet);
            var minLevel = GetLevelOriginMin(pkm, hasOriginMet);
            return GetOriginChain(pkm, -1, maxLevel, minLevel, hasOriginMet);
        }

        public static IReadOnlyList<EvoCriteria> GetOriginChain12(PKM pkm, GameVersion gameSource)
        {
            bool rby = gameSource == GameVersion.RBY;
            var maxSpecies = rby ? Legal.MaxSpeciesID_1 : Legal.MaxSpeciesID_2;

            bool hasOriginMet;
            int maxLevel, minLevel;
            if (pkm is PK2 pk2)
            {
                hasOriginMet = pk2.CaughtData != 0;
                maxLevel = rby && Future_LevelUp2.Contains(pk2.Species) ? pkm.CurrentLevel - 1 : pkm.CurrentLevel;
                minLevel = !hasOriginMet ? 2 : pkm.IsEgg ? 5 : pkm.Met_Level;
            }
            else if (pkm is PK1 pk1)
            {
                hasOriginMet = false;
                maxLevel = pk1.CurrentLevel;
                minLevel = 2;
            }
            else if (rby)
            {
                hasOriginMet = false;
                maxLevel = Future_LevelUp2.Contains(pkm.Species) ? pkm.CurrentLevel - 1 : GetLevelOriginMaxTransfer(pkm, pkm.Met_Level, 1);
                minLevel = 2;
            }
            else // GSC
            {
                hasOriginMet = false;
                maxLevel = GetLevelOriginMaxTransfer(pkm, pkm.Met_Level, 2);
                minLevel = 2;
            }

            return GetOriginChain(pkm, maxSpecies, maxLevel, minLevel, hasOriginMet);
        }

        private static IReadOnlyList<EvoCriteria> GetOriginChain(PKM pkm, int maxSpecies, int maxLevel, int minLevel, bool hasOriginMet)
        {
            if (maxLevel < minLevel)
                return Array.Empty<EvoCriteria>();

            if (hasOriginMet)
                return EvolutionChain.GetValidPreEvolutions(pkm, maxSpecies, maxLevel, minLevel);

            var bMax = pkm.CurrentLevel;
            var bMin = minLevel;
            var chain = EvolutionChain.GetValidPreEvolutions(pkm, maxSpecies, bMax, bMin);

            for (int i = chain.Count - 1; i >= 0; i--)
            {
                var evo = chain[i];
                if (evo.MinLevel > maxLevel)
                {
                    chain.RemoveAt(i);
                    continue;
                }
                evo.Level = Math.Min(maxLevel, evo.Level);
                evo.MinLevel = Math.Max(minLevel, evo.MinLevel);
            }
            return chain;
        }

        private static int GetLevelOriginMin(PKM pkm, bool hasMet)
        {
            if (!hasMet)
                return 1;
            if (pkm.Format == 3 && pkm.IsEgg)
                return 5;
            return Math.Max(1, pkm.Met_Level);
        }

        private static int GetLevelOriginMax(PKM pkm, bool hasMet)
        {
            var met = pkm.Met_Level;
            if (hasMet)
                return pkm.CurrentLevel;

            int generation = pkm.GenNumber;
            if (generation >= 4)
                return met;

            return GetLevelOriginMaxTransfer(pkm, met, generation);
        }

        private static int GetLevelOriginMaxTransfer(PKM pkm, int met, int generation)
        {
            var species = pkm.Species;

            if (Future_LevelUp.TryGetValue(species | (pkm.AltForm << 11), out var delta))
                return met - delta;

            if (generation < 4 && Future_LevelUp4.Contains(species))
                return met - 1;

            return met;
        }

        /// <summary>
        /// Species introduced in Generation 2 that require a level up to evolve into from a specimen that originated in a previous generation.
        /// </summary>
        private static readonly HashSet<int> Future_LevelUp2 = new HashSet<int>
        {
            (int)Crobat,
            (int)Espeon,
            (int)Umbreon,
            (int)Blissey,
        };

        /// <summary>
        /// Species introduced in Generation 4 that require a level up to evolve into from a specimen that originated in a previous generation.
        /// </summary>
        private static readonly HashSet<int> Future_LevelUp4 = new HashSet<int>
        {
            (int)Ambipom,
            (int)Weavile,
            (int)Magnezone,
            (int)Lickilicky,
            (int)Tangrowth,
            (int)Yanmega,
            (int)Leafeon,
            (int)Glaceon,
            (int)Mamoswine,
            (int)Gliscor,
            (int)Probopass,
        };

        /// <summary>
        /// Species introduced in Generation 6+ that require a level up to evolve into from a specimen that originated in a previous generation.
        /// </summary>
        private static readonly Dictionary<int, int> Future_LevelUp = new Dictionary<int, int>
        {
            // Gen6
            {(int)Sylveon, 1},

            // Gen7
            {(int)Marowak | (1 << 11), 1},

            // Gen8
            {(int)Weezing | (1 << 11), 1},
            {(int)MrMime | (1 << 11), 1},
            {(int)MrRime, 2},
        };
    }
}
