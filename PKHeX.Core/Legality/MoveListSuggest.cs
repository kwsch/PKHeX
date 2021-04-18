using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public static class MoveListSuggest
    {
        private static int[] GetSuggestedMoves(PKM pkm, IReadOnlyList<IReadOnlyList<EvoCriteria>> evoChains, MoveSourceType types, IEncounterTemplate enc)
        {
            if (pkm.IsEgg && pkm.Format <= 5) // pre relearn
                return MoveList.GetBaseEggMoves(pkm, pkm.Species, 0, (GameVersion)pkm.Version, pkm.CurrentLevel);

            if (types != MoveSourceType.None)
                return GetValidMoves(pkm, evoChains, types).Skip(1).ToArray(); // skip move 0

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

            return GetValidMoves(pkm, evoChains, types).Skip(1).ToArray(); // skip move 0
        }

        private static IEnumerable<int> GetValidMoves(PKM pkm, IReadOnlyList<IReadOnlyList<EvoCriteria>> evoChains, MoveSourceType types = MoveSourceType.ExternalSources, bool RemoveTransferHM = true)
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
        /// <remarks>Use <see cref="GetSuggestedRelearnMovesFromEncounter"/> instead of calling directly; this method just puts default values in without considering the final moveset.</remarks>
        public static IReadOnlyList<int> GetSuggestedRelearn(this IEncounterTemplate enc, PKM pkm)
        {
            if (VerifyRelearnMoves.ShouldNotHaveRelearnMoves(enc, pkm))
                return Empty;

            return GetSuggestedRelearnInternal(enc, pkm);
        }

        // Invalid encounters won't be recognized as an EncounterEgg; check if it *should* be a bred egg.
        private static IReadOnlyList<int> GetSuggestedRelearnInternal(this IEncounterTemplate enc, PKM pkm) => enc switch
        {
            IRelearn s when s.Relearn.Count > 0 => s.Relearn,
            EncounterEgg or EncounterInvalid {EggEncounter: true} => MoveBreed.GetExpectedMoves(pkm.RelearnMoves, enc),
            _ => Empty,
        };

        private static readonly IReadOnlyList<int> Empty = new int[4];

        /// <summary>
        /// Gets the current <see cref="PKM.RelearnMoves"/> array of four moves that might be legal.
        /// </summary>
        public static IReadOnlyList<int> GetSuggestedRelearnMovesFromEncounter(this LegalityAnalysis analysis, IEncounterTemplate? enc = null)
        {
            var info = analysis.Info;
            enc ??= info.EncounterOriginal;
            var pkm = analysis.pkm;

            if (VerifyRelearnMoves.ShouldNotHaveRelearnMoves(enc, pkm))
                return Empty;

            if (enc is EncounterEgg or EncounterInvalid {EggEncounter: true})
                return enc.GetSuggestedRelearnEgg(info.Moves, pkm);
            return enc.GetSuggestedRelearnInternal(pkm);
        }

        private static IReadOnlyList<int> GetSuggestedRelearnEgg(this IEncounterTemplate enc, IReadOnlyList<CheckMoveResult> parse, PKM pkm)
        {
            // Split-breed species like Budew & Roselia may be legal for one, and not the other.
            // If we're not a split-breed or are already legal, return.
            var result = enc.GetEggRelearnMoves(parse, pkm);
            int generation = enc.Generation;
            var split = Breeding.GetSplitBreedGeneration(generation);
            if (!split.Contains(enc.Species) || enc.Generation <= 2)
                return result;

            var tmp = pkm.Clone();
            tmp.SetRelearnMoves(result);
            var la = new LegalityAnalysis(tmp);
            if (la.Info.Moves.All(z => z.Valid))
                return result;

            // Try again with the other split-breed species if possible.
            var incense = EncounterEggGenerator.GenerateEggs(tmp, generation).FirstOrDefault();
            if (incense is null || incense.Species == enc.Species)
                return result;

            return incense.GetSuggestedRelearnEgg(parse, tmp);
        }

        private static IReadOnlyList<int> GetEggRelearnMoves(this IEncounterTemplate enc, IReadOnlyList<CheckMoveResult> parse, PKM pkm)
        {
            // Extract a list of the moves that should end up in the relearn move list.
            int ctr = 0;
            var moves = new int[4];
            for (var i = 0; i < parse.Count; i++)
            {
                var m = parse[i];
                if (!m.ShouldBeInRelearnMoves())
                    continue;
                moves[ctr++] = pkm.GetMove(i);
            }

            // Swap Volt Tackle to the end of the list.
            int volt = Array.IndexOf(moves, (int) Move.VoltTackle, 0, ctr);
            if (volt != -1)
            {
                var dest = ctr - 1;
                moves[volt] = moves[dest];
                moves[dest] = (int) Move.VoltTackle;
            }
            return MoveBreed.GetExpectedMoves(moves, enc);
        }
    }
}
