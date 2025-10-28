using System.Collections.Generic;

namespace PKHeX.Core;

public sealed record InventoryItem7 : InventoryItem, IItemFreeSpaceIndex, IItemNewFlag
{
    public uint FreeSpaceIndex { get; set; }
    public bool IsNew { get; set; }

    public override void Clear()
    {
        Index = Count = 0;
        FreeSpaceIndex = 0;
        IsNew = false;
    }

    public static InventoryItem7 GetValue(uint value) => new()
    {
        // 10bit itemID
        // 10bit count
        // 10bit freespace index
        // 1 bit new flag
        // 1 bit reserved
        Index = (int)(value & 0x3FF),
        Count = (int)((value >> 10) & 0x3FF),
        IsNew = (value & 0x40000000) != 0, // 30th bit is "NEW"
        FreeSpaceIndex = ((value >> 20) & 0x3FF), // "FREE SPACE" sortIndex
    };

    public uint GetValue(bool setNew, ICollection<int> original)
    {
        // Build Item Value
        uint value = 0;
        value |= (uint)(Index & 0x3FF);
        value |= (uint)(Count & 0x3FF) << 10;

        bool isNew = IsNew || (setNew && !original.Contains(Index));
        if (isNew)
            value |= 0x40000000;
        value |= (FreeSpaceIndex & 0x3FFu) << 20;
        return value;
    }
}
