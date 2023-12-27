using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Unpacks <see cref="Learnset"/> data from legality binary inputs.
/// </summary>
public static class LearnsetReader
{
    private static readonly Learnset EMPTY = new([], []);

    /// <summary>
    /// Loads a learnset using the 8-bit-per-move storage structure used by Generation 1 &amp; 2 games.
    /// </summary>
    /// <param name="input">Raw ROM data containing the contiguous moves</param>
    /// <param name="maxSpecies">Highest species ID for the input game.</param>
    public static Learnset[] GetArray(ReadOnlySpan<byte> input, ushort maxSpecies)
    {
        int offset = 0;
        var result = new Learnset[maxSpecies + 1];
        for (int i = 0; i < result.Length; i++)
            result[i] = ReadLearnset8(input, ref offset);
        return result;
    }

    /// <summary>
    /// Loads a learnset by reading 16-bit move,level pairs.
    /// </summary>
    /// <param name="entries">Entry data</param>
    public static Learnset[] GetArray(BinLinkerAccessor entries)
    {
        var result = new Learnset[entries.Length];
        for (int i = 0; i < result.Length; i++)
            result[i] = ReadLearnset16(entries[i]);
        return result;
    }

    /// <summary>
    /// Reads a Level up move pool definition from a contiguous chunk of GB era ROM data.
    /// </summary>
    /// <remarks>Moves and Levels are 8-bit</remarks>
    private static Learnset ReadLearnset8(ReadOnlySpan<byte> data, ref int offset)
    {
        int end = offset; // scan for count
        if (data[end] == 0)
        {
            ++offset;
            return EMPTY;
        }
        do { end += 2; } while (data[end] != 0);

        var count = (end - offset) / 2;
        var moves = new ushort[count];
        var levels = new byte[count];
        for (int i = 0; i < moves.Length; i++)
        {
            levels[i] = data[offset++];
            moves[i] = data[offset++];
        }
        ++offset;
        return new Learnset(moves, levels);
    }

    /// <summary>
    /// Reads a Level up move pool definition from a single move pool definition.
    /// </summary>
    /// <remarks>Count of moves, followed by Moves and Levels which are 16-bit</remarks>
    private static Learnset ReadLearnset16(ReadOnlySpan<byte> data)
    {
        if (data.Length <= 4)
            return EMPTY;
        var count = (data.Length / 4) - 1;
        var moves = new ushort[count];
        var levels = new byte[count];
        for (int i = 0; i < count; i++)
        {
            var move = data.Slice(i * 4, 4);
            levels[i] = move[2];
            moves[i] = ReadUInt16LittleEndian(move);
        }
        return new Learnset(moves, levels);
    }
}
