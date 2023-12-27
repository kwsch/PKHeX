using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core;

/// <summary>
/// Represents the data in a pouch pocket containing items of a similar type group.
/// </summary>
public abstract class InventoryPouch
{
    /// <inheritdoc cref="InventoryType"/>
    public readonly InventoryType Type;

    /// <summary> Valid item IDs that may be stored in the pouch. </summary>
    public readonly IItemStorage Info;

    /// <summary> Max quantity for a given item that can be stored in the pouch. </summary>
    public readonly int MaxCount;

    /// <summary> Count of item slots occupied in the pouch. </summary>
    public int Count => Items.Count(it => it.Count > 0);

    /// <summary> Checks if the player may run out of bag space when there are too many unique items to fit into the pouch. </summary>
    public bool IsCramped => Info.GetItems(Type).Length > Items.Length;

    public InventoryItem[] Items;

    /// <summary> Offset the items were read from. </summary>
    protected readonly int Offset;
    /// <summary> Size of the backing byte array that represents the pouch. </summary>
    protected readonly int PouchDataSize;

    protected InventoryPouch(InventoryType type, IItemStorage storage, int maxCount, int offset, int size = -1)
    {
        Items = [];
        Type = type;
        Info = storage;
        MaxCount = maxCount;
        Offset = offset;
        PouchDataSize = size > -1 ? size : storage.GetItems(Type).Length;
    }

    /// <summary> Reads the pouch from the backing <see cref="data"/>. </summary>
    public abstract void GetPouch(ReadOnlySpan<byte> data);
    /// <summary> Writes the pouch to the backing <see cref="data"/>. </summary>
    public abstract void SetPouch(Span<byte> data);

    /// <summary> Orders the <see cref="Items"/> based on <see cref="InventoryItem.Count"/> </summary>
    public void SortByCount(bool reverse = false) => Array.Sort(Items, (x, y) => Compare(x.Count, y.Count, reverse));

    /// <summary> Orders the <see cref="Items"/> based on <see cref="InventoryItem.Index"/> </summary>
    public void SortByIndex(bool reverse = false) => Array.Sort(Items, (x, y) => Compare(x.Index, y.Index, reverse));
    public void SortByName(string[] names, bool reverse = false) => Array.Sort(Items, (x, y) => Compare(x.Index, y.Index, names, reverse));
    public void SortByEmpty() => Array.Sort(Items, (x, y) => (x.Count == 0).CompareTo(y.Count == 0));
    public void SortBy<TItem, TCompare>(Func<TItem, TCompare> selector) where TItem : InventoryItem where TCompare : IComparable => Array.Sort(Items, (x, y) => selector((TItem)x).CompareTo(selector((TItem)y)));

    private static int Compare<TCompare>(int i1, int i2, IReadOnlyList<TCompare> n, bool rev) where TCompare : class, IComparable
    {
        if (i1 == 0 || i1 >= n.Count)
            return 1;
        if (i2 == 0 || i2 >= n.Count)
            return -1;
        return rev
            ? n[i2].CompareTo(n[i1])
            : n[i1].CompareTo(n[i2]);
    }

    private static int Compare(int i1, int i2, bool rev)
    {
        if (i1 == 0)
            return 1;
        if (i2 == 0)
            return -1;
        return rev
            ? i2.CompareTo(i1)
            : i1.CompareTo(i2);
    }

    /// <summary>
    /// Clears invalid items and clamps any item quantity that is out of range.
    /// </summary>
    /// <param name="maxItemID">Max item ID that exists in the game</param>
    /// <param name="HaX">Allow maximum-but-illegal quantities.</param>
    public void Sanitize(int maxItemID, bool HaX = false)
    {
        int ctr = 0;
        var arr = Items;
        for (int i = 0; i < arr.Length; i++)
        {
            var item = arr[i];
            if (item.Index != 0)
            {
                if ((uint)item.Index > maxItemID)
                    continue;
                if (!HaX && !Info.IsLegal(Type, item.Index, item.Count))
                    continue;
            }
            arr[ctr++] = arr[i]; // absorb down
        }
        while (ctr < arr.Length)
            arr[ctr++].Clear();
    }

    /// <summary>
    /// Clears all item slots with a quantity of zero and shifts any subsequent item slot up.
    /// </summary>
    public void ClearCount0()
    {
        int ctr = 0;
        for (int i = 0; i < Items.Length; i++)
        {
            if (Items[i].Count != 0)
                Items[ctr++] = Items[i];
        }

        while (ctr < Items.Length)
            Items[ctr++] = GetEmpty();
    }

    /// <summary>
    /// Clears all items in the pouch.
    /// </summary>
    public void RemoveAll()
    {
        foreach (var item in Items)
            item.Clear();
    }

    /// <summary>
    /// Clears all items in the pouch that match the criteria to delete.
    /// </summary>
    public void RemoveAll(Func<InventoryItem, bool> deleteCriteria)
    {
        foreach (var item in Items.Where(deleteCriteria))
            item.Clear();
    }

    /// <summary>
    /// Clears all items in the pouch that match the criteria to delete, considering index within the pouch.
    /// </summary>
    public void RemoveAll(Func<InventoryItem, int, bool> deleteCriteria)
    {
        foreach (var item in Items.Where(deleteCriteria))
            item.Clear();
    }

