using System;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

/// <summary>
/// Provides access to game-specific data for Personal and LearnSource.
/// </summary>
public static class GameData
{
    /// <summary>
    /// Gets the Personal table for the specified game version.
    /// </summary>
    /// <param name="game">The game version to retrieve data for.</param>
    /// <returns>The Personal table for the specified game version.</returns>
    public static IPersonalTable GetPersonal(GameVersion game) => Personal(game);

    /// <summary>
    /// Gets the LearnSource for the specified game version.
    /// </summary>
    /// <param name="game">The game version to retrieve data for.</param>
    /// <returns>The LearnSource for the specified game version.</returns>
    public static ILearnSource GetLearnSource(GameVersion game) => game switch
    {
        RD or GN or BU or RB => LearnSource1RB.Instance,
        YW or RBY => LearnSource1YW.Instance,
        GD or SI or GS => LearnSource2GS.Instance,
        C or GSC => LearnSource2C.Instance,

        R or S or RS or RSE => LearnSource3RS.Instance,
        E or COLO or XD or FRLG or CXD => LearnSource3E.Instance,
        FR => LearnSource3FR.Instance,
        LG => LearnSource3LG.Instance,

        D or P or DP => LearnSource4DP.Instance,
        Pt or DPPt => LearnSource4Pt.Instance,
        HG or SS or HGSS => LearnSource4HGSS.Instance,

        B or W or BW => LearnSource5BW.Instance,
        B2 or W2 or B2W2 => LearnSource5B2W2.Instance,

        X or Y or XY => LearnSource6XY.Instance,
        AS or OR or ORAS => LearnSource6AO.Instance,

        SN or MN or SM => LearnSource7SM.Instance,
        US or UM or USUM => LearnSource7USUM.Instance,
        GO or GP or GE or GG => LearnSource7GG.Instance,

        SW or SH or SWSH => LearnSource8SWSH.Instance,
        BD or SP or BDSP => LearnSource8BDSP.Instance,
        PLA => LearnSource8LA.Instance,

        SL or VL or SV => LearnSource9SV.Instance,

        Gen1 => LearnSource1YW.Instance,
        Gen2 => LearnSource2C.Instance,
        Gen3 => LearnSource3E.Instance,
        Gen4 => LearnSource4HGSS.Instance,
        Gen5 => LearnSource5B2W2.Instance,
        Gen6 => LearnSource6AO.Instance,
        Gen7 => LearnSource7USUM.Instance,
        Gen7b => LearnSource7GG.Instance,
        Gen8 => LearnSource8BDSP.Instance,
        Gen9 => LearnSource9SV.Instance,

        Stadium => LearnSource1YW.Instance,
        Stadium2 => LearnSource2GS.Instance,

        _ => throw new ArgumentOutOfRangeException(nameof(game), $"{game} is not a valid entry in the expression."),
    };

    /// <summary>
    /// Retrieves the personal table for the specified game version.
    /// </summary>
    /// <param name="game">The game version to retrieve data for.</param>
    /// <returns>The Personal table of the specified game version.</returns>
    private static IPersonalTable Personal(GameVersion game) => game switch
    {
        RD or GN or BU or RB => PersonalTable.RB,
        YW or RBY => PersonalTable.Y,
        GD or SI or GS => PersonalTable.GS,
        C or GSC => PersonalTable.C,

        R or S or RS or RSE => PersonalTable.RS,
        E or COLO or XD or FRLG or CXD => PersonalTable.E,
        FR => PersonalTable.FR,
        LG => PersonalTable.LG,

        D or P or DP => PersonalTable.DP,
        Pt or DPPt => PersonalTable.Pt,
        HG or SS or HGSS => PersonalTable.HGSS,

        B or W or BW => PersonalTable.BW,
        B2 or W2 or B2W2 => PersonalTable.B2W2,

        X or Y or XY => PersonalTable.XY,
        AS or OR or ORAS => PersonalTable.AO,

        SN or MN or SM => PersonalTable.SM,
        US or UM or USUM => PersonalTable.USUM,
        GO or GP or GE or GG => PersonalTable.GG,

        SW or SH or SWSH => PersonalTable.SWSH,
        BD or SP or BDSP => PersonalTable.BDSP,
        PLA => PersonalTable.LA,

        SL or VL or SV => PersonalTable.SV,

        Gen1 => PersonalTable.Y,
        Gen2 => PersonalTable.C,
        Gen3 => PersonalTable.E,
        Gen4 => PersonalTable.HGSS,
        Gen5 => PersonalTable.B2W2,
        Gen6 => PersonalTable.AO,
        Gen7 => PersonalTable.USUM,
        Gen7b => PersonalTable.GG,
        Gen8 => PersonalTable.SWSH,

        Stadium => PersonalTable.Y,
        Stadium2 => PersonalTable.GS,

        _ => throw new ArgumentOutOfRangeException(nameof(game), $"{game} is not a valid entry in the expression."),
    };
}
