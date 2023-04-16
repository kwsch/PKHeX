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

    internal static readonly ushort[] HeldItems_BS = ItemStorage8BDSP.GetAll();

    #region Unreleased Items
    internal static readonly bool[] ReleasedHeldItems_8b = GetPermitList(MaxItemID_8b, HeldItems_BS, ItemStorage8BDSP.Unreleased);
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
