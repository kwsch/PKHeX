using PKHeX.Core;

namespace PKHeX.Application.Services;

/// <summary>
/// Safe bounds-checked lookups into <see cref="GameInfo.Strings"/> arrays.
/// </summary>
/// <remarks>
/// Pre-Gen 3 Pokemon (PK1, PK2) return <c>Ability = -1</c> and <c>HeldItem = 0</c> because
/// those concepts did not exist in those generations. Direct indexing with a negative value
/// into <c>GameInfo.Strings.Ability[-1]</c> throws <see cref="IndexOutOfRangeException"/>,
/// which — when hidden by a broad try/catch — silently breaks the entire box display.
///
/// Use these helpers wherever a PKM property is used as an index into a string table.
/// Returns <see cref="string.Empty"/> for any out-of-range index (including negatives).
/// </remarks>
public static class StringResourceLookup
{
    public static string Species(ushort id)
        => Lookup(GameInfo.Strings.Species, id);

    public static string Ability(int id)
        => Lookup(GameInfo.Strings.Ability, id);

    public static string Item(int id)
        => Lookup(GameInfo.Strings.Item, id);

    public static string Move(int id)
        => Lookup(GameInfo.Strings.Move, id);

    public static string Nature(int id)
        => Lookup(GameInfo.Strings.Natures, id);

    /// <summary>
    /// Generic safe lookup — returns <see cref="string.Empty"/> for negative indices or
    /// indices beyond the list size.
    /// </summary>
    public static string Lookup(IReadOnlyList<string> list, int index)
    {
        if (list is null)
            return string.Empty;
        if ((uint)index >= (uint)list.Count)
            return string.Empty;
        return list[index] ?? string.Empty;
    }
}
