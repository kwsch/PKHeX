using System.Text.Json.Serialization;

namespace PKHeX.Core;

/// <summary>
/// Localized strings for <see cref="BattleTemplateParseErrorType"/> values.
/// Each enum member maps 1:1 to a property for JSON (de)serialization.
/// </summary>
public sealed class BattleTemplateParseErrorLocalization
{
    private static readonly BattleTemplateParseErrorLocalizationContext Context = new(LocalizationStorage<BattleTemplateParseErrorLocalization>.Options);
    public static readonly LocalizationStorage<BattleTemplateParseErrorLocalization> Cache = new("setparse", Context.BattleTemplateParseErrorLocalization);
    public static BattleTemplateParseErrorLocalization Get(string language = GameLanguage.DefaultLanguage) => Cache.Get(language);
    public static BattleTemplateParseErrorLocalization Get(LanguageID language) => Cache.Get(language.GetLanguageCode());

    // General / structural
    public required string LineLength { get; init; } = "Line exceeded the maximum supported length: {0}";

    // Token issues
    public required string TokenUnknown { get; init; } = "Unrecognized: {0}";
    public required string TokenFailParse { get; init; } = "Token could not be parsed: {0}";

    // Move issues
    public required string MoveCountTooMany { get; init; } = "Too many moves specified: {0}";
    public required string MoveSlotAlreadyUsed { get; init; } = "Move slot already used: {0}";
    public required string MoveDuplicate { get; init; } = "Duplicate move specified: {0}";
    public required string MoveUnrecognized { get; init; } = "Move not recognized: {0}";

    // Item
    public required string ItemUnrecognized { get; init; } = "Held item not recognized: {0}";

    // Ability
    public required string AbilityDeclaration { get; init; } = "Ability already declared: {0}";
    public required string AbilityUnrecognized { get; init; } = "Ability not recognized: {0}";
    public required string AbilityAlreadySpecified { get; init; } = "Ability already specified: {0}";

    // Nature
    public required string NatureUnrecognized { get; init; } = "Nature not recognized: {0}";
    public required string NatureAlreadySpecified { get; init; } = "Nature already specified: {0}";

    // Hidden Power
    public required string HiddenPowerUnknownType { get; init; } = "Hidden Power type not recognized: {0}";
    public required string HiddenPowerIncompatibleIVs { get; init; } = "Hidden Power type incompatible with IVs: {0}";

    // EffortValue Nature Amp (Stat modifiers with + / - )
    public required string NatureEffortAmpDeclaration { get; init; } = "Nature / effort amp already declared: {0}";
    public required string NatureEffortAmpUnknown { get; init; } = "Unknown nature effort amp token: {0}";
    public required string NatureEffortAmpAlreadySpecified { get; init; } = "Nature effort amp already specified: {0}";
    public required string NatureEffortAmpConflictNature { get; init; } = "Declared effort amp conflicts with previously specified nature.";
    public required string NatureAmpNoPlus { get; init; } = "Missing '+' nature amp token.";
    public required string NatureAmpNoMinus { get; init; } = "Missing '-' nature amp token.";
}

[JsonSerializable(typeof(BattleTemplateParseErrorLocalization))]
public sealed partial class BattleTemplateParseErrorLocalizationContext : JsonSerializerContext;
