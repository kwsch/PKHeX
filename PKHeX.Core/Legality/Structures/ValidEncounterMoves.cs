using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public class ValidEncounterMoves
    {
        public int EncounterSpecies { get; }
        public DexLevel[][] EvolutionChains { get; }
        public List<int>[] LevelUpMoves { get; } = Empty;
        public List<int>[] TMHMMoves { get; } = Empty;
        public List<int>[] TutorMoves { get; } = Empty;
        public int[] Relearn = new int[0];
        public int MinimumLevelGen1 { get; }
        public int MinimumLevelGen2 { get; }

        private const int EmptyCount = 7;
        private static readonly List<int>[] Empty = new int[EmptyCount].Select(z => new List<int>()).ToArray();

        public ValidEncounterMoves(PKM pkm, LegalInfo info)
        {
            MinimumLevelGen1 = pkm.GenNumber <= 2 ? info.EncounterMatch.LevelMin + 1 : 0;
            MinimumLevelGen2 = Legal.AllowGen2MoveReminder ? 1 : info.EncounterMatch.LevelMin + 1;
            EncounterSpecies = info.EncounterMatch.Species;
            EvolutionChains = info.EvoChainsAllGens;
            LevelUpMoves = Legal.GetValidMovesAllGens(pkm, EvolutionChains, minLvLG1: MinimumLevelGen1, minLvLG2: MinimumLevelGen2, Tutor: false, Machine: false, RemoveTransferHM: false);
            TMHMMoves = Legal.GetValidMovesAllGens(pkm, EvolutionChains, LVL: false, Tutor: false, MoveReminder: false, RemoveTransferHM: false);
            TutorMoves = Legal.GetValidMovesAllGens(pkm, EvolutionChains, LVL: false, Machine: false, MoveReminder: false, RemoveTransferHM: false);
        }

        public ValidEncounterMoves(List<int>[] levelup)
        {
            LevelUpMoves = levelup;
        }
    }
}
