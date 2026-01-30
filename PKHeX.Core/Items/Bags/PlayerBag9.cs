using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class PlayerBag9 : PlayerBag
{
    public override IReadOnlyList<InventoryPouch9> Pouches { get; } = GetPouches();
    public override ItemStorage9SV Info => ItemStorage9SV.Instance;

    private static InventoryPouch9[] GetPouches() =>
    [
        MakePouch(InventoryType.Medicine),
        MakePouch(InventoryType.Balls),
        MakePouch(InventoryType.BattleItems),
        MakePouch(InventoryType.Berries),
        MakePouch(InventoryType.Items),
        MakePouch(InventoryType.TMHMs),
        MakePouch(InventoryType.Treasure),
        MakePouch(InventoryType.Ingredients),
        MakePouch(InventoryType.KeyItems),
        MakePouch(InventoryType.Candy),
    ];

    public PlayerBag9(SAV9SV sav) : this(sav.Items.Data) { }
    public PlayerBag9(Span<byte> data) => Pouches.LoadAll(data);

    public override void CopyTo(SaveFile sav) => CopyTo((SAV9SV)sav);
    public void CopyTo(SAV9SV sav) => CopyTo(sav.Items);
    public void CopyTo(MyItem9 items)
    {
        CopyTo(items.Data);
        items.CleanIllegalSlots();
    }

    public void CopyTo(Span<byte> data) => Pouches.SaveAll(data);

    public override int GetMaxCount(InventoryType type, int itemIndex)
    {
        if (type is InventoryType.Ingredients && ItemStorage9SV.IsAccessory(itemIndex))
            return 1;
        return GetMaxCount(type);
    }

    private static InventoryPouch9 MakePouch(InventoryType type)
    {
        var info = ItemStorage9SV.Instance;
        var max = info.GetMax(type);
        return new InventoryPouch9(type, info, max, GetPouchIndex(type));
    }

    private static uint GetPouchIndex(InventoryType type) => type switch
    {
        InventoryType.Items => InventoryItem9.PouchOther,
        InventoryType.KeyItems => InventoryItem9.PouchEvent,
        InventoryType.TMHMs => InventoryItem9.PouchTMHM,
        InventoryType.Medicine => InventoryItem9.PouchMedicine,
        InventoryType.Berries => InventoryItem9.PouchBerries,
        InventoryType.Balls => InventoryItem9.PouchBall,
        InventoryType.BattleItems => InventoryItem9.PouchBattle,
        InventoryType.Treasure => InventoryItem9.PouchTreasure,
        InventoryType.Ingredients => InventoryItem9.PouchPicnic,
        InventoryType.Candy => InventoryItem9.PouchMaterial,
        _ => InventoryItem9.PouchInvalid,
    };
}
