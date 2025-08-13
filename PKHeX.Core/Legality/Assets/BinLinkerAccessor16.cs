using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Unpacks a BinLinkerAccessor generated file container into individual arrays.
/// </summary>
/// <remarks>
/// Similar to <see cref="BinLinkerAccessor"/>, but uses 16-bit integers for start/end offsets.
/// </remarks>
[DebuggerDisplay($"{{{nameof(Identifier)},nq}}[{{{nameof(Length)},nq}}]")]
public readonly ref struct BinLinkerAccessor16
{
    /// <summary> Backing data object </summary>
    private readonly ReadOnlySpan<byte> Data;

    /// <summary> Magic identifier for the file. </summary>
    public ReadOnlySpan<byte> IdentifierSpan => Data[..2];

    /// <summary> Total count of files available for accessing. </summary>
    public ushort Length => ReadUInt16LittleEndian(Data[2..]);

    /// <summary> Magic identifier for the file. </summary>
    public string Identifier => System.Text.Encoding.ASCII.GetString(IdentifierSpan);

    /// <summary>
    /// Retrieves a view of the entry at the requested <see cref="index"/>.
    /// </summary>
    /// <param name="index">Entry to retrieve.</param>
    public ReadOnlySpan<byte> this[int index] => GetEntry(index);

    private BinLinkerAccessor16(ReadOnlySpan<byte> data) => Data = data;

    private ReadOnlySpan<byte> GetEntry(int index)
    {
        int offset = 4 + (index * sizeof(ushort));
        // Start and End are both 16-bit integers, sequentially.
        // Read them in one shot a 32-bit integer and decompose.
        var startEnd = ReadUInt32LittleEndian(Data[offset..]);
        var start = (ushort)startEnd;
        var end = (ushort)(startEnd >> 16);
        return Data[start..end];
    }

    /// <summary>
    /// Sanity checks the input <see cref="data"/> only in DEBUG builds, and returns a new wrapper.
    /// </summary>
    /// <param name="data">Data reference</param>
    /// <param name="identifier">Expected identifier (debug verification only)</param>
    public static BinLinkerAccessor16 Get(ReadOnlySpan<byte> data, [Length(2, 2)] ReadOnlySpan<byte> identifier)
    {
        SanityCheckIdentifier(data, identifier);
        return new BinLinkerAccessor16(data);
    }

    /// <inheritdoc cref="Get(ReadOnlySpan{byte}, ReadOnlySpan{byte})"/>
    /// <summary>
    /// Gets a new <see cref="BinLinkerAccessor16"/> for the provided <see cref="data"/> without identifier verification.
    /// </summary>
    public static BinLinkerAccessor16 Get(ReadOnlySpan<byte> data) => new(data);

    [Conditional("DEBUG")]
    private static void SanityCheckIdentifier(ReadOnlySpan<byte> data, [Length(2, 2)] ReadOnlySpan<byte> identifier)
    {
        Debug.Assert(data.Length > 4);
        Debug.Assert(identifier[0] == data[0] && identifier[1] == data[1]);
    }
}
