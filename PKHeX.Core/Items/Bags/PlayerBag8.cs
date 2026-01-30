using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class PlayerBag8 : PlayerBag
{
    private const int Medicine = 0;
    private const int Balls = Medicine + (4 * PouchSize8.Medicine);
    private const int Battle = Balls + (4 * PouchSize8.Balls);
    private const int Berries = Battle + (4 * PouchSize8.Battle);
    private const int Items = Berries + (4 * PouchSize8.Berries);
    private const int TMs = Items + (4 * PouchSize8.Items);
    private const int Treasures = TMs + (4 * PouchSize8.TMs);
    private const int Ingredients = Treasures + (4 * PouchSize8.Treasures);
    private const int Key = Ingredients + (4 * PouchSize8.Ingredients);

    public override IReadOnlyList<InventoryPouch8> Pouches { get; } = GetPouches(ItemStorage8SWSH.Instance);
    public override ItemStorage8SWSH Info => ItemStorage8SWSH.Instance;

    private static InventoryPouch8[] GetPouches(ItemStorage8SWSH info) =>
    [
        new(InventoryType.Medicine, info, 999, Medicine, PouchSize8.Medicine),
        new(InventoryType.Balls, info, 999, Balls, PouchSize8.Balls),
        new(InventoryType.BattleItems, info, 999, Battle, PouchSize8.Battle),
        new(InventoryType.Berries, info, 999, Berries, PouchSize8.Berries),
        new(InventoryType.Items, info, 999, Items, PouchSize8.Items),
        new(InventoryType.TMHMs, info, 999, TMs, PouchSize8.TMs),
        new(InventoryType.Treasure, info, 999, Treasures, PouchSize8.Treasures),
        new(InventoryType.Candy, info, 999, Ingredients, PouchSize8.Ingredients),
        new(InventoryType.KeyItems, info, 1, Key, PouchSize8.Key),
    ];

    public PlayerBag8(SAV8SWSH sav) : this(sav.Items.Data) { }
    public PlayerBag8(Span<byte> data) => Pouches.LoadAll(data);

    public override void CopyTo(SaveFile sav) => CopyTo((SAV8SWSH)sav);
    public void CopyTo(SAV8SWSH sav) => CopyTo(sav.Items.Data);
    public void CopyTo(Span<byte> data) => Pouches.SaveAll(data);

    public override int GetMaxCount(InventoryType type, int itemIndex) => type switch
    {
        // TMs are clamped to 1, let TRs be whatever
        InventoryType.TMHMs when !ItemStorage8SWSH.IsTechRecord((ushort)itemIndex) => 1,
        _ => GetMaxCount(type)
    };
}
