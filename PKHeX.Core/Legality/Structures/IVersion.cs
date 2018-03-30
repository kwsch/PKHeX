namespace PKHeX.Core
{
    public interface IVersion
    {
        GameVersion Version { get; set; }
    }

    public static partial class Extensions
    {
        public static bool CanBeReceivedBy(this IVersion ver, GameVersion game) => ver.Version.Contains(game);
        public static GameVersion GetCompatibleVersion(this IVersion ver, GameVersion prefer)
        {
            if (ver.CanBeReceivedBy(prefer) || ver.Version <= GameVersion.Unknown)
                return prefer;
            return ver.GetVersion();
        }

        private static GameVersion GetVersion(this IVersion ver)
        {
            const int max = (int) GameVersion.RB;
            while (true) // this isn't optimal, but is low maintenance
            {
                var game = (GameVersion)Util.Rand.Next(1, max);
                if (ver.CanBeReceivedBy(game))
                    return game;
            }
        }
    }
}
