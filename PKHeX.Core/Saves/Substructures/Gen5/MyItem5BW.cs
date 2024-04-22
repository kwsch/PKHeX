using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class MyItem5BW(SAV5BW SAV, Memory<byte> raw) : MyItem(SAV, raw)
{
    // offsets/pouch sizes are the same for both B/W and B2/W2, but Key Item permissions are different
    private const int HeldItem = 0x000; // 0
    private const int KeyItem  = 0x4D8; // 1
    private const int TMHM     = 0x624; // 2
    private const int Medicine = 0x7D8; // 3
    private const int Berry    = 0x898; // 4

    public override IReadOnlyList<InventoryPouch> Inventory
    {
        get
        {
            var info = ItemStorage5BW.Instance;
            InventoryPouch4[] pouch =
            [
                new(InventoryType.Items, info, 999, HeldItem),
                new(InventoryType.KeyItems, info, 1, KeyItem),
                new(InventoryType.TMHMs, info, 1, TMHM),
                new(InventoryType.Medicine, info, 999, Medicine),
                new(InventoryType.Berries, info, 999, Berry),
            ];
            return pouch.LoadAll(Data);
        }
        set => value.SaveAll(Data);
    }
}
