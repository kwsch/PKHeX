namespace PKHeX.Core;

/// <summary>
/// Exposes contest stat value retrieval.
/// </summary>
public interface IContestStatsReadOnly
{
    byte ContestCool { get; }
    byte ContestBeauty { get; }
    byte ContestCute { get; }
    byte ContestSmart { get; }
    byte ContestTough { get; }
    byte ContestSheen { get; }
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
        if (stats.ContestCool != 0)
            return true;
        if (stats.ContestBeauty != 0)
            return true;
        if (stats.ContestCute != 0)
            return true;
        if (stats.ContestSmart != 0)
            return true;
        if (stats.ContestTough != 0)
            return true;
        if (stats.ContestSheen != 0)
            return true;
        return false;
    }

    public static bool IsContestBelow(this IContestStatsReadOnly current, IContestStatsReadOnly initial) => !current.IsContestAboveOrEqual(initial);

    public static bool IsContestAboveOrEqual(this IContestStatsReadOnly current, IContestStatsReadOnly initial)
    {
        if (current.ContestCool   < initial.ContestCool)
            return false;
        if (current.ContestBeauty < initial.ContestBeauty)
            return false;
        if (current.ContestCute   < initial.ContestCute)
            return false;
        if (current.ContestSmart  < initial.ContestSmart)
            return false;
        if (current.ContestTough  < initial.ContestTough)
            return false;
        if (current.ContestSheen  < initial.ContestSheen)
            return false;
        return true;
    }

    public static bool IsContestEqual(this IContestStatsReadOnly current, IContestStatsReadOnly initial)
    {
        if (current.ContestCool != initial.ContestCool)
            return false;
        if (current.ContestBeauty != initial.ContestBeauty)
            return false;
        if (current.ContestCute != initial.ContestCute)
            return false;
        if (current.ContestSmart != initial.ContestSmart)
            return false;
        if (current.ContestTough != initial.ContestTough)
            return false;
        if (current.ContestSheen != initial.ContestSheen)
            return false;
        return true;
    }

    public static void CopyContestStatsTo(this IContestStatsReadOnly source, IContestStats dest)
    {
        dest.ContestCool = source.ContestCool;
        dest.ContestBeauty = source.ContestBeauty;
        dest.ContestCute = source.ContestCute;
        dest.ContestSmart = source.ContestSmart;
        dest.ContestTough = source.ContestTough;
        dest.ContestSheen = source.ContestSheen;
    }

    public static void SetAllContestStatsTo(this IContestStats dest, byte value, byte sheen)
    {
        dest.ContestCool = value;
        dest.ContestBeauty = value;
        dest.ContestCute = value;
        dest.ContestSmart = value;
        dest.ContestTough = value;
        dest.ContestSheen = sheen;
    }

    private const byte CONTEST_MAX = 255;

    /// <summary>
    /// Check if any contest stat besides <see cref="IContestStatsReadOnly.ContestSheen"/> is equal to <see cref="CONTEST_MAX"/>.
    /// </summary>
    /// <param name="s">Entity to check</param>
    /// <returns>True if any equals <see cref="CONTEST_MAX"/></returns>
    public static bool IsAnyContestStatMax(this IContestStatsReadOnly s) => CONTEST_MAX == s.ContestCool
                                                                         || CONTEST_MAX == s.ContestBeauty
                                                                         || CONTEST_MAX == s.ContestCute
                                                                         || CONTEST_MAX == s.ContestSmart
                                                                         || CONTEST_MAX == s.ContestTough;
}
