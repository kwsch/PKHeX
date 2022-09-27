using System.Collections.Generic;
using static PKHeX.Core.Species;

namespace PKHeX.Core;

public static partial class Legal
{
    internal const int MaxSpeciesID_7 = 802;
    internal const int MaxMoveID_7 = 719;
    internal const int MaxItemID_7 = 920;
    internal const int MaxAbilityID_7 = 232;
    internal const int MaxBallID_7 = 0x1A; // 26
    internal const int MaxGameID_7 = 41; // Crystal (VC?)

    internal const int MaxSpeciesID_7_USUM = 807;
    internal const int MaxMoveID_7_USUM = 728;
    internal const int MaxItemID_7_USUM = 959;
    internal const int MaxAbilityID_7_USUM = 233;

    internal static readonly ushort[] Pouch_Regular_SM = // 00
    {
        068, 069, 070, 071, 072, 073, 074, 075, 076, 077, 078, 079, 080, 081, 082, 083, 084, 085, 086, 087,
        088, 089, 090, 091, 092, 093, 094, 099, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111,
        112, 116, 117, 118, 119, 135, 136, 137, 213, 214, 215, 217, 218, 219, 220, 221, 222, 223, 224, 225,
        226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245,
        246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265,
        266, 267, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285,
        286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305,
        306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325,
        326, 327, 499, 534, 535, 537, 538, 539, 540, 541, 542, 543, 544, 545, 546, 547, 548, 549, 550, 551,
        552, 553, 554, 555, 556, 557, 558, 559, 560, 561, 562, 563, 564, 571, 572, 573, 580, 581, 582, 583,
        584, 585, 586, 587, 588, 589, 590, 639, 640, 644,      646, 647, 648, 649, 650, 656, 657, 658, 659,
        660, 661, 662, 663, 664, 665, 666, 667, 668, 669, 670, 671, 672, 673, 674, 675, 676, 677, 678, 679,
        680, 681, 682, 683, 684, 685, 699, 704, 710, 711, 715, 752, 753, 754, 755, 756, 757, 758, 759, 760,
        761, 762, 763, 764, 767, 768, 769, 770, 795, 796, 844, 849, 853, 854, 855, 856, 879, 880, 881, 882,
        883, 884, 904, 905, 906, 907, 908, 909, 910, 911, 912, 913, 914, 915, 916, 917, 918, 919, 920,
    };

    internal static readonly ushort[] Pouch_Ball_SM = { // 08
        1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 492, 493, 494, 495, 496, 497, 498, 576,
        851,
    };

    internal static readonly ushort[] Pouch_Battle_SM = { // 16
        55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 577,
        846,
    };

    internal static readonly ushort[] Pouch_Items_SM = ArrayUtil.ConcatAll(Pouch_Regular_SM, Pouch_Ball_SM, Pouch_Battle_SM);

    internal static readonly ushort[] Pouch_Key_SM = {
        216, 465, 466, 628, 629, 631, 632, 638,
        705, 706, 765, 773, 797,
        841, 842, 843, 845, 847, 850, 857, 858, 860,
    };

    internal static readonly ushort[] Pouch_Key_USUM = ArrayUtil.ConcatAll(Pouch_Key_SM, new ushort[] {
        933, 934, 935, 936, 937, 938, 939, 940, 941, 942, 943, 944, 945, 946, 947, 948,
        440,
    });

    public static readonly ushort[] Pouch_Roto_USUM = {
        949, 950, 951, 952, 953, 954, 955, 956, 957, 958, 959,
    };

    internal static readonly ushort[] Pouch_TMHM_SM = { // 02
        328, 329, 330, 331, 332, 333, 334, 335, 336, 337, 338, 339, 340, 341, 342, 343, 344, 345,
        346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363,
        364, 365, 366, 367, 368, 369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 379, 380, 381,
        382, 383, 384, 385, 386, 387, 388, 389, 390, 391, 392, 393, 394, 395, 396, 397, 398, 399,
        400, 401, 402, 403, 404, 405, 406, 407, 408, 409, 410, 411, 412, 413, 414, 415, 416, 417,
        418, 419, 618, 619, 620, 690, 691, 692, 693, 694,
    };

    internal static readonly ushort[] Pouch_Medicine_SM = { // 32
        17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 65, 66, 67, 134,
        504, 565, 566, 567, 568, 569, 570, 591, 645, 708, 709,
        852,
    };

    internal static readonly ushort[] Pouch_Berries_SM = {
        149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212,
        686, 687, 688,
    };

