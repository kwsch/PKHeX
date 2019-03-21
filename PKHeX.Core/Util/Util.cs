using System.Linq;
using System.Runtime.CompilerServices;

namespace PKHeX.Core
{
    public static partial class Util
    {
        /// <summary>
        /// Parses the string into an <see cref="int"/>, skipping all characters except for valid digits.
        /// </summary>
        /// <param name="value">String to parse</param>
        /// <returns>Parsed value</returns>
        public static int ToInt32(string value)
        {
            int result = 0;
            if (string.IsNullOrEmpty(value))
                return result;

            for (int i = 0; i < value.Length; i++)
            {
                var c = value[i];
                if (IsNum(c))
                {
                    result *= 10;
                    result += c;
                    result -= '0';
                }
                else if (c == '-')
                {
                    result = -result;
                }
            }
            return result;
        }

        /// <summary>
        /// Parses the string into a <see cref="uint"/>, skipping all characters except for valid digits.
        /// </summary>
        /// <param name="value">String to parse</param>
        /// <returns>Parsed value</returns>
        public static uint ToUInt32(string value)
        {
            uint result = 0;
            if (string.IsNullOrEmpty(value))
                return result;

            for (int i = 0; i < value.Length; i++)
            {
                var c = value[i];
                if (IsNum(c))
                {
                    result *= 10;
                    result += c;
                    result -= '0';
                }
            }
            return result;
        }

        /// <summary>
        /// Parses the hex string into a <see cref="uint"/>, skipping all characters except for valid digits.
        /// </summary>
        /// <param name="value">Hex String to parse</param>
        /// <returns>Parsed value</returns>
        public static uint GetHexValue(string value)
        {
            uint result = 0;
            if (string.IsNullOrEmpty(value))
                return result;

            for (int i = 0; i < value.Length; i++)
            {
                var c = value[i];
                if (IsNum(c))
                {
                    result <<= 4;
                    result += (uint)(c - '0');
                }
                else if (IsHexUpper(c))
                {
                    result <<= 4;
                    result += (uint)(c - 'A' + 10);
                }
                else if (IsHexLower(c))
                {
                    result <<= 4;
                    result += (uint)(c - 'a' + 10);
                }
            }
            return result;
        }

        private static bool IsNum(char c) => c >= '0' && c <= '9';
        private static bool IsHexUpper(char c) => c >= 'A' && c <= 'F';
        private static bool IsHexLower(char c) => c >= 'a' && c <= 'f';
        private static bool IsHex(char c) => IsNum(c) || IsHexUpper(c) || IsHexLower(c);
        private static string TitleCase(string word) => char.ToUpper(word[0]) + word.Substring(1, word.Length - 1).ToLower();

        /// <summary>
        /// Filters the string down to only valid hex characters, returning a new string.
        /// </summary>
        /// <param name="str">Input string to filter</param>
        public static string GetOnlyHex(string str) => string.IsNullOrWhiteSpace(str) ? string.Empty : string.Concat(str.Where(IsHex));

        /// <summary>
        /// Returns a new string with each word converted to its appropriate title case.
        /// </summary>
        /// <param name="str">Input string to modify</param>
        public static string ToTitleCase(string str) => string.IsNullOrWhiteSpace(str) ? string.Empty : string.Join(" ", str.Split(' ').Select(TitleCase));

        /// <summary>
        /// Trims a string at the first instance of a 0xFFFF terminator.
        /// </summary>
        /// <param name="input">String to trim.</param>
        /// <returns>Trimmed string.</returns>
        public static string TrimFromFFFF(string input) => TrimFromFirst(input, (char)0xFFFF);

        /// <summary>
        /// Trims a string at the first instance of a 0x0000 terminator.
        /// </summary>
        /// <param name="input">String to trim.</param>
        /// <returns>Trimmed string.</returns>
        public static string TrimFromZero(string input) => TrimFromFirst(input, '\0');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string TrimFromFirst(string input, char c)
        {
            int index = input.IndexOf(c);
            return index < 0 ? input : input.Substring(0, index);
        }
    }
}
