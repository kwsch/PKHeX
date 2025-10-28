using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PKHeX.Core;

public sealed class InventoryPouch9a(InventoryType type, IItemStorage info, int maxCount, uint pouch)
    : InventoryPouch(type, info, maxCount, 0)
{
    public bool SetNew { get; set; }
    public uint PouchIndex { get; set; } = pouch;

    public override InventoryItem9a GetEmpty(int itemID = 0, int count = 0) => new() { Index = itemID, Count = count, IsNew = count != 0 };
    public static int GetItemOffset(ushort index) => InventoryItem9a.SIZE * index;
    public static Span<byte> GetItemSpan(Span<byte> block, ushort index) => block.Slice(GetItemOffset(index), InventoryItem9a.SIZE);

    public override void GetPouch(ReadOnlySpan<byte> data)
    {
        var LegalItems = Info.GetItems(Type);
        var items = new InventoryItem9a[LegalItems.Length];

        int ctr = 0;
        foreach (var index in LegalItems)
            items[ctr++] = GetItem(data, index);

        Items = items;
    }

    public static InventoryItem9a GetItem(ReadOnlySpan<byte> block, ushort itemID)
    {
        var ofs = GetItemOffset(itemID);
        return InventoryItem9a.Read(itemID, block[ofs..]);
    }

    public override void SetPouch(Span<byte> block)
    {
        // Write all the item slots still present in the pouch. Keep track of the item IDs processed.
        var items = (InventoryItem9a[])Items;
        var processed = new HashSet<ushort>(items.Length);

        var legal = Info.GetItems(Type);
        foreach (var item in items)
        {
            var index = (ushort)item.Index;
            if (index == 0)
                continue;
            if (!legal.Contains(index))
            {
                Debug.WriteLine($"Invalid Item ID returned within this pouch: {index}");
                continue;
            }

            var span = GetItemSpan(block, index);
            var original = InventoryItem9a.Read(index, span);
            EnsureFlagsConsistent(item, original);

            // In the event of duplicates, we just overwrite what was previously written by a prior duplicate.
            // Don't care if we've already processed this item, just write it again.
            item.Write(span);
            processed.Add(index);
        }

        // For all the items that were not present in the pouch, clear the data for them.
        foreach (var index in legal)
        {
            if (processed.Contains(index))
                continue;
            SetQuantityZero(block, index);
        }

        if (Type is InventoryType.KeyItems) // Key items that can never be saved with
            SetQuantityZero(block, 2598); // Tasty Trash
    }

    private void EnsureFlagsConsistent(InventoryItem9a item, InventoryItem9a original)
    {
        if (item.Count != 0)
        {
            // Ensure the flag is set; 0->X and Y->Z
            item.IsUpdated = true;
            if (!original.IsUpdated && SetNew)
                item.IsNew = true;
        }
        else
        {
            if (!item.IsUpdated)
            {
                item.IsNew = item.IsFavorite = false;
                if (item.Pouch is not (0 or uint.MaxValue))
                    item.Pouch = 0;
            }
        }

        if (item.IsUpdated)
            item.Pouch = PouchIndex; // ensure the pouch is set
    }

    public static void SetQuantityZero(Span<byte> block, ushort index)
    {
        var span = GetItemSpan(block, index);
        var exist = InventoryItem9a.Read(index, span);
        if (exist.Count == 0)
            return;
        exist.Count = 0;
        exist.IsUpdated = true;
        exist.Write(span);
    }
}
