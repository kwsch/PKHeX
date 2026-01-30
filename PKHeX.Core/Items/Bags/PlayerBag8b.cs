using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class PlayerBag8b : PlayerBag
{
    public override IReadOnlyList<InventoryPouch8b> Pouches { get; } = GetPouches();
    public override ItemStorage8BDSP Info => ItemStorage8BDSP.Instance;

    private static InventoryPouch8b[] GetPouches() =>
    [
        MakePouch(InventoryType.Items),
        MakePouch(InventoryType.KeyItems),
        MakePouch(InventoryType.TMHMs),
        MakePouch(InventoryType.Medicine),
        MakePouch(InventoryType.Berries),
        MakePouch(InventoryType.Balls),
        MakePouch(InventoryType.BattleItems),
        MakePouch(InventoryType.Treasure),
    ];

    public PlayerBag8b(SAV8BS sav) : this(sav.Items.Data) { }
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
