using System.Globalization;
using PKHeX.Core;
using PKHeX.Web.Services;
using PKHeX.Web.State;

namespace PKHeX.Web.Components;

/// <summary>
/// The text and layout used to show party positions, box slots and boxes.
/// </summary>
/// <remarks>
/// Labels are built from the position alone, plus what Core reads there, so they never depend on earlier state. Numbers are one-based
/// and formatted with the invariant culture. Names stored in the save are returned as they are; the UI renders them as text.
/// </remarks>
public static class SlotText
{
    private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

    /// <summary>Columns of a box, as the X/Y and Omega Ruby/Alpha Sapphire PC shows it (6 by 5).</summary>
    public const int BoxColumns = 6;

    /// <summary>Columns of the party, as the games' party screen shows it (2 by 3).</summary>
    public const int PartyColumns = 2;

    /// <summary>Columns used to lay out the grid that holds <paramref name="slot"/>.</summary>
    public static int Columns(SlotRef slot) => slot.IsParty ? PartyColumns : BoxColumns;

    /// <summary>The position, e.g. "Party position 2" or "Box 3, slot 7 (row 2, column 1)".</summary>
    public static string Position(SlotRef slot)
    {
        if (slot.IsParty)
        {
            return string.Create(Invariant, $"Party position {slot.Slot + 1}");
        }
        var row = (slot.Slot / BoxColumns) + 1;
        var column = (slot.Slot % BoxColumns) + 1;
        return string.Create(Invariant, $"Box {slot.Box + 1}, slot {slot.Slot + 1} (row {row}, column {column})");
    }

    /// <summary>
    /// What the position holds, as shown in the slot: the species, "Egg", "Empty" or "Bad egg" (data that fails the game's checks).
    /// </summary>
    public static string Short(SlotSummary summary) => summary switch
    {
        { Occupied: false } => "Empty",
        { Readable: false } => "Bad egg",
        { IsEgg: true } => "Egg",
        _ => Species(summary.Species),
    };

    /// <summary>
    /// What the position holds, in full: <see cref="Short"/> plus the nickname and "shiny" when they apply,
    /// e.g. <c>Zigzagoon "Ziggy", shiny</c>. An egg's name is the game's own, so it is not repeated.
    /// </summary>
    public static string Contents(SlotSummary summary)
    {
        var text = Short(summary);
        if (!summary.CanOpen)
        {
            return text;
        }
        if (summary is { IsEgg: false, Nickname: { } nickname })
        {
            text += $" \"{DisplayText.Embed(nickname)}\"";
        }
        return summary.IsShiny ? text + ", shiny" : text;
    }

    /// <summary>The accessible name of a slot: position, then contents, e.g. <c>Box 1, slot 1 (row 1, column 1): Zigzagoon</c>.</summary>
    public static string Label(SlotSummary summary) => $"{Position(summary.Ref)}: {Contents(summary)}";

    /// <summary>The box's stored name, or Core's numbered default when it stores none.</summary>
    /// <param name="box">The zero-based box index.</param>
    /// <param name="storedName">The name stored in the save, or null (see <see cref="StorageView.BoxName"/>).</param>
    /// <remarks>
    /// A stored name is shown without bidirectional controls; one made only of them is treated as blank (<see cref="DisplayText.PlainOrNull"/>).
    /// </remarks>
    public static string BoxTitle(int box, string? storedName) => DisplayText.PlainOrNull(storedName) ?? BoxDetailNameExtensions.GetDefaultBoxName(box);

    /// <summary>The box selector entry: the box number, then its title, so boxes with equal names stay distinct.</summary>
    /// <inheritdoc cref="BoxTitle" path="/param"/>
    /// <remarks>A stored name is isolated (<see cref="DisplayText.Embed"/>), so it cannot reorder the number before it.</remarks>
    public static string BoxOption(int box, string? storedName) =>
        string.Create(Invariant, $"{box + 1}. {(DisplayText.PlainOrNull(storedName) is { } shown ? DisplayText.Embed(shown) : BoxTitle(box, null))}");

    /// <summary>Core's English species name, or a label with the stored value when it is outside Core's list.</summary>
    private static string Species(ushort species)
    {
        var names = GameInfo.Strings.specieslist;
        return species < names.Length
            ? names[species]
            : string.Create(Invariant, $"Unknown species (stored value {species})");
    }
}
