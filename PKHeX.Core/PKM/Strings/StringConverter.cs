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
        /// <param name="bigendian">Encoding is BigEndian</param>
        /// <param name="count">Length of data to read.</param>
        /// <param name="offset">Offset to read from</param>
        /// <returns>Decoded string.</returns>
        public static string GetString(byte[] data, int generation, bool jp, bool bigendian, int count, int offset = 0)
        {
            if (bigendian)
                return generation == 3 ? StringConverter3.GetBEString3(data, offset, count) : StringConverter4.GetBEString4(data, offset, count);

            switch (generation)
            {
                case 1:
                case 2: return StringConverter12.GetString1(data, offset, count, jp);
                case 3: return StringConverter3.GetString3(data, offset, count, jp);
                case 4: return StringConverter4.GetString4(data, offset, count);
                case 5: return GetString5(data, offset, count);
                case 6: return GetString6(data, offset, count);
                default: return GetString7(data, offset, count);
            }
        }

        /// <summary>
        /// Gets the bytes for a Generation specific string according to the input parameters.
        /// </summary>
        /// <param name="value">Decoded string.</param>
        /// <param name="generation">Generation string format</param>
        /// <param name="jp">Encoding is Japanese</param>
        /// <param name="bigendian">Encoding is BigEndian</param>
        /// <param name="maxLength"></param>
        /// <param name="language"></param>
        /// <param name="padTo">Pad to given length</param>
        /// <param name="padWith">Pad with value</param>
        /// <returns>Encoded data.</returns>
        public static byte[] SetString(string value, int generation, bool jp, bool bigendian, int maxLength, int language = 0, int padTo = 0, ushort padWith = 0)
        {
            if (bigendian)
                return generation == 3 ? StringConverter3.SetBEString3(value, maxLength, padTo, padWith) : StringConverter4.SetBEString4(value, maxLength, padTo, padWith);

            switch (generation)
            {
                case 1:
                case 2: return StringConverter12.SetString1(value, maxLength, jp, padTo, padWith);
                case 3: return StringConverter3.SetString3(value, maxLength, jp, padTo, padWith);
                case 4: return StringConverter4.SetString4(value, maxLength, padTo, padWith);
                case 5: return SetString5(value, maxLength, padTo, padWith);
                case 6: return SetString6(value, maxLength, padTo, padWith);
                default: return SetString7(value, maxLength, language, padTo, padWith);
            }
        }

        /// <summary>Converts Generation 5 encoded data to decoded string.</summary>
        /// <param name="data">Encoded data</param>
        /// <param name="offset">Offset to read from</param>
        /// <param name="count">Length of data to read.</param>
        /// <returns>Decoded string.</returns>
        public static string GetString5(byte[] data, int offset, int count)
        {
            return SanitizeString(Util.TrimFromFFFF(Encoding.Unicode.GetString(data, offset, count)));
        }

        /// <summary>Gets the bytes for a Generation 5 string.</summary>
        /// <param name="value">Decoded string.</param>
        /// <param name="maxLength">Maximum length</param>
        /// <param name="padTo">Pad to given length</param>
        /// <param name="padWith">Pad with value</param>
        /// <returns>Encoded data.</returns>
        public static byte[] SetString5(string value, int maxLength, int padTo = 0, ushort padWith = 0)
        {
            if (value.Length > maxLength)
                value = value.Substring(0, maxLength); // Hard cap
            string temp = UnSanitizeString(value, 5)
                .PadRight(value.Length + 1, (char)0xFFFF) // Null Terminator
                .PadRight(padTo, (char)padWith); // Padding
            return Encoding.Unicode.GetBytes(temp);
        }

        /// <summary>Converts Generation 6 encoded data to decoded string.</summary>
        /// <param name="data">Encoded data</param>
        /// <param name="offset">Offset to read from</param>
        /// <param name="count">Length of data to read.</param>
        /// <returns>Decoded string.</returns>
        public static string GetString6(byte[] data, int offset, int count)
        {
            return SanitizeString(Util.TrimFromZero(Encoding.Unicode.GetString(data, offset, count)));
        }

        /// <summary>Gets the bytes for a Generation 6 string.</summary>
        /// <param name="value">Decoded string.</param>
        /// <param name="maxLength">Maximum length</param>
        /// <param name="padTo">Pad to given length</param>
        /// <param name="padWith">Pad with value</param>
        /// <returns>Encoded data.</returns>
        public static byte[] SetString6(string value, int maxLength, int padTo = 0, ushort padWith = 0)
        {
            if (value.Length > maxLength)
                value = value.Substring(0, maxLength); // Hard cap
            string temp = UnSanitizeString(value, 6)
                .PadRight(value.Length + 1, '\0') // Null Terminator
                .PadRight(padTo, (char)padWith);
            return Encoding.Unicode.GetBytes(temp);
        }

        /// <summary>Converts Generation 7 encoded data to decoded string.</summary>
        /// <param name="data">Encoded data</param>
        /// <param name="offset">Offset to read from</param>
        /// <param name="count">Length of data to read.</param>
        /// <returns>Decoded string.</returns>
        public static string GetString7(byte[] data, int offset, int count)
        {
            return ConvertBin2StringG7_zh(SanitizeString(Util.TrimFromZero(Encoding.Unicode.GetString(data, offset, count))));
        }

        /// <summary>Gets the bytes for a Generation 7 string.</summary>
        /// <param name="value">Decoded string.</param>
        /// <param name="maxLength">Maximum length</param>
        /// <param name="language">Language specific conversion (Chinese)</param>
        /// <param name="padTo">Pad to given length</param>
        /// <param name="padWith">Pad with value</param>
        /// <param name="chinese">Chinese string remapping should be attempted</param>
        /// <returns>Encoded data.</returns>
        public static byte[] SetString7(string value, int maxLength, int language, int padTo = 0, ushort padWith = 0, bool chinese = false)
        {
            if (chinese)
                value = ConvertString2BinG7_zh(value, language);
            if (value.Length > maxLength)
                value = value.Substring(0, 12); // Hard cap
            string temp = UnSanitizeString(value, 7)
                .PadRight(value.Length + 1, '\0') // Null Terminator
                .PadRight(padTo, (char)padWith);
            return Encoding.Unicode.GetBytes(temp);
        }

        /// <summary>Gets the bytes for a Generation 7 string.</summary>
        /// <param name="value">Decoded string.</param>
        /// <param name="maxLength">Maximum length</param>
        /// <param name="language">Language specific conversion (Chinese)</param>
        /// <param name="padTo">Pad to given length</param>
        /// <param name="padWith">Pad with value</param>
        /// <param name="chinese">Chinese string remapping should be attempted</param>
        /// <returns>Encoded data.</returns>
        public static byte[] SetString7b(string value, int maxLength, int language, int padTo = 0, ushort padWith = 0, bool chinese = false)
        {
            if (chinese)
                value = ConvertString2BinG7_zh(value, language);
            if (value.Length > maxLength)
                value = value.Substring(0, 12); // Hard cap
            string temp = UnSanitizeString7b(value)
                .PadRight(value.Length + 1, '\0') // Null Terminator
                .PadRight(padTo, (char)padWith);
            return Encoding.Unicode.GetBytes(temp);
        }

        /// <summary>
        /// Converts a Unicode string to Generation 7 in-game chinese string.
        /// </summary>
        /// <param name="inputstr">Unicode string.</param>
        /// <param name="lang">Detection of language for Traditional Chinese check</param>
        /// <returns>In-game chinese string.</returns>
        private static string ConvertString2BinG7_zh(string inputstr, int lang)
        {
            var str = new StringBuilder();

            bool cht = lang == 10;
            // A string cannot contain a mix of CHS and CHT characters.
            bool IsCHT = inputstr.Any(chr => G7_CHT.ContainsKey(chr) && !G7_CHS.ContainsKey(chr));
            IsCHT |= cht && !inputstr.Any(chr => G7_CHT.ContainsKey(chr) ^ G7_CHS.ContainsKey(chr)); // CHS and CHT have the same display name
            var table = IsCHT ? G7_CHT : G7_CHS;

            foreach (char chr in inputstr)
                str.Append(table.TryGetValue(chr, out int index) ? (char)(index + Gen7_ZH_Ofs) : chr);
            return str.ToString();
        }

        /// <summary>
        /// Converts a Generation 7 in-game chinese string to Unicode string.
        /// </summary>
        /// <param name="inputstr">In-game chinese string.</param>
        /// <returns>Unicode string.</returns>
        private static string ConvertBin2StringG7_zh(string inputstr)
        {
            var str = new StringBuilder();
            foreach (var val in inputstr)
                str.Append((char)Getg7zhChar(val));
            return str.ToString();
        }

        /// <summary>
        /// Shifts a character from the Chinese character tables
        /// </summary>
        /// <param name="val">Input value to shift</param>
        /// <returns>Shifted character</returns>
        private static ushort Getg7zhChar(ushort val)
        {
            if (Gen7_ZH_Ofs <= val && val < Gen7_ZH_Ofs + Gen7_ZH.Length)
                return Gen7_ZH[val - Gen7_ZH_Ofs];
            return val; // regular character
        }

        #region Gen 7 Chinese Character Tables
        private static readonly char[] Gen7_ZH = Util.GetStringList("Char", "zh")[0].ToCharArray();
        private const ushort Gen7_ZH_Ofs = 0xE800;
        private const ushort SM_ZHCharTable_Size = 0x30F;
        private const ushort USUM_CHS_Size = 0x4;
        private static bool GetisG7CHSChar(int idx) => idx < SM_ZHCharTable_Size || (SM_ZHCharTable_Size * 2 <= idx && idx < (SM_ZHCharTable_Size * 2) + USUM_CHS_Size);

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
        /// <param name="str">Input string to sanitize.</param>
        /// <returns></returns>
        internal static string SanitizeString(string str)
        {
            if (str.Length == 0)
                return str;
            var s = str.Replace('’', '\''); // farfetch'd

            // remap custom glyphs to unicode
            s = s.Replace('\uE08F', '♀'); // ♀ (gen6+)
            s = s.Replace('\uE08E', '♂'); // ♂ (gen6+)
            s = s.Replace('\u246E', '♀'); // ♀ (gen5)
            return s.Replace('\u246D', '♂'); // ♂ (gen5)
        }

        /// <summary>
        /// Converts full width to half width when appropriate
        /// </summary>
        /// <param name="str">Input string to set.</param>
        /// <returns></returns>
        private static string UnSanitizeString7b(string str)
        {
            // gender chars always full width
            return str.Replace('\'', '’'); // farfetch'd
        }

        /// <summary>
        /// Converts full width to half width when appropriate
        /// </summary>
        /// <param name="str">Input string to set.</param>
        /// <param name="generation">Generation specific context</param>
        /// <returns></returns>
        internal static string UnSanitizeString(string str, int generation)
        {
            var s = str;
            if (generation >= 6)
                s = str.Replace('\'', '’'); // farfetch'd

            if (generation <= 5)
            {
                s = s.Replace('\u2640', '\u246E'); // ♀
                return s.Replace('\u2642', '\u246D'); // ♂
            }

            var langcontext = str.Except(FullToHalf);
            bool fullwidth = langcontext.Select(c => c >> 12) // select the group the char belongs to
                .Any(c => c != 0 /* Latin */ && c != 0xE /* Special Symbols */);

            if (fullwidth) // jp/ko/zh strings
                return s; // keep as full width

            // Convert back to half width glyphs
            s = s.Replace('\u2640', '\uE08F'); // ♀
            return s.Replace('\u2642', '\uE08E'); // ♂
        }

        private static readonly char[] FullToHalf = {'\u2640', '\u2642'}; // ♀♂

        public static bool HasEastAsianScriptCharacters(IEnumerable<char> str) => str.Any(c => 0x4E00 <= c && c <= 0x9FFF);
    }
}
