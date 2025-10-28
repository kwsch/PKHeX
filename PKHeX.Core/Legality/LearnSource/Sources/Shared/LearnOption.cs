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

    /// <summary>
    /// Checks with rules assuming the move was taught via HOME -- for sharing acquired movesets between games.
    /// </summary>
    /// <remarks>
    /// Relevant for HOME sharing sanity checks.
    /// Required to be distinct in that the rules are different from the other two options. TR/TM flags aren't required if the move was learned via HOME.
    /// </remarks>
    HOME,
}

public static class LearnOptionExtensions
{
    public static bool IsCurrent(this LearnOption option) => option == LearnOption.Current;
    public static bool IsPast(this LearnOption option) => option is LearnOption.AtAnyTime or LearnOption.HOME;
    public static bool IsFlagCheckRequired(this LearnOption option) => option != LearnOption.HOME;
}
