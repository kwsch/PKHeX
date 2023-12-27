using System;
using System.Collections.Generic;
using System.Text;

namespace PKHeX.Core;

/// <summary>
/// Set of 4 moves that are in a list of moves.
/// </summary>
public readonly record struct Moveset(ushort Move1, ushort Move2 = 0, ushort Move3 = 0, ushort Move4 = 0)
{
    public const string DefaultSeparator = " / ";
    public bool HasMoves => Move1 != 0;

    public bool Contains(ushort move) => move == Move1 || move == Move2 || move == Move3 || move == Move4;
    public bool AnyAbove(int max) => Move1 > max || Move2 > max || Move3 > max || Move4 > max;

    public ushort[] ToArray() => [Move1, Move2, Move3, Move4];

    public void CopyTo(Span<ushort> moves)
    {
        moves[3] = Move4;
        moves[2] = Move3;
        moves[1] = Move2;
        moves[0] = Move1;
    }

    public bool ContainsAny(ReadOnlySpan<ushort> moves)
    {
        foreach (var move in moves)
        {
            if (Contains(move))
                return true;
        }
        return false;
    }

    public bool ContainsAll(ReadOnlySpan<ushort> needs)
    {
        foreach (var move in needs)
        {
            if (!Contains(move))
                return false;
        }
        return true;
    }

    public string GetMovesetLine(IReadOnlyList<string> names, string split = DefaultSeparator)
    {
        var sb = new StringBuilder(128);
        AddMovesetLine(sb, names, split);
        return sb.ToString();
    }

    public void AddMovesetLine(StringBuilder sb, IReadOnlyList<string> names, string split = DefaultSeparator)
    {
        // Always has at least 1 move if calling this.
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
    /// Flag each present index; having all moves will have all bitflags.
    /// </summary>
    /// <param name="span">Moves to check and index flags for</param>
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
    /// Flag each present index; having all moves will have all bitflags.
    /// </summary>
    /// <param name="moves">Base Move source</param>
    /// <param name="span">Moves to check and index flags for</param>
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
