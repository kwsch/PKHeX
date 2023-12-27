namespace PKHeX.Core;

/// <summary>
/// Exposes contest stat value retrieval.
/// </summary>
public interface IContestStatsReadOnly
{
    byte CNT_Cool { get; }
    byte CNT_Beauty { get; }
    byte CNT_Cute { get; }
    byte CNT_Smart { get; }
    byte CNT_Tough { get; }
    byte CNT_Sheen { get; }
}

public static partial class Extensions
{
    /// <summary>
    /// Checks if any contest stat value is nonzero.
    /// </summary>
    /// <param name="stats">Object containing contest stat data.</param>
    /// <returns>True if it has any nonzero contest stat, false if all are zero.</returns>
    public static bool HasContestStats(this IContestStatsReadOnly stats)
    {
        if (stats.CNT_Cool != 0)
            return true;
        if (stats.CNT_Beauty != 0)
            return true;
        if (stats.CNT_Cute != 0)
            return true;
        if (stats.CNT_Smart != 0)
            return true;
        if (stats.CNT_Tough != 0)
            return true;
        if (stats.CNT_Sheen != 0)
            return true;
        return false;
    }

    public static bool IsContestBelow(this IContestStatsReadOnly current, IContestStatsReadOnly initial) => !current.IsContestAboveOrEqual(initial);

    public static bool IsContestAboveOrEqual(this IContestStatsReadOnly current, IContestStatsReadOnly initial)
    {
        if (current.CNT_Cool   < initial.CNT_Cool)
            return false;
        if (current.CNT_Beauty < initial.CNT_Beauty)
            return false;
        if (current.CNT_Cute   < initial.CNT_Cute)
            return false;
        if (current.CNT_Smart  < initial.CNT_Smart)
            return false;
        if (current.CNT_Tough  < initial.CNT_Tough)
            return false;
        if (current.CNT_Sheen  < initial.CNT_Sheen)
            return false;
        return true;
    }

    public static bool IsContestEqual(this IContestStatsReadOnly current, IContestStatsReadOnly initial)
    {
        if (current.CNT_Cool != initial.CNT_Cool)
            return false;
        if (current.CNT_Beauty != initial.CNT_Beauty)
            return false;
        if (current.CNT_Cute != initial.CNT_Cute)
            return false;
        if (current.CNT_Smart != initial.CNT_Smart)
            return false;
        if (current.CNT_Tough != initial.CNT_Tough)
            return false;
        if (current.CNT_Sheen != initial.CNT_Sheen)
            return false;
        return true;
    }

    public static void CopyContestStatsTo(this IContestStatsReadOnly source, IContestStats dest)
    {
        dest.CNT_Cool = source.CNT_Cool;
        dest.CNT_Beauty = source.CNT_Beauty;
        dest.CNT_Cute = source.CNT_Cute;
        dest.CNT_Smart = source.CNT_Smart;
        dest.CNT_Tough = source.CNT_Tough;
        dest.CNT_Sheen = source.CNT_Sheen;
    }

    public static void SetAllContestStatsTo(this IContestStats dest, byte value, byte sheen)
    {
        dest.CNT_Cool = value;
        dest.CNT_Beauty = value;
        dest.CNT_Cute = value;
        dest.CNT_Smart = value;
        dest.CNT_Tough = value;
        dest.CNT_Sheen = sheen;
    }

    private const byte CONTEST_MAX = 255;

    /// <summary>
    /// Check if any contest stat besides <see cref="IContestStatsReadOnly.CNT_Sheen"/> is equal to <see cref="CONTEST_MAX"/>.
    /// </summary>
    /// <param name="s">Entity to check</param>
    /// <returns>True if any equals <see cref="CONTEST_MAX"/></returns>
    public static bool IsAnyContestStatMax(this IContestStatsReadOnly s) => CONTEST_MAX == s.CNT_Cool
                                                                         || CONTEST_MAX == s.CNT_Beauty
                                                                         || CONTEST_MAX == s.CNT_Cute
                                                                         || CONTEST_MAX == s.CNT_Smart
                                                                         || CONTEST_MAX == s.CNT_Tough;
}