    internal static readonly ushort[] Pouch_ZCrystal_SM = { // Bead
        807, 808, 809, 810, 811, 812, 813, 814, 815, 816, 817, 818, 819, 820, 821, 822, 823, 824, 825, 826, 827, 828, 829, 830, 831, 832, 833, 834, 835,
    };

    internal static readonly ushort[] Pouch_ZCrystalHeld_SM = { // Piece
        776, 777, 778, 779, 780, 781, 782, 783, 784, 785, 786, 787, 788, 789, 790, 791, 792, 793, 794, 798, 799, 800, 801, 802, 803, 804, 805, 806, 836,
    };

    internal static readonly ushort[] Pouch_ZCrystal_USUM = ArrayUtil.ConcatAll(Pouch_ZCrystal_SM, new ushort[] { // Bead
        927, 928, 929, 930, 931, 932,
    });

    internal static readonly ushort[] Pouch_ZCrystalHeld_USUM = ArrayUtil.ConcatAll(Pouch_ZCrystalHeld_SM, new ushort[] { // Piece
        921, 922, 923, 924, 925, 926,
    });

    public static readonly Dictionary<ushort, ushort> ZCrystalDictionary = GetDictionary(Pouch_ZCrystal_USUM, Pouch_ZCrystalHeld_USUM);

    private static Dictionary<ushort, ushort> GetDictionary(IReadOnlyList<ushort> key, IReadOnlyList<ushort> held)
    {
        var result = new Dictionary<ushort, ushort>(held.Count);
        for (int i = 0; i < key.Count; i++)
            result.Add(key[i], held[i]);
        return result;
    }

    internal static readonly ushort[] HeldItems_SM = ArrayUtil.ConcatAll(Pouch_Items_SM, Pouch_Berries_SM, Pouch_Medicine_SM, Pouch_ZCrystalHeld_SM);
    internal static readonly ushort[] HeldItems_USUM = ArrayUtil.ConcatAll(Pouch_Items_SM, Pouch_Berries_SM, Pouch_Medicine_SM, Pouch_ZCrystalHeld_USUM, Pouch_Roto_USUM);

    internal static readonly HashSet<ushort> AlolanOriginForms = new()
    {
        (int)Rattata,
        (int)Raticate,
        (int)Sandshrew,
        (int)Sandslash,
        (int)Vulpix,
        (int)Ninetales,
        (int)Diglett,
        (int)Dugtrio,
        (int)Meowth,
        (int)Persian,
        (int)Geodude,
        (int)Graveler,
        (int)Golem,
        (int)Grimer,
        (int)Muk,
    };

    internal static readonly HashSet<ushort> AlolanVariantEvolutions12 = new()
    {
        (int)Raichu,
        (int)Exeggutor,
        (int)Marowak,
    };

    public static readonly HashSet<ushort> PastGenAlolanNatives = new()
    {
        010, 011, 012, 019, 020, 021, 022, 025, 026, 027, 028, 035, 036, 037, 038, 039, 040, 041, 042, 046, 047, 050,
        051, 052, 053, 054, 055, 056, 057, 058, 059, 060, 061, 062, 063, 064, 065, 066, 067, 068, 072, 073, 074, 075,
        076, 079, 080, 081, 082, 088, 089, 090, 091, 092, 093, 094, 096, 097, 102, 103, 104, 105, 113, 115, 118, 119,
        120, 121, 123, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 142, 143, 147, 148, 149, 165,
        166, 167, 168, 169, 170, 171, 172, 173, 174, 185, 186, 196, 197, 198, 199, 200, 209, 210, 212, 215, 222, 225,
        227, 233, 235, 239, 240, 241, 242, 278, 279, 283, 284, 296, 297, 299, 302, 318, 319, 320, 321, 324, 327, 328,
        329, 330, 339, 340, 349, 350, 351, 359, 361, 362, 369, 370, 371, 372, 373, 374, 375, 376, 408, 409, 410, 411,
        422, 423, 425, 426, 429, 430, 438, 440, 443, 444, 445, 446, 447, 448, 456, 457, 461, 462, 466, 467, 470, 471,
        474, 476, 478, 506, 507, 508, 524, 525, 526, 546, 547, 548, 549, 551, 552, 553, 564, 565, 566, 567, 568, 569,
        582, 583, 584, 587, 594, 627, 628, 629, 630, 661, 662, 663, 674, 675, 700, 703, 704, 705, 706, 707, 708, 709,
        718,

        // Regular
        023, 086, 108, 122, 138, 140, 163, 177, 179, 190, 204,
        206, 214, 223, 228, 238, 246, 303, 309, 341, 343,
        345, 347, 352, 353, 357, 366, 427, 439, 458, 550,
        559, 570, 572, 592, 605, 619, 621, 622, 624, 636,
        667, 669, 676, 686, 690, 692, 696, 698, 701, 702,
        714,

        // Wormhole
        333, 334, // Altaria
        193, 469, // Yanmega
        561, // Sigilyph
        580, 581, // Swanna
        276, 277, // Swellow
        451, 452, // Drapion
        531, // Audino
        694, 695, // Heliolisk
        273, 274, 275, // Nuzleaf
        325, 326, // Gumpig
        459, 460, // Abomasnow
        307, 308, // Medicham
        449, 450, // Hippowdon
        557, 558, // Crustle
        218, 219, // Magcargo
        688, 689, // Barbaracle
        270, 271, 272, // Lombre
        618, // Stunfisk
        418, 419, // Floatzel
        194, 195, // Quagsire

        100, 101, // Voltorb & Electrode
    };

