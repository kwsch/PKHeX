using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class MyItem7USUM(SAV7USUM SAV, Memory<byte> raw) : MyItem(SAV, raw)
{
    private const int HeldItem = 0; // 427 (Case 0)
    private const int KeyItem = HeldItem + (4 * 427); // 198 (Case 4)
    private const int TMHM = KeyItem + (4 * 198); // 108 (Case 2)
    private const int Medicine = TMHM + (4 * 108); // 60 (Case 1)
    private const int Berry = Medicine + (4 * 60); // 67 (Case 3)
    private const int ZCrystals = Berry + (4 * 67); // 35 (Case 5)
    private const int BattleItems = ZCrystals + (4 * 35); // 11 (Case 6)

    public override IReadOnlyList<InventoryPouch> Inventory
    {
        get
        {
            var info = ItemStorage7USUM.Instance;
            InventoryPouch7[] pouch =
            [
                new(InventoryType.Medicine, info, 999, Medicine),
                new(InventoryType.Items, info, 999, HeldItem),
                new(InventoryType.TMHMs, info, 1, TMHM),
                new(InventoryType.Berries, info, 999, Berry),
                new(InventoryType.KeyItems, info, 1, KeyItem),
                new(InventoryType.ZCrystals, info, 1, ZCrystals),
                new(InventoryType.BattleItems, info, 999, BattleItems),
            ];
            return pouch.LoadAll(Data);
        }
        set => value.SaveAll(Data);
    }
}
