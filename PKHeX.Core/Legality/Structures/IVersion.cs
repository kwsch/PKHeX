using System.Collections.Generic;

namespace PKHeX.Core
{
    public interface IVersion
    {
        GameVersion Version { get; }
    }

    internal interface IVersionSet
    {
        GameVersion Version { set; }
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

        internal static void SetVersion<T>(this IEnumerable<T> arr, GameVersion game) where T : IVersion, IVersionSet
        {
            foreach (var z in arr)
            {
                if (((IVersion)z).Version <= 0)
                    ((IVersionSet)z).Version = game;
            }
        }

        internal static void SetVersion(this IEnumerable<EncounterArea> arr, GameVersion game)
        {
            foreach (var area in arr)
                area.Slots.SetVersion(game);
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
