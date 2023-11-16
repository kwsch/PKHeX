using System;

namespace PKHeX.Core;

public sealed class ItemStorage6XY : IItemStorage
{
    public static readonly ItemStorage6XY Instance = new();

    private static ReadOnlySpan<ushort> Pouch_Items_XY =>
    [
        001, 002, 003, 004, 005, 006, 007, 008, 009, 010, 011, 012, 013, 014, 015, 016, 055, 056,
        057, 058, 059, 060, 061, 062, 063, 064, 065, 066, 067, 068, 069, 070, 071, 072, 073, 074, 075,
        076, 077, 078, 079, 080, 081, 082, 083, 084, 085, 086, 087, 088, 089, 090, 091, 092, 093, 094,
        099, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 112, 116, 117, 118, 119, 135, 136,
        213, 214, 215, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232,
        233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251,
        252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270,
        271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289,
        290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 308,
        309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327,
        492, 493, 494, 495, 496, 497, 498, 499, 500, 537, 538, 539, 540, 541, 542, 543, 544, 545, 546,
        547, 548, 549, 550, 551, 552, 553, 554, 555, 556, 557, 558, 559, 560, 561, 562, 563, 564, 571,
        572, 573, 576, 577, 580, 581, 582, 583, 584, 585, 586, 587, 588, 589, 590, 639, 640, 644, 646,
        647, 648, 649, 650, 652, 653, 654, 655, 656, 657, 658, 659, 660, 661, 662, 663, 664, 665, 666,
        667, 668, 669, 670, 671, 672, 673, 674, 675, 676, 677, 678, 679, 680, 681, 682, 683, 684, 685,
        699, 704, 710, 711, 715,
    ];

    private static ReadOnlySpan<ushort> Pouch_Key_XY =>
    [
        216, 431, 442, 445, 446, 447, 450, 465, 466, 471, 628,
        629, 631, 632, 638, 641, 642, 643, 651, 689, 695, 696, 697, 698,
        700, 701, 702, 703, 705, 712, 713, 714,

        // Illegal
        // 716, 717, 706, 707,
    ];

    private static ReadOnlySpan<ushort> Pouch_TMHM_XY =>
    [
        328, 329, 330, 331, 332, 333, 334, 335, 336, 337,
        338, 339, 340, 341, 342, 343, 344, 345, 346, 347,
        348, 349, 350, 351, 352, 353, 354, 355, 356, 357,
        358, 359, 360, 361, 362, 363, 364, 365, 366, 367,
        368, 369, 370, 371, 372, 373, 374, 375, 376, 377,
        378, 379, 380, 381, 382, 383, 384, 385, 386, 387,
        388, 389, 390, 391, 392, 393, 394, 395, 396, 397,
        398, 399, 400, 401, 402, 403, 404, 405, 406, 407,
        408, 409, 410, 411, 412, 413, 414, 415, 416, 417,
        418, 419, // 01-92

        618, 619, 620, // 93-95
        690, 691, 692, 693, 694, // 96-100

        420, 421, 422, 423, 424,
    ];

    private static ReadOnlySpan<ushort> Pouch_Medicine_XY =>
    [
        017, 018, 019,
        020, 021, 022, 023, 024, 025, 026, 027, 028, 029,
        030, 031, 032, 033, 034, 035, 036, 037, 038, 039,
        040, 041, 042, 043, 044, 045, 046, 047, 048, 049,
        050, 051, 052, 053, 054,

        134,
        504,
        565, 566, 567, 568, 569,
        570, 571,
        591,
        645,
        708, 709,
    ];

    public static ReadOnlySpan<ushort> Pouch_Berry_XY =>
    [
        149, // Cheri Berry
        150, 151, 152, 153, 154, 155, 156, 157, 158, 159,
        160, 161, 162, 163, 164, 165, 166, 167, 168, 169,
        170, 171, 172, 173, 174, 175, 176, 177, 178, 179,
        180, 181, 182, 183, 184, 185, 186, 187, 188, 189,
        190, 191, 192, 193, 194, 195, 196, 197, 198, 199,
        200, 201, 202, 203, 204, 205, 206, 207, 208, 209,
        210, 211, 212,

        686, // Roseli Berry
        687, // Kee Berry
        688, // Maranga Berry
    ];

    public static ushort[] GetAllHeld() => [..Pouch_Items_XY, ..Pouch_Medicine_XY, ..Pouch_Berry_XY];

    internal static ReadOnlySpan<ushort> Unreleased =>
    [
        005, // Safari Ball
        016, // Cherish Ball
        492, // Fast Ball
        493, // Level Ball
        494, // Lure Ball
        495, // Heavy Ball
        496, // Love Ball
        497, // Friend Ball
        498, // Moon Ball
        499, // Sport Ball
        500, // Park Ball
        548, // Fire Gem
        549, // Water Gem
        550, // Electric Gem
        551, // Grass Gem
        552, // Ice Gem
        553, // Fighting Gem
        554, // Poison Gem
        555, // Ground Gem
        556, // Flying Gem
        557, // Psychic Gem
        558, // Bug Gem
        559, // Rock Gem
        560, // Ghost Gem
        561, // Dragon Gemt
        562, // Dark Gem
        563, // Steel Gem
        576, // Dream Ball
        584, // Relic Copper
        585, // Relic Silver
        587, // Relic Vase
        588, // Relic Band
        589, // Relic Statue
        590, // Relic Crown
        715, // Fairy Gem
    ];

    public bool IsLegal(InventoryType type, int itemIndex, int itemCount)
    {
        if (type is InventoryType.KeyItems)
            return true;
        return Unreleased.BinarySearch((ushort)itemIndex) < 0;
    }

    public ReadOnlySpan<ushort> GetItems(InventoryType type) => type switch
    {
        InventoryType.Items => Pouch_Items_XY,
        InventoryType.KeyItems => Pouch_Key_XY,
        InventoryType.TMHMs => Pouch_TMHM_XY,
        InventoryType.Medicine => Pouch_Medicine_XY,
        InventoryType.Berries => Pouch_Berry_XY,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };
}
