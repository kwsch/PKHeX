using System;

namespace PKHeX.Core;

/// <summary>
/// Moveset verifier for entities currently existing as an egg.
/// </summary>
internal static class LearnVerifierEgg
{
    public static void Verify(Span<MoveResult> result, ReadOnlySpan<int> current, IEncounterTemplate enc, PKM pk)
    {
        if (enc.Generation >= 6)
            VerifyFromRelearn(result, current, enc, pk);
        else // No relearn moves available.
            VerifyPre3DS(result, current, enc);
    }

    private static void VerifyPre3DS(Span<MoveResult> result, ReadOnlySpan<int> current, IEncounterTemplate enc)
    {
        if (enc is EncounterEgg e)
            LearnVerifierRelearn.VerifyEggMoveset(e, result, current);
        else
            VerifyFromEncounter(result, current, enc);
    }

    private static void VerifyFromEncounter(Span<MoveResult> result, ReadOnlySpan<int> current, IEncounterTemplate enc)
    {
        ReadOnlySpan<int> initial;
        if (enc is IMoveset { Moves: int[] { Length: not 0 } x })
            initial = x;
        else
            initial = GameData.GetLearnset(enc.Version, enc.Species, enc.Form).GetBaseEggMoves(enc.LevelMin);
        VerifyMovesInitial(result, current, initial);
    }

    private static void VerifyMovesInitial(Span<MoveResult> result, ReadOnlySpan<int> current, ReadOnlySpan<int> initial)
    {
        // Check that the sequence of current move matches the initial move sequence.
        for (int i = 0; i < initial.Length; i++)
            result[i] = GetMethodInitial(current[i], initial[i]);
        for (int i = initial.Length; i < current.Length; i++)
            result[i] = current[i] == 0 ? MoveResult.Empty : MoveResult.Unobtainable(0);
    }

    private static void VerifyFromRelearn(Span<MoveResult> result, ReadOnlySpan<int> current, IEncounterTemplate enc, PKM pk)
    {
        if (enc is EncounterEgg)
            VerifyMatchesRelearn(result, current, pk);
        else if (enc is IMoveset { Moves: int[] { Length: not 0 } x })
            VerifyMovesInitial(result, current, x);
        else
            VerifyFromEncounter(result, current, enc);
    }

    private static void VerifyMatchesRelearn(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk)
    {
        // Check that the sequence of current move matches the relearn move sequence.
        for (int i = 0; i < result.Length; i++)
            result[i] = GetMethodRelearn(current[i], pk.GetRelearnMove(i));
    }

    private static MoveResult GetMethodInitial(int current, int initial)
    {
        if (current != initial)
            return MoveResult.Unobtainable(initial);
        if (current == 0)
            return MoveResult.Empty;
        return MoveResult.Initial;
    }

    private static MoveResult GetMethodRelearn(int current, int relearn)
    {
        if (current != relearn)
            return MoveResult.Unobtainable(relearn);
        if (current == 0)
            return MoveResult.Empty;
        return MoveResult.Relearn;
    }
}
