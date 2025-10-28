using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Contains localized strings for legality checks, moves, encounters, and general game information.
/// </summary>
public sealed class LegalityLocalizationSet
{
    private static readonly Dictionary<string, LegalityLocalizationSet> Cache = new(1);

    public required LegalityCheckLocalization Lines { get; init; }
    public required GameStrings Strings { get; init; }
    public required EncounterDisplayLocalization Encounter { get; init; }
    public required MoveSourceLocalization Moves { get; init; }
    public required GeneralLocalization General { get; init; }

    public static LegalityLocalizationSet GetLocalization(LanguageID language) => GetLocalization(language.GetLanguageCode());

    public string Description(Severity s) => s switch
    {
        Severity.Invalid => Lines.SInvalid,
        Severity.Fishy => Lines.SFishy,
        Severity.Valid => Lines.SValid,
        _ => Lines.NotImplemented,
    };

    /// <summary>
    /// Gets the localization for the requested language.
    /// </summary>
    /// <param name="language">Language code</param>
    public static LegalityLocalizationSet GetLocalization(string language)
    {
        if (Cache.TryGetValue(language, out var result))
            return result;

        result = new LegalityLocalizationSet
        {
            Strings = GameInfo.GetStrings(language),
            Lines = LegalityCheckLocalization.Get(language),
            Encounter = EncounterDisplayLocalization.Get(language),
            Moves = MoveSourceLocalization.Get(language),
            General = GeneralLocalization.Get(language),
        };
        Cache[language] = result;
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
    public static IReadOnlyDictionary<string, LegalityLocalizationSet> GetAll()
    {
        _ = ForceLoadAll();
        return Cache;
    }
}
