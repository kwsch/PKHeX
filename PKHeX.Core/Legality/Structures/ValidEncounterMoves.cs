using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public class ValidEncounterMoves
    {
        public int EncounterSpecies { get; set; }
        public DexLevel[][] EvolutionChains { get; set; }
        public List<int>[] validLevelUpMoves { get; set; } = Empty;
        public List<int>[] validTMHMMoves { get; set; } = Empty;
        public List<int>[] validTutorMoves { get; set; } = Empty;
        public int[] Relearn = new int[0];
        public int minLvlG1 { get; set; }

        private const int EmptyCount = 7;
        public static readonly List<int>[] Empty = new int[EmptyCount].Select(z => new List<int>()).ToArray();
    }
}
