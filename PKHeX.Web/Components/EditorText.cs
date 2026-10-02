using System.Globalization;
using PKHeX.Core;
using PKHeX.Web.Services;
using PKHeX.Web.State;

namespace PKHeX.Web.Components;

/// <summary>
/// The text of the draft editor (<see cref="DraftEditor"/>): field labels and the notes on names and friendship. State reports values only.
/// </summary>
/// <remarks>Numbers are written with the invariant culture, as the inspector writes them. Names from the save are passed in and rendered as text.</remarks>
public static class EditorText
{
    /// <summary>Shown in place of the fields of an egg, which this release does not edit.</summary>
    public const string EggReadOnly = "This is an egg. Its name, language, friendship (its hatch counter), level, nature, IVs and EVs are kept as stored and cannot be changed in this release.";

    /// <summary>Shown beside the handling trainer's friendship when no handling trainer is stored.</summary>
    public const string NoHandler = "No handling trainer is stored: this Pokémon has stayed with its original trainer, so there is no friendship towards one to change.";

    /// <summary>
    /// Shown for a Pokémon that is not nicknamed when its species or stored language has no default name in this game, so clearing the flag
    /// or changing the language keeps its name.
    /// </summary>
    public const string NoDefaultName = "This game has no default name for this species in this language, so its name is kept as it is.";

    /// <summary>The fields an opened Pokémon can have changed, for the message shown when it is opened.</summary>
    public static string EditableSummary(EditableFields fields)
    {
        var names = new List<string>(8);
        if (fields.HasFlag(EditableFields.Nickname))
        {
            names.Add("nickname");
        }
        if (fields.HasFlag(EditableFields.Language))
        {
            names.Add("language");
        }
        if (fields.HasFlag(EditableFields.Friendship))
        {
            names.Add("friendship");
        }
        if (fields.HasFlag(EditableFields.Level))
        {
            names.Add("level");
            names.Add("experience points");
        }
        if (fields.HasFlag(EditableFields.Nature))
        {
            names.Add("nature");
        }
        if (fields.HasFlag(EditableFields.Ivs))
        {
            names.Add("IVs");
        }
        if (fields.HasFlag(EditableFields.Evs))
        {
            names.Add("EVs");
        }
        return names.Count switch
        {
            0 => "No fields can be changed.",
            1 => $"Its {names[0]} can be changed.",
            _ => $"Its {string.Join(", ", names.Take(names.Count - 1))} and {names[^1]} can be changed.",
        };
    }

    /// <summary>The label of the original trainer's friendship, naming the trainer.</summary>
    public static string TrainerFriendshipLabel(string trainer) => $"Friendship with original trainer {trainer} (0–255)";

    /// <summary>The label of the handling trainer's friendship, naming the trainer when one is stored.</summary>
    public static string HandlerFriendshipLabel(string handler) => handler.Length == 0
        ? "Friendship with handling trainer (none stored)"
        : $"Friendship with handling trainer {handler} (0–255)";

    /// <summary>Which of the two values the game uses now. Changing either never changes who holds the Pokémon.</summary>
    public static string CurrentFriendship(bool withHandler) =>
        $"The game currently uses the {(withHandler ? "handling" : "original")} trainer's value. Changing a value does not change who holds this Pokémon.";

    /// <summary>The label of the level field.</summary>
    public const string LevelLabel = "Level (1–100)";

    /// <summary>The label of the experience points field, with the most the species' growth rate counts.</summary>
    public static string ExperienceLabel(uint maximum) => string.Create(CultureInfo.InvariantCulture, $"Experience points (0–{maximum})");

    /// <summary>
    /// The note on the drafted level: the experience range the level spans and how far the next level is, so a level edit's effect on the
    /// experience points is visible before it is made.
    /// </summary>
    public static string LevelNote(LevelProgress progress)
    {
        var range = string.Create(CultureInfo.InvariantCulture, $"Level {progress.Level} starts at {progress.LevelMinimum} experience points");
        if (progress.NextLevel is not { } next)
        {
            return range + " and is the highest level. Changing the level sets the experience points to the start of the new level.";
        }
        var toNext = string.Create(CultureInfo.InvariantCulture, $"; {next - progress.Experience} more reach level {progress.Level + 1} at {next}.");
        return range + toNext + " Changing the level sets the experience points to the start of the new level.";
    }

    /// <summary>
    /// The note on the drafted nature: which stat it raises and which it lowers, by the stat names the inspector uses, and that in Generation 6
    /// the nature is stored apart from the PID.
    /// </summary>
    /// <param name="effect">The nature's effect (<see cref="NatureEffect.Of"/>).</param>
    public static string NatureNote(NatureEffect effect)
    {
        var stats = effect.IsNeutral
            ? "This nature does not raise or lower any stat."
            : $"This nature raises {InspectorText.StatNames[effect.Raised]} and lowers {InspectorText.StatNames[effect.Lowered]}.";
        return stats + " In this game the nature is stored apart from the PID, so changing it does not change shininess, gender or ability.";
    }

