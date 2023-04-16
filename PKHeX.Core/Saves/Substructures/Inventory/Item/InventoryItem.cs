using System;

namespace PKHeX.Core;

public record InventoryItem
{
    /// <summary> Item ID </summary>
    public int Index { get; set; }
    /// <summary> Quantity </summary>
    public int Count { get; set; }

    /// <summary>
    /// Resets all data in the object back to zero.
    /// </summary>
    public virtual void Clear() => Index = Count = 0;

    /// <summary>
    /// Item has been detected as a newly given item. Set the appropriate new details.
    /// </summary>
    /// <param name="count">Final count of items.</param>
    public virtual void SetNewDetails(int count) => Count = count;

    /// <summary>
    /// Item has been matched to a previously existing item. Copy over the misc details.
    /// </summary>
    public virtual void MergeOverwrite<T>(T other) where T : InventoryItem => Count = Math.Max(Count, other.Count);
}
