using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PKHeX.Core
{
    /// <summary>
    /// Logic for converting a <see cref="string"/> between the various generation specific encoding formats.
    /// </summary>
    public static class StringConverter
    {
        /// <summary>
        /// Converts bytes to a string according to the input parameters.
        /// </summary>
        /// <param name="data">Encoded data</param>
        /// <param name="generation">Generation string format</param>
        /// <param name="jp">Encoding is Japanese</param>
        /// <param name="isBigEndian">Encoding is Big Endian</param>
        /// <param name="count">Length of data to read.</param>
        /// <param name="offset">Offset to read from</param>
        /// <returns>Decoded string.</returns>
        public static string GetString(byte[] data, int generation, bool jp, bool isBigEndian, int count, int offset = 0)
        {
            if (isBigEndian)
                return generation == 3 ? StringConverter3.GetBEString3(data, offset, count) : StringConverter4.GetBEString4(data, offset, count);

            return generation switch
            {
                1 or 2 => StringConverter12.GetString1(data, offset, count, jp),
                3 => StringConverter3.GetString3(data, offset, count, jp),
                4 => StringConverter4.GetString4(data, offset, count),
                5 => GetString5(data, offset, count),
                6 => GetString6(data, offset, count),
                _ => GetString7(data, offset, count),
            };
        }

        /// <summary>
        /// Gets the bytes for a Generation specific string according to the input parameters.
        /// </summary>
        /// <param name="value">Decoded string.</param>
        /// <param name="generation">Generation string format</param>
        /// <param name="jp">Encoding is Japanese</param>
        /// <param name="isBigEndian">Encoding is Big Endian</param>
        /// <param name="maxLength">Maximum length of the input <see cref="value"/></param>
        /// <param name="language">Language specific conversion (Chinese)</param>
        /// <param name="padTo">Pad the input <see cref="value"/> to given length</param>
        /// <param name="padWith">Pad the input <see cref="value"/> with this character value</param>
        /// <returns>Encoded data.</returns>
        public static byte[] SetString(string value, int generation, bool jp, bool isBigEndian, int maxLength, int language = 0, int padTo = 0, ushort padWith = 0)
        {
            if (isBigEndian)
                return generation == 3 ? StringConverter3.SetBEString3(value, maxLength, padTo, padWith) : StringConverter4.SetBEString4(value, maxLength, padTo, padWith);

            return generation switch
            {
                1 or 2 => StringConverter12.SetString1(value, maxLength, jp, padTo, padWith),
                3 => StringConverter3.SetString3(value, maxLength, jp, padTo, padWith),
                4 => StringConverter4.SetString4(value, maxLength, padTo, padWith),
                5 => SetString5(value, maxLength, padTo, padWith),
                6 => SetString6(value, maxLength, padTo, padWith),
                _ => SetString7(value, maxLength, language, padTo, padWith),
            };
        }

        /// <summary>Converts Generation 5 encoded data to decoded string.</summary>
        /// <param name="data">Encoded data</param>
        /// <param name="offset">Offset to read from</param>
        /// <param name="count">Length of data to read.</param>
        /// <returns>Decoded string.</returns>
        public static string GetString5(byte[] data, int offset, int count)
        {
            var raw = Encoding.Unicode.GetString(data, offset, count);
            var sb = new StringBuilder(raw);
            Util.TrimFromFFFF(sb);
            SanitizeString(sb);
            return sb.ToString();
        }

        /// <summary>Gets the bytes for a Generation 5 string.</summary>
        /// <param name="value">Decoded string.</param>
        /// <param name="maxLength">Maximum length of the input <see cref="value"/></param>
        /// <param name="padTo">Pad the input <see cref="value"/> to given length</param>
        /// <param name="padWith">Pad the input <see cref="value"/> with this character value</param>
        /// <returns>Encoded data.</returns>
        public static byte[] SetString5(string value, int maxLength, int padTo = 0, ushort padWith = 0)
        {
            var sb = new StringBuilder(value, Math.Max(maxLength, padTo));
            var delta = sb.Length - maxLength;
            if (delta > 0)
                sb.Remove(maxLength, delta);

            // Replace Special Characters and add Terminator
            UnSanitizeString(sb, 5);
            sb.Append((char)0xFFFF);
            var d2 = padTo - sb.Length;
            if (d2 > 0)
                sb.Append((char)padWith, d2);

            return Encoding.Unicode.GetBytes(sb.ToString());
        }

        /// <summary>Converts Generation 6 encoded data to decoded string.</summary>
        /// <param name="data">Encoded data</param>
        /// <param name="offset">Offset to read from</param>
        /// <param name="count">Length of data to read.</param>
        /// <returns>Decoded string.</returns>
        public static string GetString6(byte[] data, int offset, int count)
        {
            var raw = Encoding.Unicode.GetString(data, offset, count);
            var sb = new StringBuilder(raw);
            Util.TrimFromZero(sb);
            SanitizeString(sb);
            return sb.ToString();
        }

        /// <summary>Gets the bytes for a Generation 6 string.</summary>
        /// <param name="value">Decoded string.</param>
        /// <param name="maxLength">Maximum length of the input <see cref="value"/></param>
        /// <param name="padTo">Pad the input <see cref="value"/> to given length</param>
        /// <param name="padWith">Pad the input <see cref="value"/> with this character value</param>
        /// <returns>Encoded data.</returns>
        public static byte[] SetString6(string value, int maxLength, int padTo = 0, ushort padWith = 0)
        {
            var sb = new StringBuilder(value);
            var delta = sb.Length - maxLength;
            if (delta > 0)
                sb.Remove(maxLength, delta);

            // Replace Special Characters and add Terminator
            UnSanitizeString(sb, 6);
            sb.Append((char)0);
            var d2 = padTo - sb.Length;
            if (d2 > 0)
                sb.Append((char)padWith, d2);

            return Encoding.Unicode.GetBytes(sb.ToString());
        }

        /// <summary>Converts Generation 7 encoded data to decoded string.</summary>
        /// <param name="data">Encoded data</param>
        /// <param name="offset">Offset to read from</param>
        /// <param name="count">Length of data to read.</param>
        /// <returns>Decoded string.</returns>
        public static string GetString7(byte[] data, int offset, int count)
        {
            var raw = Encoding.Unicode.GetString(data, offset, count);
            var sb = new StringBuilder(raw);
            Util.TrimFromZero(sb);
            SanitizeString(sb);
            RemapChineseGlyphsBin2String(sb);
            return sb.ToString();
        }

        /// <summary>Gets the bytes for a Generation 7 string.</summary>
        /// <param name="value">Decoded string.</param>
        /// <param name="maxLength">Maximum length of the input <see cref="value"/></param>
        /// <param name="language">Language specific conversion (Chinese)</param>
        /// <param name="padTo">Pad the input <see cref="value"/> to given length</param>
        /// <param name="padWith">Pad the input <see cref="value"/> with this character value</param>
        /// <param name="chinese">Chinese string remapping should be attempted</param>
        /// <returns>Encoded data.</returns>
        public static byte[] SetString7(string value, int maxLength, int language, int padTo = 0, ushort padWith = 0, bool chinese = false)
        {
            var sb = new StringBuilder(value);
            var delta = sb.Length - maxLength;
            if (delta > 0)
                sb.Remove(maxLength, delta);
            if (chinese)
                ConvertString2BinG7_zh(sb, language);

            // Replace Special Characters and add Terminator
            UnSanitizeString(sb, 7);
            sb.Append((char)0);
            var d2 = padTo - sb.Length;
            if (d2 > 0)
                sb.Append((char)padWith, d2);

            return Encoding.Unicode.GetBytes(sb.ToString());
        }

        /// <summary>Gets the bytes for a Generation 7 string.</summary>
        /// <param name="value">Decoded string.</param>
        /// <param name="maxLength">Maximum length of the input <see cref="value"/></param>
        /// <param name="language">Language specific conversion (Chinese)</param>
        /// <param name="padTo">Pad the input <see cref="value"/> to given length</param>
        /// <param name="padWith">Pad the input <see cref="value"/> with this character value</param>
        /// <param name="chinese">Chinese string remapping should be attempted</param>
        /// <returns>Encoded data.</returns>
        public static byte[] SetString7b(string value, int maxLength, int language, int padTo = 0, ushort padWith = 0, bool chinese = false)
        {
            var sb = new StringBuilder(value);
            var delta = sb.Length - maxLength;
            if (delta > 0)
                sb.Remove(maxLength, delta);
            if (chinese)
                ConvertString2BinG7_zh(sb, language);

            // Replace Special Characters and add Terminator
            UnSanitizeString7b(sb);
            sb.Append((char)0);
            var d2 = padTo - sb.Length;
            if (d2 > 0)
                sb.Append((char)padWith, d2);

            return Encoding.Unicode.GetBytes(sb.ToString());
        }

        /// <summary>
        /// Converts a Unicode string to Generation 7 in-game Chinese string.
        /// </summary>
        /// <param name="sb">Unicode string.</param>
        /// <param name="lang">Detection of language for Traditional Chinese check</param>
        /// <returns>In-game Chinese string.</returns>
        private static void ConvertString2BinG7_zh(StringBuilder sb, int lang)
        {
            // A string cannot contain a mix of CHS and CHT characters.
            var input = sb.ToString();
            bool traditional = input.Any(chr => G7_CHT.ContainsKey(chr) && !G7_CHS.ContainsKey(chr))
            || (lang == 10 && !input.Any(chr => G7_CHT.ContainsKey(chr) ^ G7_CHS.ContainsKey(chr))); // CHS and CHT have the same display name
            var table = traditional ? G7_CHT : G7_CHS;

            for (int i = 0; i < sb.Length; i++)
            {
                var chr = sb[i];
                if (table.TryGetValue(chr, out var index))
                    sb[i] = (char) (index + Gen7_ZH_Ofs);
            }
        }

        /// <summary>
        /// Converts a Generation 7 in-game Chinese string to Unicode string.
        /// </summary>
        /// <param name="input">In-game Chinese string.</param>
        /// <returns>Unicode string.</returns>
        private static void RemapChineseGlyphsBin2String(StringBuilder input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                char val = input[i];
                if (val < Gen7_ZH_Ofs || val >= Gen7_ZH_Ofs + Gen7_ZH.Length)
                    continue;
                input[i] = Gen7_ZH[val - Gen7_ZH_Ofs];
            }
        }

        #region Gen 7 Chinese Character Tables
        private static readonly char[] Gen7_ZH = Util.GetStringList("Char", "zh")[0].ToCharArray();
        private const ushort Gen7_ZH_Ofs = 0xE800;
        private const ushort SM_ZHCharTable_Size = 0x30F;
        private const ushort USUM_CHS_Size = 0x4;
        private static bool GetisG7CHSChar(int idx) => idx is < SM_ZHCharTable_Size or >= SM_ZHCharTable_Size * 2 and < (SM_ZHCharTable_Size * 2) + USUM_CHS_Size;

        private static readonly Dictionary<char, int> G7_CHS = Gen7_ZH
            .Select((value, index) => new { value, index })
            .Where(pair => GetisG7CHSChar(pair.index))
            .ToDictionary(pair => pair.value, pair => pair.index);

        private static readonly Dictionary<char, int> G7_CHT = Gen7_ZH
            .Select((value, index) => new { value, index })
            .Where(pair => !GetisG7CHSChar(pair.index))
            .ToDictionary(pair => pair.value, pair => pair.index);
        #endregion

        /// <summary>
        /// Converts full width to single width
        /// </summary>
        /// <param name="s">Input string to sanitize.</param>
        /// <returns></returns>
        internal static void SanitizeString(StringBuilder s)
        {
            if (s.Length == 0)
                return;
            s.Replace('’', '\''); // Farfetch'd

            // remap custom glyphs to unicode
            s.Replace('\uE08F', '♀'); // ♀ (gen6+)
            s.Replace('\uE08E', '♂'); // ♂ (gen6+)
            s.Replace('\u246E', '♀'); // ♀ (gen5)
            s.Replace('\u246D', '♂'); // ♂ (gen5)
        }

        /// <summary>
        /// Converts full width to half width when appropriate
        /// </summary>
        /// <param name="str">Input string to set.</param>
        /// <returns></returns>
        private static void UnSanitizeString7b(StringBuilder str)
        {
            // gender chars always full width
            str.Replace('\'', '’'); // Farfetch'd
        }

        /// <summary>
        /// Converts full width to half width when appropriate
        /// </summary>
        /// <param name="str">Input string to set.</param>
        /// <param name="generation">Generation specific context</param>
        /// <returns></returns>
        internal static void UnSanitizeString(StringBuilder str, int generation)
        {
            if (generation >= 6)
                str.Replace('\'', '’'); // Farfetch'd

            if (generation <= 5)
            {
                str.Replace('\u2640', '\u246E'); // ♀
                str.Replace('\u2642', '\u246D'); // ♂
                return;
            }

            var context = str.ToString().Except(FullToHalf);
            bool fullwidth = context.Select(c => c >> 12) // select the group the char belongs to
                .Any(c => c is not (0 or 0xE) /* Latin, Special Symbols */);

            if (fullwidth) // jp/ko/zh strings
                return; // keep as full width

            // Convert back to half width glyphs
            str.Replace('\u2640', '\uE08F'); // ♀
            str.Replace('\u2642', '\uE08E'); // ♂
        }

        private static readonly char[] FullToHalf = {'\u2640', '\u2642'}; // ♀♂

        public static bool HasEastAsianScriptCharacters(IEnumerable<char> str) => str.Any(c => c is >= '\u4E00' and <= '\u9FFF');
    }
}
