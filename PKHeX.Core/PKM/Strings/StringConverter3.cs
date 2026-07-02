using System;

namespace PKHeX.Core;

/// <summary>
/// Logic for converting a <see cref="string"/> for Generation 3.
/// </summary>
public static class StringConverter3
{
    public const byte TerminatorByte = 0xFF;
    private const char Terminator = (char)TerminatorByte;
    public const byte QuoteLeftByte = 0xB1;
    public const byte QuoteRightByte = 0xB2;
    private const byte ApostropheByte = 0xB4;

    private const char FGM = '♂';
    private const char FGF = '♀';
    private const char HGM = StringConverter4Util.HGM; // '♂'
    private const char HGF = StringConverter4Util.HGF; // '♀'

    /// <summary>
    /// Converts a Generation 3 encoded value array to string.
    /// </summary>
    /// <param name="data">Byte array containing string data.</param>
    /// <param name="language">Language specific conversion</param>
    /// <returns>Decoded string.</returns>
    public static string GetString(ReadOnlySpan<byte> data, int language)
    {
        Span<char> result = stackalloc char[data.Length];
        int i = LoadString(data, result, language);
        return new string(result[..i]);
    }

    /// <inheritdoc cref="GetString(ReadOnlySpan{byte},int)"/>
    /// <param name="data">Byte array containing string data.</param>
    /// <param name="jp">Value source is Japanese font.</param>
    public static string GetString(ReadOnlySpan<byte> data, bool jp) => GetString(data, jp ? (int)LanguageID.Japanese : (int)LanguageID.English);

    /// <summary>
    /// Converts a Generation 3 encoded value array to string.
    /// </summary>
    /// <param name="data">Encoded data</param>
    /// <param name="result">Decoded character result buffer</param>
    /// <param name="language">Language specific conversion</param>
    /// <returns>Character count loaded.</returns>
    public static int LoadString(ReadOnlySpan<byte> data, Span<char> result, int language)
    {
        bool jp = language == (int)LanguageID.Japanese;
        var table = jp ? G3_JP : G3_EN;
        bool zh = !jp && StringConverter3Zh.Enabled;
        int ctr = 0;
        for (int i = 0; i < data.Length; i++)
        {
            var value = data[i];
            if (zh && i + 1 < data.Length && StringConverter3Zh.TryDecode(value, data[i + 1], out var hanzi))
            {
                result[ctr++] = hanzi;
                i++;
                continue;
            }
            var c = value switch {
                QuoteLeftByte => GetQuoteLeft(language),
                QuoteRightByte => GetQuoteRight(language),
                _ => table[value],
            }; // Convert to Unicode
            if (c == Terminator) // Stop if Terminator/Invalid
                break;
            c = StringConverter4Util.NormalizeGenderSymbol(c);
            result[ctr++] = c;
        }
        return ctr;
    }

    /// <inheritdoc cref="LoadString(ReadOnlySpan{byte},Span{char},int)"/>
    /// <param name="data">Encoded data</param>
    /// <param name="result">Decoded character result buffer</param>
    /// <param name="jp">Value source is Japanese font.</param>
    public static int LoadString(ReadOnlySpan<byte> data, Span<char> result, bool jp) => LoadString(data, result, jp ? (int)LanguageID.Japanese : (int)LanguageID.English);

