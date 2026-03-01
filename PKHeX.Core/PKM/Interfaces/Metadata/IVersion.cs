namespace PKHeX.Core;

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
    extension(IVersion v)
    {
        private bool CanBeReceivedBy(GameVersion version) => v.Version.Contains(version);

        /// <summary>
        /// Gets a compatible saved version value for the given <see cref="IVersion"/>.
        /// </summary>
        /// <param name="prefer">Preferred version to use, if possible.</param>
        public GameVersion GetCompatibleVersion(GameVersion prefer)
        {
            if (!v.CanBeReceivedBy(prefer))
                return v.GetSingleVersion();
            if (!prefer.IsValidSavedVersion())
                return prefer.GetSingleVersion();
            return prefer;
        }

        public GameVersion GetSingleVersion()
        {
            var version = v.Version;
            if (version.IsValidSavedVersion())
                return version;
            return version.GetSingleVersion();
        }
    }

    public static GameVersion GetSingleVersion(this GameVersion lump)
    {
        const int max = (int)GameUtil.HighestGameID;
        var rnd = Util.Rand;
        while (true) // this isn't optimal, but is low maintenance
        {
            var game = (GameVersion)rnd.Next(1, max);
            if (!lump.Contains(game))
                continue;
            if (game == GameVersion.BU)
                continue; // Ignore this one; only can be Japanese language.
            return game;
        }
    }
}
