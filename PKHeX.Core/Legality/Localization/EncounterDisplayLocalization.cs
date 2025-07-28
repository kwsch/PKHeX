// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

using System.Text.Json.Serialization;

namespace PKHeX.Core;

/// <summary>
/// Localization strings for encounter display information.
/// </summary>
public sealed record EncounterDisplayLocalization
{
    private static readonly EncounterDisplayLocalizationContext Context = new(LocalizationStorage<EncounterDisplayLocalization>.Options);
    public static readonly LocalizationStorage<EncounterDisplayLocalization> Cache = new("encounter", Context.EncounterDisplayLocalization);
    public static EncounterDisplayLocalization Get(string language = GameLanguage.DefaultLanguage) => Cache.Get(language);
    public static EncounterDisplayLocalization Get(LanguageID language) => Cache.Get(language.GetLanguageCode());

    public required string Format { get; set; } = "{0}: {1}";
    public required string FormatLevelRange { get; set; } = "{0}: {1}-{2}";
    public required string EncounterType { get; set; } = "Encounter Type";
    public required string Version { get; set; } = "Version";
    public required string Level { get; set; } = "Level";
    public required string LevelRange { get; set; } = "Level Range";
    public required string Location { get; set; } = "Location";
    public required string OriginSeed { get; set; } = "Origin Seed";
    public required string PIDType { get; set; } = "PID Type";
    public required string Time { get; set; } = "Time";
    public required string FrameNewGame { get; set; } = "New Game: 0x{0}, Frame: {1}";
    public required string FrameInitial { get; set; } = "Initial: 0x{0}, Frame: {1}";
    public required string SuffixDays { get; set; } = " (days: {0})";
}

[JsonSerializable(typeof(EncounterDisplayLocalization))]
public sealed partial class EncounterDisplayLocalizationContext : JsonSerializerContext;
