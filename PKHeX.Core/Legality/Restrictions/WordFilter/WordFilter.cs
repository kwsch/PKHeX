using System;
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
    internal static bool TryMatch(ReadOnlySpan<char> message, ReadOnlySpan<Regex> regexes, out int regMatch)
    {
        // Clean the string
        Span<char> clean = stackalloc char[message.Length];
        int ctr = TextNormalizer.Normalize(message, clean);
        if (ctr != clean.Length)
            clean = clean[..ctr];

        for (var i = 0; i < regexes.Length; i++)
        {
            var regex = regexes[i];
            foreach (var _ in regex.EnumerateMatches(clean))
            {
                regMatch = i;
                return true;
            }
        }

        regMatch = -1;
        return false;
    }

    /// <inheritdoc cref="IsFiltered(ReadOnlySpan{char}, EntityContext, EntityContext, out WordFilterType, out int)"/>
    public static bool IsFiltered(ReadOnlySpan<char> message, EntityContext current, out WordFilterType type, out int regMatch)
        => IsFiltered(message, current, current, out type, out regMatch);

    /// <summary>
    /// Checks to see if a phrase contains filtered content.
    /// </summary>
    /// <param name="message">Phrase to check for</param>
    /// <param name="current">Current context to check.</param>
    /// <param name="original">Earliest context to check.</param>
    /// <param name="type">Word filter set that matched the phrase.</param>
    /// <param name="regMatch">Matching regex that filters the phrase.</param>
    /// <returns>Boolean result if the message is filtered or not.</returns>
    public static bool IsFiltered(ReadOnlySpan<char> message, EntityContext current, EntityContext original, out WordFilterType type, out int regMatch)
    {
        regMatch = -1;
        if (message.IsWhiteSpace() || message.Length <= 1)
        {
            type = WordFilterType.None;
            return false;
        }

        // Only check against the single filter if requested
        if (ParseSettings.Settings.WordFilter.DisableWordFilterPastGen)
        {
            if (IsFilteredCurrentOnly(message, current, original, out regMatch))
            {
                type = WordFilterTypeExtensions.GetName(current);
                return true;
            }
            type = WordFilterType.None;
            return false;
        }

        return IsFilteredLookBack(message, current, original, out type, out regMatch);
    }

    private static bool IsFilteredCurrentOnly(ReadOnlySpan<char> message, EntityContext current, EntityContext original, out int regMatch)
    {
        regMatch = 0;
        return current switch
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
    }

    private static bool IsFilteredLookBack(ReadOnlySpan<char> message, EntityContext current, EntityContext original, out WordFilterType type, out int regMatch)
    {
        // Switch 2 backwards transfer? Won't know for another couple years.
        if (WordFilterNX.IsFiltered(message, out regMatch, original))
        {
            type = WordFilterType.NintendoSwitch;
            return true;
        }

        var generation = original.Generation();
        if (generation > 7 || original is EntityContext.Gen7b)
        {
            type = WordFilterType.None;
            return false;
        }

        if (WordFilter3DS.IsFiltered(message, out regMatch, original))
        {
            type = WordFilterType.Nintendo3DS;
            return true;
        }

        if (generation == 5 && WordFilter5.IsFiltered(message, out regMatch))
        {
            type = WordFilterType.Gen5;
            return true;
        }
        // no other word filters (none in Gen3 or Gen4)
        type = WordFilterType.None;
        return false;
    }

    public static string GetPattern(WordFilterType chkArgument, int index) => chkArgument switch
    {
        WordFilterType.Gen5 => WordFilter5.GetPattern(index),
        WordFilterType.Nintendo3DS => WordFilter3DS.GetPattern(index),
        WordFilterType.NintendoSwitch => WordFilterNX.GetPattern(index),
        _ => string.Empty,
    };
}
