using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic for converting a <see cref="string"/> for to future Generations.
    /// </summary>
    public static class StringConverter345
    {
        /// <summary>
        /// Trash Bytes for Generation 3->4
        /// </summary>
        /// <remarks>String buffers are reused, data is not cleared which yields the trash bytes.</remarks>
        public static readonly byte[][] G4TransferTrashBytes = {
            Array.Empty<byte>(), // Unused
            new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
            new byte[] { 0x18, 0x20, 0x0D, 0x02, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x48, 0xA1, 0x0C, 0x02, 0xE0, 0xFF },
            new byte[] { 0x74, 0x20, 0x0D, 0x02, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xA4, 0xA1, 0x0C, 0x02, 0xE0, 0xFF },
            new byte[] { 0x54, 0x20, 0x0D, 0x02, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x84, 0xA1, 0x0C, 0x02, 0xE0, 0xFF },
            new byte[] { 0x74, 0x20, 0x0D, 0x02, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xA4, 0xA1, 0x0C, 0x02, 0xE0, 0xFF },
            Array.Empty<byte>(), // Unused
            new byte[] { 0x74, 0x20, 0x0D, 0x02, 0x42, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xA4, 0xA1, 0x0C, 0x02, 0xE0, 0xFF },
        };

        /// <summary>
        /// Remaps Gen5 Glyphs to unicode codepoint.
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Remapped string</returns>
        public static string TransferGlyphs56(string str)
        {
            var result = new char[str.Length];
            var table = Glyph56;
            for (int i = 0; i < str.Length; i++)
            {
                var c = str[i];
                result[i] = table.TryGetValue(c, out var translated) ? translated : c;
            }
            return new string(result);
        }

        private static readonly Dictionary<char, char> Glyph56 = new()
        {
            {'\u2467', '\u00d7'}, // ×
            {'\u2468', '\u00f7'}, // ÷

            {'\u246c', '\u2026'}, // …
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
}
