using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

/// <summary>
/// Provides information for what values an Egg can have, while it still is an egg.
/// </summary>
public static class EggStateLegality
{
    /// <summary>
    /// Checks if the Egg Entity's hatch counter value is within the confines of game legality.
    /// </summary>
    /// <param name="pk">Egg Entity</param>
    /// <param name="enc">Encounter the egg was generated from</param>
    /// <returns>True if valid, false if invalid.</returns>
    public static bool GetIsEggHatchCyclesValid(PKM pk, IEncounterTemplate enc)
    {
        var hatchCounter = pk.OriginalTrainerFriendship;
        var max = GetMaximumEggHatchCycles(pk, enc);
        if (hatchCounter > max)
            return false;
        var min = GetMinimumEggHatchCycles(pk);
        if (hatchCounter < min)
            return false;

        return true;
    }

    /// <summary>
    /// Gets the minimum hatch counter value allowed for an Egg Entity.
    /// </summary>
    /// <param name="pk">Egg Entity</param>
    /// <returns>Usually 0...</returns>
    public static int GetMinimumEggHatchCycles(PKM pk) => pk switch
    {
        PK2 or PB8 or PK9 => 1, // no grace period between 1 step remaining and hatch
        _ => 0, // having several Eggs in your party and then hatching one will give the rest 0... they can then be boxed!
    };

    /// <inheritdoc cref="GetMaximumEggHatchCycles(PKM, IEncounterTemplate)"/>
    /// <remarks>Will create a new <see cref="LegalityAnalysis"/> to find the encounter.</remarks>
    public static int GetMaximumEggHatchCycles(PKM pk)
    {
        var la = new LegalityAnalysis(pk);
        var enc = la.EncounterMatch;
        return GetMaximumEggHatchCycles(pk, enc);
    }

    /// <summary>
    /// Gets the original Hatch Cycle value for an Egg Entity.
    /// </summary>
    /// <param name="pk">Egg Entity</param>
    /// <param name="enc">Encounter the egg was generated from</param>
    /// <returns>Maximum value the Hatch Counter can be.</returns>
    public static int GetMaximumEggHatchCycles(PKM pk, IEncounterTemplate enc)
    {
        if (enc is IHatchCycle { EggCycles: not 0 } s)
            return s.EggCycles;
        return pk.PersonalInfo.HatchCycles;
    }

    /// <summary>
    /// Level which eggs are given to the player.
    /// </summary>
    /// <param name="generation">Generation the egg is given in</param>
    public static byte GetEggLevel(byte generation) => generation >= 4 ? (byte)1 : (byte)5;

    public const byte EggMetLevel34 = 0;
    public const byte EggMetLevel = 1;

    /// <summary>
    /// Met Level which eggs are given to the player. May change if transferred to future games.
    /// </summary>
    /// <param name="version">Game the egg is obtained in</param>
    /// <param name="generation">Generation the egg is given in</param>
    public static byte GetEggLevelMet(GameVersion version, byte generation) => generation switch
    {
        2 => version is C ? EggMetLevel : (byte)0, // GS do not store met data
        3 or 4 => EggMetLevel34,
        _ => EggMetLevel,
    };

    /// <summary>
    /// Checks if the <see cref="PKM.HandlingTrainerName"/> and associated details can be set for the provided egg <see cref="pk"/>.
    /// </summary>
    /// <param name="pk">Egg Entity</param>
    /// <returns>True if valid, false if invalid.</returns>
    public static bool IsValidHTEgg(PKM pk) => pk switch
    {
        PB8 { MetLocation: Locations.LinkTrade6NPC } pb8 when pb8.HandlingTrainerFriendship == PersonalTable.BDSP[pb8.Species].BaseFriendship => true,
        PK9 { MetLocation: Locations.LinkTrade6, HandlingTrainerLanguage: not 0 } => true, // fine regardless of handler (trade-back)
        _ => false,
    };

    /// <summary>
    /// Gets a suggested Version for a hatched egg that originally lacked a Version value.
    /// </summary>
    /// <param name="pk">Egg Entity</param>
    /// <param name="version">Potential version the egg was hatched in</param>
    /// <returns>Very roughly sanitized version the egg was hatched in.</returns>
    public static GameVersion GetEggHatchVersion(PKM pk, GameVersion version) => pk switch
    {
        PK9 => version is SL or VL ? version : SL,
        _ => version,
    };

    /// <summary>
    /// Indicates if the <see cref="PKM.IsNicknamed"/> flag should be set for an Egg entity.
    /// </summary>
    /// <param name="enc">Encounter the egg was generated with</param>
    /// <param name="pk">Egg Entity</param>
    /// <returns>True if the <see cref="PKM.IsNicknamed"/> flag should be set, otherwise false.</returns>
    public static bool IsNicknameFlagSet(IEncounterTemplate enc, PKM pk) => enc switch
    {
        EncounterStatic7 => false,
        WB8 or EncounterStatic8b when pk.IsUntraded => false,
        EncounterStatic9 { EggLocation: 60005 } => false, // Jacq Egg does not have flag set!
        { Generation: 4 } => false,
        _ => true,
    };

    /// <inheritdoc cref="IsNicknameFlagSet(IEncounterTemplate,PKM)"/>
    public static bool IsNicknameFlagSet(PKM pk) => IsNicknameFlagSet(new LegalityAnalysis(pk).EncounterMatch, pk);

    /// <summary>
    /// Gets a valid <see cref="PKM.MetLocation"/> for an egg hatched in the origin game, accounting for future format transfers altering the data.
    /// </summary>
    public static ushort GetEggHatchLocation(GameVersion game, byte format) => game switch
    {
        R or S or E or FR or LG => format switch
        {
            3 => game is FR or LG ? Locations.HatchLocationFRLG : Locations.HatchLocationRSE,
            4 => Locations.Transfer3, // Pal Park
            _ => Locations.Transfer4,
        },

        D or P or Pt => format > 4 ? Locations.Transfer4 : Locations.HatchLocationDPPt,
        HG or SS => format > 4 ? Locations.Transfer4 : Locations.HatchLocationHGSS,

        B or W or B2 or W2 => Locations.HatchLocation5,

        X or Y => Locations.HatchLocation6XY,
        AS or OR => Locations.HatchLocation6AO,
        SN or MN or US or UM => Locations.HatchLocation7,

        GSC or C when format <= 2 => Locations.HatchLocationC,
        RD or BU or GN or YW => Locations.Transfer1,
        GD or SI or C => Locations.Transfer2,

        SW or SH => Locations.HatchLocation8,
        BD or SP => Locations.HatchLocation8b,

        SL or VL => Locations.HatchLocation9,
        _ => 0,
    };

    /// <summary>
    /// Gets the initial friendship value for an egg when it is hatched.
    /// </summary>
    public static byte GetEggHatchFriendship(EntityContext context) => context switch
    {
        // From Gen2->Gen7, the value was 120.
        EntityContext.Gen2 => 120,
        EntityContext.Gen3 => 120,
        EntityContext.Gen4 => 120,
        EntityContext.Gen5 => 120,
        EntityContext.Gen6 => 120,
        EntityContext.Gen7 => 120,
        // No eggs in LGP/E.

        // Starting in SW/SH, Friendship was rescaled away from 255 (to 160-ish), so the value is lower than prior.
        _ => 100,
    };

    /// <summary>
    /// Reasonable value for the friendship of an egg when it is hatched.
    /// </summary>
    /// <remarks>Only use if you're trying to generalize a value for hatched eggs without checking context.</remarks>
    public const byte EggHatchFriendshipGeneral = 100;
}
