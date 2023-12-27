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
        {
            LearnVerifierEgg.Verify(result, current, enc, pk);
            return;
        }

        LearnVerifierHistory.Verify(result, current, enc, pk, history);
        // Check for form exclusive interlocked moves.
        FlagFormExclusiveMoves(result, current, pk);
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

    private static void FlagFormExclusiveMoves(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk)
    {
        if (pk is { Species: (int)Species.Hoopa })
            FlagFormExclusiveHoopa(result, current, pk);
        else if (pk is { Species: (int)Species.Kyurem })
            FlagFormExclusiveKyurem(result, current, pk);
    }

    private static void FlagFormExclusiveHoopa(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk)
    {
        if (pk.Format < 8)
            return;

        // Hoopa in Gen8+ cannot have Hyperspace Hole if not form 0, and cannot have Hyperspace Fury if not form 1.
        var disallow = pk.Form != 0 ? (ushort)Move.HyperspaceHole : (ushort)Move.HyperspaceFury;
        var index = current.IndexOf(disallow);
        if (index >= 0)
            result[index] = MoveResult.Unobtainable(pk.Form == 0 ? (ushort)Move.HyperspaceHole : (ushort)Move.HyperspaceFury);
    }

    private static void FlagFormExclusiveKyurem(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk)
    {
        // Kyurem forms replace Scary Face with their Fusion Move and vice versa.
        if (pk.Form is not 1) // not White
        {
            // Disallow Fusion Flare
            var index = current.IndexOf((ushort)Move.FusionFlare);
            if (index >= 0)
                result[index] = MoveResult.Unobtainable((ushort)Move.ScaryFace);
        }
        if (pk.Form is not 2) // not Black
        {
            // Disallow Fusion Flare
            var index = current.IndexOf((ushort)Move.FusionBolt);
            if (index >= 0)
                result[index] = MoveResult.Unobtainable((ushort)Move.ScaryFace);
        }
        if (pk.Form is not 0 && pk.Format < 8) // unfused
        {
            // Disallow Scary Face in formats < 8
            var index = current.IndexOf((ushort)Move.ScaryFace);
            if (index >= 0)
                result[index] = MoveResult.Unobtainable(pk.Form == 1 ? (ushort)Move.FusionFlare : (ushort)Move.FusionBolt);
        }
    }
}
