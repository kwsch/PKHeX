using System;

namespace PKHeX.Core;

public sealed class ItemStorage5BW : ItemStorage5, IItemStorage
{
    public static readonly ItemStorage5BW Instance = new();

    private static ReadOnlySpan<ushort> Pouch_Key_BW =>
    [
        437, 442, 447, 450, 465, 466, 471,
        504, 533, 574, 578, 579,
        616, 617, 621, 623, 624, 625, 626,
    ];

    public bool IsLegal(InventoryType type, int itemIndex, int itemCount) => true;

    public ReadOnlySpan<ushort> GetItems(InventoryType type) => type switch
    {
        InventoryType.Items => Pouch_Items_BW,
        InventoryType.KeyItems => Pouch_Key_BW,
        InventoryType.TMHMs => Pouch_TMHM_BW,
        InventoryType.Medicine => Pouch_Medicine_BW,
        InventoryType.Berries => Pouch_Berries_BW,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };
}
