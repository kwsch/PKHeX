using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class MyItem6XY(SAV6XY SAV, Memory<byte> raw) : MyItem(SAV, raw)
{
    private const int HeldItem = 0;     // 0
    private const int KeyItem = 0x640;  // 1
    private const int TMHM = 0x7C0;     // 2
    private const int Medicine = 0x968; // 3
    private const int Berry = 0xA68;    // 4

    public override IReadOnlyList<InventoryPouch> Inventory
    {
        get
        {
            var info = ItemStorage6XY.Instance;
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
