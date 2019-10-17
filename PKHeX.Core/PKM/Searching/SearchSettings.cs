using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core.Searching
{
    /// <summary>
    /// <see cref="PKM"/> search settings &amp; searcher
    /// </summary>
    public sealed class SearchSettings
    {
        public int Format { private get; set; }
        public int Generation { private get; set; }
        public int Species { get; set; } = -1;
        public int Ability { private get; set; } = -1;
        public int Nature { private get; set; } = -1;
        public int Item { private get; set; } = -1;
        public int Version { private get; set; } = -1;
        public int HiddenPowerType { private get; set; } = -1;

        public SearchComparison SearchFormat { private get; set; }
        public SearchComparison SearchLevel { private get; set; }

        public bool? SearchShiny { private get; set; }
        public bool? SearchLegal { private get; set; }
        public bool? SearchEgg { get; set; }
        public int? ESV { private get; set; }
        public int? Level { private get; set; }

        public int IVType { private get; set; }
        public int EVType { private get; set; }

        public CloneDetectionMethod SearchClones { private get; set; }
        public IList<string> BatchInstructions { private get; set; } = Array.Empty<string>();

        public readonly List<int> Moves = new List<int>();

        // ReSharper disable once CollectionNeverUpdated.Global
        /// <summary>
        /// Extra Filters to be checked after all other filters have been checked.
        /// </summary>
        /// <remarks>Collection is iterated right before clones are checked.</remarks>
        public List<Func<PKM, bool>> ExtraFilters { get; } = new List<Func<PKM, bool>>();

        /// <summary>
        /// Adds a move to the required move list.
        /// </summary>
        /// <param name="move"></param>
        public void AddMove(int move)
        {
            if (move > 0 && !Moves.Contains(move))
                Moves.Add(move);
        }

        /// <summary>
        /// Searches the input list, filtering out entries as specified by the settings.
        /// </summary>
        /// <param name="list">List of entries to search</param>
        /// <returns>Search results that match all criteria</returns>
        public IEnumerable<PKM> Search(IEnumerable<PKM> list)
        {
            var result = SearchSimple(list);
            result = SearchIntermediate(result);
            result = SearchComplex(result);

            foreach (var filter in ExtraFilters)
                result = result.Where(filter);

            if (SearchClones != CloneDetectionMethod.None)
                result = SearchUtil.GetClones(result, SearchClones);

            return result;
        }

        private IEnumerable<PKM> SearchSimple(IEnumerable<PKM> res)
        {
            if (Format > 0)
                res = SearchUtil.FilterByFormat(res, Format, SearchFormat);
            if (Species > -1)
                res = res.Where(pk => pk.Species == Species);
            if (Ability > -1)
                res = res.Where(pk => pk.Ability == Ability);
            if (Nature > -1)
                res = res.Where(pk => pk.Nature == Nature);
            if (Item > -1)
                res = res.Where(pk => pk.HeldItem == Item);
            if (Version > -1)
                res = res.Where(pk => pk.Version == Version);

            return res;
        }

        private IEnumerable<PKM> SearchIntermediate(IEnumerable<PKM> res)
        {
            if (Generation > 0)
                res = SearchUtil.FilterByGeneration(res, Generation);
            if (Moves.Count > 0)
                res = SearchUtil.FilterByMoves(res, Moves);
            if (HiddenPowerType > -1)
                res = res.Where(pk => pk.HPType == HiddenPowerType);
            if (SearchShiny != null)
                res = res.Where(pk => pk.IsShiny == SearchShiny);

            if (IVType > 0)
                res = SearchUtil.FilterByIVs(res, IVType);
            if (EVType > 0)
                res = SearchUtil.FilterByEVs(res, EVType);

            return res;
        }

        private IEnumerable<PKM> SearchComplex(IEnumerable<PKM> res)
        {
            if (SearchEgg != null)
                res = FilterResultEgg(res);

            if (Level != null)
                res = SearchUtil.FilterByLVL(res, SearchLevel, (int)Level);

            if (SearchLegal != null)
                res = res.Where(pk => new LegalityAnalysis(pk).Valid == SearchLegal);

            if (BatchInstructions != null)
                res = SearchUtil.FilterByBatchInstruction(res, BatchInstructions);

            return res;
        }

        private IEnumerable<PKM> FilterResultEgg(IEnumerable<PKM> res)
        {
            if (SearchEgg == false)
                return res.Where(pk => !pk.IsEgg);
            if (ESV != null)
                return res.Where(pk => pk.IsEgg && pk.PSV == ESV);
            return res.Where(pk => pk.IsEgg);
        }

        public IReadOnlyList<GameVersion> GetVersions(SaveFile SAV) => GetVersions(SAV, GetFallbackVersion(SAV));

        public IReadOnlyList<GameVersion> GetVersions(SaveFile SAV, GameVersion fallback)
        {
            if (Version > 0)
                return new[] {(GameVersion) Version};
            if (Generation != 0)
            {
                return fallback.GetGeneration() == Generation
                    ? GameUtil.GetVersionsWithinRange(SAV, Generation).ToArray()
                    : GameUtil.GameVersions;
            }

            return GameUtil.GameVersions;
        }

        private static GameVersion GetFallbackVersion(SaveFile SAV)
        {
            var parent = GameUtil.GetMetLocationVersionGroup((GameVersion)SAV.Game);
            if (parent == GameVersion.Invalid)
                parent = GameUtil.GetMetLocationVersionGroup(GameUtil.GetVersion(SAV.Generation));
            return parent;
        }
    }
}
