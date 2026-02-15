using static PKHeX.Core.LegalityCheckResultCode;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core;

internal sealed class MiscVerifierPA9 : Verifier
{
    private static readonly LegendsZAVerifier LegendsZA = new();

    protected override CheckIdentifier Identifier => Misc;

    public override void Verify(LegalityAnalysis data)
    {
        if (data.Entity is PA9 pk)
            Verify(data, pk);
    }

    internal void Verify(LegalityAnalysis data, PA9 pa9)
    {
        MiscVerifierHelpers.VerifyStatNature(data, pa9);

        LegendsZA.Verify(data);
        if (!pa9.IsBattleVersionValid(data.Info.EvoChainsAllGens))
            data.AddLine(GetInvalid(StatBattleVersionInvalid));
        if (!MiscVerifierHelpers.IsObedienceLevelValid(pa9, pa9.ObedienceLevel, pa9.MetLevel))
            data.AddLine(GetInvalid(TransferObedienceLevel));
        if (pa9.IsAlpha != data.EncounterMatch is IAlphaReadOnly { IsAlpha: true })
            data.AddLine(GetInvalid(StatAlphaInvalid));
    }
}
