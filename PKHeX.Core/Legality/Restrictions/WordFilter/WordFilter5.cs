using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PKHeX.Core;

public static class WordFilter5
{
    private static readonly string[] BadWords = Util.GetStringList("badwords_gen5");
    private static readonly Dictionary<string, int>.AlternateLookup<ReadOnlySpan<char>> Words = GetDictionary(BadWords)
            .GetAlternateLookup<ReadOnlySpan<char>>();

    public static string GetPattern(int index) => BadWords[index];

    private static Dictionary<string, int> GetDictionary(ReadOnlySpan<string> input)
    {
        var result = new Dictionary<string, int>(input.Length);
        for (int i = 0; i < input.Length; i++)
            result[input[i]] = i;
        return result;
    }

    /// <summary>
    /// Checks to see if a phrase contains filtered content.
    /// </summary>
    /// <param name="message">Phrase to check</param>
    /// <param name="match">Blocked word that filters the phrase.</param>
    /// <returns>Boolean result if the message is filtered or not.</returns>
    public static bool IsFiltered(ReadOnlySpan<char> message, out int match)
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
