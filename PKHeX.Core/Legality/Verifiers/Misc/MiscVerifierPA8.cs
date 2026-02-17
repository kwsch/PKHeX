using static PKHeX.Core.LegalityCheckResultCode;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core;

internal sealed class MiscVerifierPA8 : Verifier
{
    private static readonly LegendsArceusVerifier Arceus = new();

    protected override CheckIdentifier Identifier => Misc;

    public override void Verify(LegalityAnalysis data)
    {
        if (data.Entity is PA8 pk)
            Verify(data, pk);
    }

    internal void Verify(LegalityAnalysis data, PA8 pk)
    {
        Arceus.Verify(data);
        MiscVerifierHelpers.VerifyStatNature(data, pk);
        MiscVerifierHelpers.VerifyAbsoluteSizes(data, pk);
        MiscVerifierPK8.VerifyTechRecordFlags(data, pk); // copied from SW/SH via HOME
        FullnessRules.Verify(data, pk);

        var social = pk.Sociability;
        if (social != 0)
            data.AddLine(GetInvalid(Encounter, MemorySocialZero));

        if (!pk.IsBattleVersionValid(data.Info.EvoChainsAllGens))
            data.AddLine(GetInvalid(StatBattleVersionInvalid));

        if (pk.CanGigantamax)
            data.AddLine(GetInvalid(StatGigantamaxInvalid));

        if (pk.DynamaxLevel != 0)
            data.AddLine(GetInvalid(StatDynamaxInvalid));

        if (pk.GetMoveRecordFlagAny() && !pk.IsEgg) // already checked for eggs
            data.AddLine(GetInvalid(EggRelearnFlags));
    }
}
