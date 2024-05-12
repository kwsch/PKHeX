using System;

namespace PKHeX.Core;

/// <summary>
/// Level Up Learn Movepool Information
/// </summary>
public sealed class Learnset(ushort[] Moves, byte[] Levels)
{
    /// <summary>
    /// Moves that can be learned.
    /// </summary>
    private readonly ushort[] Moves = Moves;

    /// <summary>
    /// Levels at which a move at a given index can be learned.
    /// </summary>
    private readonly byte[] Levels = Levels;

    private const byte MagicEvolutionMoveLevel = 0;

    public ReadOnlySpan<ushort> GetAllMoves() => Moves;

    public ReadOnlySpan<ushort> GetMoveRange(int maxLevel, int minLevel = 0)
    {
        if (minLevel <= 1 && maxLevel >= 100)
            return Moves;
        if (minLevel > maxLevel)
            return default;
        int start = FindGrq(minLevel);
        if (start < 0)
            return default;
        int end = FindLastLeq(maxLevel);
        if (end < 0)
            return default;

        var length = end - start + 1;
        return Moves.AsSpan(start, length);
    }

    private int FindGrq(int level, int start = 0)
    {
        var levels = Levels;
        for (int i = start; i < levels.Length; i++)
        {
            if (levels[i] >= level)
                return i;
        }
        return -1;
    }

    private int FindGr(int level, int start)
    {
        var levels = Levels;
        for (int i = start; i < levels.Length; i++)
        {
            if (levels[i] >= level)
                return i;
        }
        return -1;
    }

    private int FindLastLeq(int level, int end = 0)
    {
        var levels = Levels;
        for (int i = levels.Length - 1; i >= end; i--)
        {
            if (levels[i] <= level)
                return i;
        }
        return -1;
    }

    /// <summary>Returns the moves a Pokémon would have if it were encountered at the specified level.</summary>
    /// <remarks>In Generation 1, it is not possible to learn any moves lower than these encounter moves.</remarks>
    /// <param name="level">The level the Pokémon was encountered at.</param>
    /// <param name="moves">Move array to write to</param>
    /// <param name="ctr">Starting index to begin overwriting at</param>
    /// <returns>Array of Move IDs</returns>
    public void SetEncounterMoves(int level, Span<ushort> moves, int ctr = 0)
    {
        for (int i = 0; i < Moves.Length; i++)
        {
            if (Levels[i] > level)
                break;

            AddMoveShiftLater(moves, ref ctr, Moves[i]);
        }
        RectifyOrderShift(moves, ctr);
    }

    private static void AddMoveShiftLater(Span<ushort> moves, ref int ctr, ushort move)
    {
        if (!moves.Contains(move))
            moves[(ctr++) & 3] = move;
    }

    private static void RectifyOrderShift(Span<ushort> moves, int ctr)
    {
        // Perform (n & 3) rotations as if we were inserting moves, but a minimal amount of times.
        // This skips the rotation for when moves are inserted and then overwritten by later inserted moves.
        if (ctr <= moves.Length)
            return;
        var rotation = ctr & 3;
        if (rotation == 0)
            return;

        // rotate n times in-place
        for (int i = 0; i < rotation; i++)
        {
            var move = moves[0];
            for (int j = 0; j < 3; j++)
                moves[j] = moves[j + 1];
            moves[3] = move;
        }
    }

    public void SetEncounterMovesBackwards(int level, Span<ushort> moves, int ctr = 0)
    {
        int index = FindLastLeq(level);

        while (true)
        {
            if (index == -1)
                return; // no moves to add?

            // In the event we have multiple moves at the same level, insert them in regular descending order.
            int start = index;
            while (start != 0 && Levels[start] == Levels[start - 1])
                start--;

            for (int i = start; i <= index; i++)
            {
                var move = Moves[i];
                if (moves.Contains(move))
                    continue;

                moves[ctr++] = move;
                if (ctr == 4)
                    return;
            }

            index = start - 1;
        }
    }

    /// <summary>Adds the learned moves by level up to the specified level.</summary>
    public void SetLevelUpMoves(int startLevel, int endLevel, Span<ushort> moves, int ctr = 0)
    {
        int startIndex = FindGrq(startLevel);
        if (startIndex == -1)
            return;
        int endIndex = FindGr(endLevel, startIndex);
        if (endIndex == -1)
            endIndex = Levels.Length;

        for (int i = startIndex; i < endIndex; i++)
            AddMoveShiftLater(moves, ref ctr, Moves[i]);
        RectifyOrderShift(moves, ctr);
    }

    /// <summary>Adds the moves that are gained upon evolving.</summary>
    /// <param name="moves">Move array to write to</param>
    /// <param name="ctr">Starting index to begin overwriting at</param>
    public void SetEvolutionMoves(Span<ushort> moves, int ctr = 0)
    {
        // Evolution moves are always at the lowest indexes of the learnset.
        for (int i = 0; i < Moves.Length; i++)
        {
            if (Levels[i] != MagicEvolutionMoveLevel)
                break;

            AddMoveShiftLater(moves, ref ctr, Moves[i]);
        }
        RectifyOrderShift(moves, ctr);
    }

    /// <summary>Adds the learned moves by level up to the specified level.</summary>
    public void SetLevelUpMoves(int startLevel, int endLevel, Span<ushort> moves, ReadOnlySpan<ushort> ignore, int ctr = 0)
    {
        int startIndex = FindGrq(startLevel);
        if (startIndex == -1)
            return; // No more remain
        int endIndex = FindGr(endLevel, startIndex);
        if (endIndex == -1)
            endIndex = Levels.Length;
        for (int i = startIndex; i < endIndex; i++)
        {
            var move = Moves[i];
            if (ignore.Contains(move))
                continue;

            AddMoveShiftLater(moves, ref ctr, move);
        }
        RectifyOrderShift(moves, ctr);
    }

    /// <summary>Adds the moves that are gained upon evolving.</summary>
    /// <param name="moves">Move array to write to</param>
    /// <param name="ignore">Ignored moves</param>
    /// <param name="ctr">Starting index to begin overwriting at</param>
    public void SetEvolutionMoves(Span<ushort> moves, ReadOnlySpan<ushort> ignore, int ctr = 0)
    {
        for (int i = 0; i < Moves.Length; i++)
        {
            if (Levels[i] != MagicEvolutionMoveLevel)
                break;

            var move = Moves[i];
            if (ignore.Contains(move))
                continue;

            AddMoveShiftLater(moves, ref ctr, move);
        }
        RectifyOrderShift(moves, ctr);
    }

    /// <summary>
    /// Checks if the specified move is learned by level up.
    /// </summary>
    /// <param name="move">Move ID</param>
    public bool GetIsLearn(ushort move) => Moves.AsSpan().Contains(move);

    /// <summary>Returns the level that a Pokémon can learn the specified move.</summary>
    /// <param name="move">Move ID</param>
    /// <returns>Level the move is learned at. If the result is below 0, the move cannot be learned by leveling up.</returns>
    public int GetLevelLearnMove(ushort move)
    {
        var index = Array.IndexOf(Moves, move);
        if (index == -1)
            return -1;
        return Levels[index];
    }

    public ReadOnlySpan<ushort> GetBaseEggMoves(byte level)
    {
        // Count moves <= level
        var count = 0;
        foreach (ref readonly var x in Levels.AsSpan())
        {
            if (x > level)
                break;
            count++;
        }

        // Return a slice containing the moves <= level.
        if (count == 0)
            return [];

        int start = 0;
        if (count > 4)
        {
            start = count - 4;
            count = 4;
        }
        return Moves.AsSpan(start, count);
    }
}
