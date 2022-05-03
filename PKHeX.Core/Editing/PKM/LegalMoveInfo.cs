using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core;

public sealed class LegalMoveInfo
{
    private int LearnCount;

    // Use a bool array instead of a HashSet; we have a limited range of moves.
    // This implementation is faster (no hashcode or bucket search) with lower memory overhead (1 byte per move ID).
    private readonly bool[] AllowedMoves = new bool[(int)Move.MAX_COUNT];

    /// <summary>
    /// Checks if the requested <see cref="move"/> is legally able to be learned.
    /// </summary>
    /// <param name="move">Move to check if can be learned</param>
    /// <returns>True if can learn the move</returns>
    public bool CanLearn(int move) => AllowedMoves[move];

    /// <summary>
    /// Reloads the legality sources to permit the provided legal <see cref="moves"/>.
    /// </summary>
    /// <param name="moves">Legal moves to allow</param>
    public bool ReloadMoves(IReadOnlyList<int> moves)
    {
        // check prior move-pool to not needlessly refresh the data set
        if (moves.Count == LearnCount && moves.All(CanLearn))
            return false;

        SetMoves(moves);
        return true;
    }

    private void SetMoves(IReadOnlyList<int> moves)
    {
        var arr = AllowedMoves;
        Array.Clear(arr, 0, arr.Length);
        foreach (var index in moves)
            arr[index] = true;
        LearnCount = moves.Count;
    }
}
