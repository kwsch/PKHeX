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
        result[0] = new MoveSource([]);
        for (int i = 1; i < result.Length; i++)
        {
            var data = entries[i];
            var moves = GetArray(data);
            result[i] = new MoveSource(moves);
        }
        return result;
    }

    public static ushort[] GetArray(ReadOnlySpan<byte> data)
    {
        // Frequently, the data is empty, and the length is 0.
        // Even though the ToArray() method will return [], check before the .Cast to avoid that work.
        if (data.Length == 0)
            return [];
        var moves = MemoryMarshal.Cast<byte, ushort>(data).ToArray();
        if (!BitConverter.IsLittleEndian)
            ReverseEndianness(moves, moves);
        return moves;
    }
}
