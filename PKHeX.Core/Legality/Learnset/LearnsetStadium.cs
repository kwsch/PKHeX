using System;
using System.Runtime.InteropServices;
using static PKHeX.Core.LearnMethodStadium2;

namespace PKHeX.Core;

/// <summary>
/// Stadium 2's Move Reminder
/// </summary>
/// <remarks>
/// Entries are sorted by ascending level; so the first entry for a move is the minimum level to learn it.
/// Moves may appear multiple times in the learnset, but only the first needs to be satisfied to be "valid" for Stadium 2's checks.
/// https://bluemoonfalls.com/pages/general/move-reminder
/// </remarks>
public sealed class LearnsetStadium
{
    private readonly StadiumTuple[] Learn;
    public LearnsetStadium(ReadOnlySpan<byte> input)
        => Learn = MemoryMarshal.Cast<byte, StadiumTuple>(input).ToArray();

    /// <summary> Gets all entries. </summary>
    public ReadOnlySpan<StadiumTuple> GetMoves() => Learn;

    /// <summary>
    /// Gets the move info for a specific move ID.
    /// </summary>
    /// <param name="move">Move ID</param>
    /// <param name="result">Move info for the specified move ID.</param>
    /// <returns>True if the move exists in the learnset. Will return the minimum level move entry if multiple entries exist.</returns>
    public bool TryGetMove(ushort move, out StadiumTuple result)
    {
        foreach (var learn in Learn)
        {
            if (move != learn.Move)
                continue;
            result = learn;
            return true;
        }
        result = default;
        return false;
    }

    /// <summary>
    /// Checks if the move can be learned at the specified level.
    /// </summary>
    /// <param name="move">Move ID</param>
    /// <param name="level">Current level of the Pokémon.</param>
    /// <returns></returns>
    public bool CanKnow(ushort move, byte level) => TryGetMove(move, out var result) && result.Level <= level;

    /// <summary>
    /// Checks if the move can be relearned by Stadium 2 to the current moves.
    /// </summary>
    /// <param name="move">Move ID to try and remember.</param>
    /// <param name="level">Current level of the Pokémon.</param>
    /// <param name="criteriaLevel">Level at which the move can be relearned, if applicable.</param>
    /// <returns>True if the move can be relearned.</returns>
    public bool CanRelearn(ushort move, byte level, out byte criteriaLevel)
    {
        criteriaLevel = 0;
        foreach (var learn in Learn)
        {
            if (level < learn.Level)
                return false; // moves are sorted by level; eager return.
            if (move != learn.Move)
                continue;
            if (!learn.Source.IsAbleToBeRelearned())
                continue;
            criteriaLevel = learn.Level; // remember the minimum level to relearn
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks the moveset.
    /// </summary>
    /// <remarks>Smeargle should be checked separately.</remarks>
    /// <param name="moves">Currently known moves.</param>
    /// <param name="level">Current level of the Pokémon.</param>
    /// <param name="flag">Invalid moves will be marked as true in this span.</param>
    /// <returns>True if all moves are valid.</returns>
    public bool Validate(ReadOnlySpan<ushort> moves, byte level, Span<bool> flag)
    {
        bool anyInvalid = false;

        // Stadium2 is not smart enough to disallow egg moves+event, or multiple event moves (Pikachu).
        // Naive checker only checks if all individual moves are in its list with sufficient level.
        for (int i = 0; i < moves.Length; i++)
        {
            var move = moves[i];
            if (move == 0)
                continue; // skip empty moves
            if (CanKnow(move, level))
                continue;
            if (i >= flag.Length)
                break; // avoid out of bounds, shouldn't happen but just in case
            anyInvalid = flag[i] = true;
        }
        return !anyInvalid;
    }

    public static bool ValidateSmeargle(ReadOnlySpan<ushort> moves, Span<bool> flag)
    {
        bool anyInvalid = false;
        for (int i = 0; i < moves.Length; i++)
            anyInvalid |= flag[i] = !IsSketchValid(moves[i]);
        return !anyInvalid;
    }

    /// <summary>
    /// All others are covered by Mainline possibility. Baton Pass is the only error.
    /// </summary>
    public static bool IsSketchValid(ushort move) => move is not (ushort)Move.BatonPass;

    /// <summary>
    /// Converts a <see cref="BinLinkerAccessor"/> into an array of <see cref="LearnsetStadium"/>.
    /// </summary>
    public static LearnsetStadium[] GetArray(BinLinkerAccessor16 entries)
    {
        var result = new LearnsetStadium[entries.Length];
        for (int i = 0; i < entries.Length; i++)
            result[i] = new LearnsetStadium(entries[i]);
        return result;
    }

    /// <summary>
    /// Gets the range of moves that can be learned between the specified levels.
    /// </summary>
    /// <param name="levelMax">Maximum level to learn moves.</param>
    /// <param name="levelMin">>Minimum level to learn moves.</param>
    public ReadOnlySpan<StadiumTuple> GetMoveRange(byte levelMax, byte levelMin = 1)
    {
        // find start
        int start = 0;
        while (start < Learn.Length && Learn[start].Level < levelMin)
            start++;
        // find end
        int end = start;
        while (end < Learn.Length && Learn[end].Level <= levelMax)
            end++;
        // return the range
        if (start >= end)
            return []; // no moves in range
        return Learn.AsSpan(start, end - start);
    }
}

/// <summary>
/// Value tuple for Stadium 2's move reminder.
/// </summary>
/// <param name="Level">Minimum level to learn the move.</param>
/// <param name="Move">>Move ID.</param>
/// <param name="Source">>Source of the move (e.g., level up, tutor, egg).</param>
public readonly record struct StadiumTuple(byte Level, byte Move, LearnMethodStadium2 Source)
{
    public override string ToString() => $"Lv{Level} {(Move)Move} // {Source}";
}

/// <summary>
/// Flags for the source of moves learned in Stadium 2.
/// </summary>
[Flags]
public enum LearnMethodStadium2 : byte
{
    None = 0,
    // Able to be remembered
    LevelUpRB = 1 << 0,
    LevelUpYW = 1 << 1,
    LevelUpGS = 1 << 2,
    LevelUpC  = 1 << 3,
    // Permitted to know by legality checker, but not able to remember.
    TutorC    = 1 << 4,
    EggC      = 1 << 5,
    EggGS     = 1 << 6,
    Event     = 1 << 7,
}

public static class LearnSourceStadiumExtensions
{
    /// <summary>
    /// Checks if the source can be relearned.
    /// </summary>
    public static bool IsAbleToBeRelearned(this LearnMethodStadium2 source) => (source & (LevelUpRB | LevelUpYW | LevelUpGS | LevelUpC)) != 0;
}
