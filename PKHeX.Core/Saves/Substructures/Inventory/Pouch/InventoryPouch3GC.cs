using System;
using System.Diagnostics.CodeAnalysis;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class InventoryPouch3GC(InventoryType type, IItemStorage info, int maxCount, int offset, [ConstantExpected] int size)
    : InventoryPouch(type, info, maxCount, offset, size)
{
    public override InventoryItem GetEmpty(int itemID = 0, int count = 0) => new() { Index = itemID, Count = count };

    public override void GetPouch(ReadOnlySpan<byte> data)
    {
        var span = data[Offset..];
        var items = new InventoryItem[PouchDataSize];
        for (int i = 0; i < items.Length; i++)
        {
            var item = span.Slice(i * 4, 4);
            items[i] = new InventoryItem
            {
                Index = ReadUInt16BigEndian(item),
                Count = ReadUInt16BigEndian(item[2..]),
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
            var slice = span.Slice(i * 4, 4);
            WriteUInt16BigEndian(slice,      (ushort)item.Index);
            WriteUInt16BigEndian(slice[2..], (ushort)item.Count);
        }
    }
}
