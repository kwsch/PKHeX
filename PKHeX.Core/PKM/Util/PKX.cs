using System;

namespace PKHeX.Core;

/// <summary>
/// Common logic for <see cref="PKM"/> data providing and manipulation.
/// </summary>
public static class PKX
{
    internal static readonly PersonalTable Personal = PersonalTable.LA;
    public const int Generation = 8;
    public const EntityContext Context = EntityContext.Gen8a;

    /// <summary>
    /// Reorders (in place) the input array of stats to have the Speed value last rather than before the SpA/SpD stats.
    /// </summary>
    /// <param name="value">Input array to reorder</param>
    /// <returns>Same array, reordered.</returns>
    public static void ReorderSpeedLast(Span<int> value)
    {
        var spe = value[3];
        value[3] = value[4];
        value[4] = value[5];
        value[5] = spe;
    }
}
