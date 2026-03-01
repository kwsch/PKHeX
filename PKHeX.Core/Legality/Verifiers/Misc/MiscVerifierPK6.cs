using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core;

internal sealed class MiscVerifierPK6 : Verifier
{
    protected override CheckIdentifier Identifier => Misc;

    public override void Verify(LegalityAnalysis data)
    {
        if (data.Entity is PK6 pk)
            Verify(data, pk);
    }

    internal void Verify(LegalityAnalysis data, PK6 pk)
    {
        FullnessRules.Verify(data, pk);
    }
}
