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
        ZA => ZA,

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

    extension(GameVersion version)
    {
        /// <summary>
        /// Gets the Generation the <see cref="GameVersion"/> belongs to.
        /// </summary>
        /// <returns>Generation ID</returns>
        public byte Generation => version.GetContextInternal().Generation;

        private EntityContext GetContextInternal()
        {
            if (version.IsValidSavedVersion())
                return version.GetContextFromSaved();

            if (Gen1.Contains(version)) return EntityContext.Gen1;
            if (Gen2.Contains(version)) return EntityContext.Gen2;
            if (Gen3.Contains(version)) return EntityContext.Gen3;
            if (Gen4.Contains(version)) return EntityContext.Gen4;
            if (Gen5.Contains(version)) return EntityContext.Gen5;
            if (Gen6.Contains(version)) return EntityContext.Gen6;
            if (Gen7.Contains(version)) return EntityContext.Gen7;
            if (Gen7b.Contains(version)) return EntityContext.Gen7b;
            if (SWSH.Contains(version)) return EntityContext.Gen8;
            if (BDSP.Contains(version)) return EntityContext.Gen8b;
            if (SV.Contains(version)) return EntityContext.Gen9;
            return 0;
        }

        /// <summary>
        /// Indicates if the <see cref="GameVersion"/> value is a value used by the games or is an aggregate indicator.
        /// </summary>
        public bool IsValidSavedVersion() => version is > 0 and <= HighestGameID;

        public EntityContext GetContextFromSaved() => version switch
        {
            S or R or E or FR or LG or CXD => EntityContext.Gen3,
            D or P or Pt or HG or SS or BATREV => EntityContext.Gen4,
            B or W or B2 or W2 => EntityContext.Gen5,
            X or Y or AS or OR => EntityContext.Gen6,
            SN or MN or US or UM => EntityContext.Gen7,
            RD or GN or BU or YW => EntityContext.Gen1,
            GD or SI or C => EntityContext.Gen2,
            GP or GE => EntityContext.Gen7b,
            PLA => EntityContext.Gen8a,
            BD or SP => EntityContext.Gen8b,
            SW or SH => EntityContext.Gen8,
            SL or VL => EntityContext.Gen9,
            ZA => EntityContext.Gen9a,
            _ => 0
        };

        /// <summary>
        /// Gets the Generation the <see cref="GameVersion"/> belongs to.
        /// </summary>
        /// <returns>Generation ID</returns>
        public ushort GetMaxSpeciesID() => version switch
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
            ZA       => Legal.MaxSpeciesID_9a,
            _ => 0
        };

        /// <summary>
        /// Checks if the <see cref="version"/> version (or subset versions) is equivalent to <see cref="g2"/>.
        /// </summary>
        /// <param name="g2">Individual version</param>
        public bool Contains(GameVersion g2)
        {
            if (version == g2 || version == Any)
                return true;
            if (version.IsValidSavedVersion())
                return false;
            return version.ContainsFromLumped(g2);
        }

        public bool IsGen1() => version is RD or GN or BU or YW;
        public bool IsGen2() => version is GD or SI or C;
        public bool IsGen3() => version is S or R or E or FR or LG or CXD;
        public bool IsGen4() => version is HG or SS or D or P or Pt;
        public bool IsGen5() => version is W or B or W2 or B2;
        public bool IsGen6() => version is X or Y or AS or OR;
        public bool IsGen7() => version is SN or MN or US or UM;
        public bool IsGen7b() => version is GP or GE;
        public bool IsGen8() => version is SW or SH or PLA or BD or SP;
        public bool IsGen9() => version is SL or VL or ZA;

        /// <summary>
        /// Checks if the <see cref="version"/> version is the lump of the requested saved <see cref="version1"/>.
        /// </summary>
        public bool ContainsFromLumped(GameVersion version1) => version switch
        {
            RB       => version1 is RD or BU or GN,
            RBY      => version1 is RD or BU or GN or YW or RB,
            Stadium  => version1 is RD or BU or GN or YW or RB or RBY,
            StadiumJ => version1 is RD or BU or GN or YW or RB or RBY,
            Gen1     => version1 is RD or BU or GN or YW or RB or RBY or Stadium,

            GS       => version1 is GD or SI,
            GSC      => version1 is GD or SI or C or GS,
            Stadium2 => version1 is GD or SI or C or GS or GSC,
            Gen2     => version1 is GD or SI or C or GS or GSC or Stadium2,

            RS     => version1 is R or S,
            RSE    => version1 is R or S or E or RS,
            FRLG   => version1 is FR or LG,
            EFL    => version1 is E or FR or LG,
            RSBOX  => version1 is R or S or E or FR or LG,
            Gen3   => version1 is R or S or E or FR or LG or CXD or RSBOX or RS or RSE or FRLG,
            COLO   => version1 is CXD,
            XD     => version1 is CXD,

            DP     => version1 is D or P,
            HGSS   => version1 is HG or SS,
            DPPt   => version1 is D or P or Pt or DP,
            Gen4   => version1 is D or P or Pt or HG or SS or BATREV or DP or HGSS or DPPt,

            BW     => version1 is B or W,
            B2W2   => version1 is B2 or W2,
            Gen5   => version1 is B or W or B2 or W2 or BW or B2W2,

            XY     => version1 is X or Y,
            ORAS   => version1 is OR or AS,
            Gen6   => version1 is X or Y or OR or AS or XY or ORAS,

            SM     => version1 is SN or MN,
            USUM   => version1 is US or UM,
            Gen7   => version1 is SN or MN or US or UM or SM or USUM,

            GG     => version1 is GP or GE,
            Gen7b  => version1 is GP or GE or GO or GG,

            SWSH   => version1 is SW or SH,
            BDSP   => version1 is BD or SP,
            Gen8   => version1 is SW or SH or BD or SP or SWSH or BDSP or PLA,

            SV     => version1 is SL or VL,
            Gen9   => version1 is SL or VL or SV or ZA,

            _      => false,
        };
    }

    /// <summary>
    /// List of possible <see cref="GameVersion"/> values within the provided <see cref="generation"/>.
    /// </summary>
    /// <param name="generation">Generation to look within</param>
    /// <param name="version">Entity version</param>
    public static GameVersion[] GetVersionsInGeneration(byte generation, GameVersion version)
    {
        if (Gen7b.Contains(version))
            return [GO, GP, GE];
        return Array.FindAll(GameVersions, z => z.Generation == generation);
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
        return versions.Where(version => version.Generation <= generation);
    }
}
