using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    internal static class MoveListSuggest
    {
        internal static int[] GetSuggestedMoves(PKM pkm, IReadOnlyList<EvoCriteria>[] evoChains, bool tm, bool tutor, bool reminder, IEncounterable enc)
        {
            if (pkm.IsEgg && pkm.Format <= 5) // pre relearn
                return MoveList.GetBaseEggMoves(pkm, pkm.Species, 0, (GameVersion)pkm.Version, pkm.CurrentLevel);

            if (!tm && !tutor && !reminder)
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
                    return MoveLevelUp.GetEncounterMoves(pkm.Species, pkm.AltForm, pkm.CurrentLevel, (GameVersion)pkm.Version);
                }
            }

            return GetValidMoves(pkm, evoChains, Tutor: tutor, Machine: tm, MoveReminder: reminder).Skip(1).ToArray(); // skip move 0
        }

        private static IEnumerable<int> GetValidMoves(PKM pkm, IReadOnlyList<EvoCriteria>[] evoChains, bool LVL = true, bool Tutor = true, bool Machine = true, bool MoveReminder = true, bool RemoveTransferHM = true)
        {
            GameVersion version = (GameVersion)pkm.Version;
            if (!pkm.IsUntraded)
                version = GameVersion.Any;
            return GetValidMoves(pkm, version, evoChains, minLvLG1: 1, minLvLG2: 1, LVL: LVL, Relearn: false, Tutor: Tutor, Machine: Machine, MoveReminder: MoveReminder, RemoveTransferHM: RemoveTransferHM);
        }

        private static IEnumerable<int> GetValidMoves(PKM pkm, GameVersion version, IReadOnlyList<IReadOnlyList<EvoCriteria>> evoChains, int minLvLG1 = 1, int minLvLG2 = 1, bool LVL = false, bool Relearn = false, bool Tutor = false, bool Machine = false, bool MoveReminder = true, bool RemoveTransferHM = true)
        {
            var r = new List<int> { 0 };
            if (Relearn && pkm.Format >= 6)
                r.AddRange(pkm.RelearnMoves);

            int start = pkm.GenNumber;
            if (start < 0)
                start = pkm.Format; // be generous instead of returning nothing
            if (pkm is IBattleVersion b)
                start = b.GetMinGeneration();

            for (int generation = start; generation <= pkm.Format; generation++)
            {
                var chain = evoChains[generation];
                if (chain.Count == 0)
                    continue;
                r.AddRange(MoveList.GetValidMoves(pkm, version, chain, generation, minLvLG1: minLvLG1, minLvLG2: minLvLG2, LVL: LVL, Relearn: false, Tutor: Tutor, Machine: Machine, MoveReminder: MoveReminder, RemoveTransferHM: RemoveTransferHM));
            }

            return r.Distinct();
        }
    }
}
