using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class MyItem5B2W2 : MyItem
{
    // offsets/pouch sizes are the same for both BW and B2W2, but Key Item permissions are different
    private const int HeldItem = 0x000; // 0
    private const int KeyItem  = 0x4D8; // 1
    private const int TMHM     = 0x624; // 2
    private const int Medicine = 0x7D8; // 3
    private const int Berry    = 0x898; // 4

    private static readonly ushort[] LegalItems = Legal.Pouch_Items_BW;
    private static readonly ushort[] LegalKeyItems = Legal.Pouch_Key_B2W2;
    private static readonly ushort[] LegalTMHMs = Legal.Pouch_TMHM_BW;
    private static readonly ushort[] LegalMedicine = Legal.Pouch_Medicine_BW;
    private static readonly ushort[] LegalBerries = Legal.Pouch_Berries_BW;

    public MyItem5B2W2(SaveFile SAV, int offset) : base(SAV) => Offset = offset;

    public override IReadOnlyList<InventoryPouch> Inventory
    {
        get
        {
            InventoryPouch4[] pouch =
            {
                new(InventoryType.Items, LegalItems, 999, Offset + HeldItem),
                new(InventoryType.KeyItems, LegalKeyItems, 1, Offset + KeyItem),
                new(InventoryType.TMHMs, LegalTMHMs, 1, Offset + TMHM),
                new(InventoryType.Medicine, LegalMedicine, 999, Offset + Medicine),
                new(InventoryType.Berries, LegalBerries, 999, Offset + Berry),
            };
            return pouch.LoadAll(Data);
        }
        set => value.SaveAll(Data);
    }
}
