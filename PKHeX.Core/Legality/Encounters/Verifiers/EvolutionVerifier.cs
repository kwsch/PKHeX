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
            var chains = info.EvoChainsAllGensReduced;
            if (chains[pkm.Format].Count == 0)
                return false; // Can't exist as current species

            // OK if un-evolved from original encounter
            int species = pkm.Species;
            if (info.EncounterMatch.Species == species)
                return true;

            var format = pkm.Format;
            var genevolved = info.EvoGenerations.Any() ? info.EvoGenerations.Last() : format;
            // Pokemon with evolution methods that do not require level up in current gen but it was required if evolved on previous generations
            if (SpeciesEvolutionLevelUpPreviousGenerations(species, genevolved, format))
            {
                var chain = info.EvoChainsAllGensReduced;
                var minlevel = chain[genevolved].First().Level;
                // Chain level are calculated using current format in wich the evolution does not require level up
                // But pokemon was evolved on a previous generation in wich level up was required
                // Example: Magnemite evolve into Magneto at level 30 and Magneton into Magnezone with a stone in gen 8 and leveling up in magnetic zone before gen 8
                // Min level is 30 for both in case of gen 8 evolution but for previous gens evolutions min level for Magnezone should be 31
                if (pkm.CurrentLevel <= minlevel)
                    return false;

                if (!pkm.HasOriginalMetLocation && pkm.Met_Level == pkm.CurrentLevel && genevolved >= 5)
                    // Magnezone, Leafeon and Glaceon evolved after gen transfer to gen 5 requires to level up after met level
                    // Gen 3 Milotic does not require this edge case, if evolved in gen 4 it loose met level in gen 5 and does not require level up in gens 5-8
                    return false;

                var enc = info.EncounterMatch;
                if (pkm.CurrentLevel <= enc.LevelMin)
                    return false;
            }

            if (species == (int)Species.Milotic)
            {
                if (pkm is IContestStats s && s.CNT_Beauty >= 170)
                    return true;

                // Feebas evolved into Milotic before Generation 5 requires beauty
                if (genevolved < 5)
                    return false;
            }

            // Bigender->Fixed (non-Genderless) destination species, accounting for PID-Gender relationship
            if (species == (int)Species.Vespiquen && info.Generation < 6 && (pkm.PID & 0xFF) >= 0x1F) // Combee->Vespiquen Invalid Evolution
                return false;

            if (chains[info.Generation].All(z => z.Species != info.EncounterMatch.Species))
                return false; // Can't exist as origin species

            return true;
        }
    }
}
