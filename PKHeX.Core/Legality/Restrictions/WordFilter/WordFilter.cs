using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace PKHeX.Core;

/// <summary>
/// Bad-word Filter class containing logic to check against unsavory regular expressions.
/// </summary>
public static class WordFilter
{
    // if you're running this as a server and don't mind a few extra seconds of startup, add RegexOptions.Compiled for slightly better checking.
    private const RegexOptions Options = RegexOptions.CultureInvariant;

    internal static Regex[] LoadPatterns(ReadOnlySpan<char> patterns)
    {
        // Make it lowercase invariant
        Span<char> lowercase = stackalloc char[patterns.Length];
        patterns.ToLowerInvariant(lowercase);

        var lineCount = 1 + lowercase.Count('\n');
        var result = new Regex[lineCount];
        int i = 0;
        foreach (var line in lowercase.EnumerateLines())
            result[i++] = new Regex(line.ToString(), Options);
        return result;
    }

    /// <summary>
    /// Checks to see if a phrase contains filtered content.
    /// </summary>
    /// <param name="message">Phrase to check</param>
    /// <param name="regexes">Console regex set to check against.</param>
    /// <param name="regMatch">Matching regex that filters the phrase.</param>
    /// <returns>Boolean result if the message is filtered or not.</returns>
    internal static bool TryMatch(ReadOnlySpan<char> message, ReadOnlySpan<Regex> regexes, [NotNullWhen(true)] out string? regMatch)
    {
        // Clean the string
        Span<char> clean = stackalloc char[message.Length];
        int ctr = TextNormalizer.Normalize(message, clean);
        if (ctr != clean.Length)
            clean = clean[..ctr];

        foreach (var regex in regexes)
        {
            foreach (var _ in regex.EnumerateMatches(clean))
            {
                regMatch = regex.ToString();
                return true;
            }
        }
        regMatch = null;
        return false;
    }

    /// <inheritdoc cref="IsFiltered(ReadOnlySpan{char}, out string?, EntityContext, EntityContext)"/>
    public static bool IsFiltered(ReadOnlySpan<char> message, [NotNullWhen(true)] out string? regMatch,
        EntityContext current)
        => IsFiltered(message, out regMatch, current, current);

    /// <summary>
    /// Checks to see if a phrase contains filtered content.
    /// </summary>
    /// <param name="message">Phrase to check for</param>
    /// <param name="regMatch">Matching regex that filters the phrase.</param>
    /// <param name="current">Current context to check.</param>
    /// <param name="original">Earliest context to check.</param>
    /// <returns>Boolean result if the message is filtered or not.</returns>
    public static bool IsFiltered(ReadOnlySpan<char> message, [NotNullWhen(true)] out string? regMatch,
        EntityContext current, EntityContext original)
    {
        regMatch = null;
        if (message.IsWhiteSpace() || message.Length <= 1)
            return false;

        // Only check against the single filter if requested
        if (ParseSettings.Settings.WordFilter.DisableWordFilterPastGen)
            return IsFilteredCurrentOnly(message, ref regMatch, current, original);

        return IsFilteredLookBack(message, out regMatch, current, original);
    }

    private static bool IsFilteredCurrentOnly(ReadOnlySpan<char> message, [NotNullWhen(true)] ref string? regMatch,
        EntityContext current, EntityContext original) => current switch
    {
        EntityContext.Gen5 => WordFilter5.IsFiltered(message, out regMatch),

        EntityContext.Gen6 => WordFilter3DS.IsFilteredGen6(message, out regMatch),
        EntityContext.Gen7 when original is EntityContext.Gen6
                           => WordFilter3DS.IsFilteredGen6(message, out regMatch),

        EntityContext.Gen7 => WordFilter3DS.IsFilteredGen7(message, out regMatch),
        _ => current.GetConsole() switch
        {
            GameConsole.NX => WordFilterNX.IsFiltered(message, out regMatch, original),
            _ => false,
        },
    };

    private static bool IsFilteredLookBack(ReadOnlySpan<char> message, [NotNullWhen(true)] out string? regMatch,
        EntityContext current, EntityContext original)
    {
        // Switch 2 backwards transfer? Won't know for another couple years.
        if (WordFilterNX.IsFiltered(message, out regMatch, original))
            return true;

        var generation = original.Generation();
        if (generation > 7 || original is EntityContext.Gen7b)
            return false;
        if (WordFilter3DS.IsFiltered(message, out regMatch, original))
            return true;

        return generation == 5 && WordFilter5.IsFiltered(message, out regMatch);
        // no other word filters (none in Gen3 or Gen4)
    }
}
