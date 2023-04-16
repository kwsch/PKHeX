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

    internal static readonly ushort[] HeldItems_RS = ItemStorage3RS.GetAllHeld();
    #endregion


    internal static readonly bool[] ReleasedHeldItems_3 = GetPermitList(MaxItemID_3, HeldItems_RS, ItemStorage3RS.Unreleased); // Safari Ball

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
