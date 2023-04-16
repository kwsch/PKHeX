using System.Collections.Generic;

namespace PKHeX.Core;

public static partial class Legal
{
    internal const int MaxSpeciesID_6 = 721;
    internal const int MaxMoveID_6_XY = 617;
    internal const int MaxMoveID_6_AO = 621;
    internal const int MaxItemID_6_XY = 717;
    internal const int MaxItemID_6_AO = 775;
    internal const int MaxAbilityID_6_XY = 188;
    internal const int MaxAbilityID_6_AO = 191;
    internal const int MaxBallID_6 = 0x19;
    internal const int MaxGameID_6 = 27; // OR

    internal static readonly ushort[] HeldItem_AO = ItemStorage6XY.GetAllHeld();

    internal static readonly HashSet<ushort> ValidMet_XY = new()
    {
                       006, 008,
        010, 012, 014, 016, 018,
        020, 022, 024, 026, 028,
        030, 032, 034, 036, 038,
        040, 042, 044, 046, 048,
        050, 052, 054, 056, 058,
        060, 062, 064, 066, 068,
        070, 072, 074, 076, 078,
             082, 084, 086, 088,
        090, 092, 094, 096, 098,
        100, 102, 104, 106, 108,
        110, 112, 114, 116, 118,
        120, 122, 124, 126, 128,
        130, 132, 134, 136, 138,
        140, 142, 144, 146, 148,
        150, 152, 154, 156, 158,
        160, 162, 164, 166, 168,
    };

    internal static readonly HashSet<ushort> ValidMet_AO = new()
    {
        170, 172, 174, 176, 178,
        180, 182, 184, 186, 188,
        190, 192, 194, 196, 198,
        200, 202, 204, 206, 208,
        210, 212, 214, 216, 218,
        220, 222, 224, 226, 228,
        230, 232, 234, 236, 238,
        240, 242, 244, 246, 248,
        250, 252, 254, 256, 258,
        260, 262, 264, 266, 268,
        270, 272, 274, 276, 278,
        280, 282, 284, 286, 288,
        290, 292, 294, 296, 298,
        300, 302, 304, 306, 308,
        310, 312, 314, 316, 318,
        320, 322, 324, 326, 328,
        330, 332, 334, 336, 338,
        340, 342, 344, 346,
        350, 352, 354,
    };

    internal static readonly bool[] ReleasedHeldItems_6 = GetPermitList(MaxItemID_6_AO, HeldItem_AO, ItemStorage6XY.Unreleased);
}
