using System;
using static PKHeX.Core.LegalityCheckResultCode;

namespace PKHeX.Core;

/// <summary>
/// Verifies the legality of Awakened Values (AVs) for Pokémon in Generation 7 (PB7).
/// Ensures that AVs and Effort Values (EVs) are within legal bounds for the species and context.
/// </summary>
public sealed class AwakenedValueVerifier : Verifier
{
    // Specifies that this verifier is for Awakened Values (AVs)
    protected override CheckIdentifier Identifier => CheckIdentifier.AVs;

    /// <summary>
    /// Performs legality checks on the AVs and EVs of a PB7 Pokémon.
    /// </summary>
    /// <param name="data">Legality analysis context containing the Pokémon entity.</param>
    public override void Verify(LegalityAnalysis data)
    {
        // Only check features relevant to LGP/E format.
        if (data.Entity is not PB7 pb7)
            return;

        // Can't obtain EVs in the game; only AVs.
        int sum = pb7.EVTotal;
        if (sum != 0)
            data.AddLine(GetInvalid(EffortShouldBeZero));

        // Check that all AVs are within the allowed cap
        if (!pb7.AwakeningAllValid())
            data.AddLine(GetInvalid(AwakenedCap));

        // Gather all AVs. When leveling up, AVs are "randomly" granted, so a mon must be at or above.
        Span<byte> required = stackalloc byte[6];
        AwakeningUtil.SetExpectedMinimumAVs(pb7, required);
        Span<byte> current = stackalloc byte[6];
        pb7.AwakeningGetVisual(current);

        // For each stat, ensure the current AV is at least the required minimum
        if (current[0] < required[0])
            data.AddLine(GetInvalid(Identifier, AwakenedStatGEQ_01, required[0], 0)); // HP
        if (current[1] < required[1])
            data.AddLine(GetInvalid(Identifier, AwakenedStatGEQ_01, required[1], 1)); // Atk
        if (current[2] < required[2])
            data.AddLine(GetInvalid(Identifier, AwakenedStatGEQ_01, required[2], 2)); // Def
        if (current[3] < required[3])
            data.AddLine(GetInvalid(Identifier, AwakenedStatGEQ_01, required[3], 4)); // SpA
        if (current[4] < required[4])
            data.AddLine(GetInvalid(Identifier, AwakenedStatGEQ_01, required[4], 5)); // SpD
        if (current[5] < required[5])
            data.AddLine(GetInvalid(Identifier, AwakenedStatGEQ_01, required[5], 3)); // Speed
    }
}
