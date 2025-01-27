using System;

namespace PKHeX.Core;

/// <summary>
/// Simplistic normalization of a string used by the Nintendo 3DS and Nintendo Switch games.
/// </summary>
public static class TextNormalizer
{
    private const string Dakuten = "ｶｷｸｹｺｻｼｽｾｿﾀﾁﾂﾃﾄﾊﾋﾌﾍﾎ"; // 'ｳ' handled separately
    private const string Handakuten = "ﾊﾋﾌﾍﾎ";
    private const string FullwidthKana = "ヲァィゥェォャュョッーアイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワン";
    private const string SmallKana = "ァィゥェォッャュョヮ"; // 'ヵ', 'ヶ' handled separately

    /// <summary>
    /// Normalize a string to a simplified form for checking against a bad-word list.
    /// </summary>
    /// <param name="input">Input string to normalize</param>
    /// <param name="output">Output buffer to write the normalized string</param>
    public static int Normalize(ReadOnlySpan<char> input, Span<char> output)
    {
        int ctr = 0;
        for (int i = 0; i < input.Length; i++)
        {
            var c = input[i];

            // Skip spaces and halfwidth dakuten/handakuten
            if (c is ' ' or '\u3000' or 'ﾞ' or 'ﾟ')
                continue;

            // Handle combining halfwidth dakuten/handakuten
            ushort ofs = 0;
            if (c is >= 'ｦ' and <= 'ﾝ' && i + 1 < input.Length)
            {
                var d = input[i + 1];
                if (d == 'ﾞ' && Dakuten.Contains(c))
                    ofs = 1;
                else if (d == 'ﾟ' && Handakuten.Contains(c))
                    ofs = 2;
                else if (d == 'ﾞ' && c == 'ｳ')
                    ofs = 'ヴ' - 'ウ'; // 0x4E (78)
            }

            // Fold characters treated identically
            c = char.ToLowerInvariant(c); // fold to lowercase
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

            output[ctr] = c;
            ctr++;
        }
        return ctr;
    }
}
