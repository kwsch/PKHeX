namespace PKHeX.Core;

/// <summary>
/// Rules for how <see cref="IContestStatsReadOnly"/> are acquired.
/// </summary>
/// <seealso cref="ContestStatGrantingSheen"/>
public enum ContestStatGranting
{
    /// <summary> Not possible to get any contest stats. </summary>
    None,
    /// <summary> Contest stats are possible to obtain, but must be correlated to sheen at most 1:1. </summary>
    CorrelateSheen,
    /// <summary> Contest stats are possible to obtain, but cannot obtain any sheen value. </summary>
    NoSheen,
    /// <summary> Contest stats are possible to obtain, and has visited a multitude of games such that any value of sheen is possible. </summary>
    Mixed,
}
