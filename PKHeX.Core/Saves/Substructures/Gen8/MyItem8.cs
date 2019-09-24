namespace PKHeX.Core
{
    public sealed class MyItem8 : MyItem
    {
        private const int Medicine = 0x0000; // 0
        private const int TM = 0x00F0; // 1
        private const int Candy = 0x02A0; // 2
        private const int PowerUp = 0x05C0; // 3
        private const int Catching = 0x0818; // 4
        private const int Battle = 0x08E0; // 5
        private const int Key = 0x0B38; // 6

        public MyItem8(SaveFile SAV) : base(SAV) { }

        public override InventoryPouch[] Inventory
        {
            get
            {
                var pouch = new InventoryPouch[]
                {
                    new InventoryPouch8(InventoryType.Medicine, Legal.Pouch_Medicine_SWSH, 999, Medicine, PouchSize8.Medicine),
                    new InventoryPouch8(InventoryType.TMHMs, Legal.Pouch_TMHM_SWSH, 1, TM, PouchSize8.TM),
                    new InventoryPouch8(InventoryType.Balls, Legal.Pouch_Berries_SWSH, 999, Catching, PouchSize8.Catching),
                    new InventoryPouch8(InventoryType.Items, Legal.Pouch_Regular_SWSH, 999, Key, PouchSize8.Items),
                    new InventoryPouch8(InventoryType.BattleItems, Legal.Pouch_Battle_SWSH, 999, Battle, PouchSize8.Battle),
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