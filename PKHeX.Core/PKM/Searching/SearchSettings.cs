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
        public int Format { private get; init; }
        public int Generation { private get; init; }
        public int Species { get; init; } = -1;
        public int Ability { private get; init; } = -1;
        public int Nature { private get; init; } = -1;
        public int Item { private get; init; } = -1;
        public int Version { private get; init; } = -1;
        public int HiddenPowerType { private get; init; } = -1;

        public SearchComparison SearchFormat { private get; init; }
        public SearchComparison SearchLevel { private get; init; }

        public bool? SearchShiny { get; set; }
        public bool? SearchLegal { private get; set; }
        public bool? SearchEgg { get; set; }
        public int? ESV { private get; set; }
        public int? Level { private get; init; }

        public int IVType { private get; init; }
        public int EVType { private get; init; }

        public CloneDetectionMethod SearchClones { private get; set; }
        public IList<string> BatchInstructions { private get; init; } = Array.Empty<string>();
        private StringInstruction[] BatchFilters { get; set; } = Array.Empty<StringInstruction>();

        public readonly List<int> Moves = new();

        // ReSharper disable once CollectionNeverUpdated.Global
        /// <summary>
        /// Extra Filters to be checked after all other filters have been checked.
        /// </summary>
        /// <remarks>Collection is iterated right before clones are checked.</remarks>
        public List<Func<PKM, bool>> ExtraFilters { get; } = new();

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
            BatchFilters = StringInstruction.GetFilters(BatchInstructions).ToArray();
            var result = SearchInner(list);

            if (SearchClones != CloneDetectionMethod.None)
            {
                var method = SearchUtil.GetCloneDetectMethod(SearchClones);
                result = SearchUtil.GetExtraClones(result, method);
            }

            return result;
        }

        /// <summary>
        /// Searches the input list, filtering out entries as specified by the settings.
        /// </summary>
        /// <param name="list">List of entries to search</param>
        /// <returns>Search results that match all criteria</returns>
        public IEnumerable<SlotCache> Search(IEnumerable<SlotCache> list)
        {
            BatchFilters = StringInstruction.GetFilters(BatchInstructions).ToArray();
            var result = SearchInner(list);

            if (SearchClones != CloneDetectionMethod.None)
            {
                var method = SearchUtil.GetCloneDetectMethod(SearchClones);
                string GetHash(SlotCache z) => method(z.Entity);
                result = SearchUtil.GetExtraClones(result, GetHash);
            }

            return result;
        }

        private IEnumerable<PKM> SearchInner(IEnumerable<PKM> list)
        {
            foreach (var pk in list)
            {
                if (!IsSearchMatch(pk))
                    continue;
                yield return pk;
            }
        }

        private IEnumerable<SlotCache> SearchInner(IEnumerable<SlotCache> list)
        {
            foreach (var entry in list)
            {
                var pk = entry.Entity;
                if (!IsSearchMatch(pk))
                    continue;
                yield return entry;
            }
        }

        private bool IsSearchMatch(PKM pk)
        {
            if (!SearchSimple(pk))
                return false;
            if (!SearchIntermediate(pk))
                return false;
            if (!SearchComplex(pk))
                return false;

            foreach (var filter in ExtraFilters)
            {
                if (!filter(pk))
                    return false;
            }
            return true;
        }

        private bool SearchSimple(PKM pk)
        {
            if (Format > 0 && !SearchUtil.SatisfiesFilterFormat(pk, Format, SearchFormat))
                return false;
            if (Species > -1 && pk.Species != Species)
                return false;
            if (Ability > -1 && pk.Ability != Ability)
                return false;
            if (Nature > -1 && pk.StatNature != Nature)
                return false;
            if (Item > -1 && pk.HeldItem != Item)
                return false;
            if (Version > -1 && pk.Version != Version)
                return false;
            return true;
        }

        private bool SearchIntermediate(PKM pk)
        {
            if (Generation > 0 && !SearchUtil.SatisfiesFilterGeneration(pk, Generation)) return false;
            if (Moves.Count > 0 && SearchUtil.SatisfiesFilterMoves(pk, Moves)) return false;
            if (HiddenPowerType > -1 && pk.HPType != HiddenPowerType) return false;
            if (SearchShiny != null && pk.IsShiny != SearchShiny) return false;

            if (IVType > 0 && SearchUtil.SatisfiesFilterIVs(pk, IVType)) return false;
            if (EVType > 0 && SearchUtil.SatisfiesFilterEVs(pk, EVType)) return false;

            return true;
        }

        private bool SearchComplex(PKM pk)
        {
            if (SearchEgg != null && !FilterResultEgg(pk)) return false;
            if (Level is not (null or 0) && !SearchUtil.SatisfiesFilterLevel(pk, SearchLevel, (int) Level)) return false;
            if (SearchLegal != null && new LegalityAnalysis(pk).Valid != SearchLegal) return false;
            if (BatchFilters.Length != 0 && !SearchUtil.SatisfiesFilterBatchInstruction(pk, BatchFilters)) return false;

            return true;
        }

        private bool FilterResultEgg(PKM pk)
        {
            if (SearchEgg == false)
                return !pk.IsEgg;
            if (ESV != null)
                return pk.IsEgg && pk.PSV == ESV;
            return pk.IsEgg;
        }

        public IReadOnlyList<GameVersion> GetVersions(SaveFile sav) => GetVersions(sav, GetFallbackVersion(sav));

        public IReadOnlyList<GameVersion> GetVersions(SaveFile sav, GameVersion fallback)
        {
            if (Version > 0)
                return new[] {(GameVersion) Version};

            return Generation switch
            {
                1 when !ParseSettings.AllowGen1Tradeback => new[] {GameVersion.RD, GameVersion.BU, GameVersion.GN, GameVersion.YW},
                2 when sav is SAV2 {Korean: true} => new[] {GameVersion.GD, GameVersion.SV},
                1 or 2 => new[]
                {
                    GameVersion.RD, GameVersion.BU, GameVersion.GN, GameVersion.YW,
                    GameVersion.GD, GameVersion.SV, GameVersion.C,
                },

                _ when fallback.GetGeneration() == Generation => GameUtil.GetVersionsWithinRange(sav, Generation).ToArray(),
                _ => GameUtil.GameVersions,
            };
        }

        private static GameVersion GetFallbackVersion(ITrainerInfo sav)
        {
            var parent = GameUtil.GetMetLocationVersionGroup((GameVersion)sav.Game);
            if (parent == GameVersion.Invalid)
                parent = GameUtil.GetMetLocationVersionGroup(GameUtil.GetVersion(sav.Generation));
            return parent;
        }
    }
}
