using System;
using System.Collections.Generic;
using static PKHeX.Core.InventoryType;

namespace PKHeX.Core;

public sealed class PlayerBag3Colosseum : PlayerBag
{
    private const int BaseOffset = 0x007F8;

    public override IReadOnlyList<InventoryPouch3GC> Pouches { get; } = GetPouches(ItemStorage3Colo.Instance);
    public override ItemStorage3Colo Info => ItemStorage3Colo.Instance;

    private static InventoryPouch3GC[] GetPouches(ItemStorage3Colo info) =>
    [
        new(0x000, 20, 099, info, Items),
        new(0x050, 43, 001, info, KeyItems),
        new(0x0FC, 16, 099, info, Balls),
        new(0x13C, 64, 099, info, TMHMs),
        new(0x23C, 46, 999, info, Berries),
        new(0x2F4, 03, 099, info, Medicine),
    ];

    public PlayerBag3Colosseum(SAV3Colosseum sav) => Pouches.LoadAll(sav.Data[BaseOffset..]);
    public PlayerBag3Colosseum(ReadOnlySpan<byte> data) => Pouches.LoadAll(data);

    public override void CopyTo(SaveFile sav) => CopyTo((SAV3Colosseum)sav);
    public void CopyTo(SAV3Colosseum sav) => CopyTo(sav.Data[BaseOffset..]);
    public void CopyTo(Span<byte> data) => Pouches.SaveAll(data);
}
