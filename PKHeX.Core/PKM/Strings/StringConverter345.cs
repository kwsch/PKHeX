using System;
using System.Buffers;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Logic for converting a <see cref="string"/> for to future Generations.
/// </summary>
public static class StringConverter345
{
    /// <summary>
    /// Trash Bytes for Generation 3->4 based on transfer to Diamond/Pearl.
    /// </summary>
    /// <remarks>String buffers are reused, data is not cleared which yields the trash bytes.</remarks>
    /// <remarks>The first 4 bytes are undocumented, as transferred Gen3 strings will always have at least 2 chars.</remarks>
    public static ReadOnlySpan<byte> GetTrashBytes(int language) => language switch
    {
        2 => [0x18, 0x20, 0x0D, 0x02, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x48, 0xA1, 0x0C, 0x02, 0xE0, 0xFF],
        3 => [0x74, 0x20, 0x0D, 0x02, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xA4, 0xA1, 0x0C, 0x02, 0xE0, 0xFF],
        4 => [0x54, 0x20, 0x0D, 0x02, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x84, 0xA1, 0x0C, 0x02, 0xE0, 0xFF],
        5 => [0x74, 0x20, 0x0D, 0x02, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xA4, 0xA1, 0x0C, 0x02, 0xE0, 0xFF],
        7 => [0x74, 0x20, 0x0D, 0x02, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xA4, 0xA1, 0x0C, 0x02, 0xE0, 0xFF],
        _ => [], // No Trash Bytes
    };

    /// <summary>
    /// Remaps Gen3 Glyphs to Gen4 Glyphs.
    /// </summary>
    public static void TransferGlyphs34(ReadOnlySpan<byte> data, int language, int maxLength, Span<byte> dest)
    {
        Span<char> result = stackalloc char[data.Length];
        int count = TransferGlyphs34(data, result, language, maxLength);
        StringConverter4.SetString(dest, result[..count], maxLength, language, StringConverterOption.None);
    }

    private static int TransferGlyphs34(ReadOnlySpan<byte> data, Span<char> result, int language, int maxLength)
    {
        var i = 0;
        for (; i < data.Length; i++)
        {
            var c = data[i];
            if (c == StringConverter3.TerminatorByte)
                break;

            var b = RemapJapanese34(c, language);

            // If an invalid character (0xF7-0xFE) is present, the nickname/OT is replaced with all question marks.
            // Based on the Gen4 game's language: "？？？？？" in Japanese, "??????????" for nicknames/"???????" for OTs in EFIGS, "?????" in Korean
            // Since we can't tell the language of the Gen4 game from the PKM alone, use the PKM language instead.
            if (b == StringConverter3.TerminatorByte)
            {
                var question = language == (int)LanguageID.Japanese ? '？' : '?';
                result[..maxLength].Fill(question);
                return maxLength;
            }

            result[i] = b;
        }
        return i;
    }

    /// <summary>
    /// Remaps Gen3 Glyphs to Gen4 Glyphs.
    /// </summary>
    public static void TransferGlyphs34(ReadOnlySpan<char> input, int language, Span<char> result)
    {
        if (language == (int)LanguageID.Japanese)
        {
            input.CopyTo(result);
            return;
        }
        for (var i = 0; i < input.Length; i++)
        {
            var b = input[i];

            // If the encoded character is in the affected range, reinterpret it as Japanese.
            result[i] = RemapJapanese34(b, language);
        }
    }

    // Pal Park always uses the Japanese character set for kana and '円' regardless of language.
    // Only legitimately affects Spanish in-game trades and default player names, where Á/Í/Ú become い/コ/つ in Gen4.
    private const byte JapaneseStartGlyph = 0x01; // 'あ'
    private const byte JapaneseEndGlyph = 0xA0; // 'ッ'
    private const byte JapaneseYenGlyph = 0xB7; // '円'

    private static bool IsJapanese3(byte glyph) => glyph is (>= JapaneseStartGlyph and <= JapaneseEndGlyph) or JapaneseYenGlyph;
    private static char RemapJapanese34(byte glyph, int language)
    {
        var lang = IsJapanese3(glyph) ? (int)LanguageID.Japanese : language;
        return StringConverter3.GetG3Char(glyph, lang);
    }
    private static char RemapJapanese34(char c, int language)
    {
        var glyph = StringConverter3.SetG3Char(c, language);
        return IsJapanese3(glyph) ? StringConverter3.GetG3Char(glyph, (int)LanguageID.Japanese) : c;
    }

    /// <summary>
    /// Remaps Gen4 Glyphs to Gen5 Glyphs.
    /// </summary>
    /// <param name="input">Input characters to transfer in place</param>
    public static void TransferGlyphs45(Span<char> input)
    {
        for (int i = 0; i < input.Length; i++)
        {
            var c = input[i];
            if (IsInvalid45(c))
                input[i] = '?';
        }
    }

    // These characters are converted to halfwidth question marks upon transfer to Gen5, in addition to the empty/invalid characters in Gen4.
    // None of these are user-enterable. Note that halfwidth '&' transfers properly, despite it not being user-enterable either.
    private static bool IsInvalid45(char c) => c == StringConverter4.Terminator || InvalidSearchVal45.Contains(c);
    private static readonly SearchValues<char> InvalidSearchVal45 = SearchValues.Create("$_ª°ºÂÃÅÆÊËÎÏÐÔÕØÛÝÞãåæðõøýþÿŒœŞş←↑→↓⑧⑨⑩⑪⑫⒅⒆⒇⒈⒉⒊⒋⒌⒍⒎⒏►♈♉♊♋♌♍♎♏円＆＿");

