using System;
using System.Collections.Generic;
using static PKHeX.Core.InventoryType;

namespace PKHeX.Core;

public sealed class PlayerBag9a : PlayerBag
{
    private const int CherishedRing = 2612;
    private const int MegaShardItemIndex = 2618;
    private const int ColorfulScrew = 2619;

    private const int MegaShardCountMaxPatched = 9_999; // In 2.0.1 update, the max count for Mega Shard was increased to 9,999.

    private readonly bool IsPatchedMegaShardCountMax;
    public override IReadOnlyList<InventoryPouch9a> Pouches { get; } = GetPouches();
    public override ItemStorage9ZA Info => ItemStorage9ZA.Instance;

    private static InventoryPouch9a[] GetPouches() =>
    [
        MakePouch(Medicine),
        MakePouch(Balls),
        MakePouch(Berries),
        MakePouch(Items),
        MakePouch(TMHMs),
        MakePouch(MegaStones),
        MakePouch(Treasure),
        MakePouch(KeyItems),
    ];

    public PlayerBag9a(SAV9ZA sav) : this(sav.Items, sav.Accessor.HasBlock(0x0ABC6547)) { }
    public PlayerBag9a(MyItem9a block, bool isMegaShardMaxPatched = true) : this(block.Data, isMegaShardMaxPatched) { }
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

    public override int GetMaxCount(InventoryType type, int itemIndex) => itemIndex switch
    {
        MegaShardItemIndex when IsPatchedMegaShardCountMax => MegaShardCountMaxPatched,
        ColorfulScrew => GetCurrentItemCount(ColorfulScrew), // Don't modify.
        CherishedRing => 0, // Quest item, never possessed.
        _ => GetMaxCount(type),
    };

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
