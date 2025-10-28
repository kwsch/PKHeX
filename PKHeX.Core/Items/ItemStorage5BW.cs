using System;

namespace PKHeX.Core;

/// <summary>
/// Item storage for <see cref="GameVersion.B"/> and <see cref="GameVersion.W"/>
/// </summary>
public sealed class ItemStorage5BW : ItemStorage5, IItemStorage
{
    public static readonly ItemStorage5BW Instance = new();

    public static ReadOnlySpan<ushort> Key =>
    [
        437, 442, 447, 450, 465, 466, 471,
        504, 533, 574, 578, 579,
        616, 617, 621, 623, 624, 625, 626,
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
