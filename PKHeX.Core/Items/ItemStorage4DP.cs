using System;

namespace PKHeX.Core;

/// <summary>
/// Item storage for <see cref="GameVersion.D"/> and <see cref="GameVersion.P"/>
/// </summary>
public sealed class ItemStorage4DP : ItemStorage4, IItemStorage
{
    public static readonly ItemStorage4DP Instance = new();

    public static ushort[] GetAllHeld() => [..GeneralDP, ..Mail, ..Medicine, ..Berry, ..BallsDPPt, ..Battle, ..Machine[..^8]];

    public bool IsLegal(InventoryType type, int itemIndex, int itemCount) => true;

    public ReadOnlySpan<ushort> GetItems(InventoryType type) => type switch
    {
        InventoryType.Items => GeneralDP,
        InventoryType.KeyItems => Key,
        InventoryType.TMHMs => Machine,
        InventoryType.MailItems => Mail,
        InventoryType.Medicine => Medicine,
        InventoryType.Berries => Berry,
        InventoryType.Balls => BallsDPPt,
        InventoryType.BattleItems => Battle,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };
}
