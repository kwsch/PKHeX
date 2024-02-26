using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PKHeX.Core;

public sealed class InventoryPouch8b(InventoryType type, IItemStorage info, int maxCount)
    : InventoryPouch(type, info, maxCount, 0)
{
    public bool SetNew { get; set; }

    public override InventoryItem8b GetEmpty(int itemID = 0, int count = 0) => new() { Index = itemID, Count = count, IsNew = true };

    public override void GetPouch(ReadOnlySpan<byte> data)
    {
        var LegalItems = Info.GetItems(Type);
        var items = new InventoryItem8b[LegalItems.Length];

        int ctr = 0;
        foreach (var index in LegalItems)
        {
            var item = GetItem(data, index);
            if (!item.IsValidSaveSortNumberCount)
                continue;
            items[ctr++] = item;
        }
        while (ctr != LegalItems.Length)
            items[ctr++] = new InventoryItem8b();

        Items = items;
        SortBy<InventoryItem8b, ushort>(z => !z.IsValidSaveSortNumberCount ? (ushort)0xFFFF : z.SortOrder);
    }

    public static InventoryItem8b GetItem(ReadOnlySpan<byte> data, ushort itemID)
    {
        var ofs = GetItemOffset(itemID);
        return InventoryItem8b.Read(itemID, data[ofs..]);
    }

    public override void SetPouch(Span<byte> data)
    {
        HashSet<ushort> processed = [];

        // Write all the item slots still present in the pouch. Keep track of the item IDs processed.
        var items = (InventoryItem8b[])Items;
        for (int i = 0; i < items.Length; i++)
            items[i].SortOrder = (ushort)(i + 1);

        var LegalItems = Info.GetItems(Type);
        foreach (var item in items)
        {
            var index = (ushort)item.Index;
            if (index == 0)
                continue;
            if (!LegalItems.Contains(index))
            {
                Debug.WriteLine($"Invalid Item ID returned within this pouch: {index}");
                continue;
            }

            if (SetNew && item.Index != 0)
            {
                var original = GetItem(data, (ushort)item.Index);
                item.IsNew |= !original.IsValidSaveSortNumberCount;
            }

            var ofs = GetItemOffset(index);
            item.Write(data[ofs..]);

            // In the event of duplicates, we just overwrite what was previously written by a prior duplicate.
            // Don't care if we've already processed this item, just write it again.
            processed.Add(index);
        }

        // For all the items that were not present in the pouch, clear the data for them.
        foreach (var index in LegalItems)
        {
            if (processed.Contains(index))
                continue;
            ClearItem(data, index);
        }
    }

    public static int GetItemOffset(ushort index) => InventoryItem8b.SIZE * index;

    public static void ClearItem(Span<byte> data, ushort index) => InventoryItem8b.Clear(data, GetItemOffset(index));
}
