using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Represents a player's inventory bag and pouch rules.
/// </summary>
public abstract class PlayerBag
{
    /// <summary>
    /// Gets the pouches represented by the bag.
    /// </summary>
    public abstract IReadOnlyList<InventoryPouch> Pouches { get; }

    public abstract IItemStorage Info { get; }
    public virtual int MaxQuantityHaX => ushort.MaxValue;

    /// <summary>
    /// Gets the pouch for the specified inventory type.
    /// </summary>
    public InventoryPouch GetPouch(InventoryType type)
    {
        foreach (var pouch in Pouches)
        {
            if (pouch.Type == type)
                return pouch;
        }
        throw new ArgumentOutOfRangeException(nameof(type));
    }

    /// <summary>
    /// Gets the base max count for the specified pouch.
    /// </summary>
    protected int GetMaxCount(InventoryType type) => GetPouch(type).MaxCount;

    /// <summary>
    /// Gets the max count for the specific item in the specified pouch.
    /// </summary>
    public virtual int GetMaxCount(InventoryType type, int itemIndex) => GetMaxCount(type);

    /// <summary>
    /// Gets a suggested count for an item after applying pouch-specific rules.
    /// </summary>
    public int Clamp(InventoryType type, int itemIndex, int requestVal)
        => Math.Clamp(requestVal, 0, GetMaxCount(type, itemIndex));

    /// <summary>
    /// Validates and clamps an item count for the specified pouch.
    /// </summary>
    public bool IsQuantitySane(InventoryType type, int itemIndex, ref int count, bool hasNew, bool HaX = false)
    {
        if (HaX)
        {
            count = Math.Clamp(count, 0, MaxQuantityHaX);
            return true;
        }
        if (count <= 0)
            return count == 0 && hasNew;

        count = Clamp(type, itemIndex, count);
        return true;
    }

    /// <summary>
    /// Checks whether an item is legal for the specified pouch.
    /// </summary>
    public bool IsLegal(InventoryType type, int itemIndex, int itemCount) => Info.IsLegal(type, itemIndex, itemCount);

    /// <summary>
    /// Persists pouch edits back to the save data source.
    /// </summary>
    public abstract void CopyTo(SaveFile sav);
}

internal sealed class EmptyPlayerBag : PlayerBag
{
    public override IReadOnlyList<InventoryPouch> Pouches { get; } = [];
    public override ItemStorage1 Info => ItemStorage1.Instance; // anything
    public override void CopyTo(SaveFile sav) { }
}
