using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Player item pouches storage
/// </summary>
/// <remarks>size=0xBB80 (<see cref="ItemSaveSize"/> items)</remarks>
public sealed class MyItem8b(SAV8BS sav, Memory<byte> raw) : MyItem(sav, raw)
{
    public const int ItemSaveSize = 3000;
    public const int SIZE = ItemSaveSize * InventoryItem8b.SIZE;

    public int GetItemQuantity(ushort itemIndex)
    {
        var ofs = InventoryPouch8b.GetItemOffset(itemIndex);
        var span = Data.Slice(ofs, InventoryItem8b.SIZE);
        var item = InventoryItem8b.Read(itemIndex, span);
        return item.Count;
    }

    public void SetItemQuantity(ushort itemIndex, int quantity)
    {
        var ofs = InventoryPouch8b.GetItemOffset(itemIndex);
        var span = Data.Slice(ofs, InventoryItem8b.SIZE);
        var item = InventoryItem8b.Read(itemIndex, span);
        item.Count = quantity;
        if (!item.IsValidSaveSortNumberCount) // not yet obtained
        {
            var type = GetType(itemIndex);
            item.SortOrder = GetNextSortIndex(type);
        }
        item.Write(span);
    }

    public static InventoryType GetType(ushort itemIndex) => ItemStorage8BDSP.GetInventoryPouch(itemIndex);

    public ushort GetNextSortIndex(InventoryType type)
    {
        var legal = ItemStorage8BDSP.GetLegal(type);
        ushort max = 0;
        foreach (var itemID in legal)
        {
            var ofs = InventoryPouch8b.GetItemOffset(itemID);
            var span = Data.Slice(ofs, InventoryItem8b.SIZE);
            var item = InventoryItem8b.Read(itemID, span);
            if (item.SortOrder > max)
                max = item.SortOrder;
        }
        return ++max;
    }

    public void CleanIllegalSlots()
    {
        var types = ItemStorage8BDSP.ValidTypes;
        var hashSet = new HashSet<ushort>(Legal.MaxItemID_8b);
        foreach (var type in types)
        {
            var items = ItemStorage8BDSP.GetLegal(type);
            foreach (var item in items)
                hashSet.Add(item);
        }

        // even though there are 3000, just overwrite the ones that people will mess up.
        var max = (ushort)sav.MaxItemID;
        for (ushort itemIndex = 0; itemIndex < max; itemIndex++)
        {
            if (!hashSet.Contains(itemIndex))
                DeleteItem(itemIndex);
        }
    }

    private void DeleteItem(ushort itemIndex) => InventoryItem8b.Clear(Data, InventoryPouch8b.GetItemOffset(itemIndex));
}
