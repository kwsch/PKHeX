using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core;

public sealed class MyItem8 : MyItem
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

    public MyItem8(SaveFile SAV, SCBlock block) : base(SAV, block.Data) { }

    public override IReadOnlyList<InventoryPouch> Inventory
    {
        get
        {
            InventoryPouch8[] pouch =
            {
                new(InventoryType.Medicine, Legal.Pouch_Medicine_SWSH, 999, Medicine, PouchSize8.Medicine, IsItemLegal),
                new(InventoryType.Balls, Legal.Pouch_Ball_SWSH, 999, Balls, PouchSize8.Balls, IsItemLegal),
                new(InventoryType.BattleItems, Legal.Pouch_Battle_SWSH, 999, Battle, PouchSize8.Battle, IsItemLegal),
                new(InventoryType.Berries, Legal.Pouch_Berries_SWSH, 999, Berries, PouchSize8.Berries, IsItemLegal),
                new(InventoryType.Items, Legal.Pouch_Regular_SWSH, 999, Items, PouchSize8.Items, IsItemLegal),
                new(InventoryType.TMHMs, Legal.Pouch_TMHM_SWSH, 999, TMs, PouchSize8.TMs, IsItemLegal),
                new(InventoryType.MailItems, Legal.Pouch_Treasure_SWSH, 999, Treasures, PouchSize8.Treasures, IsItemLegal),
                new(InventoryType.Candy, Legal.Pouch_Ingredients_SWSH, 999, Ingredients, PouchSize8.Ingredients, IsItemLegal),
                new(InventoryType.KeyItems, Legal.Pouch_Key_SWSH, 1, Key, PouchSize8.Key),
            };
            return pouch.LoadAll(Data);
        }
        set
        {
            foreach (var p in value)
                ((InventoryPouch8)p).SanitizeCounts();
            value.SaveAll(Data);
        }
    }

    public static bool IsItemLegal(ushort item)
    {
        if (Legal.IsDynamaxCrystal(item))
            return Legal.IsDynamaxCrystalAvailable(item);
        if (!Legal.HeldItems_SWSH.Contains(item))
            return true;
        return Legal.ReleasedHeldItems_8[item];
    }
}
