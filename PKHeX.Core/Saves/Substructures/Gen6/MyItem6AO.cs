using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class MyItem6AO : MyItem
{
    private const int HeldItem = 0; // 0
    private const int KeyItem = 0x640; // 1
    private const int TMHM = 0x7C0; // 2
    private const int Medicine = 0x970; // 3, +2 items shift because 2 HMs added
    private const int Berry = 0xA70; // 4

    public MyItem6AO(SAV6AO SAV, int offset) : base(SAV) => Offset = offset;
    public MyItem6AO(SAV6AODemo SAV, int offset) : base(SAV) => Offset = offset;

    public override IReadOnlyList<InventoryPouch> Inventory
    {
        get
        {
            InventoryPouch4[] pouch =
            {
                new(InventoryType.Items, Legal.Pouch_Items_AO, 999, Offset + HeldItem),
                new(InventoryType.KeyItems, Legal.Pouch_Key_AO, 1, Offset + KeyItem),
                new(InventoryType.TMHMs, Legal.Pouch_TMHM_AO, 1, Offset + TMHM),
                new(InventoryType.Medicine, Legal.Pouch_Medicine_AO, 999, Offset + Medicine),
                new(InventoryType.Berries, Legal.Pouch_Berry_XY, 999, Offset + Berry),
            };
            return pouch.LoadAll(Data);
        }
        set => value.SaveAll(Data);
    }
}
