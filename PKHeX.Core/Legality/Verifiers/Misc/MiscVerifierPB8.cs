using static PKHeX.Core.LegalityCheckResultCode;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core;

internal sealed class MiscVerifierPB8 : Verifier
{
    protected override CheckIdentifier Identifier => Misc;

    public override void Verify(LegalityAnalysis data)
    {
        if (data.Entity is PB8 pk)
            Verify(data, pk);
    }

    internal void Verify(LegalityAnalysis data, PB8 pk)
    {
        MiscVerifierHelpers.VerifyStatNature(data, pk);
        MiscVerifierPK8.VerifyTechRecordFlags(data, pk); // copied from SW/SH via HOME
        FullnessRules.Verify(data, pk);

        var social = pk.Sociability;
        if (social != 0)
            data.AddLine(GetInvalid(Encounter, MemorySocialZero));

        if (pk.IsDprIllegal)
            data.AddLine(GetInvalid(TransferFlagIllegal));
        if (pk.Species is (int)Species.Spinda or (int)Species.Nincada && !pk.BDSP)
            data.AddLine(GetInvalid(TransferNotPossible));
        if (pk.Species is (int)Species.Spinda && pk.Tracker != 0)
            data.AddLine(GetInvalid(TransferTrackerShouldBeZero));

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
