using System;

namespace PKHeX.Core;

public sealed class ItemStorage3Colo : IItemStorage
{
    public static readonly ItemStorage3Colo Instance = new();

    private static ReadOnlySpan<ushort> Pouch_Cologne_COLO => [543, 544, 545];

    private static ReadOnlySpan<ushort> Pouch_Items_COLO =>
    [
        13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 63, 64, 65, 66, 67, 68, 69, 70, 71, 73, 74, 75, 76, 77, 78, 79, 80, 81, 83, 84, 85, 86, 93, 94, 95, 96, 97, 98, 103, 104, 106, 107, 108, 109, 110, 111, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 254, 255, 256, 257, 258,

        537, // Time Flute
    ];

    private static ReadOnlySpan<ushort> Pouch_Key_COLO =>
    [
        500, 501, 502, 503, 504, 505, 506, 507, 508, 509,
        510, 511, 512, 513, 514, 515, 516, 517, 518, 519,
        520, 521, 522, 523, 524, 525, 526, 527, 528, 529,
        530, 531, 532, 533, 534, 535, 536,      538, 539,
        540, 541, 542,                546, 547,
    ];

    public bool IsLegal(InventoryType type, int itemIndex, int itemCount) => true;

    public ReadOnlySpan<ushort> GetItems(InventoryType type) => type switch
    {
        InventoryType.Items => Pouch_Items_COLO,
        InventoryType.KeyItems => Pouch_Key_COLO,
        InventoryType.Balls => ItemStorage3RS.Pouch_Ball_RS,
        InventoryType.TMHMs => ItemStorage3RS.Pouch_TM_RS,
        InventoryType.Berries => ItemStorage3RS.Pouch_Berries_RS,
        InventoryType.Medicine => Pouch_Cologne_COLO,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };
}
