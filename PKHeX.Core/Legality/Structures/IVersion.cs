namespace PKHeX.Core
{
    /// <summary>
    /// Interface that exposes a <see cref="Version"/> to see which version the data originated in.
    /// </summary>
    public interface IVersion
    {
        /// <summary>
        /// The version the data originated in.
        /// </summary>
        GameVersion Version { get; }
    }

    public static partial class Extensions
    {
        private static bool CanBeReceivedBy(this IVersion ver, GameVersion game) => ver.Version.Contains(game);

        public static GameVersion GetCompatibleVersion(this IVersion ver, GameVersion prefer)
        {
            if (ver.CanBeReceivedBy(prefer) || ver.Version <= GameVersion.Unknown)
                return prefer;
            return ver.GetSingleVersion();
        }

        private static GameVersion GetSingleVersion(this IVersion ver)
        {
            const int max = (int)GameUtil.HighestGameID;
            if ((int)ver.Version <= max)
                return ver.Version;
            var rnd = Util.Rand;
            while (true) // this isn't optimal, but is low maintenance
            {
                var game = (GameVersion)rnd.Next(1, max);
                if (game == GameVersion.BU)
                    continue; // Ignore this one; only can be Japanese language.
                if (ver.CanBeReceivedBy(game))
                    return game;
            }
        }
    }
}
