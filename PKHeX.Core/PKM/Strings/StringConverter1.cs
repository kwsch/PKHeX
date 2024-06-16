using System;

namespace PKHeX.Core;

/// <summary>
/// Logic for converting a <see cref="string"/> for Generation 1.
/// </summary>
/// <remarks>Slight differences when compared to <seealso cref="StringConverter2"/>.</remarks>
public static class StringConverter1
{
    public const byte TerminatorCode = 0x50;
    public const byte TerminatorZero = 0x00;
    public const byte TradeOTCode = 0x5D;
    public const byte SpaceCode = 0x7F;

    public const char Terminator = '\0';
    public const char TradeOT = '*';

    public static bool GetIsJapanese(ReadOnlySpan<char> str) => AllJapanese(str);

    private static bool AllJapanese(ReadOnlySpan<char> str)
    {
        foreach (var x in str)
        {
            if (!IsJapanese(x))
                return false;
        }
        return true;
        static bool IsJapanese(char c) => c is >= '\u3000' and <= '\u30FC';
    }

    public static bool GetIsEnglish(ReadOnlySpan<char> str) => !GetIsJapanese(str);
    public static bool GetIsJapanese(ReadOnlySpan<byte> raw) => AllCharsInTable(raw, TableJP);
    public static bool GetIsEnglish(ReadOnlySpan<byte> raw) => AllCharsInTable(raw, TableEN);

