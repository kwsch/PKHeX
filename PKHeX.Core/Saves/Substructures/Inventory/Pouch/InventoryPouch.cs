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
    protected readonly IItemStorage Info;

    /// <summary> Max quantity for a given item that can be stored in the pouch. </summary>
    public readonly int MaxCount;

    /// <summary> Count of item slots occupied in the pouch. </summary>
    public int Count => Items.Count(it => it.Count > 0);

    /// <summary> Checks if the player may run out of bag space when there are too many unique items to fit into the pouch. </summary>
    public bool IsCramped => Info.GetItems(Type).Length > Items.Length;

    public abstract InventoryItem[] Items { get; }

    /// <summary> Offset the items were read from. </summary>
    protected readonly int Offset;
    /// <summary> Size of the backing byte array that represents the pouch. </summary>
    protected readonly int PouchDataSize;

    protected InventoryPouch(InventoryType type, IItemStorage storage, int maxCount, int offset, int size = -1)
    {
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
                //if (!HaX && !Info.IsLegal(Type, item.Index, item.Count))
                //    continue;
            }
            arr[ctr++] = arr[i]; // absorb down
        }
        while (ctr < arr.Length)
            arr[ctr++] = GetEmpty();
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

    /// <summary>
    /// Changes all the item quantities of present items to the specified <see cref="count"/>, using pouch rules.
    /// </summary>
    public void ModifyAllCount(PlayerBag bag, int count = -1)
    {
        foreach (var item in Items.Where(z => z.Index != 0))
            item.Count = bag.Clamp(Type, item.Index, count);
    }

    public void GiveAllItems(PlayerBag bag, ReadOnlySpan<ushort> newItems, int count = -1)
    {
        foreach (var item in Items)
            item.Clear();

        foreach (var item in newItems)
        {
            if (bag.IsLegal(Type, item, count))
                GiveItem(bag, item, count);
        }
    }

    public void GiveAllItems(PlayerBag bag, int count = -1)
    {
        var items = Info.GetItems(Type);
        foreach (var item in items)
            GiveItem(bag, item, count);

        foreach (var item in Items)
        {
            if (item.Count != 0)
                bag.Clamp(Type, item.Index, count);
        }
    }

    public int GiveItem(PlayerBag bag, ushort itemID, int count = -1)
    {
        if (count <= 0)
            count = bag.GetMaxCount(Type, itemID);

        var existIndex = FindIndexFirstMatchingSlot(itemID);
        if (existIndex >= 0)
            return AddCountTo(bag, Items[existIndex], count);

        var emptyIndex = FindIndexFirstEmptySlot();
        if (emptyIndex < 0)
            return -1;

        var newItem = Items[emptyIndex];
        newItem.Index = itemID;
        newItem.SetNewDetails(0);
        return AddCountTo(bag, newItem, count);
    }

    private int AddCountTo(PlayerBag bag, InventoryItem exist, int count)
    {
        var existCount = exist.Count;
        var newCount = existCount + count;

        // Sanitize the value
        newCount = bag.Clamp(Type, exist.Index, newCount);
        return exist.Count = newCount;
    }

    public int FindIndexFirstEmptySlot() => FindIndexFirstMatchingSlot(0);

    private int FindIndexFirstMatchingSlot(ushort itemID)
    {
        for (int i = 0; i < Items.Length; i++)
        {
            if (Items[i].Index == itemID)
                return i;
        }
        return -1;
    }

    public abstract InventoryItem GetEmpty(int itemID = 0, int count = 0);

    public ReadOnlySpan<ushort> GetAllItems() => Info.GetItems(Type);
    public bool CanContain(ushort itemID) => Info.GetItems(Type).Contains(itemID);
    public bool HasItem(ushort itemID) => Items.Any(it => it.Index == itemID && it.Count > 0);
}

public static class InventoryPouchExtensions
{
    public static void LoadAll<T>(this IReadOnlyList<T> value, ReadOnlySpan<byte> data) where T : InventoryPouch
    {
        foreach (var p in value)
            p.GetPouch(data);
    }

    public static void SaveAll(this IReadOnlyList<InventoryPouch> value, Span<byte> data)
    {
        foreach (var p in value)
            p.SetPouch(data);
    }
}
