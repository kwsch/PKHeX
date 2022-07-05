namespace PKHeX.Core;
using static LearnMethod;

/// <summary>
/// Indicates the method of learning a move
/// </summary>
public enum LearnMethod : byte
{
    // Invalid
    None,
    Unobtainable,
    Duplicate,
    EmptyInvalid,

    // Valid
    Empty,
    Relearn,
    Initial,
    LevelUp,
    TMHM,
    Tutor,
    Sketch,
    EggMove,
    InheritLevelUp,
    Special,
    SpecialEgg,
    ShedinjaEvo,
    Shared,
}

public static class LearnMethodExtensions
{
    public static bool IsValid(this LearnMethod method) => method >= Empty;
}
