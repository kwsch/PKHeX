using static PKHeX.Core.LegalityCheckResultCode;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core;

internal sealed class EggVerifier : Verifier
{
    protected override CheckIdentifier Identifier => Misc;

    public override void Verify(LegalityAnalysis data)
    {
        var pk = data.Entity;
        if (pk.IsEgg)
            Verify(data, pk);
    }

    internal void Verify(LegalityAnalysis data, PKM pk)
    {
        VerifyCommon(data, pk);

        // No egg have contest stats from the encounter.
        if (pk is IContestStatsReadOnly s && s.HasContestStats())
            data.AddLine(GetInvalid(Egg, EggContest));

        // Cannot transfer eggs across contexts (must be hatched).
        var e = data.EncounterOriginal;
        if (e.Context != pk.Context)
            data.AddLine(GetInvalid(Egg, TransferEggVersion));

        switch (pk)
        {
            // Side Game: No Eggs
            case SK2 or CK3 or XK3 or BK4 or RK4 when e.Context == pk.Context:
                data.AddLine(GetInvalid(Egg, TransferEggVersion));
                break;

            // All Eggs are Japanese and flagged specially for localized string
            case PK3 when pk.Language != 1:
                data.AddLine(GetInvalid(Egg, OTLanguageShouldBe_0, (byte)LanguageID.Japanese));
                break;
        }

        if (pk is IHomeTrack { HasTracker: true })
            data.AddLine(GetInvalid(TransferTrackerShouldBeZero));
    }

    internal void VerifyCommon(LegalityAnalysis data, PKM pk)
    {
        var enc = data.EncounterMatch;
        if (!EggStateLegality.GetIsEggHatchCyclesValid(pk, enc))
            data.AddLine(GetInvalid(Egg, EggHatchCycles));

        if (pk.Format >= 6 && enc is IEncounterEgg && !MovesMatchRelearn(pk))
            data.AddLine(GetInvalid(Egg, MovesShouldMatchRelearnMoves));

        if (pk is ITechRecord record)
        {
            if (record.GetMoveRecordFlagAny())
                data.AddLine(GetInvalid(Egg, EggRelearnFlags));
            if (pk.StatNature != pk.Nature)
                data.AddLine(GetInvalid(Egg, EggNature));
        }
    }

    private static bool MovesMatchRelearn(PKM pk)
    {
        if (pk.Move1 != pk.RelearnMove1)
            return false;
        if (pk.Move2 != pk.RelearnMove2)
            return false;
        if (pk.Move3 != pk.RelearnMove3)
            return false;
        if (pk.Move4 != pk.RelearnMove4)
            return false;
        return true;
    }
}
