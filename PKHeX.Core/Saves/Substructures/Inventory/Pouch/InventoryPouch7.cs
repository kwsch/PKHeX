using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class InventoryPouch7(int offset, int size, int maxCount, IItemStorage info, InventoryType type)
    : InventoryPouch(type, info, maxCount, offset, size)
{
    public bool SetNew { get; set; }
    private int[] OriginalItems = [];

    public override InventoryItem7 GetEmpty(int itemID = 0, int count = 0) => new() { Index = itemID, Count = count };

    private InventoryItem7[] _items = [];
    public override InventoryItem7[] Items => _items;

    public override void GetPouch(ReadOnlySpan<byte> data)
    {
        var span = data[Offset..];
        var items = new InventoryItem7[PouchDataSize];
        for (int i = 0; i < items.Length; i++)
            items[i] = ReadItem(span.Slice(i * 4, 4));
        _items = items;
        OriginalItems = Array.ConvertAll(items, z => z.Index);
    }

    private static InventoryItem7 ReadItem(ReadOnlySpan<byte> item)
    {
        var value = ReadUInt32LittleEndian(item);
        return InventoryItem7.GetValue(value);
    }

    public override void SetPouch(Span<byte> data)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(_items.Length, PouchDataSize);

        var span = data[Offset..];
        var items = _items;
        for (int i = 0; i < _items.Length; i++)
            WriteItem(span.Slice(i * 4, 4), items[i]);
    }

    private void WriteItem(Span<byte> entry, InventoryItem7 item)
    {
        var value = item.GetValue(SetNew, OriginalItems);
        WriteUInt32LittleEndian(entry, value);
    }
}
