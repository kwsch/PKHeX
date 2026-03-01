using System;
using System.Collections.Generic;
using static PKHeX.Core.InventoryType;

namespace PKHeX.Core;

public sealed class PlayerBag3FRLG(bool VC) : PlayerBag, IPlayerBag3
{
    private const int BaseOffset = 0x0298;
    public override IItemStorage Info => GetInfo(VC);
    private static IItemStorage GetInfo(bool vc) => vc ? ItemStorage3FRLG_VC.Instance : ItemStorage3FRLG.Instance;
    public override IReadOnlyList<InventoryPouch3> Pouches { get; } = GetPouches(GetInfo(VC));

    private static InventoryPouch3[] GetPouches(IItemStorage info) =>
    [
        new(0x078, 42, 999, info, Items),
        new(0x120, 30, 001, info, KeyItems),
        new(0x198, 13, 999, info, Balls),
        new(0x1CC, 58, 999, info, TMHMs),
        new(0x2B4, 43, 999, info, Berries),
        new(0x000, 30, 999, info, PCItems),
    ];

    public PlayerBag3FRLG(SAV3FRLG sav) : this(sav.Large[BaseOffset..], sav.SecurityKey, sav.IsVirtualConsole) { }
    public PlayerBag3FRLG(ReadOnlySpan<byte> data, uint security, bool vc) : this(vc)
    {
        UpdateSecurityKey(security);
        Pouches.LoadAll(data);
    }

    public override void CopyTo(SaveFile sav) => CopyTo((SAV3FRLG)sav);
    public void CopyTo(SAV3FRLG sav) => CopyTo(sav.Large[BaseOffset..]);
    public void CopyTo(Span<byte> data) => Pouches.SaveAll(data);

    public override int GetMaxCount(InventoryType type, int itemIndex)
    {
        if (type is TMHMs && ItemConverter.IsItemHM3((ushort)itemIndex))
            return 1;
        return GetMaxCount(type);
    }

    public void UpdateSecurityKey(uint securityKey)
    {
        foreach (var pouch in Pouches)
        {
            if (pouch.Type != PCItems)
                pouch.SecurityKey = securityKey;
        }
    }
}
