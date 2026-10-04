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
    public const string EggReadOnly = "This is an egg. Its species, form, name, language, friendship (its hatch counter), level, nature, IVs, EVs, held item, moves, PP, ability and gender are kept as stored and cannot be changed in this release.";

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
        var names = new List<string>(17);
        if (fields.HasFlag(EditableFields.Species))
        {
            names.Add("species");
            names.Add("form");
        }
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
        if (fields.HasFlag(EditableFields.HeldItem))
        {
            names.Add("held item");
        }
        if (fields.HasFlag(EditableFields.Moves))
        {
            names.Add("moves");
        }
        if (fields.HasFlag(EditableFields.Pp))
        {
            names.Add("PP");
            names.Add("PP Ups");
        }
        if (fields.HasFlag(EditableFields.Ability))
        {
            names.Add("ability");
        }
        if (fields.HasFlag(EditableFields.Gender))
        {
            names.Add("gender");
        }
        return names.Count switch
        {
            0 => "No fields can be changed.",
            1 => $"Its {names[0]} can be changed.",
            _ => $"Its {string.Join(", ", names.Take(names.Count - 1))} and {names[^1]} can be changed.",
        };
    }

    /// <summary>The label of the original trainer's friendship, naming the trainer.</summary>
    public static string TrainerFriendshipLabel(string trainer) => $"Friendship with original trainer {DisplayText.Embed(trainer)} (0–255)";

    /// <summary>The label of the handling trainer's friendship, naming the trainer when one is stored.</summary>
    public static string HandlerFriendshipLabel(string handler) => handler.Length == 0
        ? "Friendship with handling trainer (none stored)"
        : $"Friendship with handling trainer {DisplayText.Embed(handler)} (0–255)";

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

    /// <summary>
    /// The note under the held item: the list is the game's holdable items. Core's legality check of an item (<c>ItemVerifier</c>) asks only
    /// whether it was released in the generation, not whether this species could hold it, so the note claims no more.
    /// </summary>
    public const string ItemNote = "The list holds the items this game lets a Pokémon hold. Legality analysis reports an item never released in this generation of games.";

    /// <summary>The note under the moves, always shown: the list does not say what the Pokémon can learn.</summary>
    public const string MoveListNote = "The list holds every move this game has; it does not say which moves this Pokémon can learn. Legality analysis checks that.";

    /// <summary>The visible label of every search field; the rest of its name is visually hidden (<see cref="MoveSearchSubject"/>, <see cref="ItemSearchSubject"/>).</summary>
    public const string SearchLabel = "Search";

    /// <summary>The visually hidden rest of a move box's search field name ("Search moves for Move 2").</summary>
    public static string MoveSearchSubject(int slot) => $" moves for {MoveSlotName(slot)}";

    /// <summary>The visually hidden rest of the held item's search field name ("Search held items").</summary>
    public const string ItemSearchSubject = " held items";

    /// <summary>
    /// The note under a search field: what typing does while it is blank, and otherwise how many entries match. Filtering never changes the
    /// choice, so the note says so when nothing matches.
    /// </summary>
    /// <param name="query">What is typed.</param>
    /// <param name="matches">How many entries match.</param>
    /// <param name="total">How many entries the list holds.</param>
    /// <param name="noun">What the list holds, singular ("move", "item").</param>
    public static string SearchNote(string query, int matches, int total, string noun)
    {
        if (ChoiceFilter.Fold(query).Length == 0)
        {
            return string.Create(CultureInfo.InvariantCulture, $"Type to search the {total} {noun}s.");
        }
        return matches switch
        {
            0 => $"No {noun} matches. The list below keeps only the current choice and (None).",
            1 => string.Create(CultureInfo.InvariantCulture, $"1 of {total} {noun}s matches."),
            _ => string.Create(CultureInfo.InvariantCulture, $"{matches} of {total} {noun}s match."),
        };
    }

    /// <summary>The header of a move row ("Move 1" to "Move 4").</summary>
    public static string MoveSlotName(int slot) => string.Create(CultureInfo.InvariantCulture, $"Move {slot + 1}");

    /// <summary>The header of the PP Ups column, with the range a move takes.</summary>
    public static string PpUpsHeader(int maximum) => string.Create(CultureInfo.InvariantCulture, $"PP Ups (0–{maximum})");

    /// <summary>
    /// The note on the last move or PP Ups edit: what it did to the slot's PP, so a move change's PP effects are stated, not only shown in
    /// the fields. Empty when the last edit changed neither.
    /// </summary>
    /// <param name="change">The last edit's effect, or null.</param>
    /// <param name="moveName">Gives the display name of a move.</param>
    public static string MoveChangeNote(MoveChange? change, Func<ushort, string> moveName)
    {
        if (change is not { } c)
        {
            return "";
        }
        var slot = MoveSlotName(c.Slot);
        var after = c.After;
        var name = moveName(after.Move);
        return c.Kind switch
        {
            MoveChangeKind.Emptied => $"{slot} is now empty, so its PP and PP Ups are 0.",
            MoveChangeKind.Restored => string.Create(CultureInfo.InvariantCulture,
                $"{slot} is back to its stored move, {name}, with its stored PP ({after.Pp} of {after.MaxPp}) and {PpUps(after.PpUps)}."),
            MoveChangeKind.PpUpsCleared => string.Create(CultureInfo.InvariantCulture,
                $"{slot} is now {name}. PP Ups cannot be used on it, so its {PpUps(c.Before.PpUps)} were removed; its PP is set to {after.Pp} of {after.MaxPp}."),
            MoveChangeKind.PpUpsChanged => string.Create(CultureInfo.InvariantCulture,
                $"{slot}, {name}, now has {PpUps(after.PpUps)}; its PP is {after.Pp} of {after.MaxPp}."),
            _ => string.Create(CultureInfo.InvariantCulture,
                $"{slot} is now {name}, with full PP: {after.Pp} of {after.MaxPp} ({PpUps(after.PpUps)}).") + CarriedPpUps(c, moveName),
        };

        // A new move takes the slot's last PP Ups count on a move that can take them (EditorDraft.EditMove), which differs from the replaced
        // move's count after an empty slot, a move without PP Ups, or a stored count above the most a move can take.
        static string CarriedPpUps(MoveChange c, Func<ushort, string> moveName)
        {
            if (c.Before.PpUps == c.After.PpUps)
            {
                return "";
            }
            if (c.Before.PpUps > EditorDraft.MaxPpUps)
            {
                return string.Create(CultureInfo.InvariantCulture, $" The {c.Before.PpUps} PP Ups stored before, more than a move can take, were not carried over.");
            }
            var before = c.Before.IsEmpty ? "the slot was emptied" : moveName(c.Before.Move);
            return string.Create(CultureInfo.InvariantCulture, $" It keeps the {PpUps(c.After.PpUps)} this slot had before {before}.");
        }

        static string PpUps(int count) => count switch
        {
            0 => "no PP Ups",
            1 => "1 PP Up",
            _ => string.Create(CultureInfo.InvariantCulture, $"{count} PP Ups"),
        };
    }

    /// <summary>
    /// The note under the ability, always shown: what the slots are, and that legality analysis, not the list, decides whether the Pokémon
    /// could have the hidden ability. It adds that two regular slots with the same ability are still different slots, and that
    /// a stored ability and slot that do not name one of the species' slots together are kept until a slot is chosen.
    /// </summary>
    /// <param name="regularSlotsSame">True when the first and second slots have the same ability.</param>
    /// <param name="storedUnmatched">True when the drafted ability and slot number do not name one of the slots (<see cref="EditorDraft.AbilitySlot"/> is null).</param>
    public static string AbilityNote(bool regularSlotsSame, bool storedUnmatched)
    {
        var note = "(1) and (2) are the species' regular abilities and (H) its hidden ability; choosing one sets the ability and its slot together. "
            + "Whether this Pokémon could have the hidden ability depends on how it was met, which legality analysis checks.";
        if (regularSlotsSame)
        {
            note += " Both regular abilities are the same, but the slot is still stored, and legality analysis checks it too.";
        }
        if (storedUnmatched)
        {
            note += " The stored ability and slot do not match one of this species' slots; they are kept until a slot is chosen.";
        }
        return note;
    }

    /// <summary>
    /// The name of a stored ability and slot number that do not name one of the species' slots together, so the ability box never shows a
    /// value that is not stored: the ability as the inspector names it, and the stored slot.
    /// </summary>
    public static string UnmatchedAbility(NamedValue ability, int abilityNumber) => $"{InspectorText.Named(ability)} (stored; {InspectorText.AbilitySlot(abilityNumber)})";

    /// <summary>The name of a gender value, as the inspector names it ("Male", "Female", "Genderless", or the unknown wording).</summary>
    public static string GenderName(byte gender) => InspectorText.Gender(gender);

    /// <summary>
    /// The note under the gender, always shown: which genders the species can have, and, when its form is its gender, that the form changes
    /// with it.
    /// </summary>
    public static string GenderNote(GenderRule rule, bool formFollowsGender) => rule switch
    {
        GenderRule.OnlyMale => "This species is always male.",
        GenderRule.OnlyFemale => "This species is always female.",
        GenderRule.Genderless => "This species is genderless.",
        _ when formFollowsGender => "This species' form is its gender, so changing the gender changes the form too, as PKHeX's desktop editor does. "
            + "The ability keeps its slot, taken from the new form, and the experience points become the fewest for the level; changing back gives back what the change altered, unless it has been edited since.",
        _ => "This species can be male or female. In Generation 6 the gender is stored on its own, so changing it changes nothing else.",
    };

    /// <summary>
    /// The note on the last gender edit of a species whose form is its gender: what changed besides the gender. Empty when the last edit
    /// changed nothing else.
    /// </summary>
    /// <param name="change">What the last gender edit changed besides the gender, or null.</param>
    /// <param name="abilityName">Gives the display name of an ability.</param>
    /// <param name="level">The drafted level.</param>
    public static string GenderChangeNote(GenderChange? change, Func<int, string> abilityName, byte level)
    {
        if (change is not { } c)
        {
            return "";
        }
        var parts = new List<string>(3);
        if (c.FormChanged)
        {
            parts.Add("Its form changed with its gender.");
        }
        if (c.AbilityChanged)
        {
            parts.Add($"Its ability is now {abilityName(c.After.Ability)} ({InspectorText.AbilitySlot(c.After.AbilityNumber)}).");
        }
        if (c.ExperienceChanged)
        {
            parts.Add(string.Create(CultureInfo.InvariantCulture, $"Its experience points are now {c.After.Experience} (level {level}), from {c.Before.Experience}."));
        }
        return string.Join(" ", parts);
    }

    /// <summary>
    /// The note under the species and form, always shown: a choice is previewed before it is made, what it keeps, and that legality analysis,
    /// not the lists, decides whether the Pokémon can be that species and form.
    /// </summary>
    public const string SpeciesNote = "Choosing a species or form shows what the change would alter before it is made; nothing changes until you confirm it. "
        + "Moves, IVs, EVs, nature, held item, the PID and form timers (Furfrou's trim, Hoopa's unbound days) are kept. "
        + "The lists hold every species this game has and each one's forms; legality analysis checks whether this Pokémon can be that species and form and know its moves.";

    /// <summary>The form box's only option for a species without alternate forms.</summary>
    public const string NoAlternateForms = "No alternate forms";

    /// <summary>
    /// The name of a form in the form box and the preview: Core's name, marked when the form exists only in battle or is not in this game, so
    /// such a choice is explained before it is made.
    /// </summary>
    public static string FormName(FormChoice choice)
    {
        var name = BareFormName(choice.Name, choice.Form);
        if (choice.BattleOnly)
        {
            name += " (battle only)";
        }
        if (!choice.InGame)
        {
            name += " (not in this game)";
        }
        return name;
    }

    /// <summary>Core's name of a form, or a numbered name when Core gives it none.</summary>
    private static string BareFormName(string name, byte form) => name.Length != 0
        ? name
        : string.Create(CultureInfo.InvariantCulture, $"Form {form}");

    /// <summary>The name of a stored form outside the species' form list, so the form box never shows a value that is not stored.</summary>
    public static string UnlistedForm(int form) => string.Create(CultureInfo.InvariantCulture, $"Unknown form (stored value {form})");

    /// <summary>
    /// Shown beside Apply and Download while a species or form change is previewed but not confirmed: the boxes show the previewed choice, which
    /// neither would carry.
    /// </summary>
    public const string SpeciesFormPending = "A species or form change is shown but not made yet. Make it or keep the current species and form before applying or downloading.";

    /// <summary>The label of the button that makes a previewed species or form change.</summary>
    public const string ConfirmSpeciesForm = "Make this change";

    /// <summary>The label of the button that drops a previewed species or form change.</summary>
    public const string CancelSpeciesForm = "Keep the current species and form";

    /// <summary>The question that heads a species or form change preview, naming the species and forms before and after.</summary>
    /// <param name="preview">The previewed change.</param>
    /// <param name="speciesName">Gives the display name of a species.</param>
    /// <param name="formName">Gives the display name of a form of the species before (false) or after (true) the change.</param>
    public static string PreviewTitle(SpeciesFormPreview preview, Func<ushort, string> speciesName, Func<bool, string?> formName)
    {
        var before = WithForm(speciesName(preview.Before.Species), formName(false));
        var after = WithForm(speciesName(preview.After.Species), formName(true));
        return $"Change {before} to {after}?";

        static string WithForm(string species, string? form) => form is null ? species : $"{species} ({form})";
    }

    /// <summary>The note after a species or form change is made, naming what it changed (see <see cref="SpeciesFormChanges"/>).</summary>
    public static string SpeciesFormChanged(IReadOnlyList<string> changes) => "The species and form were changed. " + string.Join(" ", changes);

    /// <summary>
    /// What a species or form change alters, one sentence each: every dependent field Core reports changed (form, experience points and level,
    /// ability, gender, PID, name) with its values before and after, the stats that change, and a party member's current HP; then whether the
    /// change gives back earlier values, and what legality analysis will report about the form.
    /// </summary>
    /// <param name="preview">The previewed change.</param>
    /// <param name="speciesName">Gives the display name of a species.</param>
    /// <param name="formName">Gives the display name of a form of the species before (false) or after (true) the change, or null when it has none.</param>
    /// <param name="abilityName">Gives the display name of an ability.</param>
    public static IReadOnlyList<string> SpeciesFormChanges(SpeciesFormPreview preview, Func<ushort, string> speciesName, Func<bool, string?> formName, Func<int, string> abilityName)
    {
        var (b, a) = (preview.Before, preview.After);
        var changes = preview.Changes;
        var lines = new List<string>(10);
        if (preview.SpeciesChanged)
        {
            lines.Add($"Species: {speciesName(b.Species)} to {speciesName(a.Species)}.");
        }
        if (changes.HasFlag(SpeciesFormChangeResult.Form))
        {
            lines.Add($"Form: {formName(false) ?? BareFormName("", b.Form)} to {formName(true) ?? BareFormName("", a.Form)}.");
        }
        if (changes.HasFlag(SpeciesFormChangeResult.EXP))
        {
            lines.Add(preview.LevelChanged
                ? string.Create(CultureInfo.InvariantCulture, $"Experience points: {b.Experience} to {a.Experience}, so the level goes from {b.Level} to {a.Level} on the new growth rate.")
                : string.Create(CultureInfo.InvariantCulture, $"Experience points: {b.Experience} to {a.Experience}, the start of level {a.Level}."));
        }
        else if (preview.LevelChanged)
        {
            lines.Add(string.Create(CultureInfo.InvariantCulture, $"Level: {b.Level} to {a.Level}, as the same {a.Experience} experience points count on the new growth rate."));
        }
        if (changes.HasFlag(SpeciesFormChangeResult.Ability))
        {
            lines.Add($"Ability: {abilityName(b.Ability)} ({InspectorText.AbilitySlot(b.AbilityNumber)}) to {abilityName(a.Ability)} ({InspectorText.AbilitySlot(a.AbilityNumber)}).");
        }
        if (changes.HasFlag(SpeciesFormChangeResult.Gender))
        {
            lines.Add($"Gender: {GenderName(b.Gender)} to {GenderName(a.Gender)}.");
        }
        if (changes.HasFlag(SpeciesFormChangeResult.PID))
        {
            lines.Add("PID: changed.");
        }
        if (changes.HasFlag(SpeciesFormChangeResult.Nickname))
        {
            lines.Add(a.IsNicknamed
                ? $"Name: {DisplayText.Embed(b.Nickname)} to {DisplayText.Embed(a.Nickname)}."
                : $"Name: {DisplayText.Embed(b.Nickname)} to {DisplayText.Embed(a.Nickname)}, as it is not nicknamed.");
        }
        if (preview.ChangedStats is { Count: > 0 } stats)
        {
            var values = string.Join(", ", stats.Select(i => string.Create(CultureInfo.InvariantCulture, $"{InspectorText.StatNames[i]} {b.Stats[i]} to {a.Stats[i]}")));
            lines.Add($"Stats: {values}.");
        }
        if (preview.HpChange is { } hp && hp.PreviousHp != hp.NewHp)
        {
            lines.Add(string.Create(CultureInfo.InvariantCulture, $"Current HP: {hp.PreviousHp} to {hp.NewHp}; it is never raised, and its status is kept."));
        }
        if (lines.Count == (preview.SpeciesChanged ? 1 : 0))
        {
            lines.Add("Nothing else changes.");
        }
        if (preview.Restores)
        {
            lines.Add("This returns to the species and form your changes started from, so the values they changed are given back.");
        }
        if (preview.BattleOnly)
        {
            lines.Add("This form exists only during battle, so legality analysis reports a Pokémon stored in it.");
        }
        if (!preview.InGame)
        {
            lines.Add("This game does not have this form, so legality analysis reports it.");
        }
        return lines;
    }

    /// <summary>
    /// The name of a stored item or move outside the game's list, as the inspector names it, so the box never shows a value that is not stored:
    /// Core's name marked as not available in this game (such as an Omega Ruby move on a Pokémon traded into X), or the unknown wording.
    /// </summary>
    public static string Unlisted(NamedValue value) => InspectorText.Named(value);

    /// <summary>A stored PP Ups count above the most a move can take, so the PP Ups box never shows a value that is not stored.</summary>
    public static string UnlistedPpUps(int value) => string.Create(CultureInfo.InvariantCulture, $"{value} (stored; above the maximum)");

    /// <summary>The name of a stored nature outside the game's list, so the nature box never shows a value that is not stored.</summary>
    public static string UnlistedNature(int value) => string.Create(CultureInfo.InvariantCulture, $"Unknown (stored value {value})");

    /// <summary>The name of a stored language outside the game's list, so the language box never shows a value that is not stored.</summary>
    public static string UnlistedLanguage(int value) => string.Create(CultureInfo.InvariantCulture, $"Unknown (stored value {value})");

    /// <summary>
    /// What the last edit, a flag or language change, did to the name: given its default name in the drafted language. Empty when it did not.
    /// It is announced as a live region; the name's own note (<see cref="NameNote"/>) follows typing, so it is not.
    /// </summary>
    /// <param name="language">The display name of the drafted language.</param>
    /// <param name="renamed">The name before and after the last edit gave the Pokémon its default name, or null when it did not.</param>
    public static string RenameNote(string language, (string From, string To)? renamed) => renamed is var (from, to)
        ? $"Not nicknamed, so its name changed from {DisplayText.Embed(from)} to its {language} default, {to}."
        : "";

    /// <summary>
    /// The note on the drafted name: a name kept from another language, the default name for the language, and any characters the game's font
    /// cannot show. Empty when there is nothing to say.
    /// </summary>
    /// <param name="status">What the drafted name means (<see cref="EditorDraft.Name"/>).</param>
    /// <param name="isNicknamed">The drafted nickname flag.</param>
    /// <param name="language">The display name of the drafted language.</param>
    public static string NameNote(NameStatus status, bool isNicknamed, string language)
    {
        var parts = new List<string>(2);
        if (status.KeptOtherLanguageName)
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
            parts.Add($"The game's font cannot show some of these characters; it would show the name as {DisplayText.Embed(status.Displayed)}.");
        }
        return string.Join(" ", parts);
    }
}
