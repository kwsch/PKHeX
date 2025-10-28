using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class MyItem6AO : MyItem
{
    private const int HeldItem = 0; // 0
    private const int KeyItem = 0x640; // 1
    private const int TMHM = 0x7C0; // 2
    private const int Medicine = 0x970; // 3, +2 items shift because 2 HMs added
    private const int Berry = 0xA70; // 4

    public MyItem6AO(SAV6AO SAV,     Memory<byte> raw) : base(SAV, raw) { }
    public MyItem6AO(SAV6AODemo SAV, Memory<byte> raw) : base(SAV, raw) { }

    public override IReadOnlyList<InventoryPouch> Inventory
    {
        get
        {
            var info = ItemStorage6AO.Instance;
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
