using System.Collections.Generic;

namespace PKHeX.Core
{
    public sealed class MyItem6XY : MyItem
    {
        private const int HeldItem = 0; // 0
        private const int KeyItem = 0x640; // 1
        private const int TMHM = 0x7C0; // 2
        private const int Medicine = 0x968; // 3
        private const int Berry = 0xA68; // 4

        public MyItem6XY(SaveFile SAV, int offset) : base(SAV) => Offset = offset;

        public override IReadOnlyList<InventoryPouch> Inventory
        {
            get
            {
                InventoryPouch[] pouch =
                {
                    new InventoryPouch4(InventoryType.Items, Legal.Pouch_Items_XY, 999, Offset + HeldItem),
                    new InventoryPouch4(InventoryType.KeyItems, Legal.Pouch_Key_XY, 1, Offset + KeyItem),
                    new InventoryPouch4(InventoryType.TMHMs, Legal.Pouch_TMHM_XY, 1, Offset + TMHM),
                    new InventoryPouch4(InventoryType.Medicine, Legal.Pouch_Medicine_XY, 999, Offset + Medicine),
                    new InventoryPouch4(InventoryType.Berries, Legal.Pouch_Berry_XY, 999, Offset + Berry),
                };
                return pouch.LoadAll(Data);
            }
            set => value.SaveAll(Data);
        }
    }
}