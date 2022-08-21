using System;
using System.Collections.Generic;
using System.Text;

namespace PKHeX.Core;

/// <summary>
/// Set of 4 moves that are in a list of moves.
/// </summary>
public readonly record struct Moveset(ushort Move1, ushort Move2 = 0, ushort Move3 = 0, ushort Move4 = 0)
{
    public bool HasMoves => Move1 != 0;

    public bool Contains(int move) => move == Move1 || move == Move2 || move == Move3 || move == Move4;
    public bool AnyAbove(int max) => Move1 > max || Move2 > max || Move3 > max || Move4 > max;

    public int[] ToArray() => new int[] { Move1, Move2, Move3, Move4 };

    public void CopyTo(Span<int> moves)
    {
        moves[3] = Move4;
        moves[2] = Move3;
        moves[1] = Move2;
        moves[0] = Move1;
    }

    public bool ContainsAny(ReadOnlySpan<int> moves)
    {
        foreach (var move in moves)
        {
            if (Contains(move))
                return true;
        }
        return false;
    }

    public bool ContainsAll(ReadOnlySpan<int> needs)
    {
        foreach (var move in needs)
        {
            if (!Contains(move))
                return false;
        }
        return true;
    }

    public string GetMovesetLine(IReadOnlyList<string> names, string split = " / ")
    {
        var sb = new StringBuilder(128);
        sb.Append(names[Move1]);
        if (Move2 != 0)
        {
            sb.Append(split).Append(names[Move2]);
            if (Move3 != 0)
            {
                sb.Append(split).Append(names[Move3]);
                if (Move4 != 0)
                    sb.Append(split).Append(names[Move4]);
            }
        }
        return sb.ToString();
    }

    public int BitOverlap(ReadOnlySpan<int> span)
    {
        // Flag each present index; having all moves will have all bitflags.
        int flags = 0;
        for (var i = 0; i < span.Length; i++)
        {
            int move = span[i];
            if (Contains(move))
                flags |= 1 << i;
        }
        return flags;
    }

    public static int BitOverlap(Span<int> moves, Span<int> span)
    {
        // Flag each present index; having all moves will have all bitflags.
        int flags = 0;
        for (var i = 0; i < span.Length; i++)
        {
            int move = span[i];
            if (moves.Contains(move))
                flags |= 1 << i;
        }
        return flags;
    }
}
