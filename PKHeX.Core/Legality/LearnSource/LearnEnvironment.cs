using System;

namespace PKHeX.Core;
using static LearnEnvironment;

/// <summary>
/// Indicates the group of game(s) that the move was learned in.
/// </summary>
public enum LearnEnvironment : byte
{
    None,

    /* Gen1 */ RB, YW,
    /* Gen2 */ GS, C,
    /* Gen3 */ RS, E, FR, LG,
    /* Gen4 */ DP, Pt, HGSS,
    /* Gen5 */ BW, B2W2,
    /* Gen6 */ XY, ORAS,
    /* Gen7 */ SM, USUM, GG,
    /* Gen8 */ SWSH, BDSP, PLA,
    /* Gen9 */ SV,
    HOME,
}

/// <summary>
/// Extension methods for <see cref="LearnEnvironment"/>.
/// </summary>
public static class LearnEnvironmentExtensions
{
    public static bool IsSpecified(this LearnEnvironment value) => value is not None;
    public static byte GetGeneration(this LearnEnvironment value) => value switch
    {
        RB or YW => 1,
        GS or C => 2,
        RS or E or FR or LG => 3,
        DP or Pt or HGSS => 4,
        BW or B2W2 => 5,
        XY or ORAS => 6,
        SM or USUM or GG => 7,
        SWSH or BDSP or PLA => 8,
        SV => 9,
        _ => 0,
    };

    public static ReadOnlySpan<EvoCriteria> GetEvolutions(this LearnEnvironment value, EvolutionHistory history) => value switch
    {
        RB or YW => history.Gen1,
        GS or C => history.Gen2,
        RS or E or FR or LG => history.Gen3,
        DP or Pt or HGSS => history.Gen4,
        BW or B2W2 => history.Gen5,
        XY or ORAS => history.Gen6,
        SM or USUM => history.Gen7,
        GG => history.Gen7b,
        SWSH => history.Gen8,
        PLA => history.Gen8a,
        BDSP => history.Gen8b,
        SV => history.Gen9,
        _ => [],
    };
}
