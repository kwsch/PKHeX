using System;

namespace PKHeX.Core;

public sealed class ItemStorage3XD : IItemStorage
{
    public static readonly ItemStorage3XD Instance = new();

    private static ReadOnlySpan<ushort> Pouch_Cologne_XD => [513, 514, 515];
    private static ReadOnlySpan<ushort> Pouch_Items_XD =>
    [
                    13, 14, 15, 16, 17, 18, 19,
        20, 21, 22, 23, 24, 25, 26, 27, 28, 29,
        30, 31, 32, 33, 34, 35, 36, 37, 38, 39,
        40, 41, 42, 43, 44, 45, 46, 47, 48, 49,
        50, 51,
                    63, 64, 65, 66, 67, 68, 69,
        70, 71,     73, 74, 75, 76, 77, 78, 79,
        80, 81, 83, 84, 85, 86,
                  93, 94, 95, 96, 97, 98,

                       103, 104, 106, 107, 108, 109,
        110, 111,
             121, 122, 123, 124, 125, 126, 127, 128, 129,
        130, 131, 132,
                                                     179,
        180, 181, 182, 183, 184, 185, 186, 187, 188, 189,
        190, 191, 192, 193, 194, 195, 196, 197, 198, 199,
        200, 201, 202, 203, 204, 205, 206, 207, 208, 209,
        210, 211, 212, 213, 214, 215, 216, 217, 218, 219,
        220, 221, 222, 223, 224, 225,
                            254, 255, 256, 257, 258,

        // XD Additions
        511, // Poké Snack
    ];

    private static ReadOnlySpan<ushort> Pouch_Key_XD =>
    [
        500, 501, 502, 503, 504, 505, 506, 507, 508, 509,
        510,      512,                516, 517, 518, 519,
        523, 524, 525, 526, 527, 528, 529,
        530, 531, 532, 533,
    ];

    private static ReadOnlySpan<ushort> Pouch_Disc_XD =>
    [
                            534, 535, 536, 537, 538, 539,
        540, 541, 542, 543, 544, 545, 546, 547, 548, 549,
        550, 551, 552, 553, 554, 555, 556, 557, 558, 559,
        560, 561, 562, 563, 564, 565, 566, 567, 568, 569,
        570, 571, 572, 573, 574, 575, 576, 577, 578, 579,
        580, 581, 582, 583, 584, 585, 586, 587, 588, 589,
        590, 591, 592, 593,
    ];

    public bool IsLegal(InventoryType type, int itemIndex, int itemCount) => true;

    public ReadOnlySpan<ushort> GetItems(InventoryType type) => type switch
    {
        InventoryType.Items => Pouch_Items_XD,
        InventoryType.KeyItems => Pouch_Key_XD,
        InventoryType.Balls => ItemStorage3RS.Pouch_Ball_RS,
        InventoryType.TMHMs => ItemStorage3RS.Pouch_TM_RS,
        InventoryType.Berries => ItemStorage3RS.Pouch_Berries_RS,
        InventoryType.Medicine => Pouch_Cologne_XD,
        InventoryType.BattleItems => Pouch_Disc_XD,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };
}
