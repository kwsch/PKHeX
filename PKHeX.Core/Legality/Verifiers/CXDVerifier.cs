using static PKHeX.Core.LegalityCheckResultCode;

namespace PKHeX.Core;

/// <summary>
/// Verifies the specific origin data of <see cref="GameVersion.CXD"/> encounters.
/// </summary>
public sealed class CXDVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.Misc;

    public override void Verify(LegalityAnalysis data)
    {
        var pk = data.Entity;
        if (data.EncounterMatch is EncounterStatic3XD { Species: (ushort)Species.Eevee })
            VerifyStarterXD(data);
        // Colo starters are already hard-verified. No need to check them here.

        if (pk.OriginalTrainerGender == 1)
            data.AddLine(GetInvalid(CheckIdentifier.Trainer, G3OTGender));

        // Trainer ID is checked in another verifier. Don't duplicate it here.
    }

    private static void VerifyStarterXD(LegalityAnalysis data)
    {
        // The starter in XD must have the correct PIDIV type.
        var info = data.Info.PIDIV;
        if (info.Type is not (PIDType.CXD or PIDType.CXD_ColoStarter))
            return; // already flagged as invalid

        // Ensure the TID/SID match the expected result, as this isn't hard-checked earlier.
        var pk = data.Entity;

        bool valid = MethodCXD.TryGetSeedStarterXD(pk, out var seed);
        if (!valid)
            data.AddLine(GetInvalid(CheckIdentifier.PID, EncConditionBadRNGFrame));
        else
            data.Info.PIDIV = new PIDIV(PIDType.CXD, seed);
    }
}
