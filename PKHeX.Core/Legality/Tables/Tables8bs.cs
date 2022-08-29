using System.Collections.Generic;

namespace PKHeX.Core;

public static partial class Legal
{
    internal const int MaxSpeciesID_8b = MaxSpeciesID_4; // Arceus-493
    internal const int MaxMoveID_8b = MaxMoveID_8_R2;
    internal const int MaxItemID_8b = 1822; // DS Sounds
    internal const int MaxBallID_8b = (int)Ball.LAOrigin;
    internal const int MaxGameID_8b = (int)GameVersion.SP;
    internal const int MaxAbilityID_8b = MaxAbilityID_8_R2;

    internal static readonly ushort[] Pouch_Regular_BS =
    {
        045, 046, 047, 048, 049, 050, 051, 052, 053, 072, 073, 074, 075, 076, 077, 078,
        079, 080, 081, 082, 083, 084, 085, 093, 094, 107, 108, 109, 110, 111, 112, 135,
        136, 213, 214, 215, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228,
        229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244,
        245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 260,
        261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 274, 275, 276,
        277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 292,
        293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 308,
        309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324,
        325, 326, 327, 537, 565, 566, 567, 568, 569, 570, 644, 645, 849,

        1231, 1232, 1233, 1234, 1235, 1236, 1237, 1238, 1239, 1240, 1241, 1242, 1243, 1244,
        1245, 1246, 1247, 1248, 1249, 1250, 1251, 1606,
    };

    internal static readonly ushort[] Pouch_Ball_BS =
    {
        001, 002, 003, 004, 005, 006, 007, 008, 009, 010, 011, 012, 013, 014, 015, 016,
        492, 493, 494, 495, 496, 497, 498, 499, 500,
        576,
        851,
    };

    internal static readonly ushort[] Pouch_Battle_BS =
    {
        055, 056, 057, 058, 059, 060, 061, 062, 063,
    };

    internal static readonly ushort[] Pouch_Items_BS = ArrayUtil.ConcatAll(Pouch_Regular_BS, Pouch_Ball_BS, Pouch_Battle_BS);

    internal static readonly ushort[] Pouch_Key_BS =
    {
        428, 431, 432, 433, 438, 439, 440, 443, 445, 446, 447, 448, 449, 450, 451, 452,
        453, 454, 455, 459, 460, 461, 462, 463, 464, 466, 467, 631, 632,

        1267, 1278, 1822,
    };

    internal static readonly ushort[] Pouch_Medicine_BS =
    {
        017, 018, 019, 020, 021, 022, 023, 024, 025, 026, 027, 028, 029, 030, 031, 032, 033, 034, 035, 036, 037,
        038, 039, 040, 041, 042, 043, 044, 054,

        // 134 Sweet Heart (future event item?)
    };

    internal static readonly ushort[] Pouch_Berries_BS =
    {
        149, 150, 151, 152, 153, 154, 155, 156, 157, 158,
        159, 160, 161, 162, 163, 164, 165, 166, 167, 168,
        169, 170, 171, 172, 173, 174, 175, 176, 177, 178,
        179, 180, 181, 182, 183, 184, 185, 186, 187, 188,
        189, 190, 191, 192, 193, 194, 195, 196, 197, 198,
        199, 200, 201, 202, 203, 204, 205, 206, 207, 208,
        209, 210, 211, 212, 686,
    };

    internal static readonly ushort[] Pouch_Treasure_BS =
    {
        086, 087, 088, 089, 090, 091, 092, 099, 100, 101, 102, 103, 104, 105, 106, 795, 796,

        1808, 1809, 1810, 1811, 1812, 1813, 1814, 1815, 1816, 1817, 1818, 1819, 1820, 1821,
    };

