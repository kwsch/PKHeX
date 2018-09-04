namespace PKHeX.Core
{
    /// <summary>
    /// <see cref="GameVersion.CXD"/> Game Language IDs
    /// </summary>
    public enum LanguageGC : byte
    {
        /// <summary>
        /// Undefined Language ID, usually indicative of a value not being set.
        /// </summary>
        /// <remarks>Gen5 Japanese In-game Trades happen to not have their Language value set, and express Language=0.</remarks>
        Hacked = 0,

        /// <summary>
        /// Japanese (日本語)
        /// </summary>
        Japanese = 1,

        /// <summary>
        /// English (US/UK/AU)
        /// </summary>
        English = 2,

        /// <summary>
        /// German (Deutsch)
        /// </summary>
        German = 3,

        /// <summary>
        /// French (Français)
        /// </summary>
        French = 4,

        /// <summary>
        /// Italian (Italiano)
        /// </summary>
        Italian = 5,

        /// <summary>
        /// Spanish (Español)
        /// </summary>
        Spanish = 6,

        /// <summary>
        /// Unused Language ID
        /// </summary>
        /// <remarks>Was reserved for Korean in Gen3 but never utilized.</remarks>
        UNUSED_6 = 7,
    }
}