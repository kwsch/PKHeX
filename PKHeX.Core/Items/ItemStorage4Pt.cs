using System;

namespace PKHeX.Core;

public sealed class ItemStorage4Pt : ItemStorage4, IItemStorage
{
    public static readonly ItemStorage4Pt Instance = new();

    private static ReadOnlySpan<ushort> Pouch_Key_Pt =>
    [
                                                428, 429,
        430, 431, 432, 433, 434, 435, 436, 437, 438, 439,
        440, 441, 442, 443, 444, 445, 446, 447, 448, 449,
        450, 451, 452, 453, 454, 455, 456, 457, 458, 459,
        460, 461, 462, 463, 464, 465, 466, 467,
    ];

    public static ushort[] GetAllHeld() => [..Pouch_Items_Pt, ..Pouch_Mail_DP, ..Pouch_Medicine_DP, ..Pouch_Berries_DP, ..Pouch_Ball_DP, ..Pouch_TMHM_DP[..^8]];

    public bool IsLegal(InventoryType type, int itemIndex, int itemCount) => true;

    public ReadOnlySpan<ushort> GetItems(InventoryType type) => type switch
    {
        InventoryType.Items => Pouch_Items_Pt,
        InventoryType.KeyItems => Pouch_Key_Pt,
        InventoryType.TMHMs => Pouch_TMHM_DP,
        InventoryType.MailItems => Pouch_Mail_DP,
        InventoryType.Medicine => Pouch_Medicine_DP,
        InventoryType.Berries => Pouch_Berries_DP,
        InventoryType.Balls => Pouch_Ball_DP,
        InventoryType.BattleItems => Pouch_Battle_DP,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };
}
