using System.Collections.Generic;

namespace PKHeX.Core;

public static partial class Legal
{
    // Current Binaries
    internal const int MaxSpeciesID_8 = MaxSpeciesID_8_R2;
    internal const int MaxMoveID_8 = MaxMoveID_8_R2;
    internal const int MaxItemID_8 = MaxItemID_8_R2;
    internal const int MaxAbilityID_8 = MaxAbilityID_8_R2;

    // Orion (No DLC)
    internal const int MaxSpeciesID_8_O0 = 890; // Eternatus
    internal const int MaxMoveID_8_O0 = 796; // Steel Beam
    internal const int MaxItemID_8_O0 = 1278; // Rotom Catalog, ignore all catalog parts
    internal const int MaxAbilityID_8_O0 = 258; // Hunger Switch

    // Rigel 1 (DLC 1: Isle of Armor)
    internal const int MaxSpeciesID_8_R1 = 893; // Zarude
    internal const int MaxMoveID_8_R1 = 818; // Surging Strikes
    internal const int MaxItemID_8_R1 = 1589; // Mark Charm
    internal const int MaxAbilityID_8_R1 = 260; // Unseen Fist

    // Rigel 2 (DLC 2: Crown Tundra)
    internal const int MaxSpeciesID_8_R2 = 898; // Calyrex
    internal const int MaxMoveID_8_R2 = 826; // Eerie Spell
    internal const int MaxItemID_8_R2 = 1607; // Reins of Unity
    internal const int MaxAbilityID_8_R2 = 267; // As One (Glastrier)

    internal const int MaxBallID_8 = 0x1A; // 26 Beast
    internal const int MaxGameID_8 = 45; // Shield

    internal static readonly ushort[] HeldItems_SWSH = ItemStorage8SWSH.GetAllHeld();

    internal static readonly HashSet<ushort> GalarOriginForms = new()
    {
        (int)Species.Meowth,
        (int)Species.Ponyta,
        (int)Species.Rapidash,
        (int)Species.Slowpoke,
        (int)Species.Farfetchd,
        (int)Species.MrMime,
        (int)Species.Corsola,
        (int)Species.Zigzagoon,
        (int)Species.Linoone,
        (int)Species.Yamask,
        (int)Species.Darumaka,
        (int)Species.Darmanitan,
        (int)Species.Stunfisk,
    };

    internal static readonly HashSet<ushort> GalarVariantFormEvolutions = new()
    {
        (int)Species.MrMime,
        (int)Species.Weezing,
    };

    internal static readonly HashSet<ushort> ValidMet_SWSH = new()
    {
        006, 008,
        012, 014, 016, 018,
        020, 022, 024,      028,
        030, 032, 034, 036,
        040,      044, 046, 048,
        052, 054, 056, 058,
        060,      064, 066, 068,
        070, 072,      076, 078,
        080,      084, 086, 088,
        090, 092, 094, 096, 098,
        102, 104, 106, 108,
        110, 112, 114, 116, 118,
        120, 122, 124, 126, 128,
        130, 132, 134, 136, 138,
        140, 142, 144, 146, 148,
        150, 152, 154, 156, 158,
        160,      164, 166, 168,
        170, 172, 174, 176, 178,
        180, 182, 184, 186, 188,
        190, 192, 194, 196, 198,
        200,

        202, 204, 206, 208, 210,
        212, 214, 216, 218, 220,
        222, 224, 226, 228, 230,
        232, 234, 236, 238, 240,
        242, 244, 246,
    };

    #region Unreleased Items

    private const int DMAX_START = 1279;
    private const int DMAX_END = 1578;
    private const int DMAX_LEGAL_END = 1290; // ★Sgr7194 (Eevee)
    public static bool IsDynamaxCrystal(ushort item) => item is >= DMAX_START and <= DMAX_END;
    public static bool IsDynamaxCrystalAvailable(ushort item) => item is >= DMAX_START and <= DMAX_LEGAL_END;

    internal static readonly bool[] ReleasedHeldItems_8 = GetPermitList(MaxItemID_8, HeldItems_SWSH, ItemStorage8SWSH.Unreleased);
    #endregion
}
