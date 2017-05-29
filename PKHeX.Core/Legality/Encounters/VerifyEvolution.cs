using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    public static class VerifyEvolution
    {
        // Evolutions
        public static CheckResult verifyEvolution(PKM pkm, IEncounterable EncounterMatch)
        {
            return isValidEvolution(pkm, EncounterMatch) 
                ? new CheckResult(CheckIdentifier.Evolution) 
                : new CheckResult(Severity.Invalid, V86, CheckIdentifier.Evolution);
        }
        private static bool isValidEvolution(PKM pkm, IEncounterable EncounterMatch)
        {
            int species = pkm.Species;
            if (EncounterMatch.Species == species)
                return true;
            if (EncounterMatch.EggEncounter && species == 350)
                return true;
            return Legal.getEvolutionValid(pkm, EncounterMatch.Species);
        }
    }
}