    internal static readonly HashSet<ushort> Totem_Alolan = new()
    {
        (int)Raticate, // (Normal, Alolan, Totem)
        (int)Marowak, // (Normal, Alolan, Totem)
        (int)Mimikyu, // (Normal, Busted, Totem, Totem_Busted)
    };

    internal static readonly HashSet<ushort> Totem_NoTransfer = new()
    {
        (int)Marowak,
        (int)Araquanid,
        (int)Togedemaru,
        (int)Ribombee,
    };

    internal static readonly HashSet<ushort> Totem_USUM = new()
    {
        (int)Raticate,
        (int)Gumshoos,
        //(int)Wishiwashi,
        (int)Salazzle,
        (int)Lurantis,
        (int)Vikavolt,
        (int)Mimikyu,
        (int)Kommoo,
        (int)Marowak,
        (int)Araquanid,
        (int)Togedemaru,
        (int)Ribombee,
    };

    internal static readonly HashSet<ushort> ValidMet_SM = new()
    {
                       006, 008,
        010, 012, 014, 016, 018,
        020, 022, 024, 026, 028,
        030, 032, 034, 036, 038,
        040, 042, 044, 046, 048,
        050, 052, 054, 056, 058,
        060, 062, 064,      068,
        070, 072, 074, 076, 078,
             082, 084, 086, 088,
        090, 092, 094,
        100, 102, 104, 106, 108,
        110, 112, 114, 116, 118,
        120, 122, 124, 126, 128,
        130, 132, 134, 136, 138,
        140, 142, 144, 146, 148,
        150, 152, 154, 156, 158,
        160, 162, 164, 166, 168,
        170, 172, 174, 176, 178,
        180, 182, 184, 186, 188,
        190, 192,

        Locations.Pelago7, // 30016
    };

    internal static readonly HashSet<ushort> ValidMet_USUM = new(ValidMet_SM)
    {
        // 194, 195, 196, 197, // Unobtainable new Locations
                            198,
        200, 202, 204, 206, 208,
        210, 212, 214, 216, 218,
        220, 222, 224, 226, 228,
        230, 232,
    };

    #region Unreleased Items
    internal static readonly bool[] ReleasedHeldItems_7 = GetPermitList(MaxItemID_7_USUM, HeldItems_USUM, stackalloc ushort[]
    {
        005, // Safari Ball
        016, // Cherish Ball
        064, // Fluffy Tail
        065, // Blue Flute
        066, // Yellow Flute
        067, // Red Flute
        068, // Black Flute
        069, // White Flute
        070, // Shoal Salt
        071, // Shoal Shell
        103, // Old Amber
        111, // Odd Keystone
        164, // Razz Berry
        166, // Nanab Berry
        167, // Wepear Berry
        175, // Cornn Berry
        176, // Magost Berry
        177, // Rabuta Berry
        178, // Nomel Berry
        179, // Spelon Berry
        180, // Pamtre Berry
        181, // Watmel Berry
        182, // Durin Berry
        183, // Belue Berry
        //208, // Enigma Berry
        //209, // Micle Berry
        //210, // Custap Berry
        //211, // Jaboca Berry
        //212, // Rowap Berry
        215, // Macho Brace
        260, // Red Scarf
        261, // Blue Scarf
        262, // Pink Scarf
        263, // Green Scarf
        264, // Yellow Scarf
        499, // Sport Ball
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
        561, // Dragon Gem
        562, // Dark Gem
        563, // Steel Gem
        576, // Dream Ball
        584, // Relic Copper
        585, // Relic Silver
        587, // Relic Vase
        588, // Relic Band
        589, // Relic Statue
        590, // Relic Crown
        699, // Discount Coupon
        715, // Fairy Gem
    });
    #endregion
}
