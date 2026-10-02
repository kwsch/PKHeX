using System.Globalization;
using PKHeX.Core;
using PKHeX.Web.Services;

namespace PKHeX.Web.Components;

/// <summary>One labelled value of an inspector section.</summary>
/// <param name="Id">Element id of the value.</param>
/// <param name="Label">What the value is.</param>
/// <param name="Value">The value as text.</param>
public sealed record InspectorRow(string Id, string Label, string Value);

/// <summary>A table of an inspector section, for values that line up in columns (stats, moves).</summary>
/// <param name="Id">Element id of the table.</param>
/// <param name="Caption">The table's caption, which also says where its values come from.</param>
/// <param name="Headers">Column headers; the first column holds each row's header.</param>
/// <param name="Rows">Cells per row, as text, in header order.</param>
public sealed record InspectorTable(string Id, string Caption, IReadOnlyList<string> Headers, IReadOnlyList<IReadOnlyList<string>> Rows);

/// <summary>One section of the inspector.</summary>
/// <param name="Id">Element id of the section heading.</param>
/// <param name="Title">The heading.</param>
/// <param name="Table">The section's table, or null when it has none.</param>
/// <param name="Rows">The section's labelled values, shown after the table.</param>
public sealed record InspectorSection(string Id, string Title, InspectorTable? Table, IReadOnlyList<InspectorRow> Rows);

/// <summary>
/// The text the read-only inspector shows for an <see cref="EntityInspection"/>, section by section.
/// </summary>
/// <remarks>
/// Numbers are formatted with the invariant culture. A value Core has no name for is shown as "Unknown" with the stored value, and a value
/// outside this game's lists keeps Core's name with a note, so nothing is guessed or hidden. Names stored in the save are returned as they
/// are; the UI renders them as text.
/// </remarks>
public static class InspectorText
{
    private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

    /// <summary>Stat names in the order of <see cref="StatsFacts.Stats"/>.</summary>
    public static IReadOnlyList<string> StatNames { get; } = ["HP", "Attack", "Defense", "Sp. Atk", "Sp. Def", "Speed"];

    /// <summary>Box marking names in the order of <see cref="AdvancedFacts.Markings"/>.</summary>
    public static IReadOnlyList<string> MarkingNames { get; } = ["Circle", "Triangle", "Square", "Heart", "Star", "Diamond"];

    /// <summary>The five sections, in display order: Identity, Stats, Moves, Origin and trainer, Advanced.</summary>
    public static IReadOnlyList<InspectorSection> Sections(EntityInspection inspection) =>
    [
        Identity(inspection.Identity),
        Stats(inspection.Stats),
        Moves(inspection.Moves),
        Origin(inspection.Origin, inspection.Identity.IsEgg),
        Advanced(inspection.Advanced),
    ];

    private static InspectorSection Identity(IdentityFacts f) => new("inspect-identity", "Identity", null,
    [
        new("inspect-species", "Species", Species(f.Species)),
        new("inspect-form", "Form", Form(f)),
        new("inspect-nickname", "Nickname", f.Nickname),
        new("inspect-nicknamed", "Nicknamed", YesNo(f.IsNicknamed)),
        new("inspect-egg", "Egg", YesNo(f.IsEgg)),
        new("inspect-gender", "Gender", Gender(f.Gender)),
        new("inspect-shiny", "Shiny", YesNo(f.IsShiny)),
        new("inspect-language", "Language", Named(f.Language)),
        new("inspect-level", "Level", Number(f.Level)),
        new("inspect-exp", "Experience points", Number(f.Experience)),
        new("inspect-nature", "Nature", Named(f.Nature)),
        new("inspect-ability", "Ability", $"{Named(f.Ability)} ({AbilitySlot(f.AbilitySlot)})"),
        new("inspect-item", "Held item", f.HeldItem.Value == 0 ? "None" : Named(f.HeldItem)),
        // An egg keeps its hatch counter (egg cycles left) in the friendship field; the desktop relabels it the same way.
        f.IsEgg
            ? new("inspect-friendship", "Hatch counter (egg cycles left)", Number(f.Friendship))
            : new("inspect-friendship", "Friendship (current trainer)", OutOf(f.Friendship, 255)),
    ]);

