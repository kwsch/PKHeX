using System;
using System.Collections.Generic;
using static PKHeX.Core.InventoryType;

namespace PKHeX.Core;

public sealed class PlayerBag2 : PlayerBag
{
    public override IReadOnlyList<InventoryPouchGB> Pouches { get; }
    public override ItemStorage2 Info { get; }
    public override int MaxQuantityHaX => byte.MaxValue;

    private static InventoryPouchGB[] GetPouches(ItemStorage2 info, SAV2Offsets offsets) =>
    [
        new(offsets.PouchTMHM, 57, 99, info, TMHMs),
        new(offsets.PouchItem, 20, 99, info, Items),
        new(offsets.PouchKey, 26, 99, info, KeyItems),
        new(offsets.PouchBall, 12, 99, info, Balls),
        new(offsets.PouchPC, 50, 99, info, PCItems),
    ];

    public PlayerBag2(SAV2 sav, ItemStorage2 info, SAV2Offsets offsets) : this(sav.Data, info, offsets) { }
    public PlayerBag2(ReadOnlySpan<byte> data, ItemStorage2 info, SAV2Offsets offsets)
    {
        Info = info;
        Pouches = GetPouches(info, offsets);
        Pouches.LoadAll(data);
    }

    public override void CopyTo(SaveFile sav) => CopyTo((SAV2)sav);
    public void CopyTo(SAV2 sav) => CopyTo(sav.Data);
    public void CopyTo(Span<byte> data) => Pouches.SaveAll(data);

    public override int GetMaxCount(InventoryType type, int itemIndex)
    {
        if (type is TMHMs && ItemConverter.IsItemHM2((ushort)itemIndex))
            return 1;
        return GetMaxCount(type);
    }
}
