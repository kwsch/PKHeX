using System;

namespace PKHeX.Core;

internal static class LearnVerifierHistory
{
    public static void Verify(Span<MoveResult> result, ReadOnlySpan<ushort> current, IEncounterTemplate enc, PKM pk, EvolutionHistory history)
    {
        if (enc.Species is (int)Species.Smeargle)
        {
            VerifySmeargle(result, current, pk);
            return;
        }

        // Empty moves are valid (unless the game does not have a deleter -- handled later).
        MarkEmptySlots(result, current);

        // Basic gist of move source identification: check if the moves can be learned in the current format.
        // If moves are still unverified, we step backwards in time to the previous game environment, until all moves are checked.
        // If moves are STILL unverified, then they must not be legal.
        var game = LearnGroupUtil.GetCurrentGroup(pk);
        MarkAndIterate(result, current, enc, pk, history, game, MoveSourceType.All, LearnOption.Current);
    }

    public static void MarkAndIterate(Span<MoveResult> result, ReadOnlySpan<ushort> current, IEncounterTemplate enc, PKM pk, EvolutionHistory history, ILearnGroup game,
        MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (!pk.IsOriginalMovesetDeleted() && types.HasFlag(MoveSourceType.Encounter))
        {
            // Knock off relearn moves if available.
            if (pk.Format >= 6)
                MarkRelearnMoves(result, current, pk);

            // Knock off initial moves if available.
            if (enc.Generation != 2) // Handle trade-backs in Gen2 separately.
                MarkSpecialMoves(result, current, enc, pk);
        }

        // Iterate games to identify move sources.
        if (Iterate(result, current, pk, history, enc, game, types, option))
            return;

        // Mark remaining as unknown source.
        for (int i = 0; i < result.Length; i++)
        {
            if (!result[i].IsParsed)
                result[i] = MoveResult.Unobtainable();
        }
    }

    private static void MarkSpecialMoves(Span<MoveResult> result, ReadOnlySpan<ushort> current, IEncounterTemplate enc, PKM pk)
    {
        if (enc is IMoveset { Moves: { HasMoves: true } moves })
        {
            var ls = GameData.GetLearnSource(enc.Version);
            MarkInitialMoves(result, current, moves, ls.Environment);
        }
        else if (enc is EncounterSlot8GO { OriginFormat: PogoImportFormat.PK7 or PogoImportFormat.PB7 } g)
        {
            Span<ushort> initial = stackalloc ushort[4];
            g.GetInitialMoves(pk.MetLevel, initial);
            MarkInitialMoves(result, current, initial, g.OriginFormat == PogoImportFormat.PK7 ? LearnEnvironment.USUM : LearnEnvironment.GG);
        }
    }

    private static bool Iterate(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk, EvolutionHistory history, IEncounterTemplate enc, ILearnGroup game, MoveSourceType types, LearnOption option)
    {
        while (true)
        {
            bool finished = game.Check(result, current, pk, history, enc, types, option);
            if (finished)
                return true;

            var next = game.GetPrevious(pk, history, enc, option);
            if (next is null)
                return false;

            game = next;
        }
    }

    public static void MarkInitialMoves(Span<MoveResult> result, ReadOnlySpan<ushort> current, Moveset moves, LearnEnvironment game)
    {
        // If the initial move is present in the current moves, mark that current move index as an initial move.
        if (moves.Move1 == 0) return;
        var index = current.IndexOf(moves.Move1); if (index != -1) result[index] = MoveResult.Initial(game);

        if (moves.Move2 == 0) return;
        index = current.IndexOf(moves.Move2); if (index != -1) result[index] = MoveResult.Initial(game);

        if (moves.Move3 == 0) return;
        index = current.IndexOf(moves.Move3); if (index != -1) result[index] = MoveResult.Initial(game);

        if (moves.Move4 == 0) return;
        index = current.IndexOf(moves.Move4); if (index != -1) result[index] = MoveResult.Initial(game);
    }

    public static void MarkInitialMoves(Span<MoveResult> result, ReadOnlySpan<ushort> current, ReadOnlySpan<ushort> moves, LearnEnvironment game)
    {
        // If the initial move is present in the current moves, mark that current move index as an initial move.
        foreach (var move in moves)
        {
            if (move == 0)
                break;
            var index = current.IndexOf(move);
            if (index != -1)
                result[index] = MoveResult.Initial(game);
        }
    }

    private static void MarkEmptySlots(Span<MoveResult> result, ReadOnlySpan<ushort> current)
    {
        // Iterate from last move, marking empty slots, until we hit a non-zero move ID.
        for (int i = current.Length - 1; i >= 0; i--)
        {
            var move = current[i];
            if (move != 0)
                return;
            result[i] = MoveResult.Empty;
        }
    }

    private static void VerifySmeargle(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk)
    {
        for (int i = current.Length - 1; i >= 0; i--)
        {
            var move = current[i];
            if (move == 0)
                result[i] = MoveResult.Empty;
            else if (MoveInfo.IsSketchValid(move, pk.Context))
                result[i] = MoveResult.Sketch;
            else
                result[i] = MoveResult.Unobtainable();
        }
    }

    private static void MarkRelearnMoves(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk)
    {
        // Check if any of the current moves can be relearned.
        for (int i = 0; i < current.Length; i++)
        {
            var move = pk.GetRelearnMove(i);
            if (move == 0) // No more relearn moves.
                return;

            var index = current.IndexOf(move);
            if (index == -1) // Not a relearn move.
                continue;

            result[index] = MoveResult.Relearn;
        }
    }
}
