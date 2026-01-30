using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class PlayerBag7USUM : PlayerBag
{
    private const int HeldItem = 0; // 427 (Case 0)
    private const int KeyItem = HeldItem + (4 * 427); // 198 (Case 4)
    private const int TMHM = KeyItem + (4 * 198); // 108 (Case 2)
    private const int Medicine = TMHM + (4 * 108); // 60 (Case 1)
    private const int Berry = Medicine + (4 * 60); // 67 (Case 3)
    private const int ZCrystals = Berry + (4 * 67); // 35 (Case 5)
    private const int BattleItems = ZCrystals + (4 * 35); // 11 (Case 6)

    public override IReadOnlyList<InventoryPouch7> Pouches { get; } = GetPouches(ItemStorage7USUM.Instance);
    public override ItemStorage7USUM Info => ItemStorage7USUM.Instance;

    private static InventoryPouch7[] GetPouches(ItemStorage7USUM info) =>
    [
        new(InventoryType.Medicine, info, 999, Medicine),
        new(InventoryType.Items, info, 999, HeldItem),
        new(InventoryType.TMHMs, info, 1, TMHM),
        new(InventoryType.Berries, info, 999, Berry),
        new(InventoryType.KeyItems, info, 1, KeyItem),
        new(InventoryType.ZCrystals, info, 1, ZCrystals),
        new(InventoryType.BattleItems, info, 999, BattleItems),
    ];

    public PlayerBag7USUM(SAV7USUM sav) : this(sav.Items.Data) { }
    public PlayerBag7USUM(ReadOnlySpan<byte> data) => Pouches.LoadAll(data);

    public override void CopyTo(SaveFile sav) => CopyTo((SAV7USUM)sav);
    public void CopyTo(SAV7USUM sav) => CopyTo(sav.Items.Data);
    public void CopyTo(Span<byte> data) => Pouches.SaveAll(data);

    public override int GetMaxCount(InventoryType type, int itemIndex)
    {
        if (type is InventoryType.KeyItems && itemIndex == 797)
            return 2;
        return GetMaxCount(type);
    }
}
