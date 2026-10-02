using System.Globalization;
using PKHeX.Web.State;

namespace PKHeX.Web.Components;

/// <summary>The text for applying a party member: what an edit keeps, and the preview of an HP reduction.</summary>
/// <remarks>Numbers are written with the invariant culture, as the inspector writes them.</remarks>
public static class PartyText
{
    /// <summary>
    /// Shown when a party member is opened: edits that do not affect stats keep its battle state as stored, and those that do recalculate
    /// its stats without healing it (<see cref="PartyStatPolicy"/>).
    /// </summary>
    public const string KeptOnEdit = "Name, language and friendship edits keep its stored stats, current HP and status. Level, experience and nature edits recalculate its stats with PKHeX.Core; its status is kept and its current HP is never raised.";

    /// <summary>
    /// The preview of an HP reduction before apply, or null when applying the draft does not lower the member's current HP.
    /// </summary>
    /// <remarks>Says that the status is kept and that nothing is healed, as <see cref="PartyStatPolicy"/> guarantees.</remarks>
    public static string? HpPreview(PartyHpChange? change)
    {
        if (change is not { IsReduction: true } reduction)
        {
            return null;
        }
        var lowered = reduction.IsFainted
            ? string.Create(CultureInfo.InvariantCulture, $"Applying this draft lowers current HP from {reduction.PreviousHp} to 0: this Pokémon will be fainted.")
            : string.Create(CultureInfo.InvariantCulture, $"Applying this draft lowers current HP from {reduction.PreviousHp} to {reduction.NewHp}.");
        var maximum = reduction.PreviousMax == reduction.NewMax
            ? ""
            : string.Create(CultureInfo.InvariantCulture, $" Maximum HP goes from {reduction.PreviousMax} to {reduction.NewMax}.");
        return lowered + maximum + " Its status is kept, and nothing is healed.";
    }
}
