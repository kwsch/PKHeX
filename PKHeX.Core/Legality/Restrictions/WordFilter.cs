using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
    private static readonly Regex[] RegexesSwitch = LoadPatterns(Util.GetStringResource("badwords_switch"));

    /// <inheritdoc cref="RegexesSwitch"/>
    private static readonly Regex[] Regexes3DS = LoadPatterns(Util.GetStringResource("badwords_3ds"));

    /// <summary>
    /// Strings to check against
    /// </summary>
    private static readonly HashSet<string>.AlternateLookup<ReadOnlySpan<char>> WordsGen5 = new HashSet<string>(Util.GetStringList("badwords_gen5")).GetAlternateLookup<ReadOnlySpan<char>>();

    // if you're running this as a server and don't mind a few extra seconds of startup, add RegexOptions.Compiled for slightly better checking.
    private const RegexOptions Options = RegexOptions.CultureInvariant;

    private static Regex[] LoadPatterns(ReadOnlySpan<char> patterns)
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
    /// <param name="message">Phrase to check for</param>
    /// <param name="regMatch">Matching regex that filters the phrase.</param>
    /// <param name="console">Console to check against.</param>
    /// <returns>Boolean result if the message is filtered or not.</returns>
    public static bool TryMatch(ReadOnlySpan<char> message, [NotNullWhen(true)] out string? regMatch, Regex[] regexes)
    {
        // Clean the string
        Span<char> clean = stackalloc char[message.Length];
        NormalizeString(message, clean);

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

    private const string Dakuten = "ｶｷｸｹｺｻｼｽｾｿﾀﾁﾂﾃﾄﾊﾋﾌﾍﾎ"; // 'ｳ' handled separately
    private const string Handakuten = "ﾊﾋﾌﾍﾎ";
    private const string FullwidthKana = "ヲァィゥェォャュョッーアイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワン";
    private const string SmallKana = "ァィゥェォッャュョヮ"; // 'ヵ', 'ヶ' handled separately

    private static void NormalizeString(ReadOnlySpan<char> message, Span<char> clean)
    {
        for (int i = 0, j = 0; i < message.Length; i++)
        {
            var c = message[i];

            // Skip spaces and halfwidth dakuten/handakuten
            if (c is ' ' or '\u3000' or 'ﾞ' or 'ﾟ')
                continue;

            // Handle combining halfwidth dakuten/handakuten
            ushort ofs = 0;
            if (c is >= 'ｦ' and <= 'ﾝ' && i + 1 < message.Length)
            {
                var d = message[i + 1];
                if (d == 'ﾞ' && Dakuten.Contains(c))
                    ofs = 1;
                else if (d == 'ﾟ' && Handakuten.Contains(c))
                    ofs = 2;
                else if (d == 'ﾞ' && c == 'ｳ')
                    ofs = 'ヴ' - 'ウ';
            }

            // Fold characters treated identically
            c = Char.ToLowerInvariant(c); // fold to lowercase
            c = (char)(c switch
            {
                >= 'ぁ' and <= 'ゖ' => c + 0x60, // shift hiragana to katakana
                >= '０' and <= '９' or >= 'ａ' and <= 'ｚ' => c - 0xFEE0, // shift fullwidth numbers/letters to halfwidth
                >= 'ｦ' and <= 'ﾝ' => FullwidthKana[c - 'ｦ'] + ofs, // shift halfwidth katakana to fullwidth
                _ => c,
            });

            // Shift small kana to normal kana
            if (c is >= 'ァ' and <= 'ヶ')
            {
                if (SmallKana.Contains(c))
                    c += (char)1;
                else if (c == 'ヵ')
                    c = 'カ';
                else if (c == 'ヶ')
                    c = 'ケ';
            }

            clean[j] = c;
            j++;
        }
    }

    private static void NormalizeStringGen5(ReadOnlySpan<char> message, Span<char> clean)
    {
        for (int i = 0; i < message.Length; i++)
        {
            var c = message[i];
            c = Char.ToUpperInvariant(c); // fold to uppercase
            c = (char)(c switch
            {
                >= 'ァ' and <= 'ヶ' => c - 0x60, // shift katakana to hiragana
                >= 'Ａ' and <= 'Ｚ' => c - 0xFEE0, // shift fullwidth letters to halfwidth
                _ => c,
            });
            clean[i] = c;
        }
    }

    /// <summary>
    /// Due to some messages repeating (Trainer names), keep a list of repeated values for faster lookup.
    /// </summary>
    private static readonly ConcurrentDictionary<string, string?>.AlternateLookup<ReadOnlySpan<char>> LookupSwitch =
        new ConcurrentDictionary<string, string?>().GetAlternateLookup<ReadOnlySpan<char>>();

    /// <inheritdoc cref="LookupSwitch"/>
    private static readonly ConcurrentDictionary<string, string?>.AlternateLookup<ReadOnlySpan<char>> Lookup3DS =
        new ConcurrentDictionary<string, string?>().GetAlternateLookup<ReadOnlySpan<char>>();

    private static bool IsSpeciesName(ReadOnlySpan<char> message, EntityContext context = PKX.Context)
    {
        var generation = context.Generation();
        var isSpeciesName = generation is >= 3 and <= 6
            ? SpeciesName.TryGetSpeciesAnyLanguage(message, out ushort species, generation)
            : SpeciesName.TryGetSpeciesAnyLanguageCaseInsensitive(message, out species, generation);
        return isSpeciesName && species <= GameUtil.GetMaxSpeciesID(context.GetSingleGameVersion());
    }

    private static bool IsFilteredGen5(ReadOnlySpan<char> message, [NotNullWhen(true)] out string? match)
    {
        Span<char> clean = stackalloc char[message.Length];
        NormalizeStringGen5(message, clean);
        if (WordsGen5.TryGetValue(clean, out match))
        {
            match = clean.ToString();
            return true;
        }
        return false;
    }

    private static bool IsFilteredRegex(ReadOnlySpan<char> message, [NotNullWhen(true)] out string? regMatch, EntityContext context = PKX.Context)
    {
        var console = context.GetConsole();
        var lookup = console switch
        {
            GameConsole._3DS => Lookup3DS,
            GameConsole.NX => LookupSwitch,
            _ => throw new ArgumentOutOfRangeException(nameof(context)),
        };
        var regexes = console switch
        {
            GameConsole._3DS => Regexes3DS,
            GameConsole.NX => RegexesSwitch,
            _ => throw new ArgumentOutOfRangeException(nameof(context)),
        };

        // Check if species name
        if (IsSpeciesName(message, context))
        {
            regMatch = null;
            return false;
        }

        // Check dictionary
        if (lookup.TryGetValue(message, out regMatch))
            return regMatch != null;

        // not in dictionary, check patterns
        if (TryMatch(message, out regMatch, regexes))
        {
            lookup.TryAdd(message, regMatch);
            return true;
        }

        // didn't match any pattern, cache result
        if ((lookup.Dictionary.Count & ~MAX_COUNT) != 0)
            lookup.Dictionary.Clear(); // reset
        lookup.TryAdd(message, regMatch = null);
        return false;
    }

    /// <summary>
    /// Checks to see if a phrase contains filtered content.
    /// </summary>
    /// <param name="message">Phrase to check for</param>
    /// <param name="regMatch">Matching regex that filters the phrase.</param>
    /// <returns>Boolean result if the message is filtered or not.</returns>
    public static bool IsFiltered(ReadOnlySpan<char> message, [NotNullWhen(true)] out string? regMatch, EntityContext context = PKX.Context, bool current = false)
    {
        regMatch = null;
        if (message.IsWhiteSpace() || message.Length <= 1)
            return false;

        var generation = context.Generation();
        if (current || ParseSettings.Settings.WordFilter.DisableWordFilterPastGen)
        {
            // Only check against the contemporaneous filter
            return generation switch
            {
                <5 => false,
                5 => IsFilteredGen5(message, out regMatch),
                >= 6 => IsFilteredRegex(message, out regMatch, context),
            };
        }

        if (IsFilteredRegex(message, out regMatch, context.GetConsole() is GameConsole.NX ? context : EntityContext.Gen9)) // Switch
            return true;
        if (generation is <= 7 && context is not EntityContext.Gen7b && IsFilteredRegex(message, out regMatch, context.GetConsole() is GameConsole._3DS ? context : EntityContext.Gen7)) // 3DS
            return true;
        if (generation is 5 && IsFilteredGen5(message, out regMatch))
            return true;
        return false;
    }

    private const int MAX_COUNT = (1 << 17) - 1; // arbitrary cap for max dictionary size
}
