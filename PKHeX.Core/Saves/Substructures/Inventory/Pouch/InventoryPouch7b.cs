using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Inventory Pouch used by <see cref="EntityContext.Gen7b"/>
/// </summary>
public sealed class InventoryPouch7b(int offset, int size, int maxCount, IItemStorage info, InventoryType type)
    : InventoryPouch(type, info, maxCount, offset, size)
{
    public bool SetNew { get; set; }
    private int[] OriginalItems = [];

    private InventoryItem7b[] _items = [];
    public override InventoryItem7b[] Items => _items;

    public override InventoryItem7b GetEmpty(int itemID = 0, int count = 0) => new() { Index = itemID, Count = count };

    public override void GetPouch(ReadOnlySpan<byte> data)
    {
        var span = data[Offset..];
        var items = new InventoryItem7b[PouchDataSize];
        for (int i = 0; i < items.Length; i++)
            items[i] = ReadItem(span.Slice(i * 4, 4));
        _items = items;
        OriginalItems = Array.ConvertAll(items, z => z.Index);
    }

    private static InventoryItem7b ReadItem(ReadOnlySpan<byte> entry)
    {
        var value = ReadUInt32LittleEndian(entry);
        return InventoryItem7b.GetValue(value);
    }

    public override void SetPouch(Span<byte> data)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(_items.Length, PouchDataSize);

        var span = data[Offset..];
        var items = _items;
        for (int i = 0; i < items.Length; i++)
            WriteItem(span.Slice(i * 4, 4), items[i]);
    }

    private void WriteItem(Span<byte> entry, InventoryItem7b item)
    {
        var value = item.GetValue(SetNew, OriginalItems);
        WriteUInt32LittleEndian(entry, value);
    }
}
