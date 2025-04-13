using System;

namespace PKHeX.Core;

/// <summary>
/// Item storage for <see cref="GameVersion.HG"/> and <see cref="GameVersion.SS"/>
/// </summary>
public sealed class ItemStorage4HGSS : ItemStorage4, IItemStorage
{
    public static readonly ItemStorage4HGSS Instance = new();

    public static ReadOnlySpan<ushort> KeyHGSS =>
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

    public static ReadOnlySpan<ushort> BallsHGSS =>
    [
        1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
        492, 493, 494, 495, 496, 497, 498, 499, 500, // Apricorn Balls
    ];

    public bool IsLegal(InventoryType type, int itemIndex, int itemCount) => true;

    public ReadOnlySpan<ushort> GetItems(InventoryType type) => type switch
    {
        InventoryType.Items => GeneralPt,
        InventoryType.KeyItems => KeyHGSS,
        InventoryType.TMHMs => Machine,
        InventoryType.MailItems => Mail,
        InventoryType.Medicine => Medicine,
        InventoryType.Berries => Berry,
        InventoryType.Balls => BallsHGSS,
        InventoryType.BattleItems => Battle,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };
}