    /// <summary>
    /// Changes all the item quantities of present items to the specified <see cref="value"/>.
    /// </summary>
    public void ModifyAllCount(int value)
    {
        foreach (var item in Items.Where(z => z.Count != 0))
            item.Count = value;
    }

    /// <summary>
    /// Changes all the item quantities of items that match the criteria to the specified <see cref="value"/>.
    /// </summary>
    public void ModifyAllCount(int value, Func<InventoryItem, bool> modifyCriteria)
    {
        foreach (var item in Items.Where(z => z.Count != 0).Where(modifyCriteria))
            item.Count = value;
    }

    /// <summary>
    /// Changes all the item quantities of items that match the criteria to the specified <see cref="value"/>, considering index within the pouch.
    /// </summary>
    public void ModifyAllCount(int value, Func<InventoryItem, int, bool> modifyCriteria)
    {
        foreach (var item in Items.Where(z => z.Index != 0).Where(modifyCriteria))
            item.Count = value;
    }

    public void ModifyAllCount(SaveFile sav, int count = -1)
    {
        if (count <= 0)
            count = 1;
        foreach (var item in Items.Where(z => z.Index != 0))
            item.Count = GetSuggestedItemCount(sav, item.Index, count);
    }

    public void GiveAllItems(SaveFile sav, ReadOnlySpan<ushort> newItems, int count = -1)
    {
        foreach (var item in Items)
            item.Clear();

        foreach (var item in newItems)
        {
            if (Info.IsLegal(Type, item, count))
                GiveItem(sav, item, count);
        }
    }

    public void GiveAllItems(SaveFile sav, int count = -1)
    {
        var items = Info.GetItems(Type);
        foreach (var item in items)
            GiveItem(sav, item, count);

        foreach (var item in Items)
        {
            if (item.Count != 0)
                GetSuggestedItemCount(sav, item.Index, count);
        }
    }

    public int GiveItem(SaveFile sav, ushort itemID, int count = -1)
    {
        if (count <= 0)
            count = MaxCount;

        var existIndex = FindFirstMatchingSlot(itemID);
        if (existIndex >= 0)
            return AddCountTo(Items[existIndex], count, sav);

        var emptyIndex = FindFirstEmptySlot();
        if (emptyIndex < 0)
            return -1;

        var newItem = Items[emptyIndex];
        newItem.Index = itemID;
        newItem.SetNewDetails(0);
        return AddCountTo(newItem, count, sav);
    }

    private int AddCountTo(InventoryItem exist, int count, SaveFile sav)
    {
        var existCount = exist.Count;
        var newCount = existCount + count;

        // Sanitize the value
        newCount = GetSuggestedItemCount(sav, exist.Index, newCount);
        return exist.Count = newCount;
    }

    private int FindFirstEmptySlot()
    {
        for (int i = 0; i < Items.Length; i++)
        {
            if (Items[i].Index == 0)
                return i;
        }
        return -1;
    }

    private int FindFirstMatchingSlot(ushort itemID)
    {
        for (int i = 0; i < Items.Length; i++)
        {
            if (Items[i].Index == itemID)
                return i;
        }
        return -1;
    }

    public bool IsValidItemAndCount(ITrainerInfo sav, int item, bool HasNew, bool HaX, ref int count)
    {
        if (HaX && sav.Generation != 7) // Gen7 has true cap at 1023, keep 999 cap.
        {
            count = sav.Generation switch
            {
                // Cap at absolute maximum
                <= 2 when count > byte.MaxValue => byte.MaxValue,
                _ when count > ushort.MaxValue => ushort.MaxValue,
                _ => count,
            };
            return true;
        }

        if (count > MaxCount)
        {
            if (item == 797 && count >= 2) // Edge case when for some reason the item count for Z-Ring was 2 in an unedited save and set 1 after using PKHeX
                count = 2;
            else
                count = MaxCount; // Cap at pouch maximum
        }
        else if (count <= 0 && !HasNew)
        {
            return false;
        }

        count = GetSuggestedItemCount(sav, item, count);
        return true;
    }

    private int GetSuggestedItemCount(ITrainerInfo sav, int item, int requestVal = 1)
    {
        if (sav is SAV7b) // mixed pouch count caps
            return InventoryPouch7b.GetSuggestedCount(Type, item, requestVal);
        if (sav is SAV8SWSH)
            return InventoryPouch8.GetSuggestedCount(Type, item, requestVal);
        if (sav is not SAV8BS && ItemConverter.IsItemHM((ushort)item, sav.Generation))
            return 1;
        if (sav is SAV9SV)
            return InventoryPouch9.GetSuggestedCount(Type, item, requestVal);
        return Math.Min(MaxCount, requestVal);
    }

    public abstract InventoryItem GetEmpty(int itemID = 0, int count = 0);

    public ReadOnlySpan<ushort> GetAllItems()
    {
        return Info.GetItems(Type);
    }

    public bool CanContain(ushort itemIndex)
    {
        return Info.GetItems(Type).Contains(itemIndex);
    }
}

public static class InventoryPouchExtensions
{
    public static IReadOnlyList<T> LoadAll<T>(this IReadOnlyList<T> value, ReadOnlySpan<byte> data) where T : InventoryPouch
    {
        foreach (var p in value)
            p.GetPouch(data);
        return value;
    }

    public static void SaveAll(this IReadOnlyList<InventoryPouch> value, Span<byte> data)
    {
        foreach (var p in value)
            p.SetPouch(data);
    }
}
