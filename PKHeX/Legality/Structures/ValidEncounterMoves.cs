using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public class ValidEncounterMoves
    {
        public int EncounterSpecies { get; set; }
        public DexLevel[][] EvolutionChains { get; set; }
        public List<int>[] validLevelUpMoves { get; set; }
        public List<int>[] validTMHMMoves { get; set; }
        public List<int>[] validTutorMoves { get; set; }
        public int minLvlG1 { get; set; }
    }
}
