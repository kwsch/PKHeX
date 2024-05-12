using System;

namespace PKHeX.Core;

public static class StringConverter2
{
    public const byte TerminatorCode = StringConverter1.TerminatorCode;
    public const byte TerminatorZero = StringConverter1.TerminatorZero;
    public const byte TradeOTCode = StringConverter1.TradeOTCode;
    public const byte SpaceCode = StringConverter1.SpaceCode;

    public const char Terminator = StringConverter1.Terminator;
    public const char TradeOT = StringConverter1.TradeOT;

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
    /// Converts Generation 1 encoded data into a string.
    /// </summary>
    /// <param name="data">Encoded data.</param>
    /// <param name="language">Data source language.</param>
    /// <returns>Decoded string.</returns>
    public static string GetString(ReadOnlySpan<byte> data, int language)
    {
        Span<char> result = stackalloc char[data.Length];
        int length = LoadString(data, result, language);
        return new string(result[..length]);
    }

    /// <inheritdoc cref="GetString(ReadOnlySpan{byte},int)"/>
    /// <param name="data">Encoded data</param>
    /// <param name="result">Decoded character result buffer</param>
    /// <param name="language">Data source language.</param>
    /// <returns>Character count loaded.</returns>
    public static int LoadString(ReadOnlySpan<byte> data, Span<char> result, int language)
    {
        if (data.Length == 0)
            return 0;
        if (data[0] == TradeOTCode) // In-game Trade
        {
            result[0] = TradeOT;
            return 1;
        }

        int i = 0;
        var dict = GetDict(language);
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

    private static ReadOnlySpan<char> GetDict(int language) => language switch
    {
        1 => TableJP,
        3 or 5 => TableFRE,
        4 or 7 => TableITA,
        _ => TableEN,
    };

    /// <summary>
    /// Converts a string to Generation 1 encoded data.
    /// </summary>
    /// <param name="destBuffer">Span of bytes to write encoded string data</param>
    /// <param name="value">Decoded string.</param>
    /// <param name="maxLength">Maximum length of the input <see cref="value"/></param>
    /// <param name="language">Data source language.</param>
    /// <param name="option">Buffer pre-formatting option</param>
    /// <returns>Encoded data.</returns>
    public static int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, int language,
        StringConverterOption option = StringConverterOption.Clear50)
    {
        ConditionBuffer(destBuffer, option);
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

        var dict = GetDict(language);
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

    private static void ConditionBuffer(Span<byte> destBuffer, StringConverterOption option)
    {
        if (option is StringConverterOption.ClearZero)
            destBuffer.Clear();
        else if (option is StringConverterOption.Clear50)
            destBuffer.Fill(TerminatorCode);
        else if (option is StringConverterOption.Clear7F)
            destBuffer.Fill(SpaceCode);
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

    /// <summary>
    /// Tries to remap the user input to a valid character.
    /// </summary>
    /// <remarks><seealso cref="StringConverter1.TryGetUserFriendlyRemap"/></remarks>
    private static bool TryGetUserFriendlyRemap(in ReadOnlySpan<char> dict, char c, out byte result)
    {
        if (StringConverter1.Hiragana.Contains(c))
        {
            int index = dict.IndexOf((char)(c + (char)0x60));
            result = (byte)index;
            return true; // Valid Hiragana will always be found if it's in the table
        }
        result = default;
        return false;
    }

    #region Gen 2 Character Tables

    private const char NUL = StringConverter1.NUL;
    private const char TOT = StringConverter1.TOT;
    private const char LPK = StringConverter1.LPK; // Pk
    private const char LMN = StringConverter1.LMN; // Mn
    private const char MNY = StringConverter1.MNY; // Yen
    private const char LPO = StringConverter1.LPO; // Po
    private const char LKE = StringConverter1.LKE; // Ke
    private const char LEA = StringConverter1.LEA; // é for Box
    private const char DOT = StringConverter1.DOT; // . for MR.MIME (U+2024, not U+002E)
    private const char SPF = StringConverter1.SPF; // Full-width space (U+3000)
    private const char SPH = StringConverter1.SPH; // Half-width space

    private const char LAP = '’'; // Apostrophe

    // 'd 'l 'm 'r 's 't 'v
    // All are apostrophe-before.
    private const string LigatureENG = "dlmrstv";

    // c' d' j' l' m' n' p'	s' 's t' u' y'
    // All are apostrophe-after besides index 8 ('s)
    private const string LigatureFRE = "cdjlmnpsstuy";

    private const char LI0 = '０'; // 'd
    private const char LI1 = '１'; // 'l
    private const char LI2 = '２'; // 'm
    private const char LI3 = '３'; // 'r
    private const char LI4 = '４'; // 's
    private const char LI5 = '５'; // 't
    private const char LI6 = '６'; // 'v	
    private const char LI7 = '７'; // 'd
    private const char LI8 = '８'; // 'l
    private const char LI9 = '９'; // 'm
    private const char LIA = 'Ａ'; // 'r
    private const char LIB = 'Ｂ'; // 's

    /// <summary>
    /// English encoding table with unused indexes merged in from other languages that use them.
    /// <see cref="StringConverter1.TableEN"/>
    /// </summary>
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
        LI0, LI1, LI2, LI3, LI4, LI5, LI6, NUL, NUL, NUL, NUL, NUL, NUL, NUL, NUL, '←', // D0-DF
        LAP, LPK, LMN, '-', '+', NUL, '?', '!', DOT, '&', LEA, '→', '▷', '▶', '▼', '♂', // E0-EF
        MNY, '×', '.', '/', ',', '♀', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', // F0-FF
    ];

    public static ReadOnlySpan<char> TableFRE => // Also German
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
        'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'à', 'è', 'é', 'ù', 'ß', 'ç', // B0-BF
        'Ä', 'Ö', 'Ü', 'ä', 'ö', 'ü', 'È', 'É', 'Ì', 'Í', 'Ñ', 'Ò', 'Ó', 'Ù', 'Ú', 'á', // C0-CF
        NUL, NUL, NUL, NUL, LI0, LI1, LI2, LI3, LI4, LI5, LI6, LI7, LI8, LI9, LIA, LIB, // D0-DF
        LAP, LPK, LMN, '-', '+', NUL, '?', '!', DOT, '&', LEA, '→', '▷', '▶', '▼', '♂', // E0-EF
        MNY, '×', '.', '/', ',', '♀', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', // F0-FF
    ];

