using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core;

/// <summary>
/// Verifies the <see cref="PKM.ID32"/>.
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
            // Game loops to ensure a nonzero full-ID
            // int.MaxValue cannot be yielded by Unity's Random.Range[min, max)
            var id32 = pk.ID32;
            if (id32 is 0 or int.MaxValue)
            {
                data.AddLine(GetInvalid(LOT_IDInvalid));
                return;
            }
        }
        else if (pk.VC)
        {
            // Only TID is used for Gen 1/2 VC
            if (pk.SID16 != 0)
                data.AddLine(GetInvalid(LOT_SID0Invalid));
            if (pk.TID16 == 0)
                data.AddLine(Get(LOT_TID0, Severity.Fishy));
            return;
        }
        else if (pk.Format <= 2)
        {
            // Only TID is used for Gen 1/2
            if (pk.TID16 == 0)
                data.AddLine(Get(LOT_TID0, Severity.Fishy));
            return;
        }

        if (pk is { ID32: 0 })
            data.AddLine(Get(LOT_IDs0, Severity.Fishy));
        else if (pk.TID16 == pk.SID16)
            data.AddLine(Get(LOT_IDEqual, Severity.Fishy));
        else if (pk.TID16 == 0)
            data.AddLine(Get(LOT_TID0, Severity.Fishy));
        else if (pk.SID16 == 0)
            data.AddLine(Get(LOT_SID0, Severity.Fishy));
        else if (IsOTIDSuspicious(pk.TID16, pk.SID16))
            data.AddLine(Get(LOTSuspicious, Severity.Fishy));
    }

    public static bool IsOTIDSuspicious(ushort tid16, ushort sid16) => (tid16, sid16) switch
    {
        (12345, 54321) => true,
        (15040, 18831) => true, // 1234_123456 (SID7_TID7)
        _ => false,
    };
}
