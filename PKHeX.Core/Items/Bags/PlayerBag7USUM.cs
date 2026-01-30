using System;
using System.Collections.Generic;
using static PKHeX.Core.InventoryType;

namespace PKHeX.Core;

public sealed class PlayerBag7USUM : PlayerBag
{
    // 2x Key Item specifically for the Z-Ring: you're given one, Hala "borrows" it in the story, and then you're given it again.
    private const int ItemIndexZRing = 797;
    private const int ItemCountZRing = 2;

    public override IReadOnlyList<InventoryPouch7> Pouches { get; } = GetPouches(ItemStorage7USUM.Instance);
    public override ItemStorage7USUM Info => ItemStorage7USUM.Instance;

    private static InventoryPouch7[] GetPouches(ItemStorage7USUM info) =>
    [
        new(0x000, 427, 999, info, Items), // 0
        new(0xB74, 060, 999, info, Medicine), // 1
        new(0x9C4, 108, 001, info, TMHMs), // 2
        new(0xC64, 067, 999, info, Berries), // 3
        new(0x6AC, 198, 001, info, KeyItems), // 4
        new(0xD70, 035, 001, info, ZCrystals), // 5
        new(0xDFC, 011, 999, info, BattleItems), // 6
    ];

    public PlayerBag7USUM(SAV7USUM sav) : this(sav.Items) { }
    public PlayerBag7USUM(MyItem7USUM block) : this(block.Data) { }
    public PlayerBag7USUM(ReadOnlySpan<byte> data) => Pouches.LoadAll(data);

    public override void CopyTo(SaveFile sav) => CopyTo((SAV7USUM)sav);
    public void CopyTo(SAV7USUM sav) => CopyTo(sav.Items);
    public void CopyTo(MyItem7USUM block) => CopyTo(block.Data);
    public void CopyTo(Span<byte> data) => Pouches.SaveAll(data);

    public override int GetMaxCount(InventoryType type, int itemIndex)
    {
        if (type is KeyItems && itemIndex == ItemIndexZRing)
            return ItemCountZRing;
        return GetMaxCount(type);
    }
}