    public static ReadOnlySpan<char> TableITA => // Also Spanish
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
        'ì', 'í', 'ñ', 'ò', 'ó', 'ú', NUL, NUL, LI0, LI1, LI2, LI3, LI4, LI5, LI6, NUL, // D0-DF
        LAP, LPK, LMN, '-', '¿', '¡', '?', '!', DOT, '&', LEA, '→', '▷', '▶', '▼', '♂', // E0-EF
        MNY, '×', '.', '/', ',', '♀', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', // F0-FF
    ];

    private static void InsertLigature(Span<char> result, char c, bool isAfter)
    {
        if (isAfter)
        {
            result[0] = c;
            result[1] = LAP;
        }
        else
        {
            result[0] = LAP;
            result[1] = c;
        }
    }

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

    public static string InflateLigatures(string result, int language)
    {
        if (language is (int)LanguageID.Japanese or (int)LanguageID.Korean)
            return result; // No ligatures in JPN/KOR

        bool after = language is (int)LanguageID.French or (int)LanguageID.German;
        var ligatures = after ? LigatureFRE : LigatureENG;
        Span<char> inflated = stackalloc char[result.Length * 2]; // worst case is double length
        int index = 0;
        foreach (var c in result)
        {
            if (!TryGetLigatureIndex(c, out var i) || i >= ligatures.Length)
            {
                inflated[index++] = c;
                continue;
            }

            var ligature = ligatures[i];
            InsertLigature(inflated[index..], ligature, after && i != 8);
            index += 2;
        }
        if (index == result.Length)
            return result; // Nothing changed.
        return new string(inflated[..index]);
    }

    private static bool TryGetLigatureIndex(char c, out int index) => -1 != (index = LigatureList.IndexOf(c));
    private static ReadOnlySpan<char> LigatureList => [LI0, LI1, LI2, LI3, LI4, LI5, LI6, LI7, LI8, LI8, LI9, LIA, LIB];
    private static char GetLigature(int ligatureIndex) => LigatureList[ligatureIndex];

    public static int DeflateLigatures(ReadOnlySpan<char> value, Span<char> result, int language)
    {
        if (language is (int)LanguageID.Japanese or (int)LanguageID.Korean)
        {
            value.CopyTo(result);
            return value.Length; // No ligatures in JPN/KOR
        }

        bool after = language is (int)LanguageID.French or (int)LanguageID.German;
        var ligatures = after ? LigatureFRE : LigatureENG;
        int index = 0;
        for (var i = 0; i < value.Length; i++)
        {
            char c = value[i];
            if (c is not (LAP or '\''))
            {
                if (index == result.Length)
                    return index; // Overflow (shouldn't happen for correctly-written strings)
                result[index++] = c;
                continue;
            }

            if (after && index != 0)
            {
                ref var prev = ref result[index - 1];
                var ligatureIndex = ligatures.IndexOf(prev);
                if (ligatureIndex != -1)
                {
                    prev = ligatures[ligatureIndex];
                    continue;
                }
            }

            if (index == result.Length)
                return index; // Overflow (shouldn't happen for correctly-written strings)
            if (i < value.Length - 1)
            {
                var next = value[i + 1];
                var ligatureIndex = ligatures.IndexOf(next);
                if (ligatureIndex != -1)
                {
                    result[index++] = GetLigature(ligatureIndex);
                    i++;
                    continue;
                }
            }

            result[index++] = c;
        }
        return index;
    }
}
