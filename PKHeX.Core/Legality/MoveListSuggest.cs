using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    internal static class MoveListSuggest
    {
        internal static int[] GetSuggestedMoves(PKM pkm, IReadOnlyList<EvoCriteria>[] evoChains, MoveSourceType types, IEncounterable enc)
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
                start = b.GetMinGeneration();

            for (int generation = start; generation <= pkm.Format; generation++)
            {
                var chain = evoChains[generation];
                if (chain.Count == 0)
                    continue;
                r.AddRange(MoveList.GetValidMoves(pkm, version, chain, generation, minLvLG1: minLvLG1, minLvLG2: minLvLG2, types: types, RemoveTransferHM: RemoveTransferHM));
            }

            return r.Distinct();
        }
    }
}
