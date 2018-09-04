using System;
using System.Linq;

namespace PKHeX.Core
{
    public static partial class Util
    {
        public static int ToInt32(string value)
        {
            string val = value?.Replace(" ", "").Replace("_", "").Trim();
            return string.IsNullOrWhiteSpace(val) ? 0 : int.Parse(val);
        }

        public static uint ToUInt32(string value)
        {
            string val = value?.Replace(" ", "").Replace("_", "").Trim();
            return string.IsNullOrWhiteSpace(val) ? 0 : uint.Parse(val);
        }

        public static uint GetHexValue(string s)
        {
            string str = GetOnlyHex(s);
            return string.IsNullOrWhiteSpace(str) ? 0 : Convert.ToUInt32(str, 16);
        }

        private static bool IsHex(char c) => (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f');
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
    }
}
