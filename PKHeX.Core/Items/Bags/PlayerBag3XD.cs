using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class PlayerBag3XD : PlayerBag
{
    public override IReadOnlyList<InventoryPouch3GC> Pouches { get; } = GetPouches(ItemStorage3XD.Instance);
    public override ItemStorage3XD Info => ItemStorage3XD.Instance;

    private static InventoryPouch3GC[] GetPouches(ItemStorage3XD info) =>
    [
        new(0x000, 30, 999, info, InventoryType.Items), // 20 COLO, 30 XD
        new(0x078, 43, 001, info, InventoryType.KeyItems),
        new(0x124, 16, 999, info, InventoryType.Balls),
        new(0x164, 64, 999, info, InventoryType.TMHMs),
        new(0x264, 46, 999, info, InventoryType.Berries),
        new(0x31C, 03, 999, info, InventoryType.Medicine), // Cologne
        new(0x328, 60, 001, info, InventoryType.BattleItems), // Disc
    ];

    public PlayerBag3XD(SAV3XD sav) : this(sav.Data[sav.OFS_Pouch..]) { }
    public PlayerBag3XD(Span<byte> data) => Pouches.LoadAll(data);

    public override void CopyTo(SaveFile sav) => CopyTo((SAV3XD)sav);
    public void CopyTo(SAV3XD sav) => CopyTo(sav.Data[sav.OFS_Pouch..]);
    public void CopyTo(Span<byte> data) => Pouches.SaveAll(data);
}
