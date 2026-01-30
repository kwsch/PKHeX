using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class PlayerBag1 : PlayerBag
{
    public override IReadOnlyList<InventoryPouchGB> Pouches { get; }
    public override ItemStorage1 Info => ItemStorage1.Instance;
    public override int MaxQuantityHaX => byte.MaxValue;

    private static InventoryPouchGB[] GetPouches(ItemStorage1 info, SAV1Offsets offsets) =>
    [
        new(offsets.Items, 20, 99, info, InventoryType.Items),
        new(offsets.PCItems, 50, 99, info, InventoryType.PCItems),
    ];

    public PlayerBag1(SAV1 sav, SAV1Offsets offsets)
    {
        Pouches = GetPouches(ItemStorage1.Instance, offsets);
        Pouches.LoadAll(sav.Data);
    }

    public override void CopyTo(SaveFile sav) => CopyTo((SAV1)sav);
    public void CopyTo(SAV1 sav) => CopyTo(sav.Data);
    public void CopyTo(Span<byte> data) => Pouches.SaveAll(data);

    public override int GetMaxCount(InventoryType type, int itemIndex)
    {
        if (type is InventoryType.TMHMs && ItemConverter.IsItemHM1((ushort)itemIndex))
            return 1;
        return GetMaxCount(type);
    }
}
