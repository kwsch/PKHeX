using System;
using static PKHeX.Core.ItemStorage3RS;

namespace PKHeX.Core;

public sealed class ItemStorage3FRLG : IItemStorage
{
    public static readonly ItemStorage3FRLG Instance = new();

    private static ReadOnlySpan<ushort> Pouch_Key_FRLG =>
    [
        // R/S
        259, 260, 261, 262, 263, 264, 265, 266, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288,
        // FR/LG
        349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363, 364, 365, 366, 367, 368, 369, 370, 371, 372, 373, 374,
    ];

    private static readonly ushort[] PCItems = [..Pouch_Items_RS, ..Pouch_Key_FRLG, ..Pouch_Ball_RS, ..Pouch_TMHM_RS, ..Pouch_Berries_RS];

    public bool IsLegal(InventoryType type, int itemIndex, int itemCount) => true;

    public ReadOnlySpan<ushort> GetItems(InventoryType type) => type switch
    {
        InventoryType.Items => Pouch_Items_RS,
        InventoryType.KeyItems => Pouch_Key_FRLG,
        InventoryType.Balls => Pouch_Ball_RS,
        InventoryType.TMHMs => Pouch_TMHM_RS,
        InventoryType.Berries => Pouch_Berries_RS,
        InventoryType.PCItems => PCItems,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };
}
