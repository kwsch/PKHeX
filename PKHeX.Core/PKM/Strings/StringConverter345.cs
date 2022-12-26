using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Logic for converting a <see cref="string"/> for to future Generations.
/// </summary>
public static class StringConverter345
{
    /// <summary>
    /// Trash Bytes for Generation 3->4
    /// </summary>
    /// <remarks>String buffers are reused, data is not cleared which yields the trash bytes.</remarks>
    private static ReadOnlySpan<byte> G4TransferTrashBytes => new byte[]
    {
        0x18, 0x20, 0x0D, 0x02, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x48, 0xA1, 0x0C, 0x02, 0xE0, 0xFF, // 2
        0x74, 0x20, 0x0D, 0x02, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xA4, 0xA1, 0x0C, 0x02, 0xE0, 0xFF, // 3
        0x54, 0x20, 0x0D, 0x02, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x84, 0xA1, 0x0C, 0x02, 0xE0, 0xFF, // 4
        0x74, 0x20, 0x0D, 0x02, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xA4, 0xA1, 0x0C, 0x02, 0xE0, 0xFF, // 5
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // 6
        0x74, 0x20, 0x0D, 0x02, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xA4, 0xA1, 0x0C, 0x02, 0xE0, 0xFF, // 7
    };

    private const int TrashByteLanguages = 6; // rows
    private const int TrashByteLayerWidth = 18; // columns

    public static ReadOnlySpan<byte> GetTrashBytes(int language)
    {
        language -= 2;
        if ((uint)language >= TrashByteLanguages)
            return ReadOnlySpan<byte>.Empty;
        var offset = language * TrashByteLayerWidth;
        return G4TransferTrashBytes.Slice(offset, TrashByteLayerWidth);
    }

    /// <summary>
    /// Remaps Gen5 Glyphs to unicode codepoint.
    /// </summary>
    /// <param name="buffer">Input characters to transfer in place</param>
    /// <returns>Remapped string</returns>
    public static void TransferGlyphs56(Span<byte> buffer)
    {
        var table = Glyph56;
        for (int i = 0; i < buffer.Length; i += 2)
        {
            var c = (char)ReadUInt16LittleEndian(buffer[i..]);
            if (table.TryGetValue(c, out c))
                WriteUInt16LittleEndian(buffer[i..], c);
        }
    }

    private static readonly Dictionary<char, char> Glyph56 = new()
    {
        {'\u2467', '\u00d7'}, // ×
        {'\u2468', '\u00f7'}, // ÷

        {'\u246c', '\uE08D'}, // …
        {'\u246d', '\uE08E'}, // ♂
        {'\u246e', '\uE08F'}, // ♀
        {'\u246f', '\uE090'}, // ♠
        {'\u2470', '\uE091'}, // ♣
        {'\u2471', '\uE092'}, // ♥
        {'\u2472', '\uE093'}, // ♦
        {'\u2473', '\uE094'}, // ★
        {'\u2474', '\uE095'}, // ◎
        {'\u2475', '\uE096'}, // ○
        {'\u2476', '\uE097'}, // □
        {'\u2477', '\uE098'}, // △
        {'\u2478', '\uE099'}, // ◇
        {'\u2479', '\uE09A'}, // ♪
        {'\u247a', '\uE09B'}, // ☀
        {'\u247b', '\uE09C'}, // ☁
        {'\u247d', '\uE09D'}, // ☂
    };
}
