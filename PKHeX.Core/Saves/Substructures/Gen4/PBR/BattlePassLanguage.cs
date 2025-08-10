namespace PKHeX.Core;

/// <summary>
/// <see cref="BattlePass"/> Language IDs
/// </summary>
public enum BattlePassLanguage : byte
{
    /// <summary>
    /// Japanese (日本語)
    /// </summary>
    Japanese = 0,

    /// <summary>
    /// English (US/UK/AU)
    /// </summary>
    English = 1,

    /// <summary>
    /// German (Deutsch)
    /// </summary>
    German = 2,

    /// <summary>
    /// French (Français)
    /// </summary>
    French = 3,

    /// <summary>
    /// Spanish (Español)
    /// </summary>
    Spanish = 4,

    /// <summary>
    /// Italian (Italiano)
    /// </summary>
    Italian = 5,
}

/// <summary>
/// Extension methods for <see cref="BattlePassLanguage"/>.
/// </summary>
public static class BattlePassLanguageExtensions
{
    /// <summary>
    /// Converts <see cref="BattlePassLanguage"/> to <see cref="LanguageID"/>.
    /// </summary>
    public static LanguageID ToLanguageID(this BattlePassLanguage lang) => lang switch
    {
        BattlePassLanguage.Japanese => LanguageID.Japanese,
        BattlePassLanguage.English => LanguageID.English,
        BattlePassLanguage.German => LanguageID.German,
        BattlePassLanguage.French => LanguageID.French,
        BattlePassLanguage.Spanish => LanguageID.Spanish,
        BattlePassLanguage.Italian => LanguageID.Italian,
        _ => LanguageID.Japanese,
    };

    /// <summary>
    /// Converts <see cref="LanguageID"/> to <see cref="BattlePassLanguage"/>.
    /// </summary>
    public static BattlePassLanguage ToBattlePassLanguage(this LanguageID lang) => lang switch
    {
        LanguageID.Japanese => BattlePassLanguage.Japanese,
        LanguageID.English => BattlePassLanguage.English,
        LanguageID.German => BattlePassLanguage.German,
        LanguageID.French => BattlePassLanguage.French,
        LanguageID.Spanish => BattlePassLanguage.Spanish,
        LanguageID.Italian => BattlePassLanguage.Italian,
        _ => BattlePassLanguage.Japanese,
    };
}
