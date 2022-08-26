using System;

namespace PKHeX.Core;

/// <summary>
/// Top-level current move verifier.
/// </summary>
internal static class LearnVerifier
{
    private static readonly MoveResult Duplicate = new(LearnMethod.Duplicate);
    private static readonly MoveResult EmptyInvalid = new(LearnMethod.EmptyInvalid);

    public static void Verify(Span<MoveResult> result, PKM pk, IEncounterTemplate enc, EvolutionHistory history)
    {
        // Clear any existing parse results.
        result.Clear();

        // Load moves.
        Span<ushort> current = stackalloc ushort[4];
        pk.GetMoves(current);

        // Verify the moves.
        VerifyMoves(result, current, pk, enc, history);

        // Finalize the checks.
        Finalize(result, current);
    }

    private static void VerifyMoves(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk, IEncounterTemplate enc, EvolutionHistory history)
    {
        if (pk.IsEgg)
            LearnVerifierEgg.Verify(result, current, enc, pk);
        else
            LearnVerifierHistory.Verify(result, current, enc, pk, history);
    }

    private static void Finalize(Span<MoveResult> result, ReadOnlySpan<ushort> current)
    {
        // Flag duplicate move indexes.
        VerifyNoEmptyDuplicates(result, current);

        // Can't have empty first move.
        if (current[0] == 0)
            result[0] = EmptyInvalid;
    }

    private static void VerifyNoEmptyDuplicates(Span<MoveResult> result, ReadOnlySpan<ushort> current)
    {
        bool emptySlot = false;
        for (int i = 0; i < result.Length; i++)
        {
            var move = current[i];
            if (move == 0)
            {
                emptySlot = true;
                continue;
            }

            // If an empty slot was noted for a prior move, flag the empty slots.
            if (emptySlot)
            {
                FlagEmptySlotsBeforeIndex(result, current, i);
                emptySlot = false;
                continue;
            }

            // Check for same move in next move slots
            FlagDuplicateMovesAfterIndex(result, current, i, move);
        }
    }

    private static void FlagDuplicateMovesAfterIndex(Span<MoveResult> result, ReadOnlySpan<ushort> current, int index, ushort move)
    {
        for (int i = result.Length - 1; i > index; i--)
        {
            if (current[i] != move)
                continue;
            result[index] = Duplicate;
            return;
        }
    }

    private static void FlagEmptySlotsBeforeIndex(Span<MoveResult> result, ReadOnlySpan<ushort> current, int index)
    {
        for (int i = index - 1; i >= 0; i--)
        {
            if (current[i] != 0)
                return;
            result[i] = EmptyInvalid;
        }
    }
}