    internal static readonly ushort[] Pouch_TMHM_BS = // TM01-TM100
    {
        328, 329, 330, 331, 332, 333, 334, 335, 336, 337,
        338, 339, 340, 341, 342, 343, 344, 345, 346, 347,
        348, 349, 350, 351, 352, 353, 354, 355, 356, 357,
        358, 359, 360, 361, 362, 363, 364, 365, 366, 367,
        368, 369, 370, 371, 372, 373, 374, 375, 376, 377,
        378, 379, 380, 381, 382, 383, 384, 385, 386, 387,
        388, 389, 390, 391, 392, 393, 394, 395, 396, 397,
        398, 399, 400, 401, 402, 403, 404, 405, 406, 407,
        408, 409, 410, 411, 412, 413, 414, 415, 416, 417,
        418, 419,
        420, 421, 422, 423, 424, 425, 426, 427, // Previously called HM0X, in BDSP they're now called TM93-TM100
    };

    internal static readonly ushort[] HeldItems_BS = ArrayUtil.ConcatAll(Pouch_Items_BS, Pouch_Berries_BS, Pouch_TMHM_BS, Pouch_Medicine_BS, Pouch_Treasure_BS);

    #region Unreleased Items
    internal static readonly bool[] ReleasedHeldItems_8b = GetPermitList(MaxItemID_8b, HeldItems_BS, stackalloc ushort[]
    {
        044, // Sacred Ash
        537, // Prism Scale
        565, // Health Feather
        566, // Muscle Feather
        567, // Resist Feather
        568, // Genius Feather
        569, // Clever Feather
        570, // Swift Feather
        849, // Ice Stone

        005, // Safari Ball
        016, // Cherish Ball
        499, // Sport Ball
        500, // Park Ball
        576, // Dream Ball
        851, // Beast Ball

        // new BDSP items, but they can't be held
        1808, // Mysterious Shard S
        1809, // Mysterious Shard L
        1810, // Digger Drill
        1811, // Kanto Slate
        1812, // Johto Slate
        1813, // Soul Slate
        1814, // Rainbow Slate
        1815, // Squall Slate
        1816, // Oceanic Slate
        1817, // Tectonic Slate
        1818, // Stratospheric Slate
        1819, // Genome Slate
        1820, // Discovery Slate
        1821, // Distortion Slate
    });
    #endregion

    private const int MaxValidHatchLocation8b = 657;

    public static bool IsValidEggHatchLocation8b(ushort location, GameVersion version)
    {
        if ((uint)location > MaxValidHatchLocation8b)
            return false;
        var loc16 = location;
        if (LocationsNoHatchBDSP.Contains(loc16))
            return false;

        // Check if the location isn't an exclusive location that is only accessible in the other game.
        var table = version == GameVersion.BD ? LocationsExclusiveSP : LocationsExclusiveBD;
        return !table.Contains(loc16);
    }

    private static readonly HashSet<ushort> LocationsExclusiveBD = new()
    {
        216, // Spear Pillar
        218, // Hall of Origin
        498, // Ramanas Park (Johto Room)
        503, // Ramanas Park (Rainbow Room)
        650, // Ramanas Park (Johto Room)
        655, // Ramanas Park (Rainbow Room)
    };

    private static readonly HashSet<ushort> LocationsExclusiveSP = new()
    {
        217, // Spear Pillar
        497, // Ramanas Park (Kanto Room)
        504, // Ramanas Park (Squall Room)
        618, // Hall of Origin
        649, // Ramanas Park (Kanto Room)
        656, // Ramanas Park (Squall Room)
    };

    private static readonly HashSet<ushort> LocationsNoHatchBDSP = new()
    {
        094, 103, 107,                // Hearthome City
        154, 155, 158,                // Sunyshore City
        181, 182, 183,                // Pokémon League
        329,                          // Lake Acuity
        337, 338,                     // Battle Park
        339, 340, 341, 342, 343, 344, // Battle Tower
        345, 353, 421,                // Mystery Zone
        474,                          // Resort Area
        483, 484,                     // Mystery Zone
        491, 492, 493,                // Mystery Zone
        495,                          // Ramanas Park
        620, 621, 622, 623,           // Grand Underground (Secret Base)
        625,                          // Sea (sailing animation)
        627, 628, 629, 630, 631, 632, // Grand Underground (Secret Base)
        633, 634, 635, 636, 637, 638, // Grand Underground (Secret Base)
        639, 640, 641, 642, 643, 644, // Grand Underground (Secret Base)
        645, 646, 647,                // Grand Underground (Secret Base)
    };
}
