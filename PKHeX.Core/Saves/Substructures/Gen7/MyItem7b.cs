using System.Collections.Generic;

namespace PKHeX.Core
{
    public sealed class MyItem7b : MyItem
    {
        private const int Medicine = 0x0000; // 0
        private const int TM       = 0x00F0; // 1
        private const int Candy    = 0x02A0; // 2
        private const int PowerUp  = 0x05C0; // 3
        private const int Catching = 0x0818; // 4
        private const int Battle   = 0x08E0; // 5
        private const int Key      = 0x0B38; // 6

        public MyItem7b(SAV7b sav, int offset) : base(sav) => Offset = offset;

        public override IReadOnlyList<InventoryPouch> Inventory
        {
            get
            {
                var pouch = new InventoryPouch[]
                {
                    new InventoryPouch7b(InventoryType.Medicine, Legal.Pouch_Medicine_GG, 999, Medicine, PouchSize7b.Medicine),
                    new InventoryPouch7b(InventoryType.TMHMs, Legal.Pouch_TM_GG, 1, TM, PouchSize7b.TM),
                    new InventoryPouch7b(InventoryType.Balls, Legal.Pouch_Catching_GG, 999, Catching, PouchSize7b.Catching),
                    new InventoryPouch7b(InventoryType.Items, Legal.Pouch_Regular_GG, 999, Key, PouchSize7b.Items),
                    new InventoryPouch7b(InventoryType.BattleItems, Legal.Pouch_Battle_GG, 999, Battle, PouchSize7b.Battle),
                    new InventoryPouch7b(InventoryType.ZCrystals, Legal.Pouch_PowerUp_GG, 999, PowerUp, PouchSize7b.PowerUp),
                    new InventoryPouch7b(InventoryType.Candy, Legal.Pouch_Candy_GG, 999, Candy, PouchSize7b.Candy),
                };
                return pouch.LoadAll(Data);
            }
            set
            {
                foreach (var p in value)
                    ((InventoryPouch7b)p).SanitizeCounts();
                value.SaveAll(Data);
            }
        }
    }
}