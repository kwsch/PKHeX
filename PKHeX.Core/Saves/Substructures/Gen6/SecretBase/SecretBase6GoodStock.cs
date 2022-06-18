using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 6 Secret Base Decoration Good Inventory stock for a given good-index.
/// </summary>
public sealed class SecretBase6GoodStock
{
    public const int SIZE = 4;

    public ushort Count { get; set; }
    public bool IsNew { get; set; }

    public SecretBase6GoodStock(ReadOnlySpan<byte> data)
    {
        Count = ReadUInt16LittleEndian(data);
        IsNew = data[2] != 0;
    }

    public void Write(Span<byte> data)
    {
        WriteUInt16LittleEndian(data, Count);
        data[2] = (byte)(IsNew ? 1 : 0);
        data[3] = 0;
    }
}
