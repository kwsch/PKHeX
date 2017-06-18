using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    public static class EvolutionVerifier
    {
        // Evolutions
        public static CheckResult VerifyEvolution(PKM pkm, LegalInfo info)
        {
            return IsValidEvolution(pkm, info) 
                ? new CheckResult(CheckIdentifier.Evolution) 
                : new CheckResult(Severity.Invalid, V86, CheckIdentifier.Evolution);
        }
        private static bool IsValidEvolution(PKM pkm, LegalInfo info)
        {
            int species = pkm.Species;
            if (info.EncounterMatch.Species == species)
                return true;
            if (info.EncounterMatch.EggEncounter && species == 350 && pkm.Format >= 5) // Prism Scale
                return true;
            if (!Legal.IsEvolutionValid(pkm, info.EncounterMatch.Species))
                return false;
            // If current species evolved with a move evolution and encounter species is not current species check if the evolution by move is valid
            // Only the evolution by move is checked, if there is another evolution before the evolution by move is covered in IsEvolutionValid
            if (Legal.SpeciesEvolutionWithMove.Contains(pkm.Species))
                return Legal.IsEvolutionValidWithMove(pkm, info);
            return true;
        }
    }
}
