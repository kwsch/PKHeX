using System;
using static PKHeX.Core.LearnMethod;

namespace PKHeX.Core;

/// <summary>
/// Top-level current move verifier.
/// </summary>
internal static class LearnVerifier
{
    private static readonly MoveResult Duplicate = new(new(LearnMethod.Duplicate, GameVersion.Any), 0);
    private static readonly MoveResult EmptyInvalid = new(new(LearnMethod.EmptyInvalid, GameVersion.Any), 0);

    public static void Verify(Span<MoveResult> result, PKM pk, IEncounterTemplate enc)
    {
        // Clear any existing parse results.
        result.Clear();

        // Load moves.
        Span<int> current = stackalloc int[4];
        pk.GetMoves(current);

        // Verify the moves.
        VerifyMoves(result, current, pk, enc);

        // Finalize the checks.
        Finalize(result, current);
    }

    private static void VerifyMoves(Span<MoveResult> result, Span<int> current, PKM pk, IEncounterTemplate enc)
    {
        // If relearn moves are present for the format, use them first.
        if (pk.Format >= 6)
        {
            if (pk.IsEgg)
            {
                VerifyEggMoves(result, pk, enc);
                return;
            }
            MarkRelearnMoves(result, pk, current, enc);
        }
    }

    private static void VerifyEggMoves(Span<MoveResult> result, PKM pk, IVersion enc)
    {
        // Check that the sequence of current move matches the relearn move sequence.
        for (int i = 0; i < result.Length; i++)
        {
            var method = GetMethodEggRelearn(pk, i);
            result[i] = new MoveResult(new MoveLearnInfo(method, enc.Version), 0);
        }
    }

    private static LearnMethod GetMethodEggRelearn(PKM pk, int moveIndex)
    {
        var current = pk.GetMove(moveIndex);
        var relearn = pk.GetRelearnMove(moveIndex);
        if (current != relearn)
            return Unobtainable;
        if (current == 0)
            return Empty;
        return Relearn;
    }

    private static void Finalize(Span<MoveResult> result, Span<int> current)
    {
        // Flag duplicate move indexes.
        for (int i = 0; i < current.Length; i++)
        {
            if (current[i] == 0)
                continue;
            for (int j = i + 1; j < current.Length; j++)
            {
                if (current[j] == 0)
                    continue;
                if (current[i] == current[j])
                    result[i] = result[j] = Duplicate;
            }
        }

        // Can't have empty first move.
        if (current[0] == 0)
            result[0] = EmptyInvalid;
    }

    private static void MarkRelearnMoves(Span<MoveResult> result, PKM pk, Span<int> current, IVersion enc)
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

            result[i] = new MoveResult(new MoveLearnInfo(Relearn, enc.Version), 0);
        }
    }
}
