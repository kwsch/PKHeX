using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PKHeX.Core;

public sealed record BattleTemplateLocalization(GameStrings Strings, BattleTemplateConfig Config)
{
    public const string DefaultLanguage = GameLanguage.DefaultLanguage;

    private static readonly Dictionary<string, BattleTemplateLocalization> Cache = new();
    public static readonly BattleTemplateLocalization Default = GetLocalization(DefaultLanguage);

    public static BattleTemplateLocalization GetLocalization(int language) =>
        GetLocalization(GameLanguage.LanguageCode(language));

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

    private static BattleTemplateConfig GetConfig(string language, string fallback = GameLanguage.DefaultLanguage)
    {
        var text = GetJson(language);
        var result = JsonSerializer.Deserialize(text, GetContext().BattleTemplateConfig);
        return result ?? GetConfig(fallback);
    }
}

[JsonSerializable(typeof(BattleTemplateConfig))]
public sealed partial class BattleTemplateConfigContext : JsonSerializerContext;
