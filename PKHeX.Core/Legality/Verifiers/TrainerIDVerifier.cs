using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core;

/// <summary>
/// Verifies the <see cref="PKM.OT_Name"/>.
/// </summary>
public sealed class TrainerIDVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.Trainer;

    public override void Verify(LegalityAnalysis data)
    {
        var pk = data.Entity;
        if (!TrainerNameVerifier.IsPlayerOriginalTrainer(data.EncounterMatch))
            return; // already verified

        if (pk.BDSP)
        {
            if (pk.TID == 0 && pk.SID == 0) // Game loops to ensure a nonzero full-ID
            {
                data.AddLine(GetInvalid(LOT_IDInvalid));
                return;
            }
            if (pk.TID == 0xFFFF && pk.SID == 0x7FFF) // int.MaxValue cannot be yielded by Unity's Random.Range[min, max)
            {
                data.AddLine(GetInvalid(LOT_IDInvalid));
                return;
            }
        }
        else if (pk.VC && pk.SID != 0)
        {
            data.AddLine(GetInvalid(LOT_SID0Invalid));
            return;
        }

        if (pk.TID == 0 && pk.SID == 0)
        {
            data.AddLine(Get(LOT_IDs0, Severity.Fishy));
        }
        else if (pk.TID == pk.SID)
        {
            data.AddLine(Get(LOT_IDEqual, Severity.Fishy));
        }
        else if (pk.TID == 0)
        {
            data.AddLine(Get(LOT_TID0, Severity.Fishy));
        }
        else if (pk.SID == 0)
        {
            data.AddLine(Get(LOT_SID0, Severity.Fishy));
        }
        else if (IsOTIDSuspicious(pk.TID, pk.SID))
        {
            data.AddLine(Get(LOTSuspicious, Severity.Fishy));
        }
    }

    public static bool IsOTIDSuspicious(int tid16, int sid16)
    {
        if (tid16 == 12345 && sid16 == 54321)
            return true;

        // 1234_123456 (SID7_TID7)
        if (tid16 == 15040 && sid16 == 18831)
            return true;

        return false;
    }
}
