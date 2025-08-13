using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PKHeX.Core;

public sealed class InventoryPouch9(InventoryType type, IItemStorage info, int maxCount, uint pouch)
    : InventoryPouch(type, info, maxCount, 0)
{
    public bool SetNew { get; set; }
    public uint PouchIndex { get; set; } = pouch;

    public override InventoryItem9 GetEmpty(int itemID = 0, int count = 0) => new() { Index = itemID, Count = count, IsNew = count != 0 };
    public static int GetItemOffset(ushort index) => InventoryItem9.SIZE * index;
    public static Span<byte> GetItemSpan(Span<byte> block, ushort index) => block.Slice(GetItemOffset(index), InventoryItem9.SIZE);

    public override void GetPouch(ReadOnlySpan<byte> data)
    {
        var LegalItems = Info.GetItems(Type);
        var items = new InventoryItem9[LegalItems.Length];

        int ctr = 0;
        foreach (var index in LegalItems)
            items[ctr++] = GetItem(data, index);

        Items = items;
    }

    public static InventoryItem9 GetItem(ReadOnlySpan<byte> block, ushort itemID)
    {
        var ofs = GetItemOffset(itemID);
        return InventoryItem9.Read(itemID, block[ofs..]);
    }

    public override void SetPouch(Span<byte> block)
    {
        // Write all the item slots still present in the pouch. Keep track of the item IDs processed.
        var items = (InventoryItem9[])Items;
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
            var original = InventoryItem9.Read(index, span);
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
    }

    private void EnsureFlagsConsistent(InventoryItem9 item, InventoryItem9 original)
    {
        if (item.Count != 0)
        {
            // Ensure the flag is set; 0->X and Y->Z
            item.IsObtained = true;
            if (!original.IsObtained && SetNew)
                item.IsNew = true;
        }
        else
        {
            if (!item.IsObtained)
            {
                item.IsNew = item.IsFavorite = false;
                if (item.Pouch is not (0 or uint.MaxValue))
                    item.Pouch = 0;
            }
        }

        if (item.IsObtained)
            item.Pouch = PouchIndex; // ensure the pouch is set
    }

    public static void SetQuantityZero(Span<byte> block, ushort index)
    {
        var span = GetItemSpan(block, index);
        var exist = InventoryItem9.Read(index, span);
        if (exist.Count == 0)
            return;
        exist.Count = 0;
        exist.IsObtained = true;
        exist.Write(span);
    }

    public static int GetSuggestedCount(InventoryType t, int item, int requestVal)
    {
        bool isPick = item is (>= 2334 and <= 2342) or (>= 2385 and <= 2394);
        bool isAccessory = item is (>= 2311 and <= 2400) or (>= 2417 and <= 2437); // tablecloths, chairs, cups, etc
        return t switch
        {
            // Picnic table accessories are clamped to 1, let actual ingredients and sandwich picks be whatever
            InventoryType.Ingredients => !isPick && isAccessory ? 1 : requestVal,
            _ => requestVal,
        };
    }
}