    /// <summary>
    /// Remaps Gen5 Glyphs to unicode codepoint.
    /// </summary>
    /// <param name="input">Input characters to transfer in place</param>
    public static void TransferGlyphs56(Span<byte> input)
    {
        for (int i = 0; i < input.Length; i += 2)
        {
            var span = input[i..];
            var c = ReadUInt16LittleEndian(span);
            if (IsPrivateUseChar(c))
                WriteUInt16LittleEndian(span, GetMigratedPrivateChar(c));
            else if (c is '\'') // Fix all apostrophes. ' -> ’
                WriteUInt16LittleEndian(span, '’');
        }
    }

    /// <summary>
    /// Remaps Gen5 Glyphs to unicode codepoint.
    /// </summary>
    /// <param name="input">Input characters to transfer in place</param>
    public static void TransferGlyphs56(Span<char> input)
    {
        for (int i = 0; i < input.Length; i += 2)
        {
            var c = input[i];
            if (IsPrivateUseChar(c))
                input[i] = (char)GetMigratedPrivateChar(c);
            else if (c is '\'') // Fix all apostrophes. ' -> ’
                input[i] = '’';
        }
    }

    /// <summary>
    /// Remaps private use unicode codepoint back to Gen5 private use codepoint.
    /// </summary>
    /// <param name="input">Input characters to transfer in place</param>
    public static void TransferGlyphs65(Span<byte> input)
    {
        for (int i = 0; i < input.Length; i += 2)
        {
            var span = input[i..];
            var c = ReadUInt16LittleEndian(span);
            if (IsPrivateUseCharUnicode(c))
                WriteUInt16LittleEndian(span, GetUnmigratedPrivateChar(c));
        }
    }

    /// <summary>
    /// Remaps private use unicode codepoint back to Gen5 private use codepoint.
    /// </summary>
    /// <param name="input">Input characters to transfer in place</param>
    public static void TransferGlyphs65(Span<char> input)
    {
        for (int i = 0; i < input.Length; i += 2)
        {
            var c = input[i];
            if (IsPrivateUseCharUnicode(c))
                input[i] = (char)GetUnmigratedPrivateChar(c);
            else if (c is '’') // Fix all apostrophes. ’ -> '
                input[i] = '\'';
        }
    }

    // Nonstandard characters for Gen 5 are stored at 0x2460 instead of the reserved section.
    // We could ignore inaccessible indexes, but we'll just remap them in bulk.
    private const ushort PrivateUseStartGlyph = 0x2460;
    private const ushort PrivateUseEndGlyph = 0x2487;
    private const ushort PrivateUseGlyphCount = PrivateUseEndGlyph - PrivateUseStartGlyph + 1;
    // 0xE000 is the first private use character in the unicode fonts.
    // The 3DS uses 0xE000-0xE080 for system characters, so any migrated glyph will be 0xE081 onwards.
    private const ushort PrivateUseStartUnicode = 0xE081;
    private const ushort PrivateUseEndUnicode = PrivateUseStartUnicode + PrivateUseGlyphCount - 1;

    /// <summary>
    /// Checks if the glyph is a private use character for Generation5 games.
    /// </summary>
    /// <param name="glyph">Codepoint glyph stored in data.</param>
    /// <returns>True if it needs to be remapped when converting to unicode.</returns>
    public static bool IsPrivateUseChar(ushort glyph)
        => glyph is (>= PrivateUseStartGlyph and <= PrivateUseEndGlyph);

    /// <summary>
    /// Checks if the glyph is a private use character for unicode compatible to Gen5 games.
    /// </summary>
    /// <param name="glyph">Codepoint glyph stored in data.</param>
    /// <returns>True if it needs to be remapped when converting from unicode.</returns>
    /// <remarks>
    /// Use two comparisons instead of one-after-subtraction.
    /// Most players never select these special characters, so the hot path will be almost always false.
    /// </remarks>
    public static bool IsPrivateUseCharUnicode(ushort glyph)
        => glyph is (>= PrivateUseStartUnicode and <= PrivateUseEndUnicode);

    /// <summary>
    /// Converts a Generation5 private use character to a unicode private use codepoint.
    /// </summary>
    /// <param name="glyph">Codepoint glyph stored in data.</param>
    /// <returns>Unicode private use codepoint.</returns>
    public static ushort GetMigratedPrivateChar(ushort glyph) =>
        (ushort)(glyph - PrivateUseStartGlyph + PrivateUseStartUnicode);

    /// <summary>
    /// Converts a unicode private use codepoint to a Generation 4/5 private use character.
    /// </summary>
    /// <param name="glyph">Codepoint glyph stored in data.</param>
    /// <returns>Gen5 private use codepoint.</returns>
    public static ushort GetUnmigratedPrivateChar(ushort glyph) =>
        (ushort)(glyph - PrivateUseStartUnicode + PrivateUseStartGlyph);
}
