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

/// <summary>
/// Extension methods for <see cref="LearnMethod"/>.
/// </summary>
public static class LearnMethodExtensions
{
    /// <summary>
    /// Checks if the <see cref="LearnMethod"/> is a valid method of learning a move.
    /// </summary>
    /// <param name="method">Method to check</param>
    /// <returns>True if the method is valid, false otherwise.</returns>
    public static bool IsValid(this LearnMethod method) => method >= Empty;

    /// <summary>
    /// Checks if the <see cref="LearnMethod"/> is expecting another move instead.
    /// </summary>
    /// <param name="method">Method to check</param>
    /// <returns>True if the method is valid, false otherwise.</returns>
    public static bool HasExpectedMove(this LearnMethod method) => method is UnobtainableExpect;

    /// <summary>
    /// Checks if the <see cref="LearnMethod"/> is valid because of it being a Relearn move.
    /// </summary>
    /// <param name="method">Method to check</param>
    /// <returns>True if the method is valid, false otherwise.</returns>
    public static bool IsRelearn(this LearnMethod method) => method is Relearn;

    /// <summary>
    /// Checks if the <see cref="LearnMethod"/> is valid because of it being an egg move.
    /// </summary>
    /// <param name="method">Method to check</param>
    /// <returns>True if the method is valid, false otherwise.</returns>
    public static bool IsEggSource(this LearnMethod method) => method is EggMove or InheritLevelUp or SpecialEgg;
}
