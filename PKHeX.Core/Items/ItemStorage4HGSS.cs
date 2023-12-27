using System;

namespace PKHeX.Core;

public sealed class ItemStorage4HGSS : ItemStorage4, IItemStorage
{
    public static readonly ItemStorage4HGSS Instance = new();

    private static ReadOnlySpan<ushort> Pouch_Key_HGSS =>
    [
                            434, 435,      437,
                            444, 445, 446, 447,
        450,                          456,
                            464, 465, 466,      468, 469,
        470, 471, 472, 473, 474, 475, 476, 477, 478, 479,
        480, 481, 482, 483, 484,
             501, 502, 503, 504,
                  532, 533, 534, 535, 536,
    ];

    private static ReadOnlySpan<ushort> Pouch_Ball_HGSS =>
    [
        1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
        492, 493, 494, 495, 496, 497, 498, 499, 500, // Apricorn Balls
    ];

    public bool IsLegal(InventoryType type, int itemIndex, int itemCount) => true;

    public ReadOnlySpan<ushort> GetItems(InventoryType type) => type switch
    {
        InventoryType.Items => Pouch_Items_Pt,
        InventoryType.KeyItems => Pouch_Key_HGSS,
        InventoryType.TMHMs => Pouch_TMHM_DP,
        InventoryType.MailItems => Pouch_Mail_DP,
        InventoryType.Medicine => Pouch_Medicine_DP,
        InventoryType.Berries => Pouch_Berries_DP,
        InventoryType.Balls => Pouch_Ball_HGSS,
        InventoryType.BattleItems => Pouch_Battle_DP,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };
}
