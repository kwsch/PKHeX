using System;
using System.Collections.Generic;
using static PKHeX.Core.InventoryType;

namespace PKHeX.Core;

public sealed class PlayerBag4DP : PlayerBag
{
    private const int BaseOffset = 0x624;

    public override IReadOnlyList<InventoryPouch4> Pouches { get; } = GetPouches(ItemStorage4DP.Instance);
    public override ItemStorage4DP Info => ItemStorage4DP.Instance;

    private static InventoryPouch4[] GetPouches(ItemStorage4DP info) =>
    [
        new(0x000, 999, info, Items),
        new(0x294, 001, info, KeyItems),
        new(0x35C, 099, info, TMHMs),
        new(0x4EC, 999, info, MailItems),
        new(0x51C, 999, info, Medicine),
        new(0x5BC, 999, info, Berries),
        new(0x6BC, 999, info, Balls),
        new(0x6F8, 999, info, BattleItems),
    ];

    public PlayerBag4DP(SAV4DP sav) : this(sav.General[BaseOffset..]) { }
    public PlayerBag4DP(ReadOnlySpan<byte> data) => Pouches.LoadAll(data);

    public override void CopyTo(SaveFile sav) => CopyTo((SAV4DP)sav);
    public void CopyTo(SAV4DP sav) => CopyTo(sav.General[BaseOffset..]);
    public void CopyTo(Span<byte> data) => Pouches.SaveAll(data);

    public override int GetMaxCount(InventoryType type, int itemIndex)
    {
        if (type is TMHMs && ItemConverter.IsItemHM4((ushort)itemIndex))
            return 1;
        return GetMaxCount(type);
    }
}
