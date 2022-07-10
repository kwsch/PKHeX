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
    internal readonly int[] Moves;

    /// <summary>
    /// Levels at which a move at a given index can be learned.
    /// </summary>
    private readonly int[] Levels;

    public Learnset(int[] moves, int[] levels)
    {
        Moves = moves;
        Levels = levels;
    }

    /// <summary>
    /// Returns the moves a Pokémon can learn between the specified level range.
    /// </summary>
    /// <param name="maxLevel">Maximum level</param>
    /// <param name="minLevel">Minimum level</param>
    /// <returns>Array of Move IDs</returns>
    public int[] GetMoves(int maxLevel, int minLevel = 0)
    {
        if (minLevel <= 1 && maxLevel >= 100)
            return Moves;
        if (minLevel > maxLevel)
            return Array.Empty<int>();
        int start = Array.FindIndex(Levels, z => z >= minLevel);
        if (start < 0)
            return Array.Empty<int>();
        int end = Array.FindLastIndex(Levels, z => z <= maxLevel);
        if (end < 0)
            return Array.Empty<int>();

        var length = end - start + 1;
        if (length == Moves.Length)
            return Moves;
        return Moves.AsSpan(start, length).ToArray();
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

    /// <summary>
    /// Adds the moves a Pokémon can learn between the specified level range.
    /// </summary>
    /// <param name="moves">Movepool</param>
    /// <param name="maxLevel">Maximum level</param>
    /// <param name="minLevel">Minimum level</param>
    /// <returns>Array of Move IDs</returns>
    public List<int> AddMoves(List<int> moves, int maxLevel, int minLevel = 0)
    {
        if (minLevel <= 1 && maxLevel >= 100)
        {
            moves.AddRange(Moves);
            return moves;
        }
        if (minLevel > maxLevel)
            return moves;
        int start = Array.FindIndex(Levels, z => z >= minLevel);
        if (start < 0)
            return moves;
        int end = Array.FindLastIndex(Levels, z => z <= maxLevel);
        if (end < 0)
            return moves;
        for (int i = start; i < end + 1; i++)
            moves.Add(Moves[i]);
        return moves;
    }

    /// <summary>
    /// Gets the moves a Pokémon can learn between the specified level range as a list.
    /// </summary>
    /// <param name="maxLevel">Maximum level</param>
    /// <param name="minLevel">Minimum level</param>
    /// <returns>Array of Move IDs</returns>
    public List<int> GetMoveList(int maxLevel, int minLevel = 0)
    {
        var list = new List<int>();
        return AddMoves(list, maxLevel, minLevel);
    }

    /// <summary>Returns the moves a Pokémon would have if it were encountered at the specified level.</summary>
    /// <remarks>In Generation 1, it is not possible to learn any moves lower than these encounter moves.</remarks>
    /// <param name="level">The level the Pokémon was encountered at.</param>
    /// <returns>Array of Move IDs</returns>
    public int[] GetEncounterMoves(int level)
    {
        const int count = 4;
        var moves = new int[count];
        SetEncounterMoves(level, moves);
        return moves;
    }

    /// <summary>Returns the moves a Pokémon would have if it were encountered at the specified level.</summary>
    /// <remarks>In Generation 1, it is not possible to learn any moves lower than these encounter moves.</remarks>
    /// <param name="level">The level the Pokémon was encountered at.</param>
    /// <param name="moves">Move array to write to</param>
    /// <param name="ctr">Starting index to begin overwriting at</param>
    /// <returns>Array of Move IDs</returns>
    public void SetEncounterMoves(int level, Span<int> moves, int ctr = 0)
    {
        for (int i = 0; i < Moves.Length; i++)
        {
            if (Levels[i] > level)
                break;

            int move = Moves[i];
            bool alreadyHasMove = moves.IndexOf(move) >= 0;
            if (alreadyHasMove)
                continue;

            moves[ctr++] = move;
            ctr &= 3;
        }
    }

    public void SetEncounterMovesBackwards(int level, Span<int> moves, int ctr = 0)
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
    public void SetLevelUpMoves(int startLevel, int endLevel, Span<int> moves, int ctr = 0)
    {
        int startIndex = Array.FindIndex(Levels, z => z >= startLevel);
        int endIndex = Array.FindIndex(Levels, z => z > endLevel);
        for (int i = startIndex; i < endIndex; i++)
        {
            int move = Moves[i];
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
    public void SetEvolutionMoves(Span<int> moves, int ctr = 0)
    {
        for (int i = 0; i < Moves.Length; i++)
        {
            if (Levels[i] != 0)
                break;

            int move = Moves[i];
            bool alreadyHasMove = moves.IndexOf(move) >= 0;
            if (alreadyHasMove)
                continue;

            moves[ctr++] = move;
            ctr &= 3;
        }
    }

    /// <summary>Adds the learned moves by level up to the specified level.</summary>
    public void SetLevelUpMoves(int startLevel, int endLevel, Span<int> moves, ReadOnlySpan<int> ignore, int ctr = 0)
    {
        int startIndex = Array.FindIndex(Levels, z => z >= startLevel);
        if (startIndex == -1)
            return; // No more remain
        int endIndex = Array.FindIndex(Levels, z => z > endLevel);
        if (endIndex == -1)
            endIndex = Levels.Length;
        for (int i = startIndex; i < endIndex; i++)
        {
            int move = Moves[i];
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
    public void SetEvolutionMoves(Span<int> moves, ReadOnlySpan<int> ignore, int ctr = 0)
    {
        for (int i = 0; i < Moves.Length; i++)
        {
            if (Levels[i] != 0)
                break;

            int move = Moves[i];
            if (ignore.IndexOf(move) >= 0)
                continue;

            bool alreadyHasMove = moves.IndexOf(move) >= 0;
            if (alreadyHasMove)
                continue;

            moves[ctr++] = move;
            ctr &= 3;
        }
    }

    public IList<int> GetUniqueMovesLearned(IEnumerable<int> seed, int maxLevel, int minLevel = 0)
    {
        int start = Array.FindIndex(Levels, z => z >= minLevel);
        int end = Array.FindLastIndex(Levels, z => z <= maxLevel);
        var list = new List<int>(seed);
        for (int i = start; i <= end; i++)
        {
            if (!list.Contains(Moves[i]))
                list.Add(Moves[i]);
        }
        return list;
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

    public int GetMoveLevel(int move)
    {
        var index = Array.LastIndexOf(Moves, move);
        if (index == -1)
            return -1;
        return Levels[index];
    }

    private Dictionary<int, int>? Learn;

    private Dictionary<int, int> GetDictionary()
    {
        var dict = new Dictionary<int, int>();
        for (int i = 0; i < Moves.Length; i++)
        {
            if (!dict.ContainsKey(Moves[i]))
                dict.Add(Moves[i], Levels[i]);
        }
        return dict;
    }

    /// <summary>Returns the level that a Pokémon can learn the specified move.</summary>
    /// <param name="move">Move ID</param>
    /// <returns>Level the move is learned at. If the result is below 0, the move cannot be learned by leveling up.</returns>
    public int GetLevelLearnMove(int move)
    {
        return (Learn ??= GetDictionary()).TryGetValue(move, out var level) ? level : -1;
    }

    /// <summary>Returns the level that a Pokémon can learn the specified move.</summary>
    /// <param name="move">Move ID</param>
    /// <param name="min">Minimum level to start looking at.</param>
    /// <returns>Level the move is learned at. If the result is below 0, the move cannot be learned by leveling up.</returns>
    public int GetLevelLearnMove(int move, int min)
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

    public ReadOnlySpan<int> GetBaseEggMoves(int level)
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
            return ReadOnlySpan<int>.Empty;

        int start = 0;
        if (count > 4)
        {
            start = count - 4;
            count = 4;
        }
        return Moves.AsSpan(start, count);
    }
}
