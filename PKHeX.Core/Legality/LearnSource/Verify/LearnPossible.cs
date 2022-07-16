using System;

namespace PKHeX.Core;

public static class LearnPossible
{
    /// <summary>
    /// Populates a list of move indexes that can be learned based on the inputs.
    /// </summary>
    /// <param name="pk">Entity to check</param>
    /// <param name="enc">Encounter it has been matched to</param>
    /// <param name="history">Environment specific evolution history</param>
    /// <param name="result">Flag array, when true, indicating if a move ID can be currently known.</param>
    public static void Get(PKM pk, IEncounterTemplate enc, EvolutionHistory history, Span<bool> result)
    {
        FlagEncounterMoves(pk, enc, result);
        IterateAndFlag(pk, enc, history, result);
    }

    /// <inheritdoc cref="Get(PKM,IEncounterTemplate,EvolutionHistory,Span{bool})"/>
    public static bool[] Get(PKM pk, IEncounterTemplate enc, EvolutionHistory history)
    {
        var result = new bool[pk.MaxMoveID];
        Get(pk, enc, history, result);
        return result;
    }

    private static void FlagEncounterMoves(PKM pk, IEncounterTemplate enc, Span<bool> result)
    {
        if (pk.IsOriginalMovesetDeleted())
            return;
        if (enc is EncounterSlot8GO g)
        {
            SetAll(g.GetInitialMoves(pk.Met_Level), result);
        }
        else if (enc.Generation >= 6)
        {
            result[pk.RelearnMove1] = true;
            result[pk.RelearnMove2] = true;
            result[pk.RelearnMove3] = true;
            result[pk.RelearnMove4] = true;
        }
    }

    private static void SetAll(ReadOnlySpan<int> moves, Span<bool> result)
    {
        foreach (var move in moves)
            result[move] = true;
    }

    private static void IterateAndFlag(PKM pk, IEncounterTemplate enc, EvolutionHistory history, Span<bool> result)
    {
        // Similar to the iteration when validating a set of currently known moves, iterate backwards.
        // Instead of checking if a single move can be learned, get an enumerable of moves and flag.
        // Keep iterating backwards, as an older game may have an exclusive move.
        var game = LearnGroupUtil.GetCurrentGroup(pk);
        while (true)
        {
            game.GetAllMoves(result, pk, history, enc);
            var next = game.GetPrevious(pk, history, enc);
            if (next is null)
                return;

            game = next;
        }
    }
}
