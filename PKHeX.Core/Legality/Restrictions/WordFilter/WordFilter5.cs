using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

public static class WordFilter5
{
    private static readonly HashSet<string>.AlternateLookup<ReadOnlySpan<char>> Words =
        new HashSet<string>(Util.GetStringList("badwords_gen5"))
            .GetAlternateLookup<ReadOnlySpan<char>>();

    public static bool IsFiltered(ReadOnlySpan<char> message, [NotNullWhen(true)] out string? match)
    {
        Span<char> clean = stackalloc char[message.Length];
        Normalize(message, clean);
        return Words.TryGetValue(clean, out match);
    }

    public static void Normalize(ReadOnlySpan<char> message, Span<char> clean)
    {
        for (int i = 0; i < message.Length; i++)
        {
            var c = message[i];
            c = char.ToUpperInvariant(c); // fold to uppercase
            c = (char)(c switch
            {
                >= 'ァ' and <= 'ヶ' => c - 0x60, // shift katakana to hiragana
                >= 'Ａ' and <= 'Ｚ' => c - 0xFEE0, // shift fullwidth letters to halfwidth
                _ => c,
            });
            clean[i] = c;
        }
    }
}
