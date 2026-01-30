using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class PlayerBag3Colosseum : PlayerBag
{
    public override IReadOnlyList<InventoryPouch3GC> Pouches { get; } = GetPouches(ItemStorage3Colo.Instance);
    public override ItemStorage3Colo Info => ItemStorage3Colo.Instance;

    private static InventoryPouch3GC[] GetPouches(ItemStorage3Colo info) =>
    [
        new(0x007F8, 20, 099, info, InventoryType.Items),
        new(0x00848, 43, 001, info, InventoryType.KeyItems),
        new(0x008F4, 16, 099, info, InventoryType.Balls),
        new(0x00934, 64, 099, info, InventoryType.TMHMs),
        new(0x00A34, 46, 999, info, InventoryType.Berries),
        new(0x00AEC, 03, 099, info, InventoryType.Medicine),
    ];

    public PlayerBag3Colosseum(SAV3Colosseum sav) => Pouches.LoadAll(sav.Data);
    public PlayerBag3Colosseum(ReadOnlySpan<byte> data) => Pouches.LoadAll(data);

    public override void CopyTo(SaveFile sav) => CopyTo((SAV3Colosseum)sav);
    public void CopyTo(SAV3Colosseum sav) => CopyTo(sav.Data);
    public void CopyTo(Span<byte> data) => Pouches.SaveAll(data);
}
