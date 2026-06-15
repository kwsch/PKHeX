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

    /// <summary>
    /// Check backwards of knowing moves within any game in the visitation chain.
    /// </summary>
    /// <remarks>
    /// Relevant for Evolution move sanity checks, where the move could have been picked up at any point in the game visitation chain.
    /// </remarks>
    AtAnyTimeChain,
}

public static class LearnOptionExtensions
{
    extension(LearnOption option)
    {
        public bool IsCurrent() => option == LearnOption.Current;
        public bool IsPast() => option is LearnOption.AtAnyTime or LearnOption.HOME or LearnOption.AtAnyTimeChain;
        public bool IsFlagCheckRequired() => option != LearnOption.HOME;
    }
}
