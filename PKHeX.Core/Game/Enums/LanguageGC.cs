namespace PKHeX.Core;

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

/// <summary>
/// Extension to convert between <see cref="LanguageGC"/> and <see cref="LanguageID"/>.
/// </summary>
public static class LanguageGCRemap
{
    /// <summary>
    /// Converts <see cref="LanguageGC"/> to <see cref="LanguageID"/>.
    /// </summary>
    public static LanguageID ToLanguageID(this LanguageGC lang) => lang switch
    {
        LanguageGC.Hacked => LanguageID.Hacked,
        LanguageGC.Japanese => LanguageID.Japanese,
        LanguageGC.English => LanguageID.English,
        LanguageGC.German => LanguageID.German,
        LanguageGC.French => LanguageID.French,
        LanguageGC.Italian => LanguageID.Italian,
        LanguageGC.Spanish => LanguageID.Spanish,
        _ => LanguageID.English,
    };

    /// <summary>
    /// Converts <see cref="LanguageID"/> to <see cref="LanguageGC"/>.
    /// </summary>
    public static LanguageGC ToLanguageGC(this LanguageID lang) => lang switch
    {
        LanguageID.Hacked => LanguageGC.Hacked,
        LanguageID.Japanese => LanguageGC.Japanese,
        LanguageID.English => LanguageGC.English,
        LanguageID.German => LanguageGC.German,
        LanguageID.French => LanguageGC.French,
        LanguageID.Italian => LanguageGC.Italian,
        LanguageID.Spanish => LanguageGC.Spanish,
        _ => LanguageGC.English,
    };
}
