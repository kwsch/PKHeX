using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class InventoryPouch7(InventoryType type, IItemStorage info, int maxCount, int offset)
    : InventoryPouch(type, info, maxCount, offset)
{
    public bool SetNew { get; set; }
    private int[] OriginalItems = [];

    public override InventoryItem7 GetEmpty(int itemID = 0, int count = 0) => new() { Index = itemID, Count = count };

    public override void GetPouch(ReadOnlySpan<byte> data)
    {
        var span = data[Offset..];
        var items = new InventoryItem7[PouchDataSize];
        for (int i = 0; i < items.Length; i++)
        {
            var item = span.Slice(i * 4, 4);
            uint val = ReadUInt32LittleEndian(item);
            items[i] = InventoryItem7.GetValue(val);
        }
        Items = items;
        OriginalItems = Array.ConvertAll(items, z => z.Index);
    }

    public override void SetPouch(Span<byte> data)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(Items.Length, PouchDataSize);

        var span = data[Offset..];
        var items = (InventoryItem7[])Items;
        for (int i = 0; i < Items.Length; i++)
        {
            var item = span.Slice(i * 4, 4);
            var value = items[i].GetValue(SetNew, OriginalItems);
            WriteUInt32LittleEndian(item, value);
        }
    }
}
