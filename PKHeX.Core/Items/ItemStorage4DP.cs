using System;

namespace PKHeX.Core;

public sealed class ItemStorage4DP : ItemStorage4, IItemStorage
{
    public static readonly ItemStorage4DP Instance = new();

    public static ushort[] GetAllHeld() => [..Pouch_Items_DP, ..Pouch_Mail_DP, ..Pouch_Medicine_DP, ..Pouch_Berries_DP, ..Pouch_Ball_DP, ..Pouch_TMHM_DP[..^8]];

    public bool IsLegal(InventoryType type, int itemIndex, int itemCount) => true;

    public ReadOnlySpan<ushort> GetItems(InventoryType type) => type switch
    {
        InventoryType.Items => Pouch_Items_DP,
        InventoryType.KeyItems => Pouch_Key_DP,
        InventoryType.TMHMs => Pouch_TMHM_DP,
        InventoryType.MailItems => Pouch_Mail_DP,
        InventoryType.Medicine => Pouch_Medicine_DP,
        InventoryType.Berries => Pouch_Berries_DP,
        InventoryType.Balls => Pouch_Ball_DP,
        InventoryType.BattleItems => Pouch_Battle_DP,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };
}
