// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

using System.Text.Json.Serialization;

namespace PKHeX.Core;

/// <summary>
/// Localization strings for general display information.
/// </summary>
public sealed record GeneralLocalization
{
    private static readonly GeneralLocalizationContext Context = new(LocalizationStorage<GeneralLocalization>.Options);
    public static readonly LocalizationStorage<GeneralLocalization> Cache = new("general", Context.GeneralLocalization);
    public static GeneralLocalization Get(string language = GameLanguage.DefaultLanguage) => Cache.Get(language);
    public static GeneralLocalization Get(LanguageID language) => Cache.Get(language.GetLanguageCode());

    public required string[] StatNames { get; init; }
    public required string OriginalTrainer { get; init; } = "Original Trainer";
    public required string HandlingTrainer { get; init; } = "Handling Trainer";

    public required string GenderMale { get; init; } = "Male";
    public required string GenderFemale { get; init; } = "Female";
    public required string GenderGenderless { get; init; } = "Genderless";
}

[JsonSerializable(typeof(GeneralLocalization))]
public sealed partial class GeneralLocalizationContext : JsonSerializerContext;
