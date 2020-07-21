namespace PKHeX.Core
{
    public sealed class MyItem6AO : MyItem
    {
        private const int HeldItem = 0; // 0
        private const int KeyItem = 0x640; // 1
        private const int TMHM = 0x7C0; // 2
        private const int Medicine = 0x970; // 3, +2 items shift because 2 HMs added
        private const int Berry = 0xA70; // 4

        public MyItem6AO(SAV6 SAV, int offset) : base(SAV) => Offset = offset;

        public override InventoryPouch[] Inventory
        {
            get
            {
                InventoryPouch[] pouch =
                {
                    new InventoryPouch4(InventoryType.Items, Legal.Pouch_Items_AO, 999, Offset + HeldItem),
                    new InventoryPouch4(InventoryType.KeyItems, Legal.Pouch_Key_AO, 1, Offset + KeyItem),
                    new InventoryPouch4(InventoryType.TMHMs, Legal.Pouch_TMHM_AO, 1, Offset + TMHM),
                    new InventoryPouch4(InventoryType.Medicine, Legal.Pouch_Medicine_AO, 999, Offset + Medicine),
                    new InventoryPouch4(InventoryType.Berries, Legal.Pouch_Berry_XY, 999, Offset + Berry),
                };
                return pouch.LoadAll(Data);
            }
            set => value.SaveAll(Data);
        }
    }
}