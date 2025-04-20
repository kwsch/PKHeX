namespace PKHeX.Core;

/// <summary>
/// <see cref="GameVersion.BATREV"/> Game Language IDs
/// </summary>
public enum LanguageBR : byte
{
    /// <summary>
    /// Japanese (日本語) or English (UK/AU)
    /// </summary>
    /// <remarks>Language value is only used by PAL, and is ignored/left as 0 by NTSC-J/NTSC-U.</remarks>
    JapaneseOrEnglish = 0,

    /// <summary>
    /// German (Deutsch)
    /// </summary>
    German = 1,

    /// <summary>
    /// Spanish (Español)
    /// </summary>
    Spanish = 2,

    /// <summary>
    /// French (Français)
    /// </summary>
    French = 3,

    /// <summary>
    /// Italian (Italiano)
    /// </summary>
    Italian = 4,
}

/// <summary>
/// Extension to convert between <see cref="LanguageBR"/> and <see cref="LanguageID"/>.
/// </summary>
public static class LanguageBRRemap
{
    /// <summary>
    /// Converts <see cref="LanguageBR"/> to <see cref="LanguageID"/>.
    /// </summary>
    public static LanguageID ToLanguageID(this LanguageBR lang) => lang switch
    {
        LanguageBR.JapaneseOrEnglish => LanguageID.English,
        LanguageBR.German => LanguageID.German,
        LanguageBR.Spanish => LanguageID.Spanish,
        LanguageBR.French => LanguageID.French,
        LanguageBR.Italian => LanguageID.Italian,
        _ => LanguageID.English,
    };

    /// <summary>
    /// Converts <see cref="LanguageID"/> to <see cref="LanguageBR"/>.
    /// </summary>
    public static LanguageBR ToLanguageBR(this LanguageID lang) => lang switch
    {
        LanguageID.Japanese or LanguageID.English => LanguageBR.JapaneseOrEnglish,
        LanguageID.German => LanguageBR.German,
        LanguageID.Spanish => LanguageBR.Spanish,
        LanguageID.French => LanguageBR.French,
        LanguageID.Italian => LanguageBR.Italian,
        _ => LanguageBR.JapaneseOrEnglish,
    };
}
