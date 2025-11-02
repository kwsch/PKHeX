using System;
using System.Collections.Generic;
using System.Text;

namespace PKHeX.Core;

/// <summary>
/// Represents a set of up to four Pokémon moves, providing utility methods for move comparison, formatting, and manipulation.
/// </summary>
/// <param name="Move1">The first move in the moveset.</param>
/// <param name="Move2">The second move in the moveset.</param>
/// <param name="Move3">The third move in the moveset.</param>
/// <param name="Move4">The fourth move in the moveset.</param>
public readonly record struct Moveset(ushort Move1, ushort Move2 = 0, ushort Move3 = 0, ushort Move4 = 0)
{
    /// <summary>
    /// The default separator string used when formatting moveset lines.
    /// </summary>
    public const string DefaultSeparator = " / ";

    /// <summary>
    /// Gets whether the moveset contains at least one move.
    /// </summary>
    public bool HasMoves => Move1 != 0;

    /// <summary>
    /// Determines whether the moveset contains the specified move.
    /// </summary>
    /// <param name="move">The move ID to check for.</param>
    /// <returns>True if the move is present; otherwise, false.</returns>
    public bool Contains(ushort move) => move == Move1 || move == Move2 || move == Move3 || move == Move4;

    /// <summary>
    /// Determines whether any move in the moveset is greater than the specified maximum value.
    /// </summary>
    /// <param name="max">The maximum move ID value.</param>
    /// <returns>True if any move is above the maximum; otherwise, false.</returns>
    public bool AnyAbove(ushort max) => Move1 > max || Move2 > max || Move3 > max || Move4 > max;

    /// <summary>
    /// Returns the moveset as an array of four move IDs.
    /// </summary>
    /// <returns>An array containing the four move IDs.</returns>
    public ushort[] ToArray() => [Move1, Move2, Move3, Move4];

    /// <summary>
    /// Copies the moveset into the provided span.
    /// </summary>
    /// <param name="moves">The span to copy the moves into. Must be at least length 4.</param>
    public void CopyTo(Span<ushort> moves)
    {
        moves[3] = Move4;
        moves[2] = Move3;
        moves[1] = Move2;
        moves[0] = Move1;
    }

    /// <summary>
    /// Determines whether the moveset contains any of the specified moves.
    /// </summary>
    /// <param name="moves">A span of move IDs to check for.</param>
    /// <returns>True if any move is present; otherwise, false.</returns>
    public bool ContainsAny(ReadOnlySpan<ushort> moves)
    {
        foreach (var move in moves)
        {
            if (Contains(move))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Determines whether the moveset contains all specified moves.
    /// </summary>
    /// <param name="needs">A span of move IDs to check for.</param>
    /// <returns>True if all moves are present; otherwise, false.</returns>
    public bool ContainsAll(ReadOnlySpan<ushort> needs)
    {
        foreach (var move in needs)
        {
            if (!Contains(move))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Returns a formatted string representing the moveset, using the provided move names and separator.
    /// </summary>
    /// <param name="names">A list of move names indexed by move ID.</param>
    /// <param name="split">The separator string to use between moves.</param>
    /// <returns>A formatted string of the moveset.</returns>
    public string GetMovesetLine(IReadOnlyList<string> names, string split = DefaultSeparator)
    {
        var sb = new StringBuilder(128);
        AddMovesetLine(sb, names, split);
        return sb.ToString();
    }

    /// <summary>
    /// Appends a formatted moveset line to the provided <see cref="StringBuilder"/>, using the given move names and separator.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to append to.</param>
    /// <param name="names">A list of move names indexed by move ID.</param>
    /// <param name="split">The separator string to use between moves.</param>
    /// <remarks>
    /// You should only call this if you're sure this has at least 1 move -- check <see cref="HasMoves"/>.
    /// </remarks>
    public void AddMovesetLine(StringBuilder sb, IReadOnlyList<string> names, string split = DefaultSeparator)
    {
        sb.Append(names[Move1]);
        if (Move2 == 0)
            return;
        sb.Append(split).Append(names[Move2]);
        if (Move3 == 0)
            return;
        sb.Append(split).Append(names[Move3]);
        if (Move4 != 0)
            sb.Append(split).Append(names[Move4]);
    }

    /// <summary>
    /// Flags each present index in the provided span; having all moves will have all bitflags set.
    /// </summary>
    /// <param name="span">Moves to check and index flags for.</param>
    /// <returns>An integer bitmask where each bit represents the presence of a move at the corresponding index.</returns>
    public int BitOverlap(ReadOnlySpan<ushort> span)
    {
        int flags = 0;
        for (var i = 0; i < span.Length; i++)
        {
            var move = span[i];
            if (Contains(move))
                flags |= 1 << i;
        }
        return flags;
    }

    /// <summary>
    /// Flags each present index in the provided span, using a base move source; having all moves will have all bitflags set.
    /// </summary>
    /// <param name="moves">Base move source.</param>
    /// <param name="span">Moves to check and index flags for.</param>
    /// <returns>An integer bitmask where each bit represents the presence of a move at the corresponding index.</returns>
    public static int BitOverlap(ReadOnlySpan<ushort> moves, ReadOnlySpan<ushort> span)
    {
        int flags = 0;
        for (var i = 0; i < span.Length; i++)
        {
            var move = span[i];
            if (moves.Contains(move))
                flags |= 1 << i;
        }
        return flags;
    }
}
