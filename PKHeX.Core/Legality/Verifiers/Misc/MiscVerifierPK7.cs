using static PKHeX.Core.LegalityCheckResultCode;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core;

internal sealed class MiscVerifierPK7 : Verifier
{
    protected override CheckIdentifier Identifier => Misc;

    public override void Verify(LegalityAnalysis data)
    {
        if (data.Entity is PK7 pk)
            Verify(data, pk);
    }

    internal void Verify(LegalityAnalysis data, PK7 pk)
    {
        FullnessRules.Verify(data, pk);

        if (pk.ResortEventStatus >= ResortEventState.MAX)
            data.AddLine(GetInvalid(TransferBad));
    }
}