    private static InspectorSection Stats(StatsFacts f)
    {
        var caption = f.Source == StatsSource.Stored
            ? "Stats as stored with this party member"
            : "Stats calculated by PKHeX.Core (boxed Pokémon do not store them)";
        var table = new InspectorTable("inspect-stats-table", caption, ["Stat", "Base", "IV", "EV", "Value"],
            f.Stats.Select((s, i) => (IReadOnlyList<string>)[StatNames[i], Number(s.Base), Number(s.Iv), Number(s.Ev), Number(s.Value)]).ToArray());
        var rows = new List<InspectorRow>
        {
            new("inspect-iv-total", "IV total", OutOf(f.IvTotal, 186)),
            new("inspect-ev-total", "EV total", OutOf(f.EvTotal, 510)),
            new("inspect-characteristic", "Characteristic", Named(f.Characteristic)),
            new("inspect-hidden-power", "Hidden Power type", Named(f.HiddenPowerType)),
        };
        if (f.CurrentHp is { } hp)
        {
            rows.Add(new("inspect-hp", "Current HP", OutOf(hp, f.Stats[0].Value)));
        }
        if (f.Status is { } status)
        {
            rows.Add(new("inspect-status", "Status condition", Named(status)));
        }
        return new("inspect-stats", "Stats", table, rows);
    }

    private static InspectorSection Moves(MovesFacts f)
    {
        var table = new InspectorTable("inspect-moves-table", "Current moves", ["Slot", "Move", "PP", "PP Ups"],
            f.Moves.Select((m, i) => (IReadOnlyList<string>)[Number(i + 1), Move(m.Move), m.Move.Value == 0 ? "–" : OutOf(m.Pp, m.MaxPp), Number(m.PpUps)]).ToArray());
        var rows = f.Relearn.Select((m, i) => new InspectorRow($"inspect-relearn-{i + 1}", $"Relearn move {i + 1}", Move(m))).ToArray();
        return new("inspect-moves", "Moves", table, rows);
    }

    private static InspectorSection Origin(OriginFacts f, bool isEgg)
    {
        var rows = new List<InspectorRow>
        {
            new("inspect-ot", "Original trainer", $"{f.TrainerName} ({TrainerGender(f.TrainerGender)})"),
            new("inspect-tid", "Trainer ID (TID)", f.DisplayTid.ToString(f.IdFormat.GetTrainerIDFormatStringTID(), Invariant)),
            new("inspect-sid", "Secret ID (SID)", f.DisplaySid.ToString(f.IdFormat.GetTrainerIDFormatStringSID(), Invariant)),
            new("inspect-origin-game", "Origin game", Named(f.OriginGame)),
            new("inspect-met-location", "Met location", Named(f.MetLocation)),
            new("inspect-met-level", "Met level", Number(f.MetLevel)),
            new("inspect-met-date", "Met date", Date(f.MetDate)),
            new("inspect-ball", "Ball", Named(f.Ball)),
        };
        if (f.EggLocation is { } egg)
        {
            rows.Add(new("inspect-egg-location", "Egg received at", Named(egg)));
            rows.Add(new("inspect-egg-date", "Egg received on", Date(f.EggDate)));
        }
        rows.Add(new("inspect-fateful", "Fateful encounter", YesNo(f.FatefulEncounter)));
        rows.Add(isEgg
            ? new("inspect-ot-friendship", "Original trainer friendship (the hatch counter, for an egg)", Number(f.TrainerFriendship))
            : new("inspect-ot-friendship", "Friendship (original trainer)", OutOf(f.TrainerFriendship, 255)));
        rows.Add(new("inspect-handler", "Handling trainer", f.HandlerName is { } name ? $"{name} ({TrainerGender(f.HandlerGender)})" : "None"));
        rows.Add(new("inspect-handler-friendship", "Friendship (handling trainer)", OutOf(f.HandlerFriendship, 255)));
        rows.Add(new("inspect-current-handler", "Currently held by", f.CurrentHandler switch
        {
            0 => "Original trainer",
            1 => "Handling trainer",
            _ => Unknown(f.CurrentHandler),
        }));
        return new("inspect-origin", "Origin and trainer", null, rows);
    }

