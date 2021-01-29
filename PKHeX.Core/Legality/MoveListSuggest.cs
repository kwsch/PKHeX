using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public static class MoveListSuggest
    {
        private static int[] GetSuggestedMoves(PKM pkm, IReadOnlyList<EvoCriteria>[] evoChains, MoveSourceType types, IEncounterable enc)
        {
            if (pkm.IsEgg && pkm.Format <= 5) // pre relearn
                return MoveList.GetBaseEggMoves(pkm, pkm.Species, 0, (GameVersion)pkm.Version, pkm.CurrentLevel);

            if (types == MoveSourceType.None)
            {
                // try to give current moves
                if (enc.Generation <= 2)
                {
                    var lvl = pkm.Format >= 7 ? pkm.Met_Level : pkm.CurrentLevel;
                    var ver = enc.Version;
                    return MoveLevelUp.GetEncounterMoves(enc.Species, 0, lvl, ver);
                }

                if (pkm.Species == enc.Species)
                {
                    return MoveLevelUp.GetEncounterMoves(pkm.Species, pkm.Form, pkm.CurrentLevel, (GameVersion)pkm.Version);
                }
            }

            return GetValidMoves(pkm, evoChains, types).Skip(1).ToArray(); // skip move 0
        }

        private static IEnumerable<int> GetValidMoves(PKM pkm, IReadOnlyList<EvoCriteria>[] evoChains, MoveSourceType types = MoveSourceType.ExternalSources, bool RemoveTransferHM = true)
        {
            GameVersion version = (GameVersion)pkm.Version;
            if (!pkm.IsUntraded)
                version = GameVersion.Any;
            return GetValidMoves(pkm, version, evoChains, minLvLG1: 1, minLvLG2: 1, types: types, RemoveTransferHM: RemoveTransferHM);
        }

        private static IEnumerable<int> GetValidMoves(PKM pkm, GameVersion version, IReadOnlyList<IReadOnlyList<EvoCriteria>> evoChains, int minLvLG1 = 1, int minLvLG2 = 1, MoveSourceType types = MoveSourceType.Reminder, bool RemoveTransferHM = true)
        {
            var r = new List<int> { 0 };
            if (types.HasFlagFast(MoveSourceType.RelearnMoves) && pkm.Format >= 6)
                r.AddRange(pkm.RelearnMoves);

            int start = pkm.Generation;
            if (start < 0)
                start = pkm.Format; // be generous instead of returning nothing
            if (pkm is IBattleVersion b)
                start = Math.Max(0, b.GetMinGeneration());

            for (int generation = start; generation <= pkm.Format; generation++)
            {
                var chain = evoChains[generation];
                if (chain.Count == 0)
                    continue;
                r.AddRange(MoveList.GetValidMoves(pkm, version, chain, generation, minLvLG1: minLvLG1, minLvLG2: minLvLG2, types: types, RemoveTransferHM: RemoveTransferHM));
            }

            return r.Distinct();
        }

        private static IEnumerable<int> AllSuggestedMoves(this LegalityAnalysis analysis)
        {
            if (!analysis.Parsed)
                return new int[4];
            return analysis.GetSuggestedCurrentMoves();
        }

        private static IEnumerable<int> AllSuggestedRelearnMoves(this LegalityAnalysis analysis)
        {
            if (!analysis.Parsed)
                return new int[4];
            var pkm = analysis.pkm;
            var enc = analysis.EncounterMatch;
            return MoveList.GetValidRelearn(pkm, enc.Species, enc.Form, (GameVersion)pkm.Version).ToArray();
        }

        public static int[] GetSuggestedMovesAndRelearn(this LegalityAnalysis analysis)
        {
            if (!analysis.Parsed)
                return new int[4];
            return analysis.AllSuggestedMoves().Concat(analysis.AllSuggestedRelearnMoves()).ToArray();
        }

        /// <summary>
        /// Gets four moves which can be learned depending on the input arguments.
        /// </summary>
        /// <param name="analysis">Parse information to generate a moveset for.</param>
        /// <param name="types">Allowed move sources for populating the result array</param>
        public static int[] GetSuggestedCurrentMoves(this LegalityAnalysis analysis, MoveSourceType types = MoveSourceType.All)
        {
            if (!analysis.Parsed)
                return new int[4];
            var pkm = analysis.pkm;
            if (pkm.IsEgg && pkm.Format >= 6)
                return pkm.RelearnMoves;

            if (pkm.IsEgg)
                types = types.ClearNonEggSources();

            var info = analysis.Info;
            return GetSuggestedMoves(pkm, info.EvoChainsAllGens, types, info.EncounterOriginal);
        }

        /// <summary>
        /// Gets the current <see cref="PKM.RelearnMoves"/> array of four moves that might be legal.
        /// </summary>
        public static IReadOnlyList<int> GetSuggestedRelearnMovesFromEncounter(this LegalityAnalysis analysis)
        {
            var info = analysis.Info;
            if (info.Generation < 6)
                return new int[4];

            var pkm = analysis.pkm;
            var enc = info.EncounterMatch;
            var parsed = VerifyRelearnMoves.GetSuggestedRelearn(pkm, enc, info.Relearn);
            if (parsed.Count == 0) // Always true for Origins < 6 and encounters without relearn permitted.
                return new int[4];

            // Invalid encounters won't be recognized as an EncounterEgg; check if it *should* be a bred egg.
            if (!enc.EggEncounter)
                return parsed;

            List<int> window = new(parsed.Where(z => z != 0));
            window.AddRange(pkm.Moves.Where((_, i) => info.Moves[i].ShouldBeInRelearnMoves()));
            window = window.Distinct().ToList();
            int[] moves = new int[4];
            int start = Math.Max(0, window.Count - 4);
            int count = Math.Min(4, window.Count);
            window.CopyTo(start, moves, 0, count);
            return moves;
        }
    }
}
