using System;
using static PKHeX.Core.LegalityCheckResultCode;

namespace PKHeX.Core;

public sealed class LegendsZAVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.RelearnMove;

    public override void Verify(LegalityAnalysis data)
    {
        if (data.Entity is not PA9 pa9)
            return;
        CheckLearnset(data, pa9);
        CheckFlagsTM(data, pa9);
        CheckFlagsPlus(data, pa9);
    }

    private void CheckLearnset(LegalityAnalysis data, PA9 pa)
    {
        if (data.EncounterMatch is not IEncounter9a e9a)
            return; // Don't bother.

        var moveCount = pa.MoveCount;
        if (moveCount == 4)
            return;

        // Flag move slots that are empty.
        if (pa.Tracker != 0 || !ParseSettings.IgnoreTransferIfNoTracker)
            return; // Can delete moves in PA8 moveset via HOME.

        // Get the bare minimum moveset.
        Span<ushort> expect = stackalloc ushort[4];
        _ = LoadBareMinimumMoveset(e9a, pa, expect);

        // Expected move can be empty due to user rearranging.
        // Account for player rearrangement of moves.
        var moves = data.Info.Moves;
        for (int i = 0; i < moves.Length; i++)
        {
            var currentMove = pa.GetMove(i);
            var index = expect.IndexOf(currentMove);
            if (index != -1 && index != i) // Swapped move. Swap the expected slot.
                (expect[i], expect[index]) = (expect[index], expect[i]);
        }

        // Now that we've rearranged the expected moves to matching slots (if any), flag any mismatches.
        // Basically only will flag "(None) => Expected {non-zero-move} instead."
        for (int i = 0; i < expect.Length; i++)
        {
            var move = expect[i];
            if (pa.GetMove(i) != move)
                moves[i] = MoveResult.Unobtainable(move);
        }
    }

    /// <summary>
    /// Gets the expected minimum count of moves, and modifies the input <see cref="moves"/> with the bare minimum move IDs.
    /// </summary>
    private static int LoadBareMinimumMoveset(IEncounter9a enc, PA9 pa, Span<ushort> moves)
    {
        // Get any encounter moves
        var ls = LearnSource8LA.Instance;
        var moveset = ls.GetLearnset(enc.Species, enc.Form);
        GetInitialMoves(enc, pa, moves);
        var count = moves.IndexOf((ushort)0);
        if ((uint)count >= 4)
            return 4;

        // If it can be leveled up in other games, level it up in other games.
        if (pa is IHomeTrack { HasTracker: true })
            return count;

        // Level up moves never repeat, so just level up to current level.
        var ms = moveset.GetMoveRange(pa.CurrentLevel, (byte)(pa.MetLevel + 1));
        foreach (var move in ms)
        {
            if (moves.Contains(move)) // just in case.
                continue;
            moves[count++] = move;
            if ((uint)count >= 4)
                return 4;
        }

        // Check if any TM/Evo/Relearn moves were learned. They'll be anything we've not been able to learn yet.
        // Don't check the validity of a foreign move.
        for (int i = 0; i < 4; i++)
        {
            var move = pa.GetMove(i);
            if (move == 0)
                continue;
            if (moves.Contains(move))
                continue;
            moves[count++] = move;
            if ((uint)count >= 4)
                return 4;
        }

        // No other tutor sources.
        return count;
    }

    private static void GetInitialMoves(IEncounter9a enc, PA9 pa9, Span<ushort> moves)
    {
        if (enc is IMoveset { Moves: { HasMoves: true } m })
        {
            m.CopyTo(moves);
            return;
        }
        var level = Math.Max((byte)1, pa9.MetLevel);
        var learn = LearnSource9ZA.Instance.GetLearnset(enc.Species, enc.Form);
        if (!enc.IsAlpha)
        {
            learn.SetEncounterMoves(level, moves);
            return;
        }
        learn.SetEncounterMovesBackwards(level, moves, sameDescend: false);
        moves[0] = PersonalTable.ZA[enc.Species, enc.Form].AlphaMove;
    }

    private void CheckFlagsTM(LegalityAnalysis data, PA9 pa9)
    {
        // Wild Alphas automatically come with a specific move.
        var enc = data.EncounterMatch;
        if (enc.Context is not EntityContext.Gen9a)
            return;
        if (enc is not IAlphaReadOnly { IsAlpha: true })
            return;

        var pi = PersonalTable.ZA[enc.Species, enc.Form];
        var move = pi.AlphaMove;
        var indexPlus = PersonalInfo9ZA.PlusMoves.IndexOf(move);

        if (indexPlus != -1)
        {
            if (!pa9.GetMovePlusFlag(indexPlus))
                data.AddLine(GetInvalid(PlusMoveAlphaMissing_0, move));
        }
    }


    private void CheckFlagsPlus(LegalityAnalysis la, PA9 pk)
    {
        var permit = (IPermitPlus)la.PersonalInfo;

        // Check for any impossible-to-set flag indexes.
        if (pk.GetMovePlusFlagAnyImpossible())
            la.AddLine(GetInvalid(PlusMoveCountInvalid));

        // Check for all required indexes.
        var (learn, plus) = LearnSource9ZA.GetLearnsetAndPlus(pk.Species, pk.Form);
        var currentLevel = pk.CurrentLevel;
        CheckPlusMoveFlags(la, pk, permit, plus, currentLevel);


        // Check for indexes set that cannot be set via TM or NPC.
        int max = permit.PlusCountUsed;
        var evos = la.Info.EvoChainsAllGens.Gen9a;
        var invalidMove = GetInvalidPlusMove(pk, max, permit, evos);
        if (invalidMove == 0)
            return;

        // The above result stores the first invalid move, or a magic value for multiple invalid moves.
        var msg = invalidMove is MultipleInvalidPlusMoves
            ? GetInvalid(PlusMoveMultipleInvalid)
            : GetInvalid(PlusMoveInvalid_0, invalidMove);
        la.AddLine(msg);
    }

    private void CheckPlusMoveFlags<T>(LegalityAnalysis la, T pk, IPermitPlus permit, Learnset plus, byte currentLevel) where T : IPlusRecord
    {
        var levels = plus.GetAllLevels();
        var moves = plus.GetAllMoves();
        for (int i = 0; i < levels.Length; i++)
        {
            var level = levels[i];
            if (level > currentLevel)
                break; // not able to be Plus'd, therefore no need to check.

            var move = moves[i];
            var index = permit.PlusMoveIndexes.IndexOf(move);
            if (index == -1)
                throw new IndexOutOfRangeException("Unexpected learn move index, not in Plus moves?");

            if (pk.GetMovePlusFlag(index))
                continue; // All good, flagged.

            // Trade evolutions forget to set the Plus flags, unlike triggered evolutions.
            // If the move is not present as a previous-evolution learnset move, and the head species is a Trade evo, skip the error.
            // Assume the best case -- evolved at current level, so none would get set.
            if (IsTradeEvoSkip(la.Info.EvoChainsAllGens.Gen9a, move))
                continue;

            la.AddLine(GetInvalid(PlusMoveSufficientLevelMissing_0, move, level));
        }
    }

    private static bool IsTradeEvoSkip(ReadOnlySpan<EvoCriteria> evos, ushort move)
    {
        if (evos.Length <= 1)
            return false;

        if (!evos[0].Method.IsTrade())
            return false;

        // Check if the pre-evolution could have learned it before evolving.
        for (int i = 1; i < evos.Length; i++)
        {
            var evo = evos[i];
            var (_, plus) = LearnSource9ZA.GetLearnsetAndPlus(evo.Species, evo.Form);
            var moves = plus.GetAllMoves();

            var index = moves.IndexOf(move);
            if (index == -1)
                continue; // can't learn

            // if the evo must have traversed this level range (and not the head's level range), then it must have been flagged.
            var levels = plus.GetAllLevels();
            var plusLevel = levels[index];
            var headLevel = evos[0].LevelMin;
            if (plusLevel <= headLevel)
                return false;
        }
        return true;
    }

    private const ushort MultipleInvalidPlusMoves = ushort.MaxValue;

    private static ushort GetInvalidPlusMove<T>(T pk, int maxIndex, IPermitPlus permit, ReadOnlySpan<EvoCriteria> evos)
        where T : IPlusRecord
    {
        ushort invalid = 0;
        for (int i = 0; i < maxIndex; i++)
        {
            if (!pk.GetMovePlusFlag(i))
                continue;
            var move = permit.PlusMoveIndexes[i];
            var index = permit.RecordPermitIndexes.IndexOf(move);
            if (CanAnyEvoLearnMovePlus<PersonalTable9ZA, PersonalInfo9ZA, LearnSource9ZA>(evos, index, move, PersonalTable.ZA, LearnSource9ZA.Instance))
                continue; // OK

            if (invalid != 0) // Multiple invalid moves
                return MultipleInvalidPlusMoves;
            invalid = move;
        }
        return invalid;
    }

    private static bool CanAnyEvoLearnMovePlus<TTable, TInfo, TSource>(ReadOnlySpan<EvoCriteria> evos, int tmIndex, ushort move,
        TTable table, TSource source)
        where TTable : IPersonalTable<TInfo>
        where TInfo : IPersonalInfo, IPersonalInfoTM
        where TSource : ILearnSourceBonus
    {
        foreach (var evo in evos)
        {
            // If the move can be learned as TM, can be marked as Plus Move regardless of level.
            var pi = table[evo.Species, evo.Form];
            if (tmIndex != -1 && pi.GetIsLearnTM(tmIndex))
                return true;

            // If the move can be learned via learnset, check if the level is at or above the Plus required level.
            var (_, plus) = source.GetLearnsetAndOther(evo.Species, evo.Form);
            if (plus.TryGetLevelLearnMove(move, out var level) && level <= evo.LevelMax)
                return true;
        }
        return false;
    }
}
