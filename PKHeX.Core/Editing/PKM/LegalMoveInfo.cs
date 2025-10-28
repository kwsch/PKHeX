using System;
using System.Buffers;

namespace PKHeX.Core;

/// <summary>
/// Caches move learn data via <see cref="ReloadMoves"/>
/// </summary>
public sealed class LegalMoveInfo
{
    // Use a bool array instead of a HashSet; we have a limited range of moves.
    // This implementation is faster (no hashcode or bucket search) with lower memory overhead (1 byte per move ID).
    private readonly bool[] AllowedMoves = new bool[(int)Move.MAX_COUNT + 1];

    /// <summary>
    /// Checks if the requested <see cref="move"/> is legally able to be learned.
    /// </summary>
    /// <param name="move">Move to check if it can be learned</param>
    /// <returns>True if it can learn the move</returns>
    public bool CanLearn(ushort move) => AllowedMoves[move];

    /// <summary>
    /// Reloads the legality sources to permit the provided legal info.
    /// </summary>
    /// <param name="la">Details of analysis, moves to allow</param>
    public bool ReloadMoves(LegalityAnalysis la)
    {
        var rent = ArrayPool<bool>.Shared.Rent(AllowedMoves.Length);
        var span = rent.AsSpan(0, AllowedMoves.Length);
        LearnPossible.Get(la.Entity, la.EncounterOriginal, la.Info.EvoChainsAllGens, span);

        // check prior move-pool to not needlessly refresh the data set
        bool diff = !span.SequenceEqual(AllowedMoves);
        if (diff) // keep
            span.CopyTo(AllowedMoves);
        span.Clear();
        ArrayPool<bool>.Shared.Return(rent);
        return diff;
    }
}
