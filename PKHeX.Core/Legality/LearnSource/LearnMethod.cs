namespace PKHeX.Core;
using static LearnMethod;

/// <summary>
/// Indicates the method of learning a move
/// </summary>
public enum LearnMethod : byte
{
    // Invalid

    #region Invalid
    /// <summary> Unable to detect a valid method of learning the move. </summary>
    None,
    /// <summary> Move is a duplicate of another move in the set. </summary>
    Duplicate,
    /// <summary> Move is empty but should be present. </summary>
    /// <remarks> Usually uses <see cref="UnobtainableExpect"/> to indicate what it was expecting instead. </remarks>
    EmptyInvalid,

    /// <summary> Move is unobtainable. </summary>
    Unobtainable,
    /// <summary> Expected a specific move instead of the one present. </summary>
    UnobtainableExpect,
    #endregion

    // Valid
    /// <summary> Nothing present in the slot. </summary>
    Empty,
    /// <summary> Initial Move from the species' level-up table. </summary>
    Initial,
    /// <summary> Learned via level-up. </summary>
    LevelUp,
    /// <summary> Learned via instructional machine (TM, HM, TR). </summary>
    TMHM,
    /// <summary> Learned via Move Tutor. </summary>
    Tutor,
    /// <summary> Learned by the move <see cref="Move.Sketch"/>. </summary>
    Sketch,
    /// <summary> Learned from another game's side-data via HOME. </summary>
    HOME,

    /// <summary> Added special for the Encounter. </summary>
    Encounter,
    /// <summary> Learned upon Evolution </summary>
    /// <remarks> Special case for Generation 2 where no move reminder exists. </remarks>
    Evolution,
    /// <summary> Shared from another species at a Daycare/Picnic. </summary>
    Shared,

    /// <summary> Evolution split happened immediately after leveling up and learning the move. </summary>
    /// <remarks>Only possible for Gen3 and Gen4.</remarks>
    ShedinjaEvo,

    #region Relearn
    /// <summary> Remembered from its special Relearn Move set. </summary>
    Relearn,
    #endregion

    #region Egg
    /// <summary> Inherited from the species' Egg table (not level-up). </summary>
    EggMove,
    /// <summary> Inherited from the species' level-up table. </summary>
    InheritLevelUp,
    /// <summary> Inserted under certain conditions </summary>
    /// <remarks><see cref="Move.VoltTackle"/></remarks>
    SpecialEgg,
    #endregion
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
