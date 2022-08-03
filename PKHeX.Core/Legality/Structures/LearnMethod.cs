namespace PKHeX.Core;
using static LearnMethod;

/// <summary>
/// Indicates the method of learning a move
/// </summary>
public enum LearnMethod : byte
{
    // Invalid
    None,
    Duplicate,
    EmptyInvalid,

    Unobtainable,
    UnobtainableExpect,

    // Valid
    Empty,
    Initial,
    LevelUp,
    TMHM,
    Tutor,
    Sketch,
    Special,
    Shared,
    ShedinjaEvo,

    // Relearn
    Relearn,

    // Egg
    EggMove,
    InheritLevelUp,
    SpecialEgg,
}

public static class LearnMethodExtensions
{
    public static bool IsValid(this LearnMethod method) => method >= Empty;
    public static bool HasExpectedMove(this LearnMethod method) => method is UnobtainableExpect;
    public static bool IsRelearn(this LearnMethod method) => method is Relearn;
    public static bool IsEggSource(this LearnMethod method) => method is EggMove or InheritLevelUp or SpecialEgg;
}
