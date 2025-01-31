using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace PKHeX.Core;

/// <summary>
/// Word filter for Switch games.
/// </summary>
public static class WordFilterNX
{
    /// <summary>
    /// Regex patterns to check against
    /// </summary>
    /// <remarks>No need to keep the original pattern strings around; the <see cref="Regex"/> object retrieves this via <see cref="Regex.ToString()"/></remarks>
    private static readonly Regex[] Regexes = WordFilter.LoadPatterns(Util.GetStringResource("badwords_switch"));

    /// <summary>
    /// Due to some messages repeating (Trainer names), keep a list of repeated values for faster lookup.
    /// </summary>
    private static readonly ConcurrentDictionary<string, string?>.AlternateLookup<ReadOnlySpan<char>> Lookup =
        new ConcurrentDictionary<string, string?>().GetAlternateLookup<ReadOnlySpan<char>>();

    private const int MAX_COUNT = (1 << 17) - 1; // arbitrary cap for max dictionary size

    /// <summary>
    /// Checks to see if a phrase contains filtered content.
    /// </summary>
    /// <param name="message">Phrase to check</param>
    /// <param name="regMatch">Matching regex that filters the phrase.</param>
    /// <param name="original">Earliest context to check.</param>
    /// <returns>Boolean result if the message is filtered or not.</returns>
    public static bool IsFiltered(ReadOnlySpan<char> message, [NotNullWhen(true)] out string? regMatch, EntityContext original)
    {
        regMatch = null;
        if (IsSpeciesName(message, original))
            return false;

        // Check dictionary
        if (Lookup.TryGetValue(message, out regMatch))
            return regMatch is not null;

        // not in dictionary, check patterns
        if (WordFilter.TryMatch(message, Regexes, out regMatch))
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

    private static bool IsSpeciesName(ReadOnlySpan<char> message, EntityContext origin)
    {
        var gen = origin.Generation();
        if (!SpeciesName.TryGetSpeciesAnyLanguageCaseInsensitive(message, out var species, gen))
            return false;
        return species <= origin.GetSingleGameVersion().GetMaxSpeciesID();
    }
}
