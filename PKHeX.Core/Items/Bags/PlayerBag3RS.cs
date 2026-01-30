using System;
using System.Collections.Generic;
using static PKHeX.Core.InventoryType;

namespace PKHeX.Core;

public sealed class PlayerBag3RS : PlayerBag
{
    private const int BaseOffset = 0x0498;

    public override IReadOnlyList<InventoryPouch3> Pouches { get; } = GetPouches(ItemStorage3RS.Instance);
    public override ItemStorage3RS Info => ItemStorage3RS.Instance;

    private static InventoryPouch3[] GetPouches(ItemStorage3RS info) =>
    [
        new(0x0C8, 20, 099, info, Items),
        new(0x118, 20, 001, info, KeyItems),
        new(0x168, 16, 099, info, Balls),
        new(0x1A8, 64, 099, info, TMHMs),
        new(0x2A8, 46, 999, info, Berries),
        new(0x000, 50, 999, info, PCItems),
    ];

    public PlayerBag3RS(SAV3RS sav) : this(sav.Large[BaseOffset..]) { }
    public PlayerBag3RS(ReadOnlySpan<byte> data) => Pouches.LoadAll(data);

    public override void CopyTo(SaveFile sav) => CopyTo((SAV3RS)sav);
    public void CopyTo(SAV3RS sav) => CopyTo(sav.Large[BaseOffset..]);
    public void CopyTo(Span<byte> data) => Pouches.SaveAll(data);

    public override int GetMaxCount(InventoryType type, int itemIndex)
    {
        if (type is TMHMs && ItemConverter.IsItemHM3((ushort)itemIndex))
            return 1;
        return GetMaxCount(type);
    }
}
