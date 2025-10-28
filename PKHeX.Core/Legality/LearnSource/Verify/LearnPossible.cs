using System;

namespace PKHeX.Core;

/// <summary>
/// Logic to check the possible moves a Pokémon can learn.
/// </summary>
public static class LearnPossible
{
    /// <summary>
    /// Populates the permission <see cref="result"/> list indicating the move ID indexes that can be learned based on the inputs.
    /// </summary>
    /// <param name="pk">Entity to check</param>
    /// <param name="enc">Encounter it has been matched to</param>
    /// <param name="history">Environment specific evolution history</param>
    /// <param name="result">Flag array, when true, indicating if a move ID can be currently known.</param>
    /// <param name="types">Move types to give</param>
    public static void Get(PKM pk, IEncounterTemplate enc, EvolutionHistory history, Span<bool> result, MoveSourceType types = MoveSourceType.All)
    {
        if (types.HasFlag(MoveSourceType.Encounter))
            FlagEncounterMoves(pk, enc, result);
        IterateAndFlag(result, pk, enc, history, types);
    }

    /// <inheritdoc cref="Get(PKM,IEncounterTemplate,EvolutionHistory,Span{bool},MoveSourceType)"/>
    public static bool[] Get(PKM pk, IEncounterTemplate enc, EvolutionHistory history, MoveSourceType types = MoveSourceType.All)
    {
        var result = new bool[pk.MaxMoveID + 1];
        Get(pk, enc, history, result, types);
        return result;
    }

    private static void FlagEncounterMoves(PKM pk, IEncounterTemplate enc, Span<bool> result)
    {
        if (pk.IsOriginalMovesetDeleted())
            return;
        if (enc is EncounterSlot8GO { OriginFormat: PogoImportFormat.PK7 or PogoImportFormat.PB7 } g)
        {
            Span<ushort> initial = stackalloc ushort[4];
            g.GetInitialMoves(pk.MetLevel, initial);
            SetAll(initial, result);
        }
        else if (enc.Generation >= 6)
        {
            static void AddIfInRange(ushort move, Span<bool> result)
            {
                if (move < result.Length)
                    result[move] = true;
            }
            AddIfInRange(pk.RelearnMove1, result);
            AddIfInRange(pk.RelearnMove2, result);
            AddIfInRange(pk.RelearnMove3, result);
            AddIfInRange(pk.RelearnMove4, result);
        }
    }

    private static void SetAll(ReadOnlySpan<ushort> moves, Span<bool> result)
    {
        foreach (var move in moves)
            result[move] = true;
    }

    private static void IterateAndFlag(Span<bool> result, PKM pk, IEncounterTemplate enc, EvolutionHistory history, MoveSourceType types)
    {
        // Similar to the iteration when validating a set of currently known moves, iterate backwards.
        // Instead of checking if a single move can be learned, get an iterable chain of sources and flag for each.
        // Keep iterating backwards, as an older game may have an exclusive move.
        var game = LearnGroupUtil.GetCurrentGroup(pk);
        while (true)
        {
            game.GetAllMoves(result, pk, history, enc, types);
            var next = game.GetPrevious(pk, history, enc, LearnOption.Current);
            if (next is null)
                return;

            game = next;
        }
    }
}
