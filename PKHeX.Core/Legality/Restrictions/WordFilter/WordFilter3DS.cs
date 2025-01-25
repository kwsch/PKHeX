using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace PKHeX.Core;

public static class WordFilter3DS
{
    private static readonly Regex[] Regexes = WordFilter.LoadPatterns(Util.GetStringResource("badwords_3ds"));

    /// <summary>
    /// Regex patterns to check against
    /// </summary>
    /// <remarks>No need to keep the original pattern strings around; the <see cref="Regex"/> object retrieves this via <see cref="Regex.ToString()"/></remarks>
    private static readonly ConcurrentDictionary<string, string?>.AlternateLookup<ReadOnlySpan<char>> Lookup =
        new ConcurrentDictionary<string, string?>().GetAlternateLookup<ReadOnlySpan<char>>();

    private const int MAX_COUNT = (1 << 17) - 1; // arbitrary cap for max dictionary size

    public static bool IsFilteredGen6(ReadOnlySpan<char> message, [NotNullWhen(true)] out string? regMatch)
        => IsFiltered(message, out regMatch, EntityContext.Gen6);

    public static bool IsFilteredGen7(ReadOnlySpan<char> message, [NotNullWhen(true)] out string? regMatch)
        => IsFiltered(message, out regMatch, EntityContext.Gen7);

    public static bool IsFiltered(ReadOnlySpan<char> message, [NotNullWhen(true)] out string? regMatch, EntityContext original)
    {
        regMatch = null;
        if (IsSpeciesName(message, original))
            return false;

        // Check dictionary
        if (Lookup.TryGetValue(message, out regMatch))
            return regMatch != null;

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

    public static bool IsSpeciesName(ReadOnlySpan<char> message, EntityContext original)
    {
        // Gen6 is case-sensitive, Gen7 is case-insensitive.
        if (original is EntityContext.Gen6) // Match case
            return IsSpeciesNameGen6(message);
        return IsSpeciesNameGen7(message);
    }

    private static bool IsSpeciesNameGen7(ReadOnlySpan<char> message)
    {
        if (!SpeciesName.TryGetSpeciesAnyLanguageCaseInsensitive(message, out var s7, 7))
            return false;
        return s7 <= Legal.MaxSpeciesID_7_USUM;
    }

    private static bool IsSpeciesNameGen6(ReadOnlySpan<char> message)
    {
        if (!SpeciesName.TryGetSpeciesAnyLanguage(message, out var s6, 6))
            return false;
        return s6 <= Legal.MaxSpeciesID_6;
    }
}
