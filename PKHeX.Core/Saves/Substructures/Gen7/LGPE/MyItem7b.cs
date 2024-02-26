using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class MyItem7b(SAV7b sav, Memory<byte> raw) : MyItem(sav, raw)
{
    private const int Medicine = 0x0000; // 0
    private const int TM       = 0x00F0; // 1
    private const int Candy    = 0x02A0; // 2
    private const int PowerUp  = 0x05C0; // 3
    private const int Catching = 0x0818; // 4
    private const int Battle   = 0x08E0; // 5
    private const int Key      = 0x0B38; // 6

    public override IReadOnlyList<InventoryPouch> Inventory
    {
        get
        {
            var info = ItemStorage7GG.Instance;
            InventoryPouch7b[] pouch =
            [
                new(InventoryType.Medicine, info, 999, Medicine, PouchSize7b.Medicine),
                new(InventoryType.TMHMs, info, 1, TM, PouchSize7b.TM),
                new(InventoryType.Balls, info, 999, Catching, PouchSize7b.Catching),
                new(InventoryType.Items, info, 999, Key, PouchSize7b.Items),
                new(InventoryType.BattleItems, info, 999, Battle, PouchSize7b.Battle),
                new(InventoryType.ZCrystals, info, 999, PowerUp, PouchSize7b.PowerUp),
                new(InventoryType.Candy, info, 999, Candy, PouchSize7b.Candy),
            ];
            return pouch.LoadAll(Data);
        }
        set
        {
            foreach (var p in value)
                ((InventoryPouch7b)p).SanitizeCounts();
            value.SaveAll(Data);
        }
    }
}
