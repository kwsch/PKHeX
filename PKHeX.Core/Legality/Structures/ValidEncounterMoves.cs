using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public class ValidEncounterMoves
    {
        public int EncounterSpecies { get; set; }
        public DexLevel[][] EvolutionChains { get; set; }
        public List<int>[] LevelUpMoves { get; set; } = Empty;
        public List<int>[] TMHMMoves { get; set; } = Empty;
        public List<int>[] TutorMoves { get; set; } = Empty;
        public int[] Relearn = new int[0];
        public int MinimumLevelGen1 { get; set; }
        public int MinimumLevelGen2 { get; set; }

        private const int EmptyCount = 7;
        private static readonly List<int>[] Empty = new int[EmptyCount].Select(z => new List<int>()).ToArray();
    }
}
