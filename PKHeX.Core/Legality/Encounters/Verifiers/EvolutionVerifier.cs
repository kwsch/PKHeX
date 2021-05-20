using System.Linq;
using static PKHeX.Core.EvolutionRestrictions;
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
        public static CheckResult VerifyEvolution(PKM pkm, LegalInfo info)
        {
            // Check if basic evolution methods are satisfiable with this encounter.
            if (!IsValidEvolution(pkm, info))
                return new CheckResult(Severity.Invalid, LEvoInvalid, CheckIdentifier.Evolution);

            // Check if complex evolution methods are satisfiable with this encounter.
            if (!IsValidEvolutionWithMove(pkm, info))
                return new CheckResult(Severity.Invalid, string.Format(LMoveEvoFCombination_0, ParseSettings.SpeciesStrings[pkm.Species]), CheckIdentifier.Evolution);

            return VALID;
        }

        private static readonly CheckResult VALID = new(CheckIdentifier.Evolution);

        /// <summary>
        /// Checks if the Evolution from the source <see cref="IEncounterable"/> is valid.
        /// </summary>
        /// <param name="pkm">Source data to verify</param>
        /// <param name="info">Source supporting information to verify with</param>
        /// <returns>Evolution is valid or not</returns>
        private static bool IsValidEvolution(PKM pkm, LegalInfo info)
        {
            var chains = info.EvoChainsAllGens;
            if (chains[pkm.Format].Count == 0)
                return false; // Can't exist as current species

            // OK if un-evolved from original encounter
            int species = pkm.Species;
            if (info.EncounterMatch.Species == species)
                return true;

            // Bigender->Fixed (non-Genderless) destination species, accounting for PID-Gender relationship
            if (species == (int)Species.Vespiquen && info.Generation < 6 && (pkm.PID & 0xFF) >= 0x1F) // Combee->Vespiquen Invalid Evolution
                return false;

            if (chains[info.Generation].All(z => z.Species != info.EncounterMatch.Species))
                return false; // Can't exist as origin species

            return true;
        }
    }
}
