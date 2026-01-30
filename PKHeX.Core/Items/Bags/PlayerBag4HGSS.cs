using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class PlayerBag4HGSS : PlayerBag
{
    public override IReadOnlyList<InventoryPouch4> Pouches { get; } = GetPouches(ItemStorage4HGSS.Instance);
    public override ItemStorage4HGSS Info => ItemStorage4HGSS.Instance;

    private static InventoryPouch4[] GetPouches(ItemStorage4HGSS info) =>
    [
        new(0x644, 999, info, InventoryType.Items),
        new(0x8D8, 001, info, InventoryType.KeyItems),
        new(0x9A0, 099, info, InventoryType.TMHMs),
        new(0xB34, 999, info, InventoryType.MailItems),
        new(0xB64, 999, info, InventoryType.Medicine),
        new(0xC04, 999, info, InventoryType.Berries),
        new(0xD04, 999, info, InventoryType.Balls),
        new(0xD64, 999, info, InventoryType.BattleItems),
    ];

    public PlayerBag4HGSS(SAV4HGSS sav) : this(sav.General) { }
    public PlayerBag4HGSS(ReadOnlySpan<byte> data) => Pouches.LoadAll(data);

    public override void CopyTo(SaveFile sav) => CopyTo((SAV4HGSS)sav);
    public void CopyTo(SAV4HGSS sav) => CopyTo(sav.General);
    public void CopyTo(Span<byte> data) => Pouches.SaveAll(data);

    public override int GetMaxCount(InventoryType type, int itemIndex)
    {
        if (type is InventoryType.TMHMs && ItemConverter.IsItemHM4((ushort)itemIndex))
            return 1;
        return GetMaxCount(type);
    }
}
