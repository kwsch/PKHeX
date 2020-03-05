namespace PKHeX.Core
{
    /// <summary>
    /// Contiguous series Game Language IDs
    /// </summary>
    public enum LanguageID : byte
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
        /// French (Français)
        /// </summary>
        French = 3,

        /// <summary>
        /// Italian (Italiano)
        /// </summary>
        Italian = 4,

        /// <summary>
        /// German (Deutsch)
        /// </summary>
        German = 5,

        /// <summary>
        /// Unused Language ID
        /// </summary>
        /// <remarks>Was reserved for Korean in Gen3 but never utilized.</remarks>
        UNUSED_6 = 6,

        /// <summary>
        /// Spanish (Español)
        /// </summary>
        Spanish = 7,

        /// <summary>
        /// Korean (한국어)
        /// </summary>
        Korean = 8,

        /// <summary>
        /// Chinese Simplified (简体中文)
        /// </summary>
        ChineseS = 9,

        /// <summary>
        /// Chinese Traditional (繁體中文)
        /// </summary>
        ChineseT = 10,
    }
}
