using System;
using static PKHeX.Core.SaveFileType;

namespace PKHeX.Core;

/// <summary>
/// Enumerates the possible save file types that can be detected and loaded.
/// </summary>
public enum SaveFileType : byte
{
    /// <summary>
    /// Invalid or unrecognized save file type.
    /// </summary>
    None = 0,

    // Main Games
    RBY,
    GSC,
    RS,
    Emerald,
    FRLG,
    DP,
    Pt,
    HGSS,
    BW,
    B2W2,
    XY,
    AO,
    AODemo,
    SM,
    USUM,
    LGPE,
    SWSH,
    BDSP,
    LA,
    SV,

    // Side Games
    Colosseum,
    XD,
    RSBox,
    BattleRevolution,
    Ranch,
    Stadium1J,
    Stadium1,
    Stadium2,

    // Bulk Storage
    Bulk3,
    Bulk4,
    Bulk7,
}

public static class SaveFileTypeExtensions
{
    /// <summary>
    /// Maps a stored <see cref="GameVersion"/> to a <see cref="SaveFileType"/>.
    /// </summary>
    /// <param name="version">Actual game version to map</param>
    /// <returns>Corresponding <see cref="SaveFileType"/> for the given <paramref name="version"/>.</returns>
    public static SaveFileType GetSaveFileType(this GameVersion version) => version switch
    {
        > GameVersion.VL => 0,
        GameVersion.RD or GameVersion.GN or GameVersion.YW or GameVersion.BU => RBY,
        GameVersion.GD or GameVersion.SI or GameVersion.C => GSC,
        GameVersion.R or GameVersion.S => RS,
        GameVersion.E => Emerald,
        GameVersion.FR or GameVersion.LG => FRLG,
        GameVersion.CXD => XD,
        GameVersion.D or GameVersion.P => DP,
        GameVersion.Pt => Pt,
        GameVersion.HG or GameVersion.SS => HGSS,
        GameVersion.BATREV => BattleRevolution,
        GameVersion.B or GameVersion.W => BW,
        GameVersion.B2 or GameVersion.W2 => B2W2,
        GameVersion.X or GameVersion.Y => XY,
        GameVersion.AS or GameVersion.OR => AO,
        GameVersion.SN or GameVersion.MN => SM,
        GameVersion.US or GameVersion.UM => USUM,
        GameVersion.GP or GameVersion.GE => LGPE,
        GameVersion.SW or GameVersion.SH => SWSH,
        GameVersion.BD or GameVersion.SP => BDSP,
        GameVersion.PLA => LA,
        GameVersion.SL or GameVersion.VL => SV,
        _ => None,
    };

    public static byte GetGeneration(this SaveFileType type) => type switch
    {
        0 => 0,
        RBY or Stadium1J or Stadium1 => 1,
        GSC or Stadium2 => 2,
        RS or Emerald or FRLG => 3,
        DP or Pt or HGSS => 4,
        BW or B2W2 => 5,
        XY or AO or AODemo => 6,
        SM or USUM or LGPE => 7,
        SWSH or BDSP or LA => 8,
        SV => 9,
        Colosseum or XD or RSBox => 3,
        Ranch or BattleRevolution => 4,
        Bulk3 => 3,
        Bulk4 => 4,
        Bulk7 => 7,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };
}