    /// <summary>The header of the IV column, with the range each IV takes.</summary>
    public static string IvHeader(int maximum) => string.Create(CultureInfo.InvariantCulture, $"IV (0–{maximum})");

    /// <summary>The header of the EV column, with the range each EV takes.</summary>
    public static string EvHeader(int maximum) => string.Create(CultureInfo.InvariantCulture, $"EV (0–{maximum})");

    /// <summary>
    /// The note on the drafted IVs: their total, and the Hidden Power type they give, since an IV edit can change it and the characteristic.
    /// </summary>
    /// <param name="total">The sum of the six IVs.</param>
    /// <param name="maximum">The highest single IV.</param>
    /// <param name="hiddenPower">The name of the Hidden Power type, or null when Core has none for it.</param>
    public static string IvNote(int total, int maximum, string? hiddenPower) => string.Create(CultureInfo.InvariantCulture,
        $"IV total {total} of {maximum * EditorDraft.StatCount}. Hidden Power type: {hiddenPower ?? "unknown"}. The IVs decide the Hidden Power type and the characteristic.");

    /// <summary>
    /// The note on the drafted EVs: their total, how many remain, and what Core's grading of the total means (<see cref="EffortValues.GetGrade"/>).
    /// </summary>
    /// <param name="total">The sum of the six EVs.</param>
    /// <param name="maximum">The highest total a Pokémon can hold.</param>
    public static string EvNote(int total, int maximum)
    {
        var sum = total > maximum
            ? string.Create(CultureInfo.InvariantCulture, $"EV total {total} of {maximum}: {total - maximum} over the limit.")
            : string.Create(CultureInfo.InvariantCulture, $"EV total {total} of {maximum}; {maximum - total} remaining.");
        var grade = EffortValues.GetGrade(total) switch
        {
            EffortValueGrade.MaxEffective => " Every EV that changes a stat is used; stats grow every 4 EVs, so the 2 remaining would change nothing, and legality analysis may note this total.",
            EffortValueGrade.MaxLegal => " This is the most a Pokémon can have.",
            EffortValueGrade.Illegal => " Lower an EV to bring the total within the limit; an EV cannot be raised until it is.",
            _ => "",
        };
        return sum + grade;
    }

    /// <summary>The name of a stored nature outside the game's list, so the nature box never shows a value that is not stored.</summary>
    public static string UnlistedNature(int value) => string.Create(CultureInfo.InvariantCulture, $"Unknown (stored value {value})");

    /// <summary>The name of a stored language outside the game's list, so the language box never shows a value that is not stored.</summary>
    public static string UnlistedLanguage(int value) => string.Create(CultureInfo.InvariantCulture, $"Unknown (stored value {value})");

    /// <summary>
    /// The note on the drafted name: a default name set by a flag or language change, a name kept from another language, the default name
    /// for the language, and any characters the game's font cannot show. Empty when there is nothing to say.
    /// </summary>
    /// <param name="status">What the drafted name means (<see cref="EditorDraft.Name"/>).</param>
    /// <param name="isNicknamed">The drafted nickname flag.</param>
    /// <param name="language">The display name of the drafted language.</param>
    /// <param name="renamed">The name before and after the last edit gave the Pokémon its default name, or null when it did not.</param>
    public static string NameNote(NameStatus status, bool isNicknamed, string language, (string From, string To)? renamed)
    {
        var parts = new List<string>(3);
        if (renamed is var (from, to))
        {
            parts.Add($"Not nicknamed, so its name changed from {from} to its {language} default, {to}.");
        }
        else if (status.KeptOtherLanguageName)
        {
            parts.Add($"Not nicknamed: its name was kept because it is the species' name in another language. The {language} default is {status.DefaultName}; legality analysis may report the difference.");
        }
        else if (!isNicknamed && !status.IsDefault && status.DefaultName is not null)
        {
            parts.Add($"Not nicknamed, but the name differs from the {language} default, {status.DefaultName}.");
        }
        else if (isNicknamed && status.DefaultName is not null)
        {
            parts.Add($"Default name in {language}: {status.DefaultName}.");
        }
        else if (!isNicknamed && status.DefaultName is null)
        {
            parts.Add(NoDefaultName);
        }
        if (status.HasUndefinedCharacters)
        {
            parts.Add($"The game's font cannot show some of these characters; it would show the name as {status.Displayed}.");
        }
        return string.Join(" ", parts);
    }
}
