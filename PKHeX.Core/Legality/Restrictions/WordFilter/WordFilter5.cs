using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

public static class WordFilter5
{
    private static readonly HashSet<string>.AlternateLookup<ReadOnlySpan<char>> Words =
        new HashSet<string>(Util.GetStringList("badwords_gen5"))
            .GetAlternateLookup<ReadOnlySpan<char>>();

    /// <summary>
    /// Checks to see if a phrase contains filtered content.
    /// </summary>
    /// <param name="message">Phrase to check</param>
    /// <param name="match">Blocked word that filters the phrase.</param>
    /// <returns>Boolean result if the message is filtered or not.</returns>
    public static bool IsFiltered(ReadOnlySpan<char> message, [NotNullWhen(true)] out string? match)
    {
        Span<char> clean = stackalloc char[message.Length];
        Normalize(message, clean);
        return Words.TryGetValue(clean, out match);
    }

    /// <summary>
    /// Normalize a string to a simplified form for checking against a bad-word list.
    /// </summary>
    /// <param name="input">Input string to normalize</param>
    /// <param name="output">Output buffer to write the normalized string</param>
    public static void Normalize(ReadOnlySpan<char> input, Span<char> output)
    {
        Debug.Assert(input.Length == output.Length);
        for (int i = 0; i < input.Length; i++)
        {
            var c = input[i];
            c = char.ToUpperInvariant(c); // fold to uppercase
            c = (char)(c switch
            {
                >= 'ァ' and <= 'ヶ' => c - 0x60, // shift katakana to hiragana
                >= 'Ａ' and <= 'Ｚ' => c - 0xFEE0, // shift fullwidth letters to halfwidth
                _ => c,
            });
            output[i] = c;
        }
    }
}
