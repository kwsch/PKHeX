using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class PlayerBag9a : PlayerBag
{
    private const int CherishedRing = 2612;
    private const int MegaShardItemIndex = 2618;
    private const int ColorfulScrew = 2619;

    private readonly bool IsPatchedMegaShardCountMax;
    public override IReadOnlyList<InventoryPouch9a> Pouches { get; } = GetPouches();
    public override ItemStorage9ZA Info => ItemStorage9ZA.Instance;

    private static InventoryPouch9a[] GetPouches() =>
    [
        MakePouch(InventoryType.Medicine),
        MakePouch(InventoryType.Balls),
        MakePouch(InventoryType.Berries),
        MakePouch(InventoryType.Items),
        MakePouch(InventoryType.TMHMs),
        MakePouch(InventoryType.MegaStones),
        MakePouch(InventoryType.Treasure),
        MakePouch(InventoryType.KeyItems),
    ];

    public PlayerBag9a(SAV9ZA sav) : this(sav.Items.Data, sav.Accessor.HasBlock(0x0ABC6547)) { }
    public PlayerBag9a(Span<byte> data, bool isMegaShardMaxPatched = true)
    {
        IsPatchedMegaShardCountMax = isMegaShardMaxPatched;
        Pouches.LoadAll(data);
    }

    public override void CopyTo(SaveFile sav) => CopyTo((SAV9ZA)sav);
    public void CopyTo(SAV9ZA sav) => CopyTo(sav.Items);
    public void CopyTo(MyItem9a items)
    {
        CopyTo(items.Data);
        items.CleanIllegalSlots();
    }

    public void CopyTo(Span<byte> data) => Pouches.SaveAll(data);

    public override int GetMaxCount(InventoryType type, int itemIndex)
    {
        if (itemIndex is MegaShardItemIndex && IsPatchedMegaShardCountMax)
            return 9_999;
        if (itemIndex is ColorfulScrew)
            return GetCurrentItemCount(ColorfulScrew);
        if (itemIndex is CherishedRing) // (quest item, never possessed)
            return 0;
        return GetMaxCount(type);
    }

    private int GetCurrentItemCount(int itemIndex)
    {
        foreach (var pouch in Pouches)
        {
            foreach (var item in pouch.Items)
            {
                if (item.Index == itemIndex)
                    return item.Count;
            }
        }
        return 0;
    }

    private static InventoryPouch9a MakePouch(InventoryType type)
    {
        var info = ItemStorage9ZA.Instance;
        var max = info.GetMax(type);
        return new InventoryPouch9a(type, info, max);
    }
}