    /// <summary>
    /// Converts a string to a Generation 3 encoded value array.
    /// </summary>
    /// <param name="buffer">Backing buffer of the string.</param>
    /// <param name="value">Decoded string.</param>
    /// <param name="maxLength">Maximum length of the input <see cref="value"/></param>
    /// <param name="language">Language specific conversion</param>
    /// <param name="option">Buffer pre-formatting option</param>
    /// <returns>Encoded data.</returns>
    public static int SetString(Span<byte> buffer, ReadOnlySpan<char> value, int maxLength, int language, StringConverterOption option = StringConverterOption.ClearFF)
    {
        if (value.Length > maxLength)
            value = value[..maxLength]; // Hard cap

        if (option is StringConverterOption.ClearFF)
            buffer.Fill(TerminatorByte);
        else if (option is StringConverterOption.ClearZero)
            buffer.Clear();

        bool jp = language == (int)LanguageID.Japanese;
        var table = jp ? G3_JP : G3_EN;
        bool zh = !jp && StringConverter3Zh.Enabled;
        int ctr = 0;
        for (int i = 0; i < value.Length; i++)
        {
            var chr = value[i];
            if (!jp)
                chr = StringConverter4Util.UnNormalizeGenderSymbol(chr);
            if (TryGetIndex(table, chr, language, out var b))
            {
                if (ctr >= buffer.Length)
                    break;
                buffer[ctr++] = b;
                continue;
            }
            if (zh && StringConverter3Zh.TryEncode(chr, out var lead, out var second) && ctr + 1 < buffer.Length)
            {
                buffer[ctr++] = lead;
                buffer[ctr++] = second;
                continue;
            }
            break;
        }

        int count = ctr;
        if (count < buffer.Length)
            buffer[count++] = TerminatorByte;
        return count;
    }

    public static int SetString(Span<byte> buffer, ReadOnlySpan<char> value, int maxLength, bool jp, StringConverterOption option = StringConverterOption.ClearFF) =>
        SetString(buffer, value, maxLength, jp ? (int)LanguageID.Japanese : (int)LanguageID.English, option);

    private static bool TryGetIndex(in ReadOnlySpan<char> dict, char c, int language, out byte result)
    {
        var index = dict.IndexOf(c);
        if (index == -1 || c == '“')
            return TryGetUserFriendlyRemap(c, language, out result);
        result = (byte)index;
        return index != TerminatorByte;
    }

    /// <summary>
    /// Decodes a character from a Generation 3 encoded value.
    /// </summary>
    /// <param name="chr">Generation 4 decoded character.</param>
    /// <param name="language">Language specific conversion</param>
    /// <returns>Generation 3 encoded value.</returns>
    public static char GetG3Char(byte chr, int language)
    {
        var table = (language == (int)LanguageID.Japanese) ? G3_JP : G3_EN;
        return chr switch
        {
            QuoteLeftByte => GetQuoteLeft(language),
            QuoteRightByte => GetQuoteRight(language),
            _ => table[chr],
        };
    }

    /// <summary>
    /// Encodes a character to a Generation 3 encoded value.
    /// </summary>
    /// <param name="chr">Generation 4 decoded character.</param>
    /// <param name="language">Language specific conversion</param>
    /// <returns>Generation 3 encoded value.</returns>
    public static byte SetG3Char(char chr, int language)
    {
        var table = (language == (int)LanguageID.Japanese) ? G3_JP : G3_EN;
        TryGetIndex(table, chr, language, out var b);
        return b;
    }

    // Quotation marks are displayed differently based on the Gen3 game language.
    // Pal Park converts these to the appropriate ones based on the PKM language.
    public static char GetQuoteLeft(int language) => language switch
    {
        (int)LanguageID.English or (int)LanguageID.Italian or (int)LanguageID.Spanish => '“',
        (int)LanguageID.French => '«',
        (int)LanguageID.German => '„',
        _ => '『', // Invalid languages use JP quote
    };

    public static char GetQuoteRight(int language) => language switch
    {
        (int)LanguageID.English or (int)LanguageID.Italian or (int)LanguageID.Spanish => '”',
        (int)LanguageID.French => '»',
        (int)LanguageID.German => '“',
        _ => '』', // Invalid languages use JP quote
    };

