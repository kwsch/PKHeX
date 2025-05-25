using System;
using System.Runtime.InteropServices;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Unpacks <see cref="Learnset"/> data from legality binary inputs.
/// </summary>
public static class LearnsetReader
{
    private static readonly Learnset EMPTY = new([], []);

    /// <summary>
    /// Loads a learnset by reading 16-bit move,level pairs.
    /// </summary>
    /// <param name="entries">Entry data</param>
    public static Learnset[] GetArray(BinLinkerAccessor16 entries)
    {
        var result = new Learnset[entries.Length];
        result[0] = EMPTY; // empty entry
        for (int i = 1; i < result.Length; i++)
            result[i] = ReadLearnset16(entries[i]);
        return result;
    }

    /// <summary>
    /// Reads a Level up move pool definition from a single move pool definition.
    /// </summary>
    private static Learnset ReadLearnset16(ReadOnlySpan<byte> data)
    {
        if (data.Length == 0)
            return EMPTY;

        // move[], .. level[]
        var count = data.Length / 3;
        var size = count << 1; // 2 bytes per move
        var moves = MemoryMarshal.Cast<byte, ushort>(data[..size]).ToArray();
        if (!BitConverter.IsLittleEndian)
            ReverseEndianness(moves, moves);
        var levels = data.Slice(size, count).ToArray();
        return new Learnset(moves, levels);
    }
}
