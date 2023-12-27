using System;

namespace PKHeX.Core;

public sealed class ItemStorage5B2W2 : ItemStorage5, IItemStorage
{
    public static readonly ItemStorage5B2W2 Instance = new();
    private static ReadOnlySpan<ushort> Pouch_Key_B2W2 =>
    [
        437, 442, 447, 450, 453, 458, 465, 466, 471,
        504, 578,
        616, 617, 621, 626, 627, 628, 629, 630, 631, 632, 633, 634, 635, 636, 637, 638,
    ];

    public bool IsLegal(InventoryType type, int itemIndex, int itemCount) => true;

    public ReadOnlySpan<ushort> GetItems(InventoryType type) => type switch
    {
        InventoryType.Items => Pouch_Items_BW,
        InventoryType.KeyItems => Pouch_Key_B2W2,
        InventoryType.TMHMs => Pouch_TMHM_BW,
        InventoryType.Medicine => Pouch_Medicine_BW,
        InventoryType.Berries => Pouch_Berries_BW,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };
}
