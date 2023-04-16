using System.Collections.Generic;

namespace PKHeX.Core;

public static partial class Legal
{
    internal const int MaxSpeciesID_5 = 649;
    internal const int MaxMoveID_5 = 559;
    internal const int MaxItemID_5_BW = 632;
    internal const int MaxItemID_5_B2W2 = 638;
    internal const int MaxAbilityID_5 = 164;
    internal const int MaxBallID_5 = 0x19;
    internal const int MaxGameID_5 = 23; // B2

    internal static readonly ushort[] HeldItems_BW = ItemStorage5.GetAllHeld();

    internal static readonly bool[] ReleasedHeldItems_5 = GetPermitList(MaxItemID_5_B2W2, HeldItems_BW, ItemStorage5.Unreleased);

    internal static readonly HashSet<ushort> ValidMet_BW = new()
    {
                            004, 005, 006, 007, 008, 009,
        010, 011, 012, 013, 014, 015, 016, 017, 018, 019,
        020, 021, 022, 023, 024, 025, 026, 027, 028, 029,
        030, 031, 032, 033, 034, 035, 036, 037, 038, 039,
        040, 041, 042, 043, 044, 045, 046, 047, 048, 049,
        050, 051, 052, 053, 054, 055, 056, 057, 058, 059,
        060, 061, 062, 063, 064, 065, 066, 067, 068, 069,
        070, 071, 072, 073, 074, 075, 076, 077, 078, 079,
        080, 081, 082, 083, 084, 085, 086, 087, 088, 089,
        090, 091, 092, 093, 094, 095, 096, 097, 098, 099,
        100, 101, 102, 103, 104, 105, 106, 107, 108, 109,
        110, 111, 112, 113, 114, 115, 116,
    };

    internal static readonly HashSet<ushort> ValidMet_B2W2 = new()
    {
                            004, 005, 006, 007, 008, 009,
        010, 011, 012, 013, 014, 015, 016, 017, 018, 019,
        020, 021, 022,      024, 025, 026, 027, 028, 029, // 023 Route 10
        030, 031, 032, 033, 034, 035, 036, 037, 038, 039,
             041, 042, 043, 044, 045, 046, 047, 048, 049, // 040->134 Victory Road
        050, 051, 052, 053, 054, 055, 056, 057, 058,      // 059 Challenger's cave
        060, 061, 062, 063, 064, 065, 066, 067, 068, 069,
        070, 071, 072,      074, 075, 076, 077, 078, 079, // 073 Trial Chamber
        080, 081, 082, 083, 084, 085, 086, 087, 088, 089,
        090, 091, 092, 093, 094, 095, 096, 097, 098, 099,
        100, 101, 102, 103, 104, 105, 106, 107, 108, 109,
        110, 111, 112, 113, 114, 115, 116,
                                           117, 118, 119,
        120, 121, 122, 123, 124, 125, 126, 127, 128, 129,
        130, 131, 132, 133, 134, 135, 136, 137,      139, // 138 ---
        140, 141, 142, 143, 144, 145, 146, 147, 148, 149,
        150, 151, 152, 153,
    };
}
