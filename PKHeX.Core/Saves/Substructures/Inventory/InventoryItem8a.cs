using System;
using System.Buffers.Binary;

namespace PKHeX.Core;

public sealed record InventoryItem8a : InventoryItem, IItemFavorite
{
    public const int SIZE = 0x10;

    public override string ToString() => $"{Index:000} x{Count}";
    public bool IsFavorite { get; set; }

    public override void Clear()
    {
        Index = Count = 0;
    }

    public static InventoryItem8a Read(ReadOnlySpan<byte> data) => new()
    {
        Index = BinaryPrimitives.ReadInt16LittleEndian(data),
        Count = BinaryPrimitives.ReadInt16LittleEndian(data[2..]),
    };

    public void Write(Span<byte> data)
    {
        // Index is not saved.
        BinaryPrimitives.WriteUInt16LittleEndian(data, (ushort)Index);
        BinaryPrimitives.WriteUInt16LittleEndian(data[2..], (ushort)Count);
    }

    public static void Clear(Span<byte> data, int offset) => data.Slice(offset, SIZE).Clear();
}