    /// <summary>
    /// Tries to remap the user input to a valid character.
    /// </summary>
    private static bool TryGetUserFriendlyRemap(char c, int language, out byte result)
    {
        result = c switch
        {
            '’' => ApostropheByte, // ’ -> '
            '“' => language != (int)LanguageID.German ? QuoteLeftByte : QuoteRightByte,
            '”' => QuoteRightByte,
            '«' => QuoteLeftByte,
            '»' => QuoteRightByte,
            '„' => QuoteLeftByte,
            '『' => QuoteLeftByte,
            '』' => QuoteRightByte,
            _ => TerminatorByte,
        };
        return result != TerminatorByte;
    }

    private static ReadOnlySpan<char> G3_EN =>
    [
        ' ',  'À',  'Á',  'Â', 'Ç',  'È',  'É',  'Ê',  'Ë',  'Ì', 'こ', 'Î',  'Ï',  'Ò',  'Ó',  'Ô',  // 0
        'Œ',  'Ù',  'Ú',  'Û', 'Ñ',  'ß',  'à',  'á',  'ね', 'ç',  'è', 'é',  'ê',  'ë',  'ì',  'ま',  // 1
        'î',  'ï',  'ò',  'ó', 'ô',  'œ',  'ù',  'ú',  'û',  'ñ',  'º', 'ª',  '⑩', '&',  '+',  'あ', // 2
        'ぃ', 'ぅ', 'ぇ', 'ぉ', 'ゃ', '=',  ';', 'が', 'ぎ', 'ぐ', 'げ', 'ご', 'ざ', 'じ', 'ず', 'ぜ', // 3
        'ぞ', 'だ', 'ぢ', 'づ', 'で', 'ど', 'ば', 'び', 'ぶ', 'べ', 'ぼ', 'ぱ', 'ぴ', 'ぷ', 'ぺ', 'ぽ',  // 4
        'っ', '¿',  '¡',  '⒆', '⒇', 'オ', 'カ', 'キ', 'ク', 'ケ', 'Í',  '%', '(', ')', 'セ', 'ソ', // 5
        'タ', 'チ', 'ツ', 'テ', 'ト', 'ナ', 'ニ', 'ヌ', 'â',  'ノ', 'ハ', 'ヒ', 'フ', 'ヘ', 'ホ', 'í',  // 6
        'ミ', 'ム', 'メ', 'モ', 'ヤ', 'ユ', 'ヨ', 'ラ', 'リ', '↑', '↓', '←', '＋', 'ヲ', 'ン', 'ァ', // 7
        'ィ', 'ゥ', 'ェ', 'ォ', '⒅', '<', '>', 'ガ', 'ギ', 'グ', 'ゲ', 'ゴ', 'ザ', 'ジ', 'ズ', 'ゼ', // 8
        'ゾ', 'ダ', 'ヂ', 'ヅ', 'デ', 'ド', 'バ', 'ビ', 'ブ', 'ベ', 'ボ', 'パ', 'ピ', 'プ', 'ペ', 'ポ', // 9
        'ッ', '0',  '1',  '2', '3',  '4',  '5',  '6',  '7',  '8',  '9',  '!', '?',  '.',  '-',  '･',// A
        '⑬',  '“',  '”', '‘', '\'', HGM,  HGF,  '$',  ',',  '⑧',  '/',  'A', 'B',  'C',  'D',  'E', // B
        'F',  'G',  'H',  'I', 'J',  'K',  'L',  'M',  'N',  'O',  'P',  'Q', 'R',  'S',  'T',  'U', // C
        'V',  'W',  'X',  'Y', 'Z',  'a',  'b',  'c',  'd',  'e',  'f',  'g', 'h',  'i',  'j',  'k', // D
        'l',  'm',  'n',  'o', 'p',  'q',  'r',  's',  't',  'u',  'v',  'w', 'x',  'y',  'z',  '►', // E
        ':',  'Ä',  'Ö',  'Ü', 'ä',  'ö',  'ü',                                                      // F

        // Make the total length 256 so that any byte access is always within the array
        Terminator, Terminator, Terminator, Terminator, Terminator, Terminator, Terminator, Terminator, Terminator,
    ];

