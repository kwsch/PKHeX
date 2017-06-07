using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    public static class VerifyEvolution
    {
        // Evolutions
        public static CheckResult verifyEvolution(PKM pkm, LegalInfo info)
        {
            return isValidEvolution(pkm, info) 
                ? new CheckResult(CheckIdentifier.Evolution) 
                : new CheckResult(Severity.Invalid, V86, CheckIdentifier.Evolution);
        }
        private static bool isValidEvolution(PKM pkm, LegalInfo info)
        {
            int species = pkm.Species;
            if (info.EncounterMatch.Species == species)
                return true;
            if (info.EncounterMatch.EggEncounter && species == 350)
                return true;
            if (!Legal.getEvolutionValid(pkm, info.EncounterMatch.Species))
                return false;
            // If current species evolved with a move evolution and encounter species is not current species check if the evolution by move is valid
            // Only the evolution by move is checked, if there is another evolution before the evolution by move is covered in getEvolutionValid
            if (Legal.SpeciesEvolutionWithMove.Contains(pkm.Species))
                return Legal.getEvolutionWithMoveValid(pkm, info);
            return true;
        }
    }
}
