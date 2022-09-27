using System.Collections.Generic;

namespace PKHeX.Core;

public static partial class Legal
{
    internal const int MaxSpeciesIndex_3 = 412;
    internal const int MaxSpeciesID_3 = 386;
    internal const int MaxMoveID_3 = 354;
    internal const int MaxItemID_3 = 374;
    internal const int MaxItemID_3_COLO = 547;
    internal const int MaxItemID_3_XD = 593;
    internal const int MaxAbilityID_3 = 77;
    internal const int MaxBallID_3 = 0xC;
    internal const int MaxGameID_3 = 15; // CXD

    #region RS
    internal static readonly ushort[] Pouch_Items_RS = {
        13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 63, 64, 65, 66, 67, 68, 69, 70, 71, 73, 74, 75, 76, 77, 78, 79, 80, 81, 83, 84, 85, 86, 93, 94, 95, 96, 97, 98, 103, 104, 106, 107, 108, 109, 110, 111, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 254, 255, 256, 257, 258,
    };

    internal static readonly ushort[] Pouch_Key_RS = {
        259, 260, 261, 262, 263, 264, 265, 266, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288,
    };

    internal static readonly ushort[] Pouch_TM_RS = {
        289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327, 328, 329, 330, 331, 332, 333, 334, 335, 336, 337, 338,
    };

    public static readonly ushort[] Pouch_HM_RS = {
        339, 340, 341, 342, 343, 344, 345, 346,
    };

    internal static readonly ushort[] Pouch_Berries_RS = {
        133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175,
    };

    internal static readonly ushort[] Pouch_Ball_RS = {
        1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12,
    };

    internal static readonly ushort[] Pouch_Key_FRLG = ArrayUtil.ConcatAll(Pouch_Key_RS, new ushort[] { 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363, 364, 365, 366, 367, 368, 369, 370, 371, 372, 373, 374 });
    internal static readonly ushort[] Pouch_Key_E = ArrayUtil.ConcatAll(Pouch_Key_FRLG, new ushort[] { 375, 376 });

    internal static readonly ushort[] Pouch_TMHM_RS = ArrayUtil.ConcatAll(Pouch_TM_RS, Pouch_HM_RS);
    internal static readonly ushort[] HeldItems_RS = ArrayUtil.ConcatAll(Pouch_Items_RS, Pouch_Ball_RS, Pouch_Berries_RS, Pouch_TM_RS);
    #endregion

    internal static readonly ushort[] Pouch_Cologne_COLO = {543, 544, 545};
    internal static readonly ushort[] Pouch_Items_COLO = ArrayUtil.ConcatAll(Pouch_Items_RS, new ushort[] {537}); // Time Flute
    internal static readonly ushort[] HeldItems_COLO = ArrayUtil.ConcatAll(Pouch_Items_COLO, Pouch_Ball_RS, Pouch_Berries_RS, Pouch_TM_RS);

    internal static readonly ushort[] Pouch_Key_COLO =
    {
        500, 501, 502, 503, 504, 505, 506, 507, 508, 509,
        510, 511, 512, 513, 514, 515, 516, 517, 518, 519,
        520, 521, 522, 523, 524, 525, 526, 527, 528, 529,
        530, 531, 532, 533, 534, 535, 536,      538, 539,
        540, 541, 542,                546, 547,
    };

    internal static readonly ushort[] Pouch_Cologne_XD = {513, 514, 515};
    internal static readonly ushort[] Pouch_Items_XD = ArrayUtil.ConcatAll(Pouch_Items_RS, new ushort[] {511}); // Poké Snack
    internal static readonly ushort[] HeldItems_XD = ArrayUtil.ConcatAll(Pouch_Items_XD, Pouch_Ball_RS, Pouch_Berries_RS, Pouch_TM_RS);

    internal static readonly ushort[] Pouch_Key_XD =
    {
        500, 501, 502, 503, 504, 505, 506, 507, 508, 509,
        510,      512,                516, 517, 518, 519,
        523, 524, 525, 526, 527, 528, 529,
        530, 531, 532, 533,
    };

    internal static readonly ushort[] Pouch_Disc_XD =
    {
                            534, 535, 536, 537, 538, 539,
        540, 541, 542, 543, 544, 545, 546, 547, 548, 549,
        550, 551, 552, 553, 554, 555, 556, 557, 558, 559,
        560, 561, 562, 563, 564, 565, 566, 567, 568, 569,
        570, 571, 572, 573, 574, 575, 576, 577, 578, 579,
        580, 581, 582, 583, 584, 585, 586, 587, 588, 589,
        590, 591, 592, 593,
    };

    internal static readonly bool[] ReleasedHeldItems_3 = GetPermitList(MaxItemID_3, HeldItems_RS, stackalloc ushort[] {005}); // Safari Ball

    // 064 is an unused location for Meteor Falls
    // 084 is Inside of a truck, no possible pokemon can be hatched there
    // 071 is Mirage island, cannot be obtained as the player is technically still on Route 130's map.
    // 075 is an unused location for Fiery Path
    // 077 is an unused location for Jagged Pass
    internal static readonly HashSet<byte> ValidMet_RS = new()
    {
        000, 001, 002, 003, 004, 005, 006, 007, 008, 009,
        010, 011, 012, 013, 014, 015, 016, 017, 018, 019,
        020, 021, 022, 023, 024, 025, 026, 027, 028, 029,
        030, 031, 032, 033, 034, 035, 036, 037, 038, 039,
        040, 041, 042, 043, 044, 045, 046, 047, 048, 049,
        050, 051, 052, 053, 054, 055, 056, 057, 058, 059,
        060, 061, 062, 063,      065, 066, 067, 068, 069,
        070, 072, 073,      074,      076,      078, 079,
        080, 081, 082, 083, 085, 086, 087,
    };

    // 155 - 158 Sevii Isle 6-9 Unused
    // 171 - 173 Sevii Isle 22-24 Unused
    internal static readonly HashSet<byte> ValidMet_FRLG = new()
    {
                                           087, 088, 089,
        090, 091, 092, 093, 094, 095, 096, 097, 098, 099,
        100, 101, 102, 103, 104, 105, 106, 107, 108, 109,
        110, 111, 112, 113, 114, 115, 116, 117, 118, 119,
        120, 121, 122, 123, 124, 125, 126, 127, 128, 129,
        130, 131, 132, 133, 134, 135, 136, 137, 138, 139,
        140, 141, 142, 143, 144, 145, 146, 147, 148, 149,
        150, 151, 152, 153, 154,                     159,
        160, 161, 162, 163, 164, 165, 166, 167, 168, 169,
        170,                174, 175, 176, 177, 178, 179,
        180, 181, 182, 183, 184, 185, 186, 187, 188, 189,
        190, 191, 192, 193, 194, 195, 196,
    };

    internal static readonly HashSet<byte> ValidMet_E = new(ValidMet_RS)
    {
                                      196, 197, 198, 199,
        200, 201, 202, 203, 204, 205, 206, 207, 208, 209,
        210, 211, 212,
    };
}
