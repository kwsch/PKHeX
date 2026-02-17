using static PKHeX.Core.LegalityCheckResultCode;
using static PKHeX.Core.CheckIdentifier;
using static PKHeX.Core.CheckResult;
using static PKHeX.Core.Severity;

namespace PKHeX.Core;

public static class FullnessRules
{
    internal static void Verify<T>(LegalityAnalysis data, T pk) where T : PKM, IFullnessEnjoyment
    {
        if (pk.IsEgg)
        {
            if (pk.Fullness != 0)
                data.AddLine(Get(Invalid, Encounter, MemoryStatFullness_0, 0));
            if (pk.Enjoyment != 0)
                data.AddLine(Get(Invalid, Encounter, MemoryStatEnjoyment_0, 0));
            return;
        }

        if (pk.Format >= 8)
        {
            if (pk.Fullness > 245) // Exiting camp is -10, so a 255=>245 is max.
                data.AddLine(Get(Invalid, Encounter, MemoryStatFullnessLEQ_0, 245));
            else if (pk.Fullness is not 0 && pk is not PK8) // BD/SP and PLA do not set this field, even via HOME.
                data.AddLine(Get(Invalid, Encounter, MemoryStatFullness_0, 0));

            if (pk.Enjoyment != 0)
                data.AddLine(Get(Invalid, Encounter, MemoryStatEnjoyment_0, 0));
            return;
        }

        if (pk.Format == 6)
            VerifyFullness6(data, pk);
    }

    private static void VerifyFullness6<T>(LegalityAnalysis data, T pk) where T : PKM, IFullnessEnjoyment
    {
        if (!pk.IsUntraded || pk.XY)
            return;

        // OR/AS does not set fullness (no Amie).
        if (pk.Fullness == 0)
            return;

        // If it can't be fed, it can't have fullness.
        // If it had a pre-evolution (which can be fed, always), it's OK.
        // Therefore, anything evolved from its original encounter must have existed in a feedable state.
        var currentSpecies = pk.Species;
        if (currentSpecies != data.EncounterMatch.Species) // evolved
            return; // OK
        if (IsUnfeedable6(currentSpecies))
            data.AddLine(Get(Invalid, Encounter, MemoryStatFullness_0, 0));
    }

    /// <summary>
    /// Checks if the species is one of the few that cannot be fed in Gen 6's Pokémon-Amie.
    /// </summary>
    public static bool IsUnfeedable6(ushort species) => species is
        (int)Species.Metapod or
        (int)Species.Kakuna or
        (int)Species.Pineco or
        (int)Species.Silcoon or
        (int)Species.Cascoon or
        (int)Species.Shedinja or
        (int)Species.Spewpa;
}
