namespace PKHeX.Core;

/// <summary>
/// Indicates the method of learning a move
/// </summary>
public enum LearnMethod : byte
{
    None,
    Unobtainable,
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
