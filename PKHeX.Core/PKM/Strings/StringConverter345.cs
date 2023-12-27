using System;
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
    /// <remarks>
    /// Use two comparisons instead of one-after-subtraction.
    /// Most players never select these special characters, so the hot path will be almost always false.
    /// </remarks>
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
