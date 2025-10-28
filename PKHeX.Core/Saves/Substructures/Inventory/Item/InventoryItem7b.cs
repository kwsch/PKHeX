using System.Collections.Generic;

namespace PKHeX.Core;

public sealed record InventoryItem7b : InventoryItem, IItemFreeSpace, IItemNewFlag
{
    public bool IsFreeSpace { get; set; }
    public bool IsNew { get; set; }

    public override void Clear()
    {
        Index = Count = 0;
        IsFreeSpace = IsNew = false;
    }

    public static InventoryItem7b GetValue(uint value) => new()
    {
        // 15bit itemID
        // 15bit count
        // 1 bit new flag
        // 1 bit reserved
        Index = (int)(value & 0x7FF),
        Count = (int)((value >> 15) & 0x3FF), // clamp to sane values
        IsNew = (value & 0x40000000) != 0, // 30th bit is "NEW"
    };

    public uint GetValue(bool setNew, ICollection<int> original)
    {
        // 15bit itemID
        // 15bit count
        // 1 bit new flag
        // 1 bit reserved
        uint value = 0;
        value |= (uint)(Index & 0x7FF);
        value |= (uint)(Count & 0x3FF) << 15; // clamped to sane limit

        bool isNew = IsNew || (setNew && !original.Contains(Index));
        if (isNew)
            value |= 0x40000000;
        return value;
    }

    public override void SetNewDetails(int count)
    {
        base.SetNewDetails(count);
        IsNew = true;
    }

    /// <summary>
    /// Item has been matched to a previously existing item. Copy over the misc details.
    /// </summary>
    public override void MergeOverwrite<T>(T other)
    {
        base.MergeOverwrite(other);
        if (other is not InventoryItem7b item)
            return;
        IsNew = item.IsNew;
        IsFreeSpace = item.IsFreeSpace;
    }
}
