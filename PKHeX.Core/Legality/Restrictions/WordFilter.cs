using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace PKHeX.Core;

/// <summary>
/// Bad-word Filter class containing logic to check against unsavory regular expressions.
/// </summary>
public static class WordFilter
{
    /// <summary>
    /// Regex patterns to check against
    /// </summary>
    /// <remarks>No need to keep the original pattern strings around; the <see cref="Regex"/> object retrieves this via <see cref="Regex.ToString()"/></remarks>
    private static readonly Regex[] Regexes = LoadPatterns(Util.GetStringResource("badwords"));

    // if you're running this as a server and don't mind a few extra seconds of startup, add RegexOptions.Compiled for slightly better checking.
    private const RegexOptions Options = RegexOptions.CultureInvariant;

    private static Regex[] LoadPatterns(ReadOnlySpan<char> patterns)
    {
        var lineCount = 1 + patterns.Count('\n');
        var result = new Regex[lineCount];
        int i = 0;
        foreach (var line in patterns.EnumerateLines())
            result[i++] = new Regex(line.ToString(), Options);
        return result;
    }

    /// <summary>
    /// Checks to see if a phrase contains filtered content.
    /// </summary>
    /// <param name="message">Phrase to check for</param>
    /// <param name="regMatch">Matching regex that filters the phrase.</param>
    /// <returns>Boolean result if the message is filtered or not.</returns>
    public static bool TryMatch(ReadOnlySpan<char> message, [NotNullWhen(true)] out string? regMatch)
    {
        foreach (var regex in Regexes)
        {
            foreach (var _ in regex.EnumerateMatches(message))
            {
                regMatch = regex.ToString();
                return true;
            }
        }
        regMatch = null;
        return false;
    }

    /// <summary>
    /// Due to some messages repeating (Trainer names), keep a list of repeated values for faster lookup.
    /// </summary>
    private static readonly ConcurrentDictionary<string, string?>.AlternateLookup<ReadOnlySpan<char>> Lookup =
        new ConcurrentDictionary<string, string?>().GetAlternateLookup<ReadOnlySpan<char>>();

    /// <summary>
    /// Checks to see if a phrase contains filtered content.
    /// </summary>
    /// <param name="message">Phrase to check for</param>
    /// <param name="regMatch">Matching regex that filters the phrase.</param>
    /// <returns>Boolean result if the message is filtered or not.</returns>
    public static bool IsFiltered(ReadOnlySpan<char> message, [NotNullWhen(true)] out string? regMatch)
    {
        if (message.IsWhiteSpace() || message.Length <= 1)
        {
            regMatch = null;
            return false;
        }

        // Check dictionary
        if (Lookup.TryGetValue(message, out regMatch))
            return regMatch != null;

        // Make the string lowercase invariant
        Span<char> lowercase = stackalloc char[message.Length];
        message.ToLowerInvariant(lowercase);

        // not in dictionary, check patterns
        if (TryMatch(lowercase, out regMatch))
        {
            Lookup.TryAdd(message, regMatch);
            return true;
        }

        // didn't match any pattern, cache result
        if ((Lookup.Dictionary.Count & ~MAX_COUNT) != 0)
            Lookup.Dictionary.Clear(); // reset
        Lookup.TryAdd(message, regMatch = null);
        return false;
    }

    private const int MAX_COUNT = (1 << 17) - 1; // arbitrary cap for max dictionary size
}