    private static ReadOnlySpan<char> G3_JP =>
    [
        '　', 'あ', 'い', 'う', 'え', 'お', 'か', 'き', 'く', 'け', 'こ', 'さ', 'し', 'す', 'せ', 'そ', // 0
        'た', 'ち', 'つ', 'て', 'と', 'な', 'に', 'ぬ', 'ね', 'の', 'は', 'ひ', 'ふ', 'へ', 'ほ', 'ま', // 1
        'み', 'む', 'め', 'も', 'や', 'ゆ', 'よ', 'ら', 'り', 'る', 'れ', 'ろ', 'わ', 'を', 'ん', 'ぁ', // 2
        'ぃ', 'ぅ', 'ぇ', 'ぉ', 'ゃ', 'ゅ', 'ょ', 'が', 'ぎ', 'ぐ', 'げ', 'ご', 'ざ', 'じ', 'ず', 'ぜ', // 3
        'ぞ', 'だ', 'ぢ', 'づ', 'で', 'ど', 'ば', 'び', 'ぶ', 'べ', 'ぼ', 'ぱ', 'ぴ', 'ぷ', 'ぺ', 'ぽ', // 4
        'っ', 'ア', 'イ', 'ウ', 'エ', 'オ', 'カ', 'キ', 'ク', 'ケ', 'コ', 'サ', 'シ', 'ス', 'セ', 'ソ', // 5
        'タ', 'チ', 'ツ', 'テ', 'ト', 'ナ', 'ニ', 'ヌ', 'ネ', 'ノ', 'ハ', 'ヒ', 'フ', 'ヘ', 'ホ', 'マ', // 6
        'ミ', 'ム', 'メ', 'モ', 'ヤ', 'ユ', 'ヨ', 'ラ', 'リ', 'ル', 'レ', 'ロ', 'ワ', 'ヲ', 'ン', 'ァ', // 7
        'ィ', 'ゥ', 'ェ', 'ォ', 'ャ', 'ュ', 'ョ', 'ガ', 'ギ', 'グ', 'ゲ', 'ゴ', 'ザ', 'ジ', 'ズ', 'ゼ', // 8
        'ゾ', 'ダ', 'ヂ', 'ヅ', 'デ', 'ド', 'バ', 'ビ', 'ブ', 'ベ', 'ボ', 'パ', 'ピ', 'プ', 'ペ', 'ポ', // 9
        'ッ', '０', '１', '２', '３', '４', '５', '６', '７', '８', '９', '！', '？', '。', 'ー', '・', // A
        '…',  '『', '』', '「', '」',  FGM,  FGF, '円', '．', '×', '／', 'Ａ', 'Ｂ', 'Ｃ', 'Ｄ', 'Ｅ', // B
        'Ｆ', 'Ｇ', 'Ｈ', 'Ｉ', 'Ｊ', 'Ｋ', 'Ｌ', 'Ｍ', 'Ｎ', 'Ｏ', 'Ｐ', 'Ｑ', 'Ｒ', 'Ｓ', 'Ｔ', 'Ｕ', // C
        'Ｖ', 'Ｗ', 'Ｘ', 'Ｙ', 'Ｚ', 'ａ', 'ｂ', 'ｃ', 'ｄ', 'ｅ', 'ｆ', 'ｇ', 'ｈ', 'ｉ', 'ｊ', 'ｋ', // D
        'ｌ', 'ｍ', 'ｎ', 'ｏ', 'ｐ', 'ｑ', 'ｒ', 'ｓ', 'ｔ', 'ｕ', 'ｖ', 'ｗ', 'ｘ', 'ｙ', 'ｚ', '►',  // E
        '：',  'Ä',  'Ö',  'Ü',  'ä',  'ö', 'ü', '↑',  '↓', '←',  '→', '＋',                        // F

        // Make the total length 256 so that any byte access is always within the array
        Terminator, Terminator, Terminator, Terminator,
    ];
}
