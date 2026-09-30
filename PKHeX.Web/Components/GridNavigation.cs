namespace PKHeX.Web.Components;

/// <summary>
/// Keyboard movement within a grid of slots laid out in rows, following the ARIA grid pattern: no wrapping at the edges.
/// </summary>
public static class GridNavigation
{
    /// <summary>
    /// The index a key moves to from <paramref name="index"/>: arrows move one slot, Home and End go to the ends of the row, and
    /// Ctrl+Home and Ctrl+End to the first and last slot. Any other key, a move past an edge, or a key held with Alt or Meta (which
    /// browsers and systems use for their own shortcuts, and grid-keys.js leaves alone) returns <paramref name="index"/>.
    /// </summary>
    /// <param name="key">The <c>KeyboardEvent.key</c> value.</param>
    /// <param name="ctrl">True when Ctrl is held.</param>
    /// <param name="altOrMeta">True when Alt or Meta is held.</param>
    /// <param name="index">The slot that has focus.</param>
    /// <param name="count">Number of slots.</param>
    /// <param name="columns">Slots per row.</param>
    public static int Target(string key, bool ctrl, bool altOrMeta, int index, int count, int columns)
    {
        if (altOrMeta)
        {
            return index;
        }
        var last = count - 1;
        var rowStart = index - (index % columns);
        return key switch
        {
            "ArrowRight" when index < last && (index % columns) < columns - 1 => index + 1,
            "ArrowLeft" when index % columns > 0 => index - 1,
            "ArrowDown" when index + columns <= last => index + columns,
            "ArrowUp" when index - columns >= 0 => index - columns,
            "Home" when ctrl => 0,
            "End" when ctrl => last,
            "Home" => rowStart,
            "End" => Math.Min(rowStart + columns - 1, last),
            _ => index,
        };
    }
}
