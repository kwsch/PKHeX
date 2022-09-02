using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Level Up Learn Movepool Information
/// </summary>
public sealed class Learnset
{
    /// <summary>
    /// Moves that can be learned.
    /// </summary>
    internal readonly ushort[] Moves;

    /// <summary>
    /// Levels at which a move at a given index can be learned.
    /// </summary>
    private readonly byte[] Levels;

    public Learnset(ushort[] moves, byte[] levels)
    {
        Moves = moves;
        Levels = levels;
    }

    public (bool HasMoves, int Start, int End) GetMoveRange(int maxLevel, int minLevel = 0)
    {
        if (minLevel <= 1 && maxLevel >= 100)
            return (true, 0, Moves.Length - 1);
        if (minLevel > maxLevel)
            return default;
        int start = Array.FindIndex(Levels, z => z >= minLevel);
        if (start < 0)
            return default;
        int end = Array.FindLastIndex(Levels, z => z <= maxLevel);
        if (end < 0)
            return default;

        return (true, start, end);
    }

    /// <summary>Returns the moves a Pokémon would have if it were encountered at the specified level.</summary>
    /// <remarks>In Generation 1, it is not possible to learn any moves lower than these encounter moves.</remarks>
    /// <param name="level">The level the Pokémon was encountered at.</param>
    /// <returns>Array of Move IDs</returns>
    public ushort[] GetEncounterMoves(int level)
    {
        const int count = 4;
        var moves = new ushort[count];
        SetEncounterMoves(level, moves);
        return moves;
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

            var move = Moves[i];
            bool alreadyHasMove = moves.IndexOf(move) >= 0;
            if (alreadyHasMove)
                continue;

            moves[ctr++] = move;
            ctr &= 3;
        }
    }

    public void SetEncounterMovesBackwards(int level, Span<ushort> moves, int ctr = 0)
    {
        int index = Array.FindLastIndex(Levels, z => z <= level);

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
                if (moves.IndexOf(move) == -1)
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
        int startIndex = Array.FindIndex(Levels, z => z >= startLevel);
        int endIndex = Array.FindIndex(Levels, z => z > endLevel);
        for (int i = startIndex; i < endIndex; i++)
        {
            var move = Moves[i];
            bool alreadyHasMove = moves.IndexOf(move) >= 0;
            if (alreadyHasMove)
                continue;

            moves[ctr++] = move;
            ctr &= 3;
        }
    }

    /// <summary>Adds the moves that are gained upon evolving.</summary>
    /// <param name="moves">Move array to write to</param>
    /// <param name="ctr">Starting index to begin overwriting at</param>
    public void SetEvolutionMoves(Span<ushort> moves, int ctr = 0)
    {
        for (int i = 0; i < Moves.Length; i++)
        {
            if (Levels[i] != 0)
                break;

            var move = Moves[i];
            bool alreadyHasMove = moves.IndexOf(move) >= 0;
            if (alreadyHasMove)
                continue;

            moves[ctr++] = move;
            ctr &= 3;
        }
    }

    /// <summary>Adds the learned moves by level up to the specified level.</summary>
    public void SetLevelUpMoves(int startLevel, int endLevel, Span<ushort> moves, ReadOnlySpan<ushort> ignore, int ctr = 0)
    {
        int startIndex = Array.FindIndex(Levels, z => z >= startLevel);
        if (startIndex == -1)
            return; // No more remain
        int endIndex = Array.FindIndex(Levels, z => z > endLevel);
        if (endIndex == -1)
            endIndex = Levels.Length;
        for (int i = startIndex; i < endIndex; i++)
        {
            var move = Moves[i];
            if (ignore.IndexOf(move) >= 0)
                continue;

            bool alreadyHasMove = moves.IndexOf(move) >= 0;
            if (alreadyHasMove)
                continue;

            moves[ctr++] = move;
            ctr &= 3;
        }
    }

    /// <summary>Adds the moves that are gained upon evolving.</summary>
    /// <param name="moves">Move array to write to</param>
    /// <param name="ignore">Ignored moves</param>
    /// <param name="ctr">Starting index to begin overwriting at</param>
    public void SetEvolutionMoves(Span<ushort> moves, ReadOnlySpan<ushort> ignore, int ctr = 0)
    {
        for (int i = 0; i < Moves.Length; i++)
        {
            if (Levels[i] != 0)
                break;

            var move = Moves[i];
            if (ignore.IndexOf(move) >= 0)
                continue;

            bool alreadyHasMove = moves.IndexOf(move) >= 0;
            if (alreadyHasMove)
                continue;

            moves[ctr++] = move;
            ctr &= 3;
        }
    }

    /// <summary>Returns the index of the lowest level move if the Pokémon were encountered at the specified level.</summary>
    /// <remarks>Helps determine the minimum level an encounter can be at.</remarks>
    /// <param name="level">The level the Pokémon was encountered at.</param>
    /// <returns>Array of Move IDs</returns>
    public int GetMinMoveLevel(int level)
    {
        if (Levels.Length == 0)
            return 1;

        int end = Array.FindLastIndex(Levels, z => z <= level);
        return Math.Max(end - 4, 1);
    }

    public int GetMoveLevel(ushort move)
    {
        var index = Array.LastIndexOf(Moves, move);
        if (index == -1)
            return -1;
        return Levels[index];
    }

    private Dictionary<ushort, byte>? Learn;

    private Dictionary<ushort, byte> GetDictionary()
    {
        // Create a dictionary, with the move as the key and the level as the value.
        // Due to the ordering of the object, this will result in fetching the lowest level for a move.
        var dict = new Dictionary<ushort, byte>(Moves.Length);
        for (int i = Moves.Length - 1; i >= 0; i--)
            dict[Moves[i]] = Levels[i];
        return dict;
    }

    /// <summary>Returns the level that a Pokémon can learn the specified move.</summary>
    /// <param name="move">Move ID</param>
    /// <returns>Level the move is learned at. If the result is below 0, the move cannot be learned by leveling up.</returns>
    public int GetLevelLearnMove(ushort move)
    {
        return (Learn ??= GetDictionary()).TryGetValue(move, out var level) ? level : -1;
    }

    /// <summary>Returns the level that a Pokémon can learn the specified move.</summary>
    /// <param name="move">Move ID</param>
    /// <param name="min">Minimum level to start looking at.</param>
    /// <returns>Level the move is learned at. If the result is below 0, the move cannot be learned by leveling up.</returns>
    public int GetLevelLearnMove(ushort move, int min)
    {
        for (int i = 0; i < Moves.Length; i++)
        {
            if (move != Moves[i])
                continue;

            var lv = Levels[i];
            if (lv >= min)
                return lv;
        }
        return -1;
    }

    public ReadOnlySpan<ushort> GetBaseEggMoves(int level)
    {
        // Count moves <= level
        var count = 0;
        foreach (var x in Levels)
        {
            if (x > level)
                break;
            count++;
        }

        // Return a slice containing the moves <= level.
        if (count == 0)
            return ReadOnlySpan<ushort>.Empty;

        int start = 0;
        if (count > 4)
        {
            start = count - 4;
            count = 4;
        }
        return Moves.AsSpan(start, count);
    }
}
