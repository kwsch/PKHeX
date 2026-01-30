using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class PlayerBag3RS : PlayerBag
{
    private const int OFS_PCItem = 0x0498;
    private const int OFS_PouchHeldItem = 0x0560;
    private const int OFS_PouchKeyItem = 0x05B0;
    private const int OFS_PouchBalls = 0x0600;
    private const int OFS_PouchTMHM = 0x0640;
    private const int OFS_PouchBerry = 0x0740;

    public override IReadOnlyList<InventoryPouch3> Pouches { get; } = GetPouches(ItemStorage3RS.Instance);
    public override ItemStorage3RS Info => ItemStorage3RS.Instance;

    private static InventoryPouch3[] GetPouches(ItemStorage3RS info) =>
    [
        new(InventoryType.Items, info, 099, OFS_PouchHeldItem, (OFS_PouchKeyItem - OFS_PouchHeldItem) / 4),
        new(InventoryType.KeyItems, info, 1, OFS_PouchKeyItem, (OFS_PouchBalls - OFS_PouchKeyItem) / 4),
        new(InventoryType.Balls, info, 099, OFS_PouchBalls, (OFS_PouchTMHM - OFS_PouchBalls) / 4),
        new(InventoryType.TMHMs, info, 099, OFS_PouchTMHM, (OFS_PouchBerry - OFS_PouchTMHM) / 4),
        new(InventoryType.Berries, info, 999, OFS_PouchBerry, 46),
        new(InventoryType.PCItems, info, 999, OFS_PCItem, (OFS_PouchHeldItem - OFS_PCItem) / 4),
    ];

    public PlayerBag3RS(SAV3RS sav) : this(sav.Large, sav.SecurityKey) { }
    public PlayerBag3RS(ReadOnlySpan<byte> data, uint security)
    {
        ApplySecurityKey(Pouches, security);
        Pouches.LoadAll(data);
    }

    public override void CopyTo(SaveFile sav) => CopyTo((SAV3RS)sav);
    public void CopyTo(SAV3RS sav) => CopyTo(sav.Large);
    public void CopyTo(Span<byte> data) => Pouches.SaveAll(data);

    public override int GetMaxCount(InventoryType type, int itemIndex)
    {
        if (type is InventoryType.TMHMs && ItemConverter.IsItemHM3((ushort)itemIndex))
            return 1;
        return GetMaxCount(type);
    }

    private static void ApplySecurityKey(IEnumerable<InventoryPouch3> pouches, uint securityKey)
    {
        foreach (var pouch in pouches)
        {
            if (pouch.Type != InventoryType.PCItems)
                pouch.SecurityKey = securityKey;
        }
    }
}
