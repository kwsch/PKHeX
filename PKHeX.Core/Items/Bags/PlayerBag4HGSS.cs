using System;
using System.Collections.Generic;
using static PKHeX.Core.InventoryType;

namespace PKHeX.Core;

public sealed class PlayerBag4HGSS : PlayerBag
{
    private const int BaseOffset = 0x644;

    public override IReadOnlyList<InventoryPouch4> Pouches { get; } = GetPouches(ItemStorage4HGSS.Instance);
    public override ItemStorage4HGSS Info => ItemStorage4HGSS.Instance;

    private static InventoryPouch4[] GetPouches(ItemStorage4HGSS info) =>
    [
        new(0x000, 999, info, Items),
        new(0x294, 001, info, KeyItems),
        new(0x35C, 099, info, TMHMs),
        new(0x4F0, 999, info, MailItems),
        new(0x520, 999, info, Medicine),
        new(0x5C0, 999, info, Berries),
        new(0x6C0, 999, info, Balls),
        new(0x720, 999, info, BattleItems),
    ];

    public PlayerBag4HGSS(SAV4HGSS sav) : this(sav.General[BaseOffset..]) { }
    public PlayerBag4HGSS(ReadOnlySpan<byte> data) => Pouches.LoadAll(data);

    public override void CopyTo(SaveFile sav) => CopyTo((SAV4HGSS)sav);
    public void CopyTo(SAV4HGSS sav) => CopyTo(sav.General[BaseOffset..]);
    public void CopyTo(Span<byte> data) => Pouches.SaveAll(data);

    public override int GetMaxCount(InventoryType type, int itemIndex)
    {
        if (type is TMHMs && ItemConverter.IsItemHM4((ushort)itemIndex))
            return 1;
        return GetMaxCount(type);
    }
}
