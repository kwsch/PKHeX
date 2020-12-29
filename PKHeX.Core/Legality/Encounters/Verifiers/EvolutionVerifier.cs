using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    /// <summary>
    /// Verify Evolution Information for a matched <see cref="IEncounterable"/>
    /// </summary>
    public static class EvolutionVerifier
    {
        /// <summary>
        /// Verifies Evolution scenarios of an <see cref="IEncounterable"/> for an input <see cref="PKM"/> and relevant <see cref="LegalInfo"/>.
        /// </summary>
        /// <param name="pkm">Source data to verify</param>
        /// <param name="info">Source supporting information to verify with</param>
        /// <returns></returns>
        public static CheckResult VerifyEvolution(PKM pkm, LegalInfo info)
        {
            return IsValidEvolution(pkm, info)
                ? new CheckResult(CheckIdentifier.Evolution)
                : new CheckResult(Severity.Invalid, LEvoInvalid, CheckIdentifier.Evolution);
        }

        /// <summary>
        /// Checks if the Evolution from the source <see cref="IEncounterable"/> is valid.
        /// </summary>
        /// <param name="pkm">Source data to verify</param>
        /// <param name="info">Source supporting information to verify with</param>
        /// <returns>Evolution is valid or not</returns>
        private static bool IsValidEvolution(PKM pkm, LegalInfo info)
        {
            if (info.EvoChainsAllGens[pkm.Format].Count == 0)
                return false; // Can't exist as current species

            int species = pkm.Species;
            if (info.EncounterMatch.Species == species)
                return true;
            if (species == (int)Species.Vespiquen && info.Generation < 6 && (pkm.PID & 0xFF) >= 0x1F) // Combee->Vespiquen Invalid Evolution
                return false;

            if (info.Generation > 0 && info.EvoChainsAllGens[info.Generation].All(z => z.Species != info.EncounterMatch.Species))
                return false; // Can't exist as origin species

            // If current species evolved with a move evolution and encounter species is not current species check if the evolution by move is valid
            // Only the evolution by move is checked, if there is another evolution before the evolution by move is covered in IsEvolutionValid
            if (EvolutionRestrictions.SpeciesEvolutionWithMove.Contains(species))
                return EvolutionRestrictions.IsEvolutionValidWithMove(pkm, info);

            return true;
        }
    }
}
