using System;

namespace PKHeX.Core;

/// <summary>
/// Item storage for <see cref="GameVersion.R"/> and <see cref="GameVersion.S"/>
/// </summary>
public sealed class ItemStorage3RS : IItemStorage
{
    public static readonly ItemStorage3RS Instance = new();

    public static ReadOnlySpan<ushort> General =>
    [
        13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 63, 64, 65, 66, 67, 68, 69, 70, 71, 73, 74, 75, 76, 77, 78, 79, 80, 81, 83, 84, 85, 86, 93, 94, 95, 96, 97, 98, 103, 104, 106, 107, 108, 109, 110, 111, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 254, 255, 256, 257, 258,
    ];

    public static ReadOnlySpan<ushort> Key =>
    [
        259, 260, 261, 262, 263, 264, 265, 266, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288,
    ];

    private const int COUNT_TM = 50;
    private const int COUNT_HM = 8;

    public static ReadOnlySpan<ushort> Machine =>
    [
        289, 290, 291, 292, 293, 294, 295, 296, 297, 298,
        299, 300, 301, 302, 303, 304, 305, 306, 307, 308,
        309, 310, 311, 312, 313, 314, 315, 316, 317, 318,
        319, 320, 321, 322, 323, 324, 325, 326, 327, 328,
        329, 330, 331, 332, 333, 334, 335, 336, 337, 338,

        // HMs
        339, 340, 341, 342, 343, 344, 345, 346,
    ];

    public static ReadOnlySpan<ushort> MachineOnlyTM => Machine[..COUNT_TM];

    public static bool IsTMHM(ushort itemID) => itemID - 289u < COUNT_TM + COUNT_HM;
    public static bool IsTM(ushort itemID) => itemID - 289u < COUNT_TM;
    public static bool IsHM(ushort itemID) => itemID - 339u < COUNT_HM;

    public static ReadOnlySpan<ushort> Berry =>
    [
                       133, 134, 135, 136, 137, 138, 139,
        140, 141, 142, 143, 144, 145, 146, 147, 148, 149,
        150, 151, 152, 153, 154, 155, 156, 157, 158, 159,
        160, 161, 162, 163, 164, 165, 166, 167, 168, 169,
        170, 171, 172, 173, 174, 175,
    ];

    public static ReadOnlySpan<ushort> Balls =>
    [
        1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12,
    ];

    internal static ReadOnlySpan<ushort> Unreleased => [005]; // Safari Ball

    public static ushort[] GetAllHeld() => [..General, ..Balls, ..Berry, ..MachineOnlyTM];

    private static readonly ushort[] PCItems = [..General, ..Key, .. Berry, ..Balls, ..Machine];

    public bool IsLegal(InventoryType type, int itemIndex, int itemCount) => true;

    public ReadOnlySpan<ushort> GetItems(InventoryType type) => type switch
    {
        InventoryType.Items => General,
        InventoryType.KeyItems => Key,
        InventoryType.Balls => Balls,
        InventoryType.TMHMs => Machine,
        InventoryType.Berries => Berry,
        InventoryType.PCItems => PCItems,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };
}
