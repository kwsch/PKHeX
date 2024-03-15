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
    public static string TransferGlyphs34(ReadOnlySpan<byte> data, int language, int maxLength)
    {
        Span<char> result = stackalloc char[data.Length];
        var i = 0;
        for (; i < data.Length; i++)
        {
            var c = data[i];
            if (c == StringConverter3.TerminatorByte)
                break;

            // If the encoded character is in the affected range, treat it as Japanese.
            var b = StringConverter3.GetG3Char(c, IsJapanese3(c) ? (int)LanguageID.Japanese : language);

            // If an invalid character (0xF7-0xFE) is present, the nickname/OT is replaced with all question marks.
            // Based on the Gen4 game's language: "？？？？？" in Japanese, "??????????" for nicknames/"???????" for OTs in EFIGS, "?????" in Korean
            // Since we can't tell the language of the Gen4 game from the PKM alone, use the PKM language instead.
            if (b == StringConverter3.TerminatorByte)
                return new string(language == (int)LanguageID.Japanese ? '？' : '?', maxLength);

            result[i] = b;
        }
        return new string(result[..i]);
    }

    /// <summary>
    /// Remaps Gen3 Glyphs to Gen4 Glyphs.
    /// </summary>
    public static string TransferGlyphs34(ReadOnlySpan<char> data, int language)
    {
        if (language == (int)LanguageID.Japanese)
            return new string(data);

        Span<char> result = stackalloc char[data.Length];
        for (var i = 0; i < data.Length; i++)
        {
            var b = data[i];

            // If the encoded character is in the affected range, reinterpret it as Japanese.
            var c = StringConverter3.SetG3Char(b, language);
            result[i] = IsJapanese3(c) ? StringConverter3.GetG3Char(c, (int)LanguageID.Japanese) : b;
        }
        return new string(result);
    }

    // Pal Park always uses the Japanese character set for kana and '円' regardless of language.
    // Only legitimately affects Spanish in-game trades and default player names, where Á/Í/Ú become い/コ/つ in Gen4.
    private const byte JapaneseStartGlyph = 0x01; // 'あ'
    private const byte JapaneseEndGlyph = 0xA0; // 'ッ'
    private const byte JapaneseYenGlyph = 0xB7; // '円'

    private static bool IsJapanese3(byte glyph) => glyph is (>= JapaneseStartGlyph and <= JapaneseEndGlyph) or JapaneseYenGlyph;

    /// <summary>
    /// Remaps Gen5 Glyphs to unicode codepoint.
    /// </summary>
    /// <param name="buffer">Input characters to transfer in place</param>
    /// <returns>Remapped string</returns>
    public static void TransferGlyphs56(Span<byte> buffer)
    {
        for (int i = 0; i < buffer.Length; i += 2)
        {
            var span = buffer[i..];
            var c = ReadUInt16LittleEndian(span);
            if (IsPrivateUseChar(c))
                WriteUInt16LittleEndian(span, GetMigratedPrivateChar(c));
        }
    }

    /// <summary>
    /// Remaps private use unicode codepoint back to Gen5 private use codepoint.
    /// </summary>
    /// <param name="buffer">Input characters to transfer in place</param>
    /// <returns>Remapped string</returns>
    public static void TransferGlyphs65(Span<byte> buffer)
    {
        for (int i = 0; i < buffer.Length; i += 2)
        {
            var span = buffer[i..];
            var c = ReadUInt16LittleEndian(span);
            if (IsPrivateUseCharUnicode(c))
                WriteUInt16LittleEndian(span, GetUnmigratedPrivateChar(c));
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
