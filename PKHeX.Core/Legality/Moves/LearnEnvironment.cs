namespace PKHeX.Core;
using static LearnEnvironment;

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
    
    Any = byte.MaxValue,
}

public static class LearnEnvironmentExtensions
{
    public static bool IsSpecified(this LearnEnvironment value) => value is not (None or Any);
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
        _ => 0,
    };
}
