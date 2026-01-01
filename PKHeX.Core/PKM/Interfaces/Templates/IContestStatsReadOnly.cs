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
    extension(IContestStatsReadOnly stats)
    {
        /// <summary>
        /// Checks if any contest stat value is nonzero.
        /// </summary>
        /// <returns>True if it has any nonzero contest stat, false if all are zero.</returns>
        public bool HasContestStats()
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

        public bool IsContestBelow(IContestStatsReadOnly initial) => !stats.IsContestAboveOrEqual(initial);

        public bool IsContestAboveOrEqual(IContestStatsReadOnly initial)
        {
            if (stats.ContestCool   < initial.ContestCool)
                return false;
            if (stats.ContestBeauty < initial.ContestBeauty)
                return false;
            if (stats.ContestCute   < initial.ContestCute)
                return false;
            if (stats.ContestSmart  < initial.ContestSmart)
                return false;
            if (stats.ContestTough  < initial.ContestTough)
                return false;
            if (stats.ContestSheen  < initial.ContestSheen)
                return false;
            return true;
        }

        public bool IsContestEqual(IContestStatsReadOnly initial)
        {
            if (stats.ContestCool != initial.ContestCool)
                return false;
            if (stats.ContestBeauty != initial.ContestBeauty)
                return false;
            if (stats.ContestCute != initial.ContestCute)
                return false;
            if (stats.ContestSmart != initial.ContestSmart)
                return false;
            if (stats.ContestTough != initial.ContestTough)
                return false;
            if (stats.ContestSheen != initial.ContestSheen)
                return false;
            return true;
        }

        public void CopyContestStatsTo(IContestStats dest)
        {
            dest.ContestCool = stats.ContestCool;
            dest.ContestBeauty = stats.ContestBeauty;
            dest.ContestCute = stats.ContestCute;
            dest.ContestSmart = stats.ContestSmart;
            dest.ContestTough = stats.ContestTough;
            dest.ContestSheen = stats.ContestSheen;
        }
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
