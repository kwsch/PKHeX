using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

/// <summary>
/// Utility class for <see cref="GameVersion"/> logic.
/// </summary>
public static class GameUtil
{
    /// <summary>
    /// All possible <see cref="GameVersion"/> values a <see cref="PKM.Version"/> can have.
    /// </summary>
    /// <remarks>Ordered roughly by most recent games first.</remarks>
    public static readonly GameVersion[] GameVersions = GetValidGameVersions();

    private static GameVersion[] GetValidGameVersions()
    {
        var all = Enum.GetValues<GameVersion>();
        var valid = Array.FindAll(all, IsValidSavedVersion);
        Array.Reverse(valid);
        return valid;
    }

    /// <summary>
    /// Indicates if the <see cref="GameVersion"/> value is a value used by the games or is an aggregate indicator.
    /// </summary>
    /// <param name="version">Game to check</param>
    public static bool IsValidSavedVersion(this GameVersion version) => version is > 0 and <= HighestGameID;

    /// <summary>
    /// Most recent game ID utilized by official games.
    /// </summary>
    internal const GameVersion HighestGameID = RB - 1;

    /// <summary>Determines the Version Grouping of an input Version ID</summary>
    /// <param name="version">Version of which to determine the group</param>
    /// <returns>Version Group Identifier or Invalid if type cannot be determined.</returns>
    public static GameVersion GetMetLocationVersionGroup(GameVersion version) => version switch
    {
        // Side games
        CXD => CXD,
        GO => GO,

        // VC Transfers
        RD or BU or YW or GN or GD or SI or C => USUM,

        // Gen2 -- PK2
        GS or GSC => GSC,

        // Gen3
        R or S => RS,
        E => E,
        FR or LG => FR,

        // Gen4
        D or P => DP,
        Pt => Pt,
        HG or SS => HGSS,

        // Gen5
        B or W => BW,
        B2 or W2 => B2W2,

        // Gen6
        X or Y => XY,
        OR or AS => ORAS,

        // Gen7
        SN or MN => SM,
        US or UM => USUM,
        GP or GE => GG,

        // Gen8
        SW or SH => SWSH,
        BD or SP => BDSP,
        PLA => PLA,

        // Gen9
        SL or VL => SV,
        _ => Invalid,
    };

    /// <summary>
    /// Gets a Version ID from the end of that Generation
    /// </summary>
    /// <param name="generation">Generation ID</param>
    /// <returns>Version ID from requested generation. If none, return <see cref="Invalid"/>.</returns>
    public static GameVersion GetVersion(byte generation) => generation switch
    {
        1 => RBY,
        2 => C,
        3 => E,
        4 => SS,
        5 => W2,
        6 => AS,
        7 => UM,
        8 => SH,
        9 => VL,
        _ => Invalid,
    };

    /// <summary>
    /// Gets the Generation the <see cref="GameVersion"/> belongs to.
    /// </summary>
    /// <param name="version">Game to retrieve the generation for</param>
    /// <returns>Generation ID</returns>
    public static byte GetGeneration(this GameVersion version)
    {
        if (version.IsValidSavedVersion())
            return version.GetGenerationFromSaved();

        if (Gen1.Contains(version)) return 1;
        if (Gen2.Contains(version)) return 2;
        if (Gen3.Contains(version)) return 3;
        if (Gen4.Contains(version)) return 4;
        if (Gen5.Contains(version)) return 5;
        if (Gen6.Contains(version)) return 6;
        if (Gen7.Contains(version)) return 7;
        if (Gen7b.Contains(version)) return 7;
        if (Gen8.Contains(version)) return 8;
        if (Gen9.Contains(version)) return 9;
        return 0;
    }

    public static byte GetGenerationFromSaved(this GameVersion version) => version switch
    {
        RD or GN or BU or YW => 1,
        GD or SI or C => 2,
        S or R or E or FR or LG or CXD => 3,
        D or P or Pt or HG or SS => 4,
        B or W or B2 or W2 => 5,
        X or Y or AS or OR => 6,
        GP or GE => 7,
        SN or MN => 7,
        US or UM => 7,
        PLA => 8,
        BD or SP => 8,
        SW or SH => 8,
        SL or VL => 9,
        _ => 0
    };

    /// <summary>
    /// Gets the Generation the <see cref="GameVersion"/> belongs to.
    /// </summary>
    /// <param name="version">Game to retrieve the generation for</param>
    /// <returns>Generation ID</returns>
    public static ushort GetMaxSpeciesID(this GameVersion version) => version switch
    {
        RD or GN or BU or YW => Legal.MaxSpeciesID_1,
        GD or SI or C        => Legal.MaxSpeciesID_2,
        S or R or E or FR or LG or CXD => Legal.MaxSpeciesID_3,
        D or P or Pt or HG or SS       => Legal.MaxSpeciesID_4,
        B or W or B2 or W2 => Legal.MaxSpeciesID_5,
        X or Y or AS or OR => Legal.MaxSpeciesID_6,
        GP or GE => Legal.MaxSpeciesID_7b,
        SN or MN => Legal.MaxSpeciesID_7,
        US or UM => Legal.MaxSpeciesID_7_USUM,
        PLA      => Legal.MaxSpeciesID_8a,
        BD or SP => Legal.MaxSpeciesID_8b,
        SW or SH => Legal.MaxSpeciesID_8,
        SL or VL => Legal.MaxSpeciesID_9,
        _ => 0
    };

