using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class PlayerBag3FRLG : PlayerBag
{
    private const int OFS_PCItem = 0x0298;
    private const int OFS_PouchHeldItem = 0x0310;
    private const int OFS_PouchKeyItem = 0x03B8;
    private const int OFS_PouchBalls = 0x0430;
    private const int OFS_PouchTMHM = 0x0464;
    private const int OFS_PouchBerry = 0x054C;

    public override IReadOnlyList<InventoryPouch3> Pouches { get; } = GetPouches(ItemStorage3FRLG.Instance);
    public override ItemStorage3FRLG Info => ItemStorage3FRLG.Instance;

    private static InventoryPouch3[] GetPouches(ItemStorage3FRLG info) =>
    [
        new(InventoryType.Items, info, 999, OFS_PouchHeldItem, (OFS_PouchKeyItem - OFS_PouchHeldItem) / 4),
        new(InventoryType.KeyItems, info, 001, OFS_PouchKeyItem, (OFS_PouchBalls - OFS_PouchKeyItem) / 4),
        new(InventoryType.Balls, info, 999, OFS_PouchBalls, (OFS_PouchTMHM - OFS_PouchBalls) / 4),
        new(InventoryType.TMHMs, info, 999, OFS_PouchTMHM, (OFS_PouchBerry - OFS_PouchTMHM) / 4),
        new(InventoryType.Berries, info, 999, OFS_PouchBerry, 43),
        new(InventoryType.PCItems, info, 999, OFS_PCItem, (OFS_PouchHeldItem - OFS_PCItem) / 4),
    ];

    public PlayerBag3FRLG(SAV3FRLG sav) : this(sav.Large, sav.SecurityKey) { }
    public PlayerBag3FRLG(ReadOnlySpan<byte> data, uint security)
    {
        ApplySecurityKey(Pouches, security);
        Pouches.LoadAll(data);
    }

    public override void CopyTo(SaveFile sav) => CopyTo((SAV3FRLG)sav);
    public void CopyTo(SAV3FRLG sav) => CopyTo(sav.Large);
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
