using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class PlayerBag4Pt : PlayerBag
{
    public override IReadOnlyList<InventoryPouch4> Pouches { get; } = GetPouches(ItemStorage4Pt.Instance);
    public override ItemStorage4Pt Info => ItemStorage4Pt.Instance;

    private static InventoryPouch4[] GetPouches(ItemStorage4Pt info) =>
    [
        new(0x630, 999, info, InventoryType.Items),
        new(0x8C4, 001, info, InventoryType.KeyItems),
        new(0x98C, 099, info, InventoryType.TMHMs),
        new(0xB1C, 999, info, InventoryType.MailItems),
        new(0xB4C, 999, info, InventoryType.Medicine),
        new(0xBEC, 999, info, InventoryType.Berries),
        new(0xCEC, 999, info, InventoryType.Balls),
        new(0xD28, 999, info, InventoryType.BattleItems),
    ];

    public PlayerBag4Pt(SAV4Pt sav) : this(sav.General) { }
    public PlayerBag4Pt(ReadOnlySpan<byte> data) => Pouches.LoadAll(data);

    public override void CopyTo(SaveFile sav) => CopyTo((SAV4Pt)sav);
    public void CopyTo(SAV4Pt sav) => CopyTo(sav.General);
    public void CopyTo(Span<byte> data) => Pouches.SaveAll(data);

    public override int GetMaxCount(InventoryType type, int itemIndex)
    {
        if (type is InventoryType.TMHMs && ItemConverter.IsItemHM4((ushort)itemIndex))
            return 1;
        return GetMaxCount(type);
    }
}
