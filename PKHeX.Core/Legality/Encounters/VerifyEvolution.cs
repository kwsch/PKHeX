using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    public static class VerifyEvolution
    {
        // Evolutions
        public static CheckResult verifyEvolution(PKM pkm, IEncounterable EncounterMatch)
        {
            if (!isValidEvolution(pkm, EncounterMatch))
                return new CheckResult(Severity.Invalid, V86, CheckIdentifier.Evolution);
            return new CheckResult(CheckIdentifier.Evolution);
        }
        private static bool isValidEvolution(PKM pkm, IEncounterable EncounterMatch)
        {
            if (pkm.WasEgg && !Legal.getEvolutionValid(pkm) && pkm.Species != 350)
                return false;

            if (EncounterMatch.Species == pkm.Species)
                return true;

            var matchEvo = Legal.getValidPreEvolutions(pkm).FirstOrDefault(z => z.Species == EncounterMatch.Species);
            if (matchEvo == null)
                return false;
            return matchEvo.RequiresLvlUp
                ? matchEvo.Level > EncounterMatch.LevelMin
                : matchEvo.Level >= EncounterMatch.LevelMin;
        }
    }
}