    private static bool AllCharsInTable(ReadOnlySpan<byte> data, ReadOnlySpan<char> table)
    {
        foreach (var c in data)
        {
            var b = table[c];
            if (b == Terminator && c is not (TerminatorCode or TerminatorZero))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if the input byte array is definitely of German origin (any ÄÖÜäöü)
    /// </summary>
    /// <param name="data">Raw string bytes</param>
    /// <returns>Indication if the data is from a definitely-german string</returns>
    public static bool IsG12German(ReadOnlySpan<byte> data)
    {
        foreach (var b in data)
        {
            if (IsGermanicGlyph(b))
                return true;
        }
        return false;
    }

    private static bool IsGermanicGlyph(byte b) => b - 0xC0u <= 6;

    private static bool IsGermanicGlyph(char c)
    {
        ulong i = c - (uint)'Ä';
        ulong shift = 0x8000208080002080ul << (int)i;
        ulong mask = i - 64;
        return (long)(shift & mask) < 0;
    }

    /// <summary>
    /// Checks if the input byte array is definitely of German origin (any ÄÖÜäöü)
    /// </summary>
    /// <param name="value">Input string</param>
    /// <returns>Indication if the data is from a definitely-german string</returns>
    public static bool IsG12German(ReadOnlySpan<char> value)
    {
        foreach (var c in value)
        {
            if (IsGermanicGlyph(c))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Converts Generation 1 encoded data into a string.
    /// </summary>
    /// <param name="data">Encoded data.</param>
    /// <param name="jp">Data source is Japanese.</param>
    /// <returns>Decoded string.</returns>
    public static string GetString(ReadOnlySpan<byte> data, bool jp)
    {
        Span<char> result = stackalloc char[data.Length];
        int length = LoadString(data, result, jp);
        return new string(result[..length]);
    }

    /// <inheritdoc cref="GetString(ReadOnlySpan{byte},bool)"/>
    /// <param name="data">Encoded data</param>
    /// <param name="result">Decoded character result buffer</param>
    /// <param name="jp">Data source is Japanese.</param>
    /// <returns>Character count loaded.</returns>
    public static int LoadString(ReadOnlySpan<byte> data, Span<char> result, bool jp)
    {
        if (data.Length == 0)
            return 0;
        if (data[0] == TradeOTCode) // In-game Trade
        {
            result[0] = TradeOT;
            return 1;
        }

        int i = 0;
        var dict = jp ? TableJP : TableEN;
        for (; i < data.Length; i++)
        {
            var value = data[i];
            var c = dict[value];
            if (c == Terminator) // Stop if Terminator
                break;
            result[i] = c;
        }
        return i;
    }

    /// <summary>
    /// Converts a string to Generation 1 encoded data.
    /// </summary>
    /// <param name="destBuffer">Span of bytes to write encoded string data</param>
    /// <param name="value">Decoded string.</param>
    /// <param name="maxLength">Maximum length of the input <see cref="value"/></param>
    /// <param name="jp">Data destination is Japanese.</param>
    /// <param name="option">Buffer pre-formatting option</param>
    /// <returns>Encoded data.</returns>
    public static int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, bool jp,
        StringConverterOption option = StringConverterOption.Clear50)
    {
        if (option is StringConverterOption.ClearZero)
            destBuffer.Clear();
        else if (option is StringConverterOption.Clear50)
            destBuffer.Fill(TerminatorCode);
        else if (option is StringConverterOption.Clear7F)
            destBuffer.Fill(SpaceCode);

        if (value.Length == 0)
            return 0;
        if (value[0] == TradeOT) // Handle "[TRAINER]"
        {
            destBuffer[0] = TradeOTCode;
            destBuffer[1] = TerminatorCode;
            return 2;
        }

        if (value.Length > maxLength)
            value = value[..maxLength]; // Hard cap

        var dict = jp ? TableJP : TableEN;
        int i = 0;
        for (; i < value.Length; i++)
        {
            if (!TryGetIndex(dict, value[i], out var index))
                break;
            destBuffer[i] = index;
        }

        int count = i;
        if (count == destBuffer.Length)
            return count;
        destBuffer[count] = TerminatorCode;
        return count + 1;
    }

    private static bool TryGetIndex(in ReadOnlySpan<char> dict, char c, out byte result)
    {
        var index = dict.IndexOf(c);
        if (index == -1)
            return TryGetUserFriendlyRemap(dict, c, out result);
        // \0 shouldn't really be user-entered, but just in case
        result = (byte)index;
        return index != default;
    }

    // べ (U+3079), ぺ (U+307A), へ (U+3078), and り (U+308A)
    internal const string Hiragana = "べぺへり";

    /// <summary>
    /// Tries to remap the user input to a valid character.
    /// </summary>
    private static bool TryGetUserFriendlyRemap(in ReadOnlySpan<char> dict, char c, out byte result)
    {
        if (Hiragana.Contains(c))
        {
            int index = dict.IndexOf((char)(c + (char)0x60));
            result = (byte)index;
            return true; // Valid Hiragana will always be found if it's in the table
        }
        result = default;
        return false;
    }

    #region Gen 1 Character Tables

    internal const char NUL = Terminator;
    internal const char TOT = TradeOT;
    internal const char LPK = '{'; // Pk
    internal const char LMN = '}'; // Mn
    internal const char MNY = '¥'; // Yen
    internal const char LPO = '@'; // Po
    internal const char LKE = '#'; // Ke
    internal const char LEA = '%'; // é for Box
    public const char DOT = '․'; // . for MR.MIME (U+2024, not U+002E)
    internal const char SPF = '　'; // Full-width space (U+3000)
    public const char SPH = ' '; // Half-width space

    public static ReadOnlySpan<char> TableEN =>
    [
        NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, // 00-0F
        NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, // 10-1F
        NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, // 20-2F
        NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, // 30-3F
        NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, // 40-4F
        NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, TOT, NUL, NUL, // 50-5F
        NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, // 60-6F
        LPO, LKE, '“', '”', NUL, '…', NUL, NUL, NUL, '┌', '─', '┐', '│', '└', '┘', SPH, // 70-7F
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', // 80-8F
        'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '(', ')', ':', ';', '[', ']', // 90-9F
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', // A0-AF
        'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'à', 'è', 'é', 'ù', 'À', 'Á', // B0-BF
        'Ä', 'Ö', 'Ü', 'ä', 'ö', 'ü', 'È', 'É', 'Ì', 'Í', 'Ñ', 'Ò', 'Ó', 'Ù', 'Ú', 'á', // C0-CF
        'ì', 'í', 'ñ', 'ò', 'ó', 'ú', NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, '←', '\'', // D0-DF
        '’', LPK, LMN, '-', NUL, NUL, '?', '!', DOT, '&', LEA, '→', '▷', '▶', '▼', '♂', // E0-EF
        MNY, '×', '.', '/', ',', '♀', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', // F0-FF
    ];

    public static ReadOnlySpan<char> TableJP =>
    [
        NUL, NUL, NUL, NUL, NUL, 'ガ', 'ギ', 'グ', 'ゲ', 'ゴ', 'ザ', 'ジ', 'ズ', 'ゼ', 'ゾ', 'ダ', // 00-0F
        'ヂ', 'ヅ', 'デ', 'ド', NUL, NUL, NUL, NUL, NUL, 'バ', 'ビ', 'ブ', 'ボ', NUL,  NUL, NUL, // 10-1F
        NUL, NUL, NUL, NUL, NUL, NUL, 'が', 'ぎ', 'ぐ', 'げ', 'ご', 'ざ', 'じ', 'ず', 'ぜ', 'ぞ', // 20-2F
        'だ', 'ぢ', 'づ', 'で', 'ど', NUL, NUL, NUL, NUL,  NUL, 'ば', 'び', 'ぶ', 'ベ', 'ぼ', NUL, // 30-3F
        'パ', 'ピ', 'プ', 'ポ', 'ぱ', 'ぴ', 'ぷ', 'ペ', 'ぽ', NUL, NUL, NUL, NUL, NUL, NUL, NUL, // 40-4F
        NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, TOT, NUL, NUL, // 50-5F
        NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, // 60-6F
        '「', '」', '『', '』', '・', '⋯', NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, SPF, // 70-7F
        'ア', 'イ', 'ウ', 'エ', 'オ', 'カ', 'キ', 'ク', 'ケ', 'コ', 'サ', 'シ', 'ス', 'セ', 'ソ', 'タ', // 80-8F
        'チ', 'ツ', 'テ', 'ト', 'ナ', 'ニ', 'ヌ', 'ネ', 'ノ', 'ハ', 'ヒ', 'フ', 'ホ', 'マ', 'ミ', 'ム', // 90-9F
        'メ', 'モ', 'ヤ', 'ユ', 'ヨ', 'ラ', 'ル', 'レ', 'ロ', 'ワ', 'ヲ', 'ン', 'ッ', 'ャ', 'ュ', 'ョ', // A0-AF
        'ィ', 'あ', 'い', 'う', 'え', 'お', 'か', 'き', 'く', 'け', 'こ', 'さ', 'し', 'す', 'せ', 'そ', // B0-BF
        'た', 'ち', 'つ', 'て', 'と', 'な', 'に', 'ぬ', 'ね', 'の', 'は', 'ひ', 'ふ', 'ヘ', 'ほ', 'ま', // C0-CF
        'み', 'む', 'め', 'も', 'や', 'ゆ', 'よ', 'ら', 'リ', 'る', 'れ', 'ろ', 'わ', 'を', 'ん', 'っ', // D0-DF
        'ゃ', 'ゅ', 'ょ', 'ー', 'ﾟ', 'ﾞ', '？', '！', '。', 'ァ', 'ゥ', 'ェ', NUL, NUL, NUL, '♂', // E0-EF
        MNY, NUL, '．', '／', 'ォ', '♀', '０', '１', '２', '３', '４', '５', '６', '７', '８', '９', // F0-FF
    ];

    #endregion
}
