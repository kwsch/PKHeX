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
    ZA,

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
    extension(GameVersion version)
    {
        /// <summary>
        /// Maps a stored <see cref="GameVersion"/> to a <see cref="Core.SaveFileType"/>.
        /// </summary>
        /// <returns>Corresponding <see cref="Core.SaveFileType"/> for the given <paramref name="version"/>.</returns>
        public SaveFileType SaveFileType => version switch
        {
            > GameUtil.HighestGameID => 0,
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
            GameVersion.ZA => ZA,
            _ => None,
        };
    }

    extension(SaveFileType type)
    {
        public EntityContext Context => type switch
        {
            RBY or Stadium1J or Stadium1 => EntityContext.Gen1,
            GSC or Stadium2 => EntityContext.Gen2,
            RS or Emerald or FRLG => EntityContext.Gen3,
            DP or Pt or HGSS => EntityContext.Gen4,
            BW or B2W2 => EntityContext.Gen5,
            XY or AO or AODemo => EntityContext.Gen6,
            SM or USUM => EntityContext.Gen7,
            LGPE => EntityContext.Gen7b,
            SWSH => EntityContext.Gen8,
            LA => EntityContext.Gen8a,
            BDSP => EntityContext.Gen8b,
            SV => EntityContext.Gen9,
            ZA => EntityContext.Gen9a,

            Colosseum or XD or RSBox => EntityContext.Gen3,
            Ranch or BattleRevolution => EntityContext.Gen4,
            Bulk3 => EntityContext.Gen3,
            Bulk4 => EntityContext.Gen4,
            Bulk7 => EntityContext.Gen7,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };

        public byte Generation => type.Context.Generation;
    }
}
