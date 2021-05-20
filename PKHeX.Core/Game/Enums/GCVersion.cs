namespace PKHeX.Core
{
    /// <summary>
    /// <see cref="GameVersion"/> analogues used by Colosseum/XD instead of the main-series values.
    /// </summary>
#pragma warning disable CA1027 // Mark enums with FlagsAttribute
    public enum GCVersion : byte
#pragma warning restore CA1027 // Mark enums with FlagsAttribute
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
        public static GCVersion GetCXDVersionID(this GameVersion gbaVersion) => gbaVersion switch
        {
            GameVersion.S => GCVersion.S,
            GameVersion.R => GCVersion.R,
            GameVersion.E => GCVersion.E,
            GameVersion.FR => GCVersion.FR,
            GameVersion.LG => GCVersion.LG,
            GameVersion.CXD => GCVersion.CXD,
            _ => GCVersion.None,
        };

        /// <summary>
        /// Translates a <see cref="GCVersion"/> to the corresponding main-series <see cref="GameVersion"/> value.
        /// </summary>
        /// <param name="gcVersion">Version ID while present in the GameCube games</param>
        /// <returns>Version ID while present in the main-series games</returns>
        public static GameVersion GetG3VersionID(this GCVersion gcVersion) => gcVersion switch
        {
            GCVersion.S => GameVersion.S,
            GCVersion.R => GameVersion.R,
            GCVersion.E => GameVersion.E,
            GCVersion.FR => GameVersion.FR,
            GCVersion.LG => GameVersion.LG,
            GCVersion.CXD => GameVersion.CXD,
            _ => GameVersion.Unknown
        };
    }
}
