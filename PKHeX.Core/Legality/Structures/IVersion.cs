namespace PKHeX.Core
{
    public interface IVersion
    {
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
            const int max = (int) GameVersion.RB;
            if ((int)ver.Version < max)
                return ver.Version;
            var rnd = Util.Rand;
            while (true) // this isn't optimal, but is low maintenance
            {
                var game = (GameVersion)rnd.Next(1, max);
                if (ver.CanBeReceivedBy(game))
                    return game;
            }
        }
    }
}
