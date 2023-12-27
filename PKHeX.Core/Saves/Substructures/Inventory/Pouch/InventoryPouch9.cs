using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PKHeX.Core;

public sealed class InventoryPouch9(InventoryType type, IItemStorage info, int maxCount, uint pouch)
    : InventoryPouch(type, info, maxCount, 0)
{
    public bool SetNew { get; set; }
    public uint PouchIndex { get; set; } = pouch;

    public override InventoryItem9 GetEmpty(int itemID = 0, int count = 0) => new() { Index = itemID, Count = count, IsNew = true };

    public override void GetPouch(ReadOnlySpan<byte> data)
    {
        var LegalItems = Info.GetItems(Type);
        var items = new InventoryItem9[LegalItems.Length];

        int ctr = 0;
        foreach (var index in LegalItems)
        {
            var item = GetItem(data, index);
            if (!item.IsValidPouch)
                continue;
            items[ctr++] = item;
        }
        while (ctr != LegalItems.Length)
            items[ctr++] = new InventoryItem9();

        Items = items;
    }

    public static InventoryItem9 GetItem(ReadOnlySpan<byte> data, ushort itemID)
    {
        var ofs = GetItemOffset(itemID);
        return InventoryItem9.Read(itemID, data[ofs..]);
    }

    public override void SetPouch(Span<byte> data)
    {
        HashSet<ushort> processed = [];

        // Write all the item slots still present in the pouch. Keep track of the item IDs processed.
        var items = (InventoryItem9[])Items;

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
                item.IsNew |= !original.IsValidPouch;
            }

            item.Pouch = PouchIndex;

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

    public static int GetItemOffset(ushort index) => InventoryItem9.SIZE * index;

    public static void ClearItem(Span<byte> data, ushort index) => InventoryItem9.Clear(data, GetItemOffset(index));

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
