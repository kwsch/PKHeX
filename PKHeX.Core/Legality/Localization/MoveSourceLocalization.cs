// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

using System;
using System.Text.Json.Serialization;
using static PKHeX.Core.LearnMethod;

namespace PKHeX.Core;

/// <summary>
/// Localization strings for move learning source information.
/// </summary>
public sealed class MoveSourceLocalization
{
    private static readonly MoveSourceLocalizationContext Context = new(LocalizationStorage<MoveSourceLocalization>.Options);
    public static readonly LocalizationStorage<MoveSourceLocalization> Cache = new("movesource", Context.MoveSourceLocalization);
    public static MoveSourceLocalization Get(string language = GameLanguage.DefaultLanguage) => Cache.Get(language);
    public static MoveSourceLocalization Get(LanguageID language) => Cache.Get(language.GetLanguageCode());

    /// <summary>Format text for exporting a legality check result for a Move.</summary>
    public required string FormatMove { get; init; } = "{0} Move {1}: {2}";

    /// <summary>Format text for exporting a legality check result for a Relearn Move.</summary>
    public required string FormatRelearn { get; init; } = "{0} Relearn Move {1}: {2}";

    // Basic source types
    public required string SourceDefault { get; init; } = "Default move.";
    public required string SourceDuplicate { get; init; } = "Duplicate Move.";
    public required string SourceEmpty { get; init; } = "Empty Move.";
    public required string SourceInvalid { get; init; } = "Invalid Move.";
    public required string SourceLevelUp { get; init; } = "Learned by Level-up.";
    public required string SourceRelearn { get; init; } = "Relearnable Move.";
    public required string SourceSpecial { get; init; } = "Special Non-Relearn Move.";
    public required string SourceTMHM { get; init; } = "Learned by TM/HM.";
    public required string SourceTutor { get; init; } = "Learned by Move Tutor.";
    public required string SourceShared { get; init; } = "Shared Non-Relearn Move.";

    // Egg-related sources
    public required string RelearnEgg { get; init; } = "Base Egg move.";
    public required string EggInherited { get; init; } = "Inherited Egg move.";
    public required string EggTMHM { get; init; } = "Inherited TM/HM move.";
    public required string EggInheritedTutor { get; init; } = "Inherited tutor move.";
    public required string EggInvalid { get; init; } = "Not an expected Egg move.";
    public required string EggLevelUp { get; init; } = "Inherited move learned by Level-up.";
    public required string LevelUpSuffix { get; init; } = " @ lv{0}";

    public string Localize(LearnMethod method) => method switch
    {
        Empty => SourceEmpty,
        Relearn => SourceRelearn,
        Initial => SourceDefault,
        LevelUp => SourceLevelUp,
        TMHM => SourceTMHM,
        Tutor => SourceTutor,
        Sketch => SourceShared,
        EggMove => RelearnEgg,
        InheritLevelUp => EggInherited,

        HOME => SourceSpecial,
        Evolution => SourceSpecial,
        Encounter => SourceSpecial,
        SpecialEgg => SourceSpecial,
        ShedinjaEvo => SourceSpecial,

        Shared => SourceShared,

        // Invalid
        None => SourceInvalid,
        Unobtainable or UnobtainableExpect => SourceInvalid,
        Duplicate => SourceDuplicate,
        EmptyInvalid => SourceEmpty,

        _ => throw new ArgumentOutOfRangeException(nameof(method), method, null),
    };
}

[JsonSerializable(typeof(MoveSourceLocalization))]
public sealed partial class MoveSourceLocalizationContext : JsonSerializerContext;
