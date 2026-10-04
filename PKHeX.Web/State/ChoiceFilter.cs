using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using PKHeX.Core;

namespace PKHeX.Web.State;

/// <summary>What <see cref="ChoiceFilter.Filter"/> kept of a list.</summary>
/// <param name="Items">The entries to offer, in the list's order: the matches, plus the drafted value and "(None)" whether or not they match.</param>
/// <param name="Matches">How many distinct values other than "(None)" match the search; every one when the search is blank.</param>
/// <param name="Total">How many distinct values other than "(None)" the whole list holds.</param>
public sealed record ChoiceFilterResult(IReadOnlyList<ComboItem> Items, int Matches, int Total);

/// <summary>
/// Narrows one of Core's long lists (moves, held items) to the entries whose names contain a search, for the editor's searchable boxes.
/// </summary>
/// <remarks>
/// <para>
/// A name matches when it contains the search after both are folded: case and accents are ignored, and only letters and digits count, so
/// "poke" finds "Poké Ball", "uturn" finds "U-turn" and "kings rock" finds "King's Rock". Folding is done here rather than through culture
/// collation, so it does not depend on which globalization data the browser runtime has loaded.
/// </para>
/// <para>
/// Filtering never changes a choice: the drafted value is always kept, so the box goes on showing it, and so is "(None)" (value 0), so a
/// move slot or the held item can still be cleared. The list's own order (Core's) is kept.
/// </para>
/// </remarks>
public static class ChoiceFilter
{
    /// <summary>Each list's folded names, worked out once per list; the lists live for their session.</summary>
    private static readonly ConditionalWeakTable<IReadOnlyList<ComboItem>, Folded> Cache = [];

    /// <summary>The value of "(None)", which every move and item list starts with.</summary>
    public const int None = 0;

    /// <summary>
    /// The entries of <paramref name="items"/> whose names contain <paramref name="query"/>, plus the entries for <paramref name="keep"/>
    /// and <see cref="None"/>. A search with no letter or digit keeps <paramref name="items"/> itself, the same instance, so the box is not
    /// rebuilt.
    /// </summary>
    /// <param name="items">The list, from the session's Core lists.</param>
    /// <param name="query">What the user typed, or null.</param>
    /// <param name="keep">The drafted value, which is always kept.</param>
    public static ChoiceFilterResult Filter(IReadOnlyList<ComboItem> items, string? query, int keep)
    {
        var folded = Cache.GetValue(items, static list => new Folded(list));
        var search = Fold(query);
        if (search.Length == 0)
        {
            return new ChoiceFilterResult(items, folded.Total, folded.Total);
        }
        var kept = new List<ComboItem>();
        var matched = new HashSet<int>();
        for (var i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var match = folded.Names[i].Contains(search, StringComparison.Ordinal);
            if (match && item.Value != None)
            {
                matched.Add(item.Value);
            }
            if (match || item.Value == keep || item.Value == None)
            {
                kept.Add(item);
            }
        }
        return new ChoiceFilterResult(kept, matched.Count, folded.Total);
    }

    /// <summary>
    /// <paramref name="text"/> with accents removed (canonical decomposition, then dropping non-spacing marks), letters upper-cased in the
    /// invariant culture, and everything but letters and digits dropped.
    /// </summary>
    public static string Fold(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return "";
        }
        var decomposed = text.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(decomposed.Length);
        foreach (var c in decomposed)
        {
            if (char.IsLetterOrDigit(c) && CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(char.ToUpperInvariant(c));
            }
        }
        return builder.ToString();
    }

    /// <summary>A list's folded names, by index, and its count of distinct values other than "(None)".</summary>
    private sealed class Folded(IReadOnlyList<ComboItem> items)
    {
        public string[] Names { get; } = [.. items.Select(i => Fold(i.Text))];

        public int Total { get; } = items.Select(i => i.Value).Where(v => v != None).Distinct().Count();
    }
}
