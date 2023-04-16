using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core;

public static partial class Legal
{
    internal const int MaxSpeciesID_4 = 493;
    internal const int MaxMoveID_4 = 467;
    internal const int MaxItemID_4_DP = 464;
    internal const int MaxItemID_4_Pt = 467;
    internal const int MaxItemID_4_HGSS = 536;
    internal const int MaxAbilityID_4 = 123;
    internal const int MaxBallID_4 = 0x18;
    internal const int MaxGameID_4 = 15; // CXD

    internal static readonly ushort[] HeldItems_DP = ItemStorage4DP.GetAllHeld();
    internal static readonly ushort[] HeldItems_Pt = ItemStorage4Pt.GetAllHeld(); // Griseous Orb Added
    internal static readonly ushort[] HeldItems_HGSS = HeldItems_Pt;

    internal static readonly bool[] ReleasedHeldItems_4 = GetPermitList(MaxItemID_4_HGSS, HeldItems_HGSS, ItemStorage4.Unreleased);

    internal static readonly HashSet<ushort> ValidMet_DP = new()
    {
        // 063: Flower Paradise unreleased DP event
        // 079: Newmoon Island unreleased DP event
        // 085: Seabreak Path unreleased DP event
        // 086: Hall of Origin unreleased event
             001, 002, 003, 004, 005, 006, 007, 008, 009,
        010, 011, 012, 013, 014, 015, 016, 017, 018, 019,
        020, 021, 022, 023, 024, 025, 026, 027, 028, 029,
        030, 031, 032, 033, 034, 035, 036, 037, 038, 039,
        040, 041, 042, 043, 044, 045, 046, 047, 048, 049,
        050, 051, 052, 053, 054, 055, 056, 057, 058, 059,
        060, 061, 062,      064, 065, 066, 067, 068, 069,
        070, 071, 072, 073, 074, 075, 076, 077, 078,
        080, 081, 082, 083, 084,           087, 088, 089,
        090, 091, 092, 093, 094, 095, 096, 097, 098, 099,
        100, 101, 102, 103, 104, 105, 106, 107, 108, 109,
        110, 111,
    };

    internal static readonly HashSet<ushort> ValidMet_Pt = new(ValidMet_DP)
    {
        // 086: Hall of Origin unreleased event
        63, 79, 85,
                  112, 113, 114, 115, 116, 117, 118, 119,
        120, 121, 122, 123, 124, 125,
    };

    internal static readonly HashSet<ushort> ValidMet_HGSS = new()
    {
        080,
                  112, 113, 114, 115, 116,
                                      126, 127, 128, 129,
        130, 131, 132, 133, 134, 135, 136, 137, 138, 139,
        140, 141, 142, 143, 144, 145, 146, 147, 148, 149,
        150, 151, 152, 153, 154, 155, 156, 157, 158, 159,
        160, 161, 162, 163, 164, 165, 166, 167, 168, 169,
        170,      172, 173, 174, 175, 176, 177, 178, 179, //171: Route 23 no longer exists
        180, 181, 182, 183, 184, 185, 186, 187, 188, 189,
        190, 191, 192, 193, 194, 195, 196, 197, 198, 199,
        200, 201, 202, 203, 204, 205, 206, 207, 208, 209,
        210, 211, 212, 213, 214, 215, 216, 217, 218, 219,
        220, 221, 222, 223, 224, 225, 226, 227, 228, 229,
        230, 231, 232,      234,                          //233: Pokéwalker
    };

    internal static readonly HashSet<ushort> ValidMet_4 = new(ValidMet_Pt.Concat(ValidMet_HGSS));

    internal static readonly HashSet<ushort> GiftEggLocation4 = new()
    {
        2009, 2010, 2011, 2013, 2014,
    };

    internal static int GetTransfer45MetLocation(PKM pk)
    {
        // Everything except for crown beasts and Celebi get the default transfer location.
        // Crown beasts and Celebi are 100% identifiable by the species ID and fateful encounter, originating from Gen4.
        if (!pk.Gen4 || !pk.FatefulEncounter)
            return Locations.Transfer4; // Pokétransfer

        return pk.Species switch
        {
            // Crown Beast
            (int)Species.Raikou or (int)Species.Entei or (int)Species.Suicune => Locations.Transfer4_CrownUnused,
            // Celebi
            (int)Species.Celebi => Locations.Transfer4_CelebiUnused,
            // Default
            _ => Locations.Transfer4,
        };
    }
}
