using System;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

/// <summary>
/// Language code &amp; string asset loading.
/// </summary>
public static class GameLanguage
{
    public const string DefaultLanguage = "en"; // English
    public static int DefaultLanguageIndex => Array.IndexOf(LanguageCodes, DefaultLanguage);
    public static string LanguageCode(int lang) => (uint)lang >= LanguageCodes.Length ? DefaultLanguage : LanguageCodes[lang];
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

    private static readonly string[] LanguageCodes = ["ja", "en", "fr", "it", "de", "es", "ko", "zh-Hans", "zh-Hant"];

    /// <summary>
    /// Pokétransporter location names, ordered per index of <see cref="LanguageCodes"/>
    /// </summary>
    private static readonly string[] ptransp = ["ポケシフター", "Poké Transfer", "Poké Fret", "Pokétrasporto", "Poképorter", "Pokétransfer", "포케시프터", "宝可传送", "寶可傳送"];

    /// <summary>
    /// Gets the Met Location display name for the Pokétransporter.
    /// </summary>
    /// <param name="language">Language Index from <see cref="LanguageCodes"/></param>
    public static string GetTransporterName(int language)
    {
        if ((uint)language >= ptransp.Length)
            language = 2;
        return ptransp[language];
    }

    /// <inheritdoc cref="GetTransporterName(int)"/>
    /// <param name="lang">Language name from <see cref="LanguageCodes"/></param>
    public static string GetTransporterName(string lang) => GetTransporterName(GetLanguageIndex(lang));

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
