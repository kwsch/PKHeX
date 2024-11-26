using System;
using System.Runtime.InteropServices;
using static PKHeX.Core.LegalityCheckStrings;

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

        if (pk.OriginalTrainerGender == 1)
            data.AddLine(GetInvalid(LG3OTGender, CheckIdentifier.Trainer));
    }

    private static void VerifyStarterXD(LegalityAnalysis data)
    {
        var info = data.Info.PIDIV;
        if (info.Type is not (PIDType.CXD or PIDType.CXD_ColoStarter))
            return; // already flagged as invalid

        // Ensure the TID/SID match the expected result, as this isn't hard-checked earlier.
        var pk = data.Entity;
        Span<int> ivs = stackalloc int[6];
        pk.GetIVs(ivs);

        var u32 = MemoryMarshal.Cast<int, uint>(ivs);
        bool valid = MethodCXD.TryGetOriginSeedStarterXD(pk, u32, out var seed);
        if (!valid)
            data.AddLine(GetInvalid(LEncConditionBadRNGFrame, CheckIdentifier.PID));
        else
            data.Info.PIDIV = new PIDIV(PIDType.CXD, seed);
    }
}
