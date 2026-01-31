using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Inventory Pouch with 4 bytes per item (u16 ID, u16 count)
/// </summary>
public sealed class InventoryPouch4(int offset, int maxCount, IItemStorage info, InventoryType type)
    : InventoryPouch(type, info, maxCount, offset)
{
    // size: 32bit
    // u16 id
    // u16 count

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

    private static InventoryItem ReadItem(ReadOnlySpan<byte> entry) => new()
    {
        Index = ReadUInt16LittleEndian(entry),
        Count = ReadUInt16LittleEndian(entry[2..]),
    };

    public override void SetPouch(Span<byte> data)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(_items.Length, PouchDataSize);

        var span = data[Offset..];
        for (int i = 0; i < _items.Length; i++)
            WriteItem(span.Slice(i * 4, 4), _items[i]);
    }

    private static void WriteItem(Span<byte> entry, InventoryItem item)
    {
        WriteUInt16LittleEndian(entry, (ushort)item.Index);
        WriteUInt16LittleEndian(entry[2..], (ushort)item.Count);
    }
}
