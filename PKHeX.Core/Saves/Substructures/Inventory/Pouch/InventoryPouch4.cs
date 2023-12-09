using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Inventory Pouch with 4 bytes per item (u16 ID, u16 count)
/// </summary>
public sealed class InventoryPouch4(InventoryType type, IItemStorage info, int maxCount, int offset)
    : InventoryPouch(type, info, maxCount, offset)
{
    // size: 32bit
    // u16 id
    // u16 count

    public override InventoryItem GetEmpty(int itemID = 0, int count = 0) => new() { Index = itemID, Count = count };

    public override void GetPouch(ReadOnlySpan<byte> data)
    {
        var span = data[Offset..];
        var items = new InventoryItem[PouchDataSize];
        for (int i = 0; i < items.Length; i++)
        {
            var entry = span.Slice(i * 4, 4);
            items[i] = new InventoryItem
            {
                Index = ReadUInt16LittleEndian(entry),
                Count = ReadUInt16LittleEndian(entry[2..]),
            };
        }
        Items = items;
    }

    public override void SetPouch(Span<byte> data)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(Items.Length, PouchDataSize);

        var span = data[Offset..];
        for (int i = 0; i < Items.Length; i++)
        {
            var item = Items[i];
            var entry = span.Slice(i * 4, 4);
            WriteUInt16LittleEndian(entry, (ushort)item.Index);
            WriteUInt16LittleEndian(entry[2..], (ushort)item.Count);
        }
    }
}
