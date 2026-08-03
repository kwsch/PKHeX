using System;
using static PKHeX.Core.LegalityCheckResultCode;
using static PKHeX.Core.CheckIdentifier;

namespace PKHeX.Core;

internal sealed class EggVerifier : Verifier
{
    protected override CheckIdentifier Identifier => Misc;

    public override void Verify(LegalityAnalysis data)
    {
        var pk = data.Entity;
        if (data.EncounterOriginal is IEncounterEgg egg)
            VerifyEggBreedingChain(data, pk, egg);
        if (pk.IsEgg)
            VerifyWhileEgg(data, pk);
    }

    private void VerifyWhileEgg(LegalityAnalysis data, PKM pk)
    {
        // Not hatched yet, must have sane properties while still an egg.
        var enc = data.EncounterOriginal;
        VerifyCommon(data, pk, enc);

        // No egg have contest stats from the encounter.
        if (pk is IContestStatsReadOnly s && s.HasContestStats())
            data.AddLine(GetInvalid(Egg, EggContest));

        // Cannot transfer eggs across contexts (must be hatched).
        if (enc.Context != pk.Context)
            data.AddLine(GetInvalid(Egg, TransferEggVersion));

        switch (pk)
        {
            // Side Game: No Eggs
            case SK2 or CK3 or XK3 or BK4 or RK4 when enc.Context == pk.Context: // same context to not double-flag
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

    internal void VerifyCommon(LegalityAnalysis data, PKM pk, IEncounterTemplate enc)
    {
        if (!EggStateLegality.GetIsEggHatchCyclesValid(pk, enc))
            data.AddLine(GetInvalid(Egg, EggHatchCycles));

        if (pk.Format >= 6 && enc is IEncounterEgg && !MovesMatchRelearn(pk))
            data.AddLine(GetInvalid(Egg, MovesShouldMatchRelearnMoves));

        if (pk is ITechRecord record)
        {
            if (record.GetMoveRecordFlagAny())
                data.AddLine(GetInvalid(Egg, EggRelearnFlags));
            if (pk.StatAlignment != pk.Nature)
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

    private void VerifyEggBreedingChain(LegalityAnalysis data, PKM pk, IEncounterEgg egg)
    {
        // Check if we have any moves that are as a result of breeding chain.
        Span<ushort> moves = stackalloc ushort[4];
        if (egg.Generation >= 6)
            pk.GetRelearnMoves(moves);
        else
            pk.GetMoves(moves);
        var personal = GameData.GetPersonal(egg.Version)[egg.Species, egg.Form];
        var includeInheritedLevelUp = personal.OnlyFemale
            || Breeding.IsGenderSpeciesDetermination(egg.Species);

        var source = egg.Generation >= 6 ? data.Info.Relearn : data.Info.Moves;
        var count = GatherInheritedMoves(moves, source, includeInheritedLevelUp);
        if (count == 0 || (count == 1 && !includeInheritedLevelUp))
            return;
        moves = moves[..count];

        if (!ChainBreedLegality.IsValid(egg.Species, egg.Form, egg.Version, moves))
            data.AddLine(GetInvalid(Egg, EggMoveCombination));
        else
            data.AddLine(GetValid(EggBreedChain_0));
    }

    private static int GatherInheritedMoves(Span<ushort> moves, ReadOnlySpan<MoveResult> parse, bool includeInheritedLevelUp)
    {
        // Collapse the list of moves to only those that are relevant for breeding chain verification.
        int count = 0;
        for (int i = 0; i < moves.Length; i++)
        {
            if (moves[i] == 0)
                break;

            if (!IsInherited(parse[i], includeInheritedLevelUp))
                continue;

            moves[count] = moves[i];
            count++;
        }
        return count;
    }

    private static bool IsInherited(MoveResult parsed, bool includeInheritedLevelUp)
    {
        var method = parsed.Info.Method;
        return method is LearnMethod.EggMove
            || (includeInheritedLevelUp && method is LearnMethod.InheritLevelUp);
    }
}
