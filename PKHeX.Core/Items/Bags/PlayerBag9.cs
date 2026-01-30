using System;
using System.Collections.Generic;
using static PKHeX.Core.InventoryType;

namespace PKHeX.Core;

public sealed class PlayerBag9 : PlayerBag
{
    public override IReadOnlyList<InventoryPouch9> Pouches { get; } = GetPouches();
    public override ItemStorage9SV Info => ItemStorage9SV.Instance;

    private static InventoryPouch9[] GetPouches() =>
    [
        MakePouch(Medicine),
        MakePouch(Balls),
        MakePouch(BattleItems),
        MakePouch(Berries),
        MakePouch(Items),
        MakePouch(TMHMs),
        MakePouch(Treasure),
        MakePouch(Ingredients),
        MakePouch(KeyItems),
        MakePouch(Candy),
    ];

    public PlayerBag9(SAV9SV sav) : this(sav.Items) { }
    public PlayerBag9(MyItem9 block) : this(block.Data) { }
    public PlayerBag9(ReadOnlySpan<byte> data) => Pouches.LoadAll(data);

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
        if (type is Ingredients && ItemStorage9SV.IsAccessory(itemIndex))
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
        Items => InventoryItem9.PouchOther,
        KeyItems => InventoryItem9.PouchEvent,
        TMHMs => InventoryItem9.PouchTMHM,
        Medicine => InventoryItem9.PouchMedicine,
        Berries => InventoryItem9.PouchBerries,
        Balls => InventoryItem9.PouchBall,
        BattleItems => InventoryItem9.PouchBattle,
        Treasure => InventoryItem9.PouchTreasure,
        Ingredients => InventoryItem9.PouchPicnic,
        Candy => InventoryItem9.PouchMaterial,
        _ => InventoryItem9.PouchInvalid,
    };
}
