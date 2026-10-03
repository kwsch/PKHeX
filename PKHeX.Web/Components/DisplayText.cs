using System.Text;

namespace PKHeX.Web.Components;

/// <summary>
/// Prepares names stored in a save (nicknames, trainer and box names) and file names for display, so that a hostile name cannot reorder the
/// text around it.
/// </summary>
/// <remarks>
/// Names are always rendered as text, so markup in them stays inert. What text rendering does not contain is bidirectional control: an
/// override such as U+202E in a nickname reverses the rest of its line, including the app's own words. For display only, every Unicode
/// Bidi_Control character is removed (<see cref="Plain"/>), and a name placed inside a sentence or label is also wrapped in a first-strong
/// isolate (<see cref="Embed"/>), so a right-to-left name keeps its own direction without moving its neighbours. With the controls removed,
/// nothing in the name can close the isolate early. The draft keeps the stored name unchanged; the nickname box shows it as stored.
/// </remarks>
public static class DisplayText
{
    /// <summary>First strong isolate: the enclosed text takes its direction from its first strong character.</summary>
    public const char FirstStrongIsolate = '\u2068';

    /// <summary>Pop directional isolate: ends <see cref="FirstStrongIsolate"/>.</summary>
    public const char PopIsolate = '\u2069';

    /// <summary>
    /// True for the Unicode Bidi_Control characters: the Arabic letter mark, the left-to-right and right-to-left marks, the embeddings,
    /// overrides and their pop, and the isolates and their pop.
    /// </summary>
    public static bool IsBidiControl(char c) => c is '\u061C' or '\u200E' or '\u200F' or (>= '\u202A' and <= '\u202E') or (>= '\u2066' and <= '\u2069');

    /// <summary>
    /// <paramref name="name"/> without Bidi_Control characters, for display on its own (a value in its own element). Null stays null.
    /// </summary>
    public static string? Plain(string? name)
    {
        if (name is null || !name.Any(IsBidiControl))
        {
            return name;
        }
        var kept = new StringBuilder(name.Length);
        foreach (var c in name)
        {
            if (!IsBidiControl(c))
            {
                kept.Append(c);
            }
        }
        return kept.ToString();
    }

    /// <summary>
    /// <paramref name="name"/> without Bidi_Control characters, or null when nothing but whitespace would be left, so a stored name made only of
    /// controls falls back to the same label as a blank one.
    /// </summary>
    public static string? PlainOrNull(string? name) => Plain(name) is { } shown && !string.IsNullOrWhiteSpace(shown) ? shown : null;

    /// <summary>
    /// <paramref name="name"/> without Bidi_Control characters, wrapped in a first-strong isolate, for display inside a sentence or label.
    /// It is stable: embedding an embedded name returns it unchanged.
    /// </summary>
    public static string Embed(string name)
    {
        if (name.Length >= 2 && name[0] == FirstStrongIsolate && name[^1] == PopIsolate && !name.AsSpan(1, name.Length - 2).ContainsAny(BidiControls))
        {
            return name;
        }
        return $"{FirstStrongIsolate}{Plain(name)}{PopIsolate}";
    }

    /// <summary>Every Bidi_Control character, for span searches.</summary>
    private static readonly System.Buffers.SearchValues<char> BidiControls = System.Buffers.SearchValues.Create("\u061C\u200E\u200F\u202A\u202B\u202C\u202D\u202E\u2066\u2067\u2068\u2069");
}
