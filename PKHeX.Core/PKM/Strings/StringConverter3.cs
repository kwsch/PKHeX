using System;

namespace PKHeX.Core;

/// <summary>
/// Logic for converting a <see cref="string"/> for Generation 3.
/// </summary>
public static class StringConverter3
{
    private const byte TerminatorByte = 0xFF;
    private const char Terminator = (char)TerminatorByte;
    private const char Apostrophe = '\''; // ’
    private const byte ApostropheByte = 0xB4;

    /// <summary>
    /// Converts a Generation 3 encoded value array to string.
    /// </summary>
    /// <param name="data">Byte array containing string data.</param>
    /// <param name="jp">Value source is Japanese font.</param>
    /// <returns>Decoded string.</returns>
    public static string GetString(ReadOnlySpan<byte> data, bool jp)
    {
        Span<char> result = stackalloc char[data.Length];
        int i = LoadString(data, result, jp);
        return new string(result[..i]);
    }

    /// <inheritdoc cref="GetString(ReadOnlySpan{byte},bool)"/>
    /// <param name="data">Encoded data</param>
    /// <param name="result">Decoded character result buffer</param>
    /// <param name="jp">Data source is Japanese.</param>
    /// <returns>Character count loaded.</returns>
    public static int LoadString(ReadOnlySpan<byte> data, Span<char> result, bool jp)
    {
        var table = jp ? G3_JP : G3_EN;
        int i = 0;
        for (; i < data.Length; i++)
        {
            var value = data[i];
            var c = table[value]; // Convert to Unicode
            if (c == Terminator) // Stop if Terminator/Invalid
                break;
            result[i] = c;
        }
        return i;
    }

    /// <summary>
    /// Converts a string to a Generation 3 encoded value array.
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="value">Decoded string.</param>
    /// <param name="maxLength">Maximum length of the input <see cref="value"/></param>
    /// <param name="jp">String destination is Japanese font.</param>
    /// <param name="option">Buffer pre-formatting option</param>
    /// <returns>Encoded data.</returns>
    public static int SetString(Span<byte> buffer, ReadOnlySpan<char> value, int maxLength, bool jp,
        StringConverterOption option = StringConverterOption.ClearFF)
    {
        if (value.Length > maxLength)
            value = value[..maxLength]; // Hard cap

        if (option is StringConverterOption.ClearFF)
            buffer.Fill(TerminatorByte);
        else if (option is StringConverterOption.ClearZero)
            buffer.Clear();

        var table = jp ? G3_JP : G3_EN;
        int i = 0;
        for (; i < value.Length; i++)
        {
            var chr = value[i];
            if (chr == Apostrophe) // ’
                return ApostropheByte;
            var b = (byte)table.IndexOf(chr);
            if (b == TerminatorByte)
                break;
            buffer[i] = b;
        }

        int count = i;
        if (count < buffer.Length)
            buffer[count++] = TerminatorByte;
        return count;
    }

    /// <summary>
    /// Decodes a character from a Generation 3 encoded value.
    /// </summary>
    /// <param name="chr">Generation 4 decoded character.</param>
    /// <param name="jp">Character destination is Japanese font.</param>
    /// <returns>Generation 3 encoded value.</returns>
    public static char GetG3Char(byte chr, bool jp)
    {
        var table = jp ? G3_JP : G3_EN;
        return table[chr];
    }

    /// <summary>
    /// Encodes a character to a Generation 3 encoded value.
    /// </summary>
    /// <param name="chr">Generation 4 decoded character.</param>
    /// <param name="jp">Character destination is Japanese font.</param>
    /// <returns>Generation 3 encoded value.</returns>
    public static byte SetG3Char(char chr, bool jp)
    {
        if (chr == Apostrophe)
            return ApostropheByte;
        var table = jp ? G3_JP : G3_EN;
        var index = table.IndexOf(chr);
        return (byte)index;
    }

