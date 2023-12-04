using static PKHeX.Core.EvolutionRestrictions;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core;

/// <summary>
/// Verify Evolution Information for a matched <see cref="IEncounterable"/>
/// </summary>
public static class EvolutionVerifier
{
    private static readonly CheckResult VALID = new(CheckIdentifier.Evolution);

    /// <summary>
    /// Verifies Evolution scenarios of <see cref="IEncounterable"/> templates for an input <see cref="PKM"/> and relevant <see cref="LegalInfo"/>.
    /// </summary>
    /// <param name="pk">Source data to verify</param>
    /// <param name="info">Source supporting information to verify with</param>
    public static CheckResult VerifyEvolution(PKM pk, LegalInfo info)
    {
        // Check if basic evolution methods are satisfiable with this encounter.
        if (!IsValidEvolution(pk, info.EvoChainsAllGens, info.EncounterOriginal))
            return new CheckResult(Severity.Invalid, CheckIdentifier.Evolution, LEvoInvalid);

        // Check if complex evolution methods are satisfiable with this encounter.
        if (!IsValidEvolutionWithMove(pk, info))
            return new CheckResult(Severity.Invalid, CheckIdentifier.Evolution, string.Format(LMoveEvoFCombination_0, ParseSettings.SpeciesStrings[pk.Species]));

        return VALID;
    }

    /// <summary>
    /// Checks if the Evolution from the source <see cref="IEncounterable"/> is valid.
    /// </summary>
    /// <param name="pk">Source data to verify</param>
    /// <param name="history">Source supporting information to verify with</param>
    /// <param name="enc">Matched encounter</param>
    /// <returns>Evolution is valid or not</returns>
    private static bool IsValidEvolution(PKM pk, EvolutionHistory history, IEncounterTemplate enc)
    {
        // OK if un-evolved from original encounter
        var encSpecies = enc.Species;
        var curSpecies = pk.Species;
        if (curSpecies == encSpecies)
            return true; // never evolved

        var current = history.Get(pk.Context);
        if (!EvolutionHistory.HasVisited(current, curSpecies))
            return false; // Can't exist as current species

        // Double check that our encounter was able to exist as the encounter species.
        var original = history.Get(enc.Context);
        if (!EvolutionHistory.HasVisited(original, encSpecies))
            return false;

        // Bigender->Fixed (non-Genderless) destination species, accounting for PID-Gender relationship
        if (curSpecies == (int)Species.Vespiquen && enc.Generation < 6 && (pk.EncryptionConstant & 0xFF) >= 0x1F) // Combee->Vespiquen Invalid Evolution
            return false;

        return true;
    }
}
