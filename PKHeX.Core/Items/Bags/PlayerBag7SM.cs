using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class PlayerBag7SM : PlayerBag
{
    private const int HeldItem = 0; // 430 (Case 0)
    private const int KeyItem = HeldItem + (4 * 430); // 184 (Case 4)
    private const int TMHM = KeyItem + (4 * 184); // 108 (Case 2)
    private const int Medicine = TMHM + (4 * 108); // 64 (Case 1)
    private const int Berry = Medicine + (4 * 64); // 72 (Case 3)
    private const int ZCrystals = Berry + (4 * 72); // 30 (Case 5)

    public override IReadOnlyList<InventoryPouch7> Pouches { get; } = GetPouches(ItemStorage7SM.Instance);
    public override ItemStorage7SM Info => ItemStorage7SM.Instance;

    private static InventoryPouch7[] GetPouches(ItemStorage7SM info) =>
    [
        new(InventoryType.Medicine, info, 999, Medicine),
        new(InventoryType.Items, info, 999, HeldItem),
        new(InventoryType.TMHMs, info, 1, TMHM),
        new(InventoryType.Berries, info, 999, Berry),
        new(InventoryType.KeyItems, info, 1, KeyItem),
        new(InventoryType.ZCrystals, info, 1, ZCrystals),
    ];

    public PlayerBag7SM(SAV7SM sav) : this(sav.Items.Data) { }
    public PlayerBag7SM(ReadOnlySpan<byte> data) => Pouches.LoadAll(data);

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
