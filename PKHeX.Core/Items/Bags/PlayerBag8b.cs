using System;
using System.Collections.Generic;
using static PKHeX.Core.InventoryType;

namespace PKHeX.Core;

public sealed class PlayerBag8b : PlayerBag
{
    public override IReadOnlyList<InventoryPouch8b> Pouches { get; } = GetPouches();
    public override ItemStorage8BDSP Info => ItemStorage8BDSP.Instance;

    private static InventoryPouch8b[] GetPouches() =>
    [
        MakePouch(Items),
        MakePouch(KeyItems),
        MakePouch(TMHMs),
        MakePouch(Medicine),
        MakePouch(Berries),
        MakePouch(Balls),
        MakePouch(BattleItems),
        MakePouch(Treasure),
    ];

    public PlayerBag8b(SAV8BS sav) : this(sav.Items) { }
    public PlayerBag8b(MyItem8b block) : this(block.Data) { }
    public PlayerBag8b(ReadOnlySpan<byte> data) => Pouches.LoadAll(data);


    public override void CopyTo(SaveFile sav) => CopyTo((SAV8BS)sav);

    public void CopyTo(SAV8BS sav) => CopyTo(sav.Items);
    public void CopyTo(MyItem8b items)
    {
        CopyTo(items.Data);
        items.CleanIllegalSlots();
    }

    public void CopyTo(Span<byte> data) => Pouches.SaveAll(data);

    private static InventoryPouch8b MakePouch(InventoryType type)
    {
        var info = ItemStorage8BDSP.Instance;
        var max = info.GetMax(type);
        return new InventoryPouch8b(type, info, max);
    }
}
