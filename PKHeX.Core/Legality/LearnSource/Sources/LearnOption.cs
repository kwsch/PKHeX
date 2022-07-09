namespace PKHeX.Core;

public enum LearnOption
{
    /// <summary>
    /// Checks with rules assuming the move is in the current moveset.
    /// </summary>
    Current,

    /// <summary>
    /// Checks with rules assuming the move was known while present in the game.
    /// </summary>
    /// <remarks>
    /// Only relevant for memory checks. For not-transferable moves like Gen4/5 HM moves, no.
    /// </remarks>
    AtAnyTime,
}
