using System;

namespace PKHeX.Core;

public interface IItemStorage
{
    bool IsLegal(InventoryType type, int itemIndex, int itemCount);
    ReadOnlySpan<ushort> GetItems(InventoryType type);
}
