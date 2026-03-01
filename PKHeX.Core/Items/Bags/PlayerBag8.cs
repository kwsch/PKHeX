using System;
using System.Collections.Generic;
using static PKHeX.Core.InventoryType;

namespace PKHeX.Core;

public sealed class PlayerBag8 : PlayerBag
{
    public override IReadOnlyList<InventoryPouch8> Pouches { get; } = GetPouches(ItemStorage8SWSH.Instance);
    public override ItemStorage8SWSH Info => ItemStorage8SWSH.Instance;

    private static InventoryPouch8[] GetPouches(ItemStorage8SWSH info) =>
    [
        new(0x0000, 060, 999, info, Medicine),
        new(0x00F0, 030, 999, info, Balls),
        new(0x0168, 020, 999, info, BattleItems),
        new(0x01B8, 080, 999, info, Berries),
        new(0x02F8, 550, 999, info, Items),
        new(0x0B90, 210, 999, info, TMHMs),
        new(0x0ED8, 100, 999, info, Treasure),
        new(0x1068, 100, 999, info, Candy),
        new(0x11F8, 064, 001, info, KeyItems),
    ];

    public PlayerBag8(SAV8SWSH sav) : this(sav.Items) { }
    public PlayerBag8(MyItem8 block) : this(block.Data) { }
    public PlayerBag8(ReadOnlySpan<byte> data) => Pouches.LoadAll(data);

    public override void CopyTo(SaveFile sav) => CopyTo((SAV8SWSH)sav);
    public void CopyTo(SAV8SWSH sav) => CopyTo(sav.Items);
    public void CopyTo(MyItem8 block) => CopyTo(block.Data);
    public void CopyTo(Span<byte> data) => Pouches.SaveAll(data);

    public override int GetMaxCount(InventoryType type, int itemIndex)
    {
        // TMs are clamped to 1, let TRs be whatever
        if (type is TMHMs && !ItemStorage8SWSH.IsTechRecord((ushort)itemIndex))
            return 1;
        return GetMaxCount(type);
    }
}
