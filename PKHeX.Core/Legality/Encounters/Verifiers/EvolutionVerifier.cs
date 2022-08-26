using static PKHeX.Core.EvolutionRestrictions;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core;

/// <summary>
/// Verify Evolution Information for a matched <see cref="IEncounterable"/>
/// </summary>
public static class EvolutionVerifier
{
    /// <summary>
    /// Verifies Evolution scenarios of an <see cref="IEncounterable"/> for an input <see cref="PKM"/> and relevant <see cref="LegalInfo"/>.
    /// </summary>
    /// <param name="pk">Source data to verify</param>
    /// <param name="info">Source supporting information to verify with</param>
    public static CheckResult VerifyEvolution(PKM pk, LegalInfo info)
    {
        // Check if basic evolution methods are satisfiable with this encounter.
        if (!IsValidEvolution(pk, info))
            return new CheckResult(Severity.Invalid, LEvoInvalid, CheckIdentifier.Evolution);

        // Check if complex evolution methods are satisfiable with this encounter.
        if (!IsValidEvolutionWithMove(pk, info))
            return new CheckResult(Severity.Invalid, string.Format(LMoveEvoFCombination_0, ParseSettings.SpeciesStrings[pk.Species]), CheckIdentifier.Evolution);

        return VALID;
    }

    private static readonly CheckResult VALID = new(CheckIdentifier.Evolution);

    /// <summary>
    /// Checks if the Evolution from the source <see cref="IEncounterable"/> is valid.
    /// </summary>
    /// <param name="pk">Source data to verify</param>
    /// <param name="info">Source supporting information to verify with</param>
    /// <returns>Evolution is valid or not</returns>
    private static bool IsValidEvolution(PKM pk, LegalInfo info)
    {
        var chains = info.EvoChainsAllGens;
        if (chains.Get(pk.Context).Length == 0)
            return false; // Can't exist as current species

        // OK if un-evolved from original encounter
        ushort species = pk.Species;
        var enc = info.EncounterMatch;
        if (species == enc.Species) // never evolved
            return true;

        // Bigender->Fixed (non-Genderless) destination species, accounting for PID-Gender relationship
        if (species == (int)Species.Vespiquen && enc.Generation < 6 && (pk.EncryptionConstant & 0xFF) >= 0x1F) // Combee->Vespiquen Invalid Evolution
            return false;

        // Double check that our encounter was able to exist as the encounter species.
        foreach (var z in chains.Get(enc.Context))
        {
            if (z.Species == enc.Species)
                return true;
        }
        return false;
    }
}
