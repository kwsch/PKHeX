using System;
using System.Collections.Generic;
using static PKHeX.Core.Species;

namespace PKHeX.Core
{
    public static class EncounterOrigin
    {
        public static IReadOnlyList<EvoCriteria> GetOriginChain(PKM pkm, GameVersion gameSource)
        {
            var max = GetMaxSpecies(gameSource);
            var useCurrentLevelAsMax = pkm is PK1 || (pkm is PK2 pk2 && pk2.CaughtData == 0);
            return GetOriginChain(pkm, max, useCurrentLevelAsMax);
        }

        public static IReadOnlyList<EvoCriteria> GetOriginChain(PKM pkm, int maxSpecies = -1, bool hasMetMaxOverride = false)
        {
            bool hasOriginMet = pkm.HasOriginalMetLocation;
            var maxLevel = GetLevelOriginMax(pkm, hasOriginMet || hasMetMaxOverride);
            var minLevel = GetLevelOriginMin(pkm, hasOriginMet);
            return GetOriginChain(pkm, maxSpecies, maxLevel, minLevel, hasOriginMet);
        }

        private static int GetMaxSpecies(GameVersion gameSource)
        {
            if (gameSource == GameVersion.RBY)
                return Legal.MaxSpeciesID_1;
            if (GameVersion.GSC.Contains(gameSource))
                return Legal.MaxSpeciesID_2;
            return -1;
        }

        private static IReadOnlyList<EvoCriteria> GetOriginChain(PKM pkm, int maxSpecies, int maxLevel, int minLevel, bool hasOriginMet)
        {
            if (maxLevel < minLevel)
                return Array.Empty<EvoCriteria>();

            var chain = EvolutionChain.GetValidPreEvolutions(pkm, maxSpecies, maxLevel, minLevel, false);
            if (hasOriginMet)
                return chain;

            foreach (var c in chain)
                c.Level = Math.Min(maxLevel, c.Level);
            return chain;
        }

        private static int GetLevelOriginMin(PKM pkm, bool hasMet)
        {
            if (!hasMet)
                return 1;
            if (pkm.Format <= 3 && pkm.IsEgg)
                return 5;
            return Math.Max(1, pkm.Met_Level);
        }

        private static int GetLevelOriginMax(PKM pkm, bool hasMet)
        {
            var met = pkm.Met_Level;
            if (hasMet)
                return pkm.CurrentLevel;

            int generation = pkm.GenNumber;
            if (!pkm.InhabitedGeneration(generation))
                return met;

            if (generation >= 4)
                return met;

            var species = pkm.Species;

            if (Future_LevelUp.TryGetValue(species | (pkm.AltForm << 11), out var delta))
                return met - delta;

            if (generation == 1 && Future_LevelUp2.Contains(species))
                return pkm.Format >= 7 ? met - 1 : pkm.CurrentLevel - 1; // Gen2 won't have Met Level

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