    private static ReadOnlySpan<char> G3_EN =>
    [
        ' ',  'À',  'Á',  'Â', 'Ç',  'È',  'É',  'Ê',  'Ë',  'Ì', 'こ', 'Î',  'Ï',  'Ò',  'Ó',  'Ô',  // 0
        'Œ',  'Ù',  'Ú',  'Û', 'Ñ',  'ß',  'à',  'á',  'ね', 'Ç',  'È', 'é',  'ê',  'ë',  'ì',  'í',  // 1
        'î',  'ï',  'ò',  'ó', 'ô',  'œ',  'ù',  'ú',  'û',  'ñ',  'º', 'ª',  '⒅', '&',  '+',  'あ', // 2
        'ぃ', 'ぅ', 'ぇ', 'ぉ', 'ゃ', '=',  'ょ', 'が', 'ぎ', 'ぐ', 'げ', 'ご', 'ざ', 'じ', 'ず', 'ぜ', // 3
        'ぞ', 'だ', 'ぢ', 'づ', 'で', 'ど', 'ば', 'び', 'ぶ', 'べ', 'ぼ', 'ぱ', 'ぴ', 'ぷ', 'ぺ', 'ぽ',  // 4
        'っ', '¿',  '¡',  '⒆', '⒇', 'オ', 'カ', 'キ', 'ク', 'ケ', 'Í',  'コ', 'サ', 'ス', 'セ', 'ソ', // 5
        'タ', 'チ', 'ツ', 'テ', 'ト', 'ナ', 'ニ', 'ヌ', 'â',  'ノ', 'ハ', 'ヒ', 'フ', 'ヘ', 'ホ', 'í',  // 6
        'ミ', 'ム', 'メ', 'モ', 'ヤ', 'ユ', 'ヨ', 'ラ', 'リ', 'ル', 'レ', 'ロ', 'ワ', 'ヲ', 'ン', 'ァ', // 7
        'ィ', 'ゥ', 'ェ', 'ォ', 'ャ', 'ュ', 'ョ', 'ガ', 'ギ', 'グ', 'ゲ', 'ゴ', 'ザ', 'ジ', 'ズ', 'ゼ', // 8
        'ゾ', 'ダ', 'ヂ', 'ヅ', 'デ', 'ド', 'バ', 'ビ', 'ブ', 'ベ', 'ボ', 'パ', 'ピ', 'プ', 'ペ', 'ポ', // 9
        'ッ', '0',  '1',  '2', '3',  '4',  '5',  '6',  '7',  '8',  '9',  '!', '?',  '.',  '-',  '・',// A
        '⑬',  '“',  '”',  '‘', '’',  '♂',  '♀',  '$',  ',',  '⑧',  '/',  'A', 'B',  'C',  'D',  'E', // B
        'F',  'G',  'H',  'I', 'J',  'K',  'L',  'M',  'N',  'O',  'P',  'Q', 'R',  'S',  'T',  'U', // C
        'V',  'W',  'X',  'Y', 'Z',  'a',  'b',  'c',  'd',  'e',  'f',  'g', 'h',  'i',  'j',  'k', // D
        'l',  'm',  'n',  'o', 'p',  'q',  'r',  's',  't',  'u',  'v',  'w', 'x',  'y',  'z',  '▶', // E
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
        '⋯',  '『', '』', '「', '」', '♂',  '♀',  '$',  '.', '⑧',  '/',  'Ａ', 'Ｂ', 'Ｃ', 'Ｄ', 'Ｅ', // B
        'Ｆ', 'Ｇ', 'Ｈ', 'Ｉ', 'Ｊ', 'Ｋ', 'Ｌ', 'Ｍ', 'Ｎ', 'Ｏ', 'Ｐ', 'Ｑ', 'Ｒ', 'Ｓ', 'Ｔ', 'Ｕ', // C
        'Ｖ', 'Ｗ', 'Ｘ', 'Ｙ', 'Ｚ', 'ａ', 'ｂ', 'ｃ', 'ｄ', 'ｅ', 'ｆ', 'ｇ', 'ｈ', 'ｉ', 'ｊ', 'ｋ', // D
        'ｌ', 'ｍ', 'ｎ', 'ｏ', 'ｐ', 'ｑ', 'ｒ', 'ｓ', 'ｔ', 'ｕ', 'ｖ', 'ｗ', 'ｘ', 'ｙ', 'ｚ', '▶',  // E
        ':',  'Ä',  'Ö',  'Ü',  'ä',  'ö', 'ü',                                                      // F

        // Make the total length 256 so that any byte access is always within the array
        Terminator, Terminator, Terminator, Terminator, Terminator, Terminator, Terminator, Terminator, Terminator,
    ];
}