    private static InspectorSection Advanced(AdvancedFacts f) => new("inspect-advanced", "Advanced", null,
    [
        new("inspect-pid", "PID", Hex(f.Pid)),
        new("inspect-ec", "Encryption constant", Hex(f.EncryptionConstant)),
        new("inspect-ribbons", "Ribbons", Number(f.RibbonCount)),
        new("inspect-pokerus", "Pokérus", Pokerus(f.PokerusStrain, f.PokerusDays)),
        new("inspect-markings", "Markings", Markings(f.Markings)),
        new("inspect-format", "Format", f.Format),
        new("inspect-checksum", "Checksum", f.ChecksumValid ? "Valid" : "Invalid"),
    ]);

    /// <summary>
    /// Core's name; with a note when the value is outside this game's lists; "Unknown (stored value N)" when Core has no name for it.
    /// </summary>
    public static string Named(NamedValue value) => value switch
    {
        { Name: null } => Unknown(value.Value),
        { OutsideGameList: true } => $"{value.Name} (not available in this game)",
        _ => value.Name,
    };

    /// <summary>The species name followed by its national dex number.</summary>
    public static string Species(NamedValue species) => species.Name is null
        ? Unknown(species.Value)
        : string.Create(Invariant, $"{Named(species)} (No. {species.Value:000})");

    /// <summary>A move, or "Empty" for an empty slot.</summary>
    public static string Move(NamedValue move) => move.Value == 0 ? "Empty" : Named(move);

    /// <summary>The ability slot the stored ability number names.</summary>
    public static string AbilitySlot(int number) => number switch
    {
        1 => "first ability",
        2 => "second ability",
        4 => "hidden ability",
        _ => string.Create(Invariant, $"unknown slot, stored value {number}"),
    };

    /// <summary>The Pokérus state, as the summary screen tells it apart.</summary>
    public static string Pokerus(int strain, int days) => (strain, days) switch
    {
        (0, 0) => "Never infected",
        (_, 0) => string.Create(Invariant, $"Cured (strain {strain})"),
        _ => string.Create(Invariant, $"Infected (strain {strain}, {days} days left)"),
    };

    private static string Form(IdentityFacts f) => !f.HasForms && f.Form.Value == 0 ? "No alternate forms" : Named(f.Form);

    private static string Gender(byte gender) => gender switch
    {
        0 => "Male",
        1 => "Female",
        2 => "Genderless",
        _ => Unknown(gender),
    };

    private static string TrainerGender(byte gender) => gender switch
    {
        0 => "male",
        1 => "female",
        _ => Unknown(gender),
    };

    private static string Markings(IReadOnlyList<bool> markings)
    {
        var set = markings.Select((on, i) => on ? MarkingNames[i] : null).OfType<string>().ToArray();
        return set.Length == 0 ? "None" : string.Join(", ", set);
    }

    private static string YesNo(bool value) => value ? "Yes" : "No";

    private static string Number(long value) => value.ToString("N0", Invariant);

    private static string OutOf(long value, long max) => string.Create(Invariant, $"{value:N0} of {max:N0}");

    private static string Hex(uint value) => string.Create(Invariant, $"0x{value:X8}");

    private static string Date(DateOnly? date) => date?.ToString("yyyy-MM-dd", Invariant) ?? "Not a valid date";

    private static string Unknown(int value) => string.Create(Invariant, $"Unknown (stored value {value})");
}
