using System.Collections.Generic;

namespace PKHeX.Core
{
    public sealed class MyItem7SM : MyItem
    {
        private const int HeldItem = 0; // 430 (Case 0)
        private const int KeyItem = HeldItem + (4 * 430); // 184 (Case 4)
        private const int TMHM = KeyItem + (4 * 184); // 108 (Case 2)
        private const int Medicine = TMHM + (4 * 108); // 64 (Case 1)
        private const int Berry = Medicine + (4 * 64); // 72 (Case 3)
        private const int ZCrystals = Berry + (4 * 72); // 30 (Case 5)

        public MyItem7SM(SAV7SM SAV, int offset) : base(SAV) => Offset = offset;

        public override IReadOnlyList<InventoryPouch> Inventory
        {
            get
            {
                InventoryPouch[] pouch =
                {
                    new InventoryPouch7(InventoryType.Medicine, Legal.Pouch_Medicine_SM, 999, Offset + Medicine),
                    new InventoryPouch7(InventoryType.Items, Legal.Pouch_Items_SM, 999, Offset + HeldItem),
                    new InventoryPouch7(InventoryType.TMHMs, Legal.Pouch_TMHM_SM, 1, Offset + TMHM),
                    new InventoryPouch7(InventoryType.Berries, Legal.Pouch_Berries_SM, 999, Offset + Berry),
                    new InventoryPouch7(InventoryType.KeyItems, Legal.Pouch_Key_SM, 1, Offset + KeyItem),
                    new InventoryPouch7(InventoryType.ZCrystals, Legal.Pouch_ZCrystal_SM, 1, Offset + ZCrystals),
                };
                return pouch.LoadAll(Data);
            }
            set => value.SaveAll(Data);
        }
    }
}