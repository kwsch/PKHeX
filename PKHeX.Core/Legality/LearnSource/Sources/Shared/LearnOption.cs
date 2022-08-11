namespace PKHeX.Core;

/// <summary>
/// Option for checking how a move may be learned.
/// </summary>
public enum LearnOption
{
    /// <summary>
    /// Checks with rules assuming the move is in the current moveset.
    /// </summary>
    Current,

    /// <summary>
    /// Checks with rules assuming the move was known at any time while existing inside the game source it is being checked in.
    /// </summary>
    /// <remarks>
    /// Only relevant for memory checks.
    /// For not-transferable moves like Gen4/5 HM moves, no -- there's no point in checking them as they aren't requisites for anything.
    /// Evolution criteria is handled separately.
    /// </remarks>
    AtAnyTime,
}
