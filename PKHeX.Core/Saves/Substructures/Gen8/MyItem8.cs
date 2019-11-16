namespace PKHeX.Core
{
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

        public override InventoryPouch[] Inventory
        {
            get
            {
                var pouch = new InventoryPouch[]
                {
                    new InventoryPouch8(InventoryType.Medicine, Legal.Pouch_Medicine_SWSH, 999, Medicine, PouchSize8.Medicine),
                    new InventoryPouch8(InventoryType.Balls, Legal.Pouch_Ball_SWSH, 999, Balls, PouchSize8.Balls),
                    new InventoryPouch8(InventoryType.BattleItems, Legal.Pouch_Battle_SWSH, 999, Battle, PouchSize8.Battle),
                    new InventoryPouch8(InventoryType.Berries, Legal.Pouch_Berries_SWSH, 999, Berries, PouchSize8.Berries),
                    new InventoryPouch8(InventoryType.Items, Legal.Pouch_Regular_SWSH, 999, Items, PouchSize8.Items),
                    new InventoryPouch8(InventoryType.TMHMs, Legal.Pouch_TMHM_SWSH, 999, TMs, PouchSize8.TMs),
                    new InventoryPouch8(InventoryType.MailItems, Legal.Pouch_Treasure_SWSH, 999, Treasures, PouchSize8.Treasures),
                    new InventoryPouch8(InventoryType.Candy, Legal.Pouch_Ingredients_SWSH, 999, Ingredients, PouchSize8.Ingredients),
                    new InventoryPouch8(InventoryType.KeyItems, Legal.Pouch_Key_SWSH, 1, Key, PouchSize8.Key),
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
    }
}