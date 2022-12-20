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
            if (pk is { TID16: 0, SID16: 0 }) // Game loops to ensure a nonzero full-ID
            {
                data.AddLine(GetInvalid(LOT_IDInvalid));
                return;
            }
            if (pk is { SID16: 0x7FFF, TID16: 0xFFFF }) // int.MaxValue cannot be yielded by Unity's Random.Range[min, max)
            {
                data.AddLine(GetInvalid(LOT_IDInvalid));
                return;
            }
        }
        else if (pk.VC && pk.SID16 != 0)
        {
            data.AddLine(GetInvalid(LOT_SID0Invalid));
            return;
        }

        if (pk is { TID16: 0, SID16: 0 })
        {
            data.AddLine(Get(LOT_IDs0, Severity.Fishy));
        }
        else if (pk.TID16 == pk.SID16)
        {
            data.AddLine(Get(LOT_IDEqual, Severity.Fishy));
        }
        else if (pk.TID16 == 0)
        {
            data.AddLine(Get(LOT_TID0, Severity.Fishy));
        }
        else if (pk.SID16 == 0)
        {
            data.AddLine(Get(LOT_SID0, Severity.Fishy));
        }
        else if (IsOTIDSuspicious(pk.TID16, pk.SID16))
        {
            data.AddLine(Get(LOTSuspicious, Severity.Fishy));
        }
    }

    public static bool IsOTIDSuspicious(uint tid16, uint sid16) => (tid16, sid16) switch
    {
        (12345, 54321) => true,
        (15040, 18831) => true, // 1234_123456 (SID7_TID7)
        _ => false,
    };
}
