using System;

namespace PKHeX.Core;

/// <summary>
/// Item storage for <see cref="GameVersion.B2"/> and <see cref="GameVersion.W2"/>
/// </summary>
public sealed class ItemStorage5B2W2 : ItemStorage5, IItemStorage
{
    public static readonly ItemStorage5B2W2 Instance = new();

    public static ReadOnlySpan<ushort> Key =>
    [
        437, 442, 447, 450, 453, 458, 465, 466, 471,
        504, 578,
        616, 617, 621, 626, 627, 628, 629, 630, 631, 632, 633, 634, 635, 636, 637, 638,
    ];

    public bool IsLegal(InventoryType type, int itemIndex, int itemCount) => true;

    public ReadOnlySpan<ushort> GetItems(InventoryType type) => type switch
    {
        InventoryType.Items => General,
        InventoryType.KeyItems => Key,
        InventoryType.TMHMs => Machine,
        InventoryType.Medicine => Medicine,
        InventoryType.Berries => Berry,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };
}
