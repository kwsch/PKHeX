using System;
using System.Diagnostics.CodeAnalysis;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Base class for Side-game data used to store <see cref="PKM"/> data transferred into HOME.
/// </summary>
public abstract class HomeOptional1
{
    // Internal Attributes set on creation
    private readonly Memory<byte> Buffer; // Raw Storage
    protected Span<byte> Data => Buffer.Span;
    public int SerializedSize => HeaderSize + Buffer.Length;

    public const int HeaderSize = 3; // u8 format, u16 length(data[u8])
    protected abstract HomeGameDataFormat Format { get; }

    protected HomeOptional1([ConstantExpected] ushort size) => Buffer = new byte[size];
    protected HomeOptional1(Memory<byte> buffer) => Buffer = buffer;

    protected void EnsureSize([ConstantExpected] int size)
    {
        if (Buffer.Length != size)
            throw new ArgumentOutOfRangeException(nameof(size), size, $"Expected size {Buffer.Length} but received {size}.");
    }

    protected byte[] ToArray() => Data.ToArray();
    protected int WriteWithHeader(Span<byte> result)
    {
        result[0] = (byte)Format;
        WriteUInt16LittleEndian(result[1..], (ushort)Data.Length);
        return HeaderSize + WriteWithoutHeader(result[HeaderSize..]);
    }

    private int WriteWithoutHeader(Span<byte> result)
    {
        var span = Data;
        span.CopyTo(result);
        return span.Length;
    }
}
