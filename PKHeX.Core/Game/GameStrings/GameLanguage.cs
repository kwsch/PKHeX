using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

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

/// <summary>
/// Wrapper to store language-specific data that is lazily loaded, with non-negligible load time/allocation.
/// </summary>
/// <remarks>
/// Provides a thread-safe way to cache loaded objects for only the languages that are supported.
/// Slightly faster than using a ConcurrentDictionary, as we only need a fixed number of entries (one for each language).
/// </remarks>
public abstract record LanguageStorage<T> where T : notnull
{
    private readonly T?[] _entries = new T[GameLanguage.LanguageCount];

    // Lock for thread safety. Get operations are frequent, and usually will not require entering the lock as the entry is already populated.
    private readonly Lock _sync = new();

    /// <summary>
    /// Not present in the cache, create a new instance for the specified language.
    /// </summary>
    protected abstract T Create(string language);

    private bool IsAllLoaded()
    {
        using var scope = _sync.EnterScope();
        foreach (var entry in _entries)
        {
            if (entry is null)
                return false;
        }
        return true;
    }

    public T Get(string language)
    {
        int index = GameLanguage.GetLanguageIndex(language);
        var current = _entries[index];
        if (current is not null)
            return current;

        using var scope = _sync.EnterScope();
        // Now that we have the lock, check again. Another thread may have populated it while we were waiting.
        current = _entries[index];
        if (current is not null)
            return current;
        return _entries[index] = Create(language);
    }

    /// <summary>
    /// Force loads all localizations.
    /// </summary>
    public bool ForceLoadAll()
    {
        var result = !IsAllLoaded();
        // Load all languages if not already loaded.
        foreach (var lang in GameLanguage.AllSupportedLanguages)
            _ = Get(lang);
        return result;
    }

    /// <summary>
    /// Gets all localizations.
    /// </summary>
    /// <remarks>
    /// If the entries are not already loaded, this will load all entries via <see cref="ForceLoadAll"/>.
    /// </remarks>
    public IEnumerable<(string Key, T Value)> GetAll()
    {
        _ = ForceLoadAll();
        for (var i = 0; i < _entries.Length; i++)
        {
            var entry = _entries[i]!;
            var lang = GameLanguage.LanguageCode(i);
            yield return (lang, entry);
        }
    }
}
