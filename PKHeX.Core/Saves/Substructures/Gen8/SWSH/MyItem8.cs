using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class MyItem8(SAV8SWSH SAV, SCBlock block) : MyItem(SAV, block.Data)
{
    public const int Medicine = 0;
    public const int Balls = Medicine + (4 * PouchSize8.Medicine);
    public const int Battle = Balls + (4 * PouchSize8.Balls);
    public const int Berries = Battle + (4 * PouchSize8.Battle);
    public const int Items = Berries + (4 * PouchSize8.Berries);
    public const int TMs = Items + (4 * PouchSize8.Items);
    public const int Treasures = TMs + (4 * PouchSize8.TMs);
    public const int Ingredients = Treasures + (4 * PouchSize8.Treasures);
    public const int Key = Ingredients + (4 * PouchSize8.Ingredients);

    public override IReadOnlyList<InventoryPouch> Inventory
    {
        get
        {
            var info = ItemStorage8SWSH.Instance;
            InventoryPouch8[] pouch =
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
            return pouch.LoadAll(Data);
        }
        set
        {
            foreach (var p in value)
                ((InventoryPouch8)p).SanitizeCounts();
            value.SaveAll(Data);
        }
    }
}
