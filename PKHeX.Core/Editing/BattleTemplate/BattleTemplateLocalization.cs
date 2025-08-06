using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

    private static readonly ConcurrentDictionary<string, BattleTemplateLocalization> Cache = new(concurrencyLevel: Environment.ProcessorCount, capacity: 1);
    private static readonly BattleTemplateConfigContext Context = new(LocalizationStorage<BattleTemplateConfig>.Options);
    public static readonly LocalizationStorage<BattleTemplateConfig> ConfigCache = new("battle", Context.BattleTemplateConfig);
    public static readonly BattleTemplateLocalization Default = GetLocalization(DefaultLanguage);

    /// <summary>
    /// Gets the localization for the requested language.
    /// </summary>
    /// <param name="language">Language code</param>
    public static BattleTemplateConfig GetConfig(string language) => ConfigCache.Get(language);

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
        return Cache.GetOrAdd(language, static lang =>
        {
            var strings = GameInfo.GetStrings(lang);
            var cfg = GetConfig(lang);
            return new BattleTemplateLocalization(strings, cfg);
        });
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