    /// <summary>
    /// Checks if the <see cref="g1"/> version (or subset versions) is equivalent to <see cref="g2"/>.
    /// </summary>
    /// <param name="g1">Version (set)</param>
    /// <param name="g2">Individual version</param>
    public static bool Contains(this GameVersion g1, GameVersion g2)
    {
        if (g1 == g2 || g1 == Any)
            return true;
        if (g1.IsValidSavedVersion())
            return false;
        return g1.ContainsFromLumped(g2);
    }

    public static bool IsGen1(this GameVersion version) => version is RD or GN or BU or YW;
    public static bool IsGen2(this GameVersion version) => version is GD or SI or C;
    public static bool IsGen3(this GameVersion version) => version is S or R or E or FR or LG or CXD;
    public static bool IsGen4(this GameVersion version) => version is HG or SS or D or P or Pt;
    public static bool IsGen5(this GameVersion version) => version is W or B or W2 or B2;
    public static bool IsGen6(this GameVersion version) => version is X or Y or AS or OR;
    public static bool IsGen7(this GameVersion version) => version is SN or MN or US or UM;
    public static bool IsGen7b(this GameVersion version) => version is GP or GE;
    public static bool IsGen8(this GameVersion version) => version is SW or SH or PLA or BD or SP;
    public static bool IsGen9(this GameVersion version) => version is SL or VL;

    /// <summary>
    /// Checks if the <see cref="lump"/> version is the lump of the requested saved <see cref="version"/>.
    /// </summary>
    public static bool ContainsFromLumped(this GameVersion lump, GameVersion version) => lump switch
    {
        RB       => version is RD or BU or GN,
        RBY      => version is RD or BU or GN or YW or RB,
        Stadium  => version is RD or BU or GN or YW or RB or RBY,
        StadiumJ => version is RD or BU or GN or YW or RB or RBY,
        Gen1     => version is RD or BU or GN or YW or RB or RBY or Stadium,

        GS       => version is GD or SI,
        GSC      => version is GD or SI or C or GS,
        Stadium2 => version is GD or SI or C or GS or GSC,
        Gen2     => version is GD or SI or C or GS or GSC or Stadium2,

        RS     => version is R or S,
        RSE    => version is R or S or E or RS,
        FRLG   => version is FR or LG,
        EFL    => version is E or FR or LG,
        RSBOX  => version is R or S or E or FR or LG,
        Gen3   => version is R or S or E or FR or LG or CXD or RSBOX or RS or RSE or FRLG,
        COLO   => version is CXD,
        XD     => version is CXD,

        DP     => version is D or P,
        HGSS   => version is HG or SS,
        DPPt   => version is D or P or Pt or DP,
        Gen4   => version is D or P or Pt or HG or SS or BATREV or DP or HGSS or DPPt,

        BW     => version is B or W,
        B2W2   => version is B2 or W2,
        Gen5   => version is B or W or B2 or W2 or BW or B2W2,

        XY     => version is X or Y,
        ORAS   => version is OR or AS,
        Gen6   => version is X or Y or OR or AS or XY or ORAS,

        SM     => version is SN or MN,
        USUM   => version is US or UM,
        Gen7   => version is SN or MN or US or UM or SM or USUM,

        GG     => version is GP or GE,
        Gen7b  => version is GP or GE or GO or GG,

        SWSH   => version is SW or SH,
        BDSP   => version is BD or SP,
        Gen8   => version is SW or SH or BD or SP or SWSH or BDSP or PLA,

        SV     => version is SL or VL,
        Gen9   => version is SL or VL or SV,

        _      => false,
    };

    /// <summary>
    /// List of possible <see cref="GameVersion"/> values within the provided <see cref="generation"/>.
    /// </summary>
    /// <param name="generation">Generation to look within</param>
    /// <param name="version">Entity version</param>
    public static GameVersion[] GetVersionsInGeneration(byte generation, GameVersion version)
    {
        if (Gen7b.Contains(version))
            return [GO, GP, GE];
        return Array.FindAll(GameVersions, z => z.GetGeneration() == generation);
    }

    /// <summary>
    /// List of possible <see cref="GameVersion"/> values within the provided <see cref="IGameValueLimit"/> criteria.
    /// </summary>
    /// <param name="obj">Criteria for retrieving versions</param>
    /// <param name="generation">Generation format minimum (necessary for the CXD/Gen4 swap etc.)</param>
    public static IEnumerable<GameVersion> GetVersionsWithinRange(IGameValueLimit obj, byte generation = 0)
    {
        var max = obj.MaxGameID;
        if (max == Legal.MaxGameID_7b) // edge case
            return [GO, GP, GE];
        var versions = GameVersions
            .Where(version => obj.MinGameID <= version && version <= max);
        if (max != BATREV)
            versions = versions.Where(static version => version != BATREV);
        if (generation == 0)
            return versions;
        if (max == Legal.MaxGameID_7 && generation == 7)
            versions = versions.Where(static version => version != GO);

        // HOME allows up-reach to Gen9
        if (generation >= 8)
            generation = 9;
        return versions.Where(version => version.GetGeneration() <= generation);
    }
}
