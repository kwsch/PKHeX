using System;
using System.Runtime.InteropServices;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Move list interaction wrapper.
/// </summary>
public readonly struct MoveSource
{
    private readonly ushort[] moves;
    private MoveSource(ushort[] moves) => this.moves = moves;
    public ReadOnlySpan<ushort> Moves => moves;
    public bool GetHasMove(ushort move) => Moves.Contains(move);

    public static MoveSource[] GetArray(BinLinkerAccessor16 entries)
    {
        var result = new MoveSource[entries.Length];
        var empty = result[0] = new MoveSource([]);
        for (int i = 1; i < result.Length; i++)
        {
            var data = entries[i];
            if (data.Length == 0)
            {
                result[i] = empty; // empty entry
                continue;
            }
            var moves = GetArray(data);
            result[i] = new MoveSource(moves);
        }
        return result;
    }

    public static ushort[] GetArray(ReadOnlySpan<byte> data)
    {
        var moves = MemoryMarshal.Cast<byte, ushort>(data).ToArray();
        if (!BitConverter.IsLittleEndian)
            ReverseEndianness(moves, moves);
        return moves;
    }
}
