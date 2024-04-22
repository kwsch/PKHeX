using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Unpacks a BinLinkerAccessor generated file container into individual arrays.
/// </summary>
[DebuggerDisplay($"{{{nameof(Identifier)},nq}}[{{{nameof(Length)},nq}}]")]
public readonly ref struct BinLinkerAccessor
{
    /// <summary> Backing data object </summary>
    private readonly ReadOnlySpan<byte> Data;

    /// <summary> Total count of files available for accessing. </summary>
    public ushort Length => ReadUInt16LittleEndian(Data[2..]);

    /// <summary> Magic identifier for the file. </summary>
    public string Identifier => new([(char)Data[0], (char)Data[1]]);

    /// <summary>
    /// Retrieves a view of the entry at the requested <see cref="index"/>.
    /// </summary>
    /// <param name="index">Entry to retrieve.</param>
    public ReadOnlySpan<byte> this[int index] => GetEntry(index);

    private BinLinkerAccessor(ReadOnlySpan<byte> data) => Data = data;

    private ReadOnlySpan<byte> GetEntry(int index)
    {
        int offset = 4 + (index * sizeof(int));
        // Start and End are both 32-bit integers, sequentially.
        // Read them in one shot a 64-bit integer and decompose.
        var startEnd = ReadUInt64LittleEndian(Data[offset..]);
        int start = (int)startEnd;
        int end = (int)(startEnd >> 32);
        return Data[start..end];
    }

    /// <summary>
    /// Sanity checks the input <see cref="data"/> only in DEBUG builds, and returns a new wrapper.
    /// </summary>
    /// <param name="data">Data reference</param>
    /// <param name="identifier">Expected identifier (debug verification only)</param>
    public static BinLinkerAccessor Get(ReadOnlySpan<byte> data, [Length(2, 2)] ReadOnlySpan<byte> identifier)
    {
        SanityCheckIdentifier(data, identifier);
        return new BinLinkerAccessor(data);
    }

    [Conditional("DEBUG")]
    private static void SanityCheckIdentifier(ReadOnlySpan<byte> data, [Length(2, 2)] ReadOnlySpan<byte> identifier)
    {
        Debug.Assert(data.Length > 4);
        Debug.Assert(identifier[0] == data[0] && identifier[1] == data[1]);
    }
}
