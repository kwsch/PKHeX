using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace PKHeX.Core;

public sealed record LocalizationStorage<T>(string Name, JsonTypeInfo<T> Info)
{
#pragma warning disable RCS1158 // Static member in generic type should use a type parameter
    public static JsonSerializerOptions Options => new()
    {
        RespectNullableAnnotations = true,
        AllowTrailingCommas = true,
        WriteIndented = true,
        IndentCharacter = ' ',
        IndentSize = 4,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };
#pragma warning restore RCS1158 // Static member in generic type should use a type parameter

    private readonly Dictionary<string, T> _cache = new(1); // usually only localized to 1 language during runtime

    private string GetJson(string language) => Util.GetStringResource(GetFileName(language));
    public string GetFileName(string language) => $"{Name}_{language}.json";

    /// <param name="language"><see cref="LanguageID"/> index</param>
    /// <inheritdoc cref="Get(LanguageID)"/>
    public T Get(LanguageID language) => Get(language.GetLanguageCode());

    /// <summary>
    /// Gets the localization for the requested language.
    /// </summary>
    /// <param name="language">Language code</param>
    public T Get(string language)
    {
        if (_cache.TryGetValue(language, out var result))
            return result;

        result = GetConfig(language);
        _cache[language] = result;
        return result;
    }

    private T GetConfig(string language)
    {
        var text = GetJson(language);
        return JsonSerializer.Deserialize(text, Info)
               ?? throw new JsonException($"Failed to deserialize {nameof(T)} for {language}");
    }

    /// <summary>
    /// Force loads all localizations.
    /// </summary>
    public bool ForceLoadAll()
    {
        bool anyLoaded = false;
        foreach (var lang in GameLanguage.AllSupportedLanguages)
        {
            if (_cache.ContainsKey(lang))
                continue;
            _ = Get(lang);
            anyLoaded = true;
        }
        return anyLoaded;
    }

    /// <summary>
    /// Gets all localizations.
    /// </summary>
    public IReadOnlyDictionary<string, T> GetAll()
    {
        _ = ForceLoadAll();
        return _cache;
    }
}
