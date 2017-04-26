using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public class ValidEncounterMoves
    {
        public int EncounterSpecies;
        public DexLevel[][] EvolutionChains;
        public List<int>[] validLevelUpMoves;
        public List<int>[] validTMHMMoves;
        public List<int>[] validTutorMoves;
        public int minLvlG1;
    }
}
