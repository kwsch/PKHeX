using System.Collections.Generic;

namespace PKHeX.Core;

public static partial class Legal
{
    internal const int MaxSpeciesID_9 = (int)Species.IronLeaves;
    internal const int MaxMoveID_9 = (int)Move.MagicalTorque;
    internal const int MaxItemID_9 = 2400; // Yellow Dish
    internal const int MaxAbilityID_9 = (int)Ability.MyceliumMight;

    internal const int MaxBallID_9 = (int)Ball.LAOrigin;
    internal const int MaxGameID_9 = (int)GameVersion.VL;

    internal static readonly ushort[] HeldItems_SV = ItemStorage9SV.GetAllHeld();

    internal static readonly HashSet<ushort> ValidMet_SV = new()
    {
                       006,
        010, 012, 014, 016, 018,
        020, 022, 024, 026, 028,
        030, 032, 034, 036, 038,
        040,      044, 046, 048,
        050, 052,      056, 058,
        060, 062, 064,

        067, 069, 070, 072, 076,
        078, 080, 082, 084, 086,
        088, 090, 092, 094, 096,

        099, 101, 103, 105, 107,
        109, 111, 113, 115, 117,
        118, 124,           130,
        131,
    };

    public static bool IsValidEggHatchLocation9(ushort location, GameVersion version)
    {
        if (version == GameVersion.SL && location == 131) // Uva Academy does not exist in Scarlet
            return false;
        if (version == GameVersion.VL && location == 130) // Naranja Academy does not exist in Violet
            return false;
        return ValidMet_SV.Contains(location);
    }

    #region Unreleased Items

    internal static readonly bool[] ReleasedHeldItems_9 = GetPermitList(MaxItemID_9, HeldItems_SV, ItemStorage9SV.Unreleased);
    #endregion
}
