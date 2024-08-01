using System;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

public sealed class InventoryPouchGB(InventoryType type, IItemStorage info, int maxCount, int offset, [ConstantExpected] int size)
    : InventoryPouch(type, info, maxCount, offset, size)
{
    public override InventoryItem GetEmpty(int itemID = 0, int count = 0) => new() { Index = itemID, Count = count };

    public override void GetPouch(ReadOnlySpan<byte> data)
    {
        var LegalItems = Info.GetItems(Type);
        var items = new InventoryItem[PouchDataSize];
        if (Type == InventoryType.TMHMs)
        {
            int slot = 0;
            for (int i = 0; i < items.Length; i++)
            {
                if (data[Offset + i] != 0)
                {
                    items[slot++] = new InventoryItem
                    {
                        Index = LegalItems[i],
                        Count = data[Offset + i],
                    };
                }
            }
            while (slot < items.Length)
            {
                items[slot++] = new InventoryItem
                {
                    Index = 0,
                    Count = 0,
                };
            }
        }
        else
        {
            int numStored = data[Offset];
            if (numStored > PouchDataSize) // uninitialized yellow (0xFF), sanity check for out-of-bounds values
                numStored = 0;
            for (int i = 0; i < numStored; i++)
            {
                items[i] = Type switch
                {
                    InventoryType.KeyItems =>
                        new InventoryItem {Index = data[Offset + i + 1], Count = 1},
                    _ =>
                        new InventoryItem {Index = data[Offset + (i * 2) + 1], Count = data[Offset + (i * 2) + 2]},
                };
            }
            for (int i = numStored; i < items.Length; i++)
            {
                items[i] = new InventoryItem
                {
                    Index = 0,
                    Count = 0,
                };
            }
        }
        Items = items;
    }

    public override void SetPouch(Span<byte> data)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(Items.Length, PouchDataSize);

        ClearCount0();

        switch (Type)
        {
            case InventoryType.TMHMs:
                var LegalItems = Info.GetItems(Type);
                foreach (InventoryItem t in Items)
                {
                    int index = LegalItems.IndexOf((ushort)t.Index);
                    if (index < 0) // enforce correct pouch
                        continue;
                    data[Offset + index] = (byte)t.Count;
                }
                break;
            case InventoryType.KeyItems:
                for (int i = 0; i < Items.Length; i++)
                {
                    data[Offset + i + 1] = (byte)Items[i].Index;
                }
                data[Offset] = (byte)Count;
                data[Offset + 1 + Count] = 0xFF;
                break;
            default:
                for (int i = 0; i < Items.Length; i++)
                {
                    data[Offset + (i * 2) + 1] = (byte)Items[i].Index;
                    data[Offset + (i * 2) + 2] = (byte)Items[i].Count;
                }
                data[Offset] = (byte)Count;
                data[Offset + 1 + (2 * Count)] = 0xFF;
                break;
        }
    }
}
