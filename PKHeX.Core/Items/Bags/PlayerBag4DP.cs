using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class PlayerBag4DP : PlayerBag
{
    public override IReadOnlyList<InventoryPouch4> Pouches { get; } = GetPouches(ItemStorage4DP.Instance);
    public override ItemStorage4DP Info => ItemStorage4DP.Instance;

    private static InventoryPouch4[] GetPouches(ItemStorage4DP info) =>
    [
        new(0x624, 999, info, InventoryType.Items),
        new(0x8B8, 1, info, InventoryType.KeyItems),
        new(0x980, 99, info, InventoryType.TMHMs),
        new(0xB10, 999, info, InventoryType.MailItems),
        new(0xB40, 999, info, InventoryType.Medicine),
        new(0xBE0, 999, info, InventoryType.Berries),
        new(0xCE0, 999, info, InventoryType.Balls),
        new(0xD1C, 999, info, InventoryType.BattleItems),
    ];

    public PlayerBag4DP(SAV4DP sav) : this(sav.General) { }
    public PlayerBag4DP(ReadOnlySpan<byte> data) => Pouches.LoadAll(data);

    public override void CopyTo(SaveFile sav) => CopyTo((SAV4DP)sav);
    public void CopyTo(SAV4DP sav) => CopyTo(sav.General);
    public void CopyTo(Span<byte> data) => Pouches.SaveAll(data);

    public override int GetMaxCount(InventoryType type, int itemIndex)
    {
        if (type is InventoryType.TMHMs && ItemConverter.IsItemHM4((ushort)itemIndex))
            return 1;
        return GetMaxCount(type);
    }
}
