namespace PKHeX.Core
{
    public sealed class MyItem5BW : MyItem
    {
        // offsets/pouch sizes are the same for both BW and B2W2, but Key Item permissions are different
        private const int HeldItem = 0x000; // 0
        private const int KeyItem  = 0x4D8; // 1
        private const int TMHM     = 0x624; // 2
        private const int Medicine = 0x7D8; // 3
        private const int Berry    = 0x898; // 4

        private static readonly ushort[] LegalItems = Legal.Pouch_Items_BW;
        private static readonly ushort[] LegalKeyItems = Legal.Pouch_Key_BW;
        private static readonly ushort[] LegalTMHMs = Legal.Pouch_TMHM_BW;
        private static readonly ushort[] LegalMedicine = Legal.Pouch_Medicine_BW;
        private static readonly ushort[] LegalBerries = Legal.Pouch_Berries_BW;

        public MyItem5BW(SaveFile SAV, int offset) : base(SAV) => Offset = offset;

        public override InventoryPouch[] Inventory
        {
            get
            {
                InventoryPouch[] pouch =
                {
                    new InventoryPouch4(InventoryType.Items, LegalItems, 999, Offset + HeldItem),
                    new InventoryPouch4(InventoryType.KeyItems, LegalKeyItems, 1, Offset + KeyItem),
                    new InventoryPouch4(InventoryType.TMHMs, LegalTMHMs, 1, Offset + TMHM),
                    new InventoryPouch4(InventoryType.Medicine, LegalMedicine, 999, Offset + Medicine),
                    new InventoryPouch4(InventoryType.Berries, LegalBerries, 999, Offset + Berry),
                };
                return pouch.LoadAll(Data);
            }
            set => value.SaveAll(Data);
        }
    }
}