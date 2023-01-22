using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class InventoryPouch7 : InventoryPouch
{
    public bool SetNew { get; set; }
    private int[] OriginalItems = Array.Empty<int>();

    public InventoryPouch7(InventoryType type, ushort[] legal, int maxCount, int offset)
        : base(type, legal, maxCount, offset) { }

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
        if (Items.Length != PouchDataSize)
            throw new ArgumentException("Item array length does not match original pouch size.");

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
