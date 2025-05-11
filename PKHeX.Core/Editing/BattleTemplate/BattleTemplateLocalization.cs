using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PKHeX.Core;

/// <summary>
/// Provides information for localizing <see cref="IBattleTemplate"/> sets.
/// </summary>
/// <param name="Strings">In-game strings</param>
/// <param name="Config">Grammar and prefix/suffix tokens</param>
public sealed record BattleTemplateLocalization(GameStrings Strings, BattleTemplateConfig Config)
{
    public const string DefaultLanguage = GameLanguage.DefaultLanguage; // English

    private static readonly Dictionary<string, BattleTemplateLocalization> Cache = new();
    public static readonly BattleTemplateLocalization Default = GetLocalization(DefaultLanguage);

    /// <param name="language"><see cref="LanguageID"/> index</param>
    /// <inheritdoc cref="GetLocalization(string)"/>
    public static BattleTemplateLocalization GetLocalization(LanguageID language) =>
        GetLocalization(language.GetLanguageCode());

    /// <summary>
    /// Gets the localization for the requested language.
    /// </summary>
    /// <param name="language">Language code</param>
    public static BattleTemplateLocalization GetLocalization(string language)
    {
        if (Cache.TryGetValue(language, out var result))
            return result;

        var strings = GameInfo.GetStrings(language);
        var cfg = GetConfig(language);
        result = new BattleTemplateLocalization(strings, cfg);
        Cache[language] = result;
        return result;
    }

    private static string GetJson(string language) => Util.GetStringResource($"battle_{language}.json");
    private static BattleTemplateConfigContext GetContext() => new();

    private static BattleTemplateConfig GetConfig(string language)
    {
        var text = GetJson(language);
        var result = JsonSerializer.Deserialize(text, GetContext().BattleTemplateConfig)
            ?? throw new JsonException($"Failed to deserialize {nameof(BattleTemplateConfig)} for {language}");
        return result;
    }

    /// <summary>
    /// Force loads all localizations.
    /// </summary>
    public static bool ForceLoadAll()
    {
        bool anyLoaded = false;
        foreach (var lang in GameLanguage.AllSupportedLanguages)
        {
            if (Cache.ContainsKey(lang))
                continue;
            _ = GetLocalization(lang);
            anyLoaded = true;
        }
        return anyLoaded;
    }

    /// <summary>
    /// Gets all localizations.
    /// </summary>
    public static IReadOnlyDictionary<string, BattleTemplateLocalization> GetAll()
    {
        _ = ForceLoadAll();
        return Cache;
    }
}

[JsonSerializable(typeof(BattleTemplateConfig))]
public sealed partial class BattleTemplateConfigContext : JsonSerializerContext;
