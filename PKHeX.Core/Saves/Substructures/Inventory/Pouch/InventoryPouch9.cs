using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PKHeX.Core;

public sealed class InventoryPouch9 : InventoryPouch
{
    public bool SetNew { get; set; }
    public uint PouchIndex { get; set; }

    public InventoryPouch9(InventoryType type, ushort[] legal, int maxCount, uint pouch,
        Func<ushort, bool>? isLegal)
        : base(type, legal, maxCount, 0, isLegal: isLegal)
    {
        PouchIndex = pouch;
    }

    public override InventoryItem GetEmpty(int itemID = 0, int count = 0) => new InventoryItem9 { Index = itemID, Count = count, IsNew = true };

    public override void GetPouch(ReadOnlySpan<byte> data)
    {
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

    public InventoryItem9 GetItem(ReadOnlySpan<byte> data, ushort itemID)
    {
        var ofs = GetItemOffset(itemID);
        return InventoryItem9.Read(itemID, data[ofs..]);
    }

    public override void SetPouch(Span<byte> data)
    {
        HashSet<ushort> processed = new();

        // Write all the item slots still present in the pouch. Keep track of the item IDs processed.
        var items = (InventoryItem9[])Items;

        foreach (var item in items)
        {
            var index = (ushort)item.Index;
            if (index == 0)
                continue;
            var isInLegal = Array.IndexOf(LegalItems, index);
            if (isInLegal == -1)
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

            if (!processed.Contains(index)) // we will allow duplicate item definitions, but they'll overwrite instead of sum/separate.
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

    public static int GetItemOffset(ushort index) => (InventoryItem9.SIZE * index);

    public void ClearItem(Span<byte> data, ushort index) => InventoryItem9.Clear(data, GetItemOffset(index));
}
