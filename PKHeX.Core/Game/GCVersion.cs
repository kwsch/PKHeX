namespace PKHeX.Core
{
    /// <summary>
    /// <see cref="GameVersion"/> analogues used by Colosseum/XD instead of the main-series values.
    /// </summary>
    public enum GCVersion
    {
        None = 0,
        FR = 1,
        LG = 2,
        S = 8,
        R = 9,
        E = 10,
        CXD = 11,
    }

    public static class GCVersionExtensions
    {
        /// <summary>
        /// Translates a main-series <see cref="GameVersion"/> to the corresponding <see cref="GCVersion"/> value.
        /// </summary>
        /// <param name="gbaVersion">Version ID while present in the main-series games</param>
        /// <returns>Version ID while present in the GameCube games</returns>
        public static GCVersion GetCXDVersionID(this GameVersion gbaVersion)
        {
            switch (gbaVersion)
            {
                case GameVersion.S:   return GCVersion.S;
                case GameVersion.R:   return GCVersion.R;
                case GameVersion.E:   return GCVersion.E;
                case GameVersion.FR:  return GCVersion.FR;
                case GameVersion.LG:  return GCVersion.LG;
                case GameVersion.CXD: return GCVersion.CXD;
                default: return 0;
            }
        }

        /// <summary>
        /// Translates a <see cref="GCVersion"/> to the corresponding main-series <see cref="GameVersion"/> value.
        /// </summary>
        /// <param name="gcVersion">Version ID while present in the GameCube games</param>
        /// <returns>Version ID while present in the main-series games</returns>
        public static GameVersion GetG3VersionID(this GCVersion gcVersion)
        {
            switch (gcVersion)
            {
                case GCVersion.S:   return GameVersion.S;
                case GCVersion.R:   return GameVersion.R;
                case GCVersion.E:   return GameVersion.E;
                case GCVersion.FR:  return GameVersion.FR;
                case GCVersion.LG:  return GameVersion.LG;
                case GCVersion.CXD: return GameVersion.CXD;
                default: return 0;
            }
        }
    }
}
