using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class PlayerBag7b : PlayerBag
{
    public override IReadOnlyList<InventoryPouch7b> Pouches { get; } = GetPouches(ItemStorage7GG.Instance);
    public override ItemStorage7GG Info => ItemStorage7GG.Instance;

    private static InventoryPouch7b[] GetPouches(ItemStorage7GG info) =>
    [
        new(0x0000, 060, 999, info, InventoryType.Medicine), // 0
        new(0x00F0, 108, 001, info, InventoryType.TMHMs), // 1
        new(0x02A0, 200, 999, info, InventoryType.Candy), // 2
        new(0x05C0, 150, 999, info, InventoryType.ZCrystals), // 3
        new(0x0818, 050, 999, info, InventoryType.Balls), // 4
        new(0x08E0, 150, 999, info, InventoryType.BattleItems), // 5 - Battle Items and Mega Stones mixed.
        new(0x0B38, 150, 999, info, InventoryType.Items), // 6 - Items and Key Items mixed.
    ];

    public PlayerBag7b(SAV7b sav) : this(sav.Items.Data) { }
    public PlayerBag7b(Span<byte> data) => Pouches.LoadAll(data);

    public override void CopyTo(SaveFile sav) => CopyTo((SAV7b)sav);
    public void CopyTo(SAV7b sav) => CopyTo(sav.Items.Data);
    public void CopyTo(Span<byte> data) => Pouches.SaveAll(data);

    public override int GetMaxCount(InventoryType type, int itemIndex) => type switch
    {
        InventoryType.BattleItems when itemIndex > 100 => 1, // mixed regular battle items & mega stones
        InventoryType.Items when ItemStorage7GG.Key.Contains((ushort)itemIndex) => 1, // mixed regular items & key items
        _ => GetMaxCount(type)
    };
}
