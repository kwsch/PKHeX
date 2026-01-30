using System;
using System.Collections.Generic;
using static PKHeX.Core.InventoryType;

namespace PKHeX.Core;

public sealed class PlayerBag7SM : PlayerBag
{
    // 2x Key Item specifically for the Z-Ring: you're given one, Hala "borrows" it in the story, and then you're given it again.
    private const int ItemIndexZRing = 797;
    private const int ItemCountZRing = 2;

    public override IReadOnlyList<InventoryPouch7> Pouches { get; } = GetPouches(ItemStorage7SM.Instance);
    public override ItemStorage7SM Info => ItemStorage7SM.Instance;

    private static InventoryPouch7[] GetPouches(ItemStorage7SM info) =>
    [
        new(0x000, 430, 999, info, Items), // 0
        new(0xB48, 064, 999, info, Medicine), // 1
        new(0x998, 108, 001, info, TMHMs), // 2
        new(0xC48, 072, 999, info, Berries), // 3
        new(0x6B8, 184, 001, info, KeyItems), // 4
        new(0xD68, 030, 001, info, ZCrystals), // 5
    ];

    public PlayerBag7SM(SAV7SM sav) : this(sav.Items) { }
    public PlayerBag7SM(MyItem7SM block) : this(block.Data) { }
    public PlayerBag7SM(ReadOnlySpan<byte> data) => Pouches.LoadAll(data);

    public override void CopyTo(SaveFile sav) => CopyTo((SAV7SM)sav);
    public void CopyTo(SAV7SM sav) => CopyTo(sav.Items);
    public void CopyTo(MyItem7SM block) => CopyTo(block.Data);
    public void CopyTo(Span<byte> data) => Pouches.SaveAll(data);

    public override int GetMaxCount(InventoryType type, int itemIndex)
    {
        if (type is KeyItems && itemIndex == ItemIndexZRing)
            return ItemCountZRing;
        return GetMaxCount(type);
    }
}
