using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class InventoryPouch3GC(int offset, int size, int maxCount, IItemStorage info, InventoryType type)
    : InventoryPouch(type, info, maxCount, offset, size)
{
    public override InventoryItem GetEmpty(int itemID = 0, int count = 0) => new() { Index = itemID, Count = count };
    private InventoryItem[] _items = [];
    public override InventoryItem[] Items => _items;

    public override void GetPouch(ReadOnlySpan<byte> data)
    {
        var span = data[Offset..];
        var items = new InventoryItem[PouchDataSize];
        for (int i = 0; i < items.Length; i++)
            items[i] = ReadItem(span.Slice(i * 4, 4));
        _items = items;
    }

    private static InventoryItem ReadItem(ReadOnlySpan<byte> data) => new()
    {
        Index = ReadUInt16BigEndian(data),
        Count = ReadUInt16BigEndian(data[2..]),
    };

    public override void SetPouch(Span<byte> data)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(_items.Length, PouchDataSize);

        var span = data[Offset..];
        for (int i = 0; i < _items.Length; i++)
            WriteItem(span.Slice(i * 4, 4), _items[i]);
    }

    private static void WriteItem(Span<byte> slice, InventoryItem item)
    {
        WriteUInt16BigEndian(slice,      (ushort)item.Index);
        WriteUInt16BigEndian(slice[2..], (ushort)item.Count);
    }
}
