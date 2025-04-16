using System;

namespace PKHeX.Core;

/// <summary>
/// Item storage for <see cref="GameVersion.Pt"/>
/// </summary>
public sealed class ItemStorage4Pt : ItemStorage4, IItemStorage
{
    public static readonly ItemStorage4Pt Instance = new();

    public static ReadOnlySpan<ushort> KeyPt =>
    [
                                                428, 429,
        430, 431, 432, 433, 434, 435, 436, 437, 438, 439,
        440, 441, 442, 443, 444, 445, 446, 447, 448, 449,
        450, 451, 452, 453, 454, 455, 456, 457, 458, 459,
        460, 461, 462, 463, 464, 465, 466, 467,
    ];

    public static ushort[] GetAllHeld() => [..GeneralPt, ..Mail, ..Medicine, ..Berry, ..BallsDPPt, ..Battle, ..Machine[..^8]];

    public bool IsLegal(InventoryType type, int itemIndex, int itemCount) => true;

    public ReadOnlySpan<ushort> GetItems(InventoryType type) => type switch
    {
        InventoryType.Items => GeneralPt,
        InventoryType.KeyItems => KeyPt,
        InventoryType.TMHMs => Machine,
        InventoryType.MailItems => Mail,
        InventoryType.Medicine => Medicine,
        InventoryType.Berries => Berry,
        InventoryType.Balls => BallsDPPt,
        InventoryType.BattleItems => Battle,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };
}
