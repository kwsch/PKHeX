using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Side game data base class for <see cref="PKM"/> data transferred into HOME.
/// </summary>
public abstract class HomeOptional1
{
    // Internal Attributes set on creation
    protected readonly byte[] Data; // Raw Storage
    protected readonly int Offset;

    private const int HeaderBias = 3;

    protected HomeOptional1(HomeGameDataFormat format, ushort size)
    {
        Data = new byte[size + HeaderBias];
        Offset = HeaderBias;

        Data[0] = (byte)format;
        WriteUInt16LittleEndian(Data.AsSpan(1, 2), size);
    }

    protected HomeOptional1(HomeGameDataFormat format, ushort size, byte[] data, int offset = 0)
    {
        // Sanity check input format value with backing data value.
        if ((HomeGameDataFormat)data[offset] != format)
            throw new ArgumentOutOfRangeException(nameof(format), format, $"Invalid {nameof(HomeGameDataFormat)} for {format}");

        // Sanity check input structure size with backing data value.
        var length = ReadUInt16LittleEndian(data.AsSpan(offset + 1));
        if (length != size)
            throw new ArgumentOutOfRangeException(nameof(size), length, $"Invalid structure size for {format}");

        Data = data;
        Offset = HeaderBias + offset;
    }

    protected Span<byte> ToSpan(int size) => Data.AsSpan(Offset - HeaderBias, HeaderBias + size);
    protected byte[] ToArray(int size) => ToSpan(size).ToArray();
    protected int CopyTo(Span<byte> result, int size)
    {
        var span = ToSpan(size);
        span.CopyTo(result);
        return span.Length;
    }
}
