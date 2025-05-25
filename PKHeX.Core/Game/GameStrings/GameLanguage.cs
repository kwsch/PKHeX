using System;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

/// <summary>
/// Language code &amp; string asset loading.
/// </summary>
public static class GameLanguage
{
    public const string DefaultLanguage = "en"; // English
    public const int DefaultLanguageIndex = 1;

    private static readonly string[] LanguageCodes = ["ja", "en", "fr", "it", "de", "es", "ko", "zh-Hans", "zh-Hant"];

    public static string LanguageCode(int localizationIndex) => (uint)localizationIndex >= LanguageCodes.Length ? DefaultLanguage : LanguageCodes[localizationIndex];
    public static int LanguageCount => LanguageCodes.Length;

    /// <summary>
    /// Gets the language from the requested language code.
    /// </summary>
    /// <param name="lang">Language code</param>
    /// <returns>Index of the language code; if not a valid language code, returns the <see cref="DefaultLanguageIndex"/>.</returns>
    public static int GetLanguageIndex(string lang)
    {
        int l = Array.IndexOf(LanguageCodes, lang);
        return l < 0 ? DefaultLanguageIndex : l;
    }

    public static LanguageID GetLanguage(string lang) => lang switch
    {
        "ja" => LanguageID.Japanese,
        "en" => LanguageID.English,
        "fr" => LanguageID.French,
        "it" => LanguageID.Italian,
        "de" => LanguageID.German,
        "es" => LanguageID.Spanish,
        "ko" => LanguageID.Korean,
        "zh-Hans" => LanguageID.ChineseS,
        "zh-Hant" => LanguageID.ChineseT,
        _ => LanguageID.English,
    };

    /// <summary>
    /// Checks whether the language code is supported.
    /// </summary>
    /// <param name="lang">Language code</param>
    /// <returns>True if valid, False otherwise</returns>
    public static bool IsLanguageValid(string lang) => Array.IndexOf(LanguageCodes, lang) != -1;

    /// <summary>
    /// Language codes supported for loading string resources
    /// </summary>
    /// <see cref="ProgramLanguage"/>
    public static ReadOnlySpan<string> AllSupportedLanguages => LanguageCodes;

    /// <summary>
    /// Gets a list of strings for the specified language and file type.
    /// </summary>
    public static string[] GetStrings(string ident, string lang, [ConstantExpected] string type = "text")
    {
        string[] data = Util.GetStringList(ident, lang, type);
        if (data.Length == 0)
            data = Util.GetStringList(ident, DefaultLanguage, type);

        return data;
    }
}
