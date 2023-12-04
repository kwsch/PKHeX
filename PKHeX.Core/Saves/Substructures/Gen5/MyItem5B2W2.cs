using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class MyItem5B2W2 : MyItem
{
    // offsets/pouch sizes are the same for both B/W and B2/W2, but Key Item permissions are different
    private const int HeldItem = 0x000; // 0
    private const int KeyItem  = 0x4D8; // 1
    private const int TMHM     = 0x624; // 2
    private const int Medicine = 0x7D8; // 3
    private const int Berry    = 0x898; // 4
    public MyItem5B2W2(SaveFile SAV, int offset) : base(SAV) => Offset = offset;

    public override IReadOnlyList<InventoryPouch> Inventory
    {
        get
        {
            var info = ItemStorage5B2W2.Instance;
            InventoryPouch4[] pouch =
            [
                new(InventoryType.Items, info, 999, Offset + HeldItem),
                new(InventoryType.KeyItems, info, 1, Offset + KeyItem),
                new(InventoryType.TMHMs, info, 1, Offset + TMHM),
                new(InventoryType.Medicine, info, 999, Offset + Medicine),
                new(InventoryType.Berries, info, 999, Offset + Berry),
            ];
            return pouch.LoadAll(Data);
        }
        set => value.SaveAll(Data);
    }
}
