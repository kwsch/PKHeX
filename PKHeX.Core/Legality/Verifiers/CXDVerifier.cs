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
        if (data.EncounterMatch is EncounterStatic3Colo { IsColoStarter: true })
            VerifyStarterColo(data);
        else if (data.EncounterMatch is EncounterStatic3XD { Species: (ushort)Species.Eevee })
            VerifyStarterXD(data);

        if (pk.OriginalTrainerGender == 1)
            data.AddLine(GetInvalid(LG3OTGender, CheckIdentifier.Trainer));
    }

    private static void VerifyStarterColo(LegalityAnalysis data)
    {
        var type = data.Info.PIDIV.Type;
        if (type is not (PIDType.CXD or PIDType.CXDAnti or PIDType.CXD_ColoStarter))
            return; // already flagged as invalid
        if (type != PIDType.CXD_ColoStarter)
            data.AddLine(GetInvalid(LEncConditionBadRNGFrame, CheckIdentifier.PID));
    }

    private static void VerifyStarterXD(LegalityAnalysis data)
    {
        var info = data.Info.PIDIV;
        if (info.Type is not (PIDType.CXD or PIDType.CXDAnti or PIDType.CXD_ColoStarter))
            return; // already flagged as invalid

        bool valid;
        var pk = data.Entity;
        if (info.Type == PIDType.CXD_ColoStarter && pk.Species == (int)Species.Umbreon)
        {
            // reset pidiv type to be CXD -- ColoStarter is same correlation as Eevee->Umbreon
            data.Info.PIDIV = new PIDIV(PIDType.CXD, info.OriginSeed);
            valid = true;
        }
        else
        {
            var seed = info.OriginSeed;
            valid = LockFinder.IsXDStarterValid(seed, pk.TID16, pk.SID16);
            if (valid) // unroll seed to origin that generated TID16/SID16->pkm
                data.Info.PIDIV = new PIDIV(PIDType.CXD, seed) { EncounterSeed = XDRNG.Prev4(seed) };
        }
        if (!valid)
            data.AddLine(GetInvalid(LEncConditionBadRNGFrame, CheckIdentifier.PID));
    }
}
