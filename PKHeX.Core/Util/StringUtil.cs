using System;
using System.Collections.Generic;
using System.Globalization;

namespace PKHeX.Core
{
    /// <summary>
    /// Utility for searching strings within arrays or within another string.
    /// </summary>
    public static class StringUtil
    {
        private static readonly CompareInfo CompareInfo = CultureInfo.CurrentCulture.CompareInfo;

        /// <summary>
        /// Finds the index of the string within the array by ignoring casing, spaces, and punctuation.
        /// </summary>
        /// <param name="arr">Array of strings to search in</param>
        /// <param name="value">Value to search for</param>
        /// <returns>Index within <see cref="arr"/></returns>
        public static int FindIndexIgnoreCase(string[] arr, string value) => Array.FindIndex(arr, z => IsMatchIgnoreCase(z, value));

        /// <summary>
        /// Gets the indexes by calling <see cref="FindIndexIgnoreCase"/> for all <see cref="items"/>.
        /// </summary>
        /// <param name="arr">Array of strings to search in</param>
        /// <param name="items">Items to search for</param>
        /// <returns>Index within <see cref="arr"/></returns>
        public static int[] GetIndexes(string[] arr, IReadOnlyList<string> items) => GetIndexes(arr, items, 0, items.Count);

        /// <summary>
        /// Gets the indexes by calling <see cref="FindIndexIgnoreCase"/> for all <see cref="items"/>.
        /// </summary>
        /// <param name="arr">Array of strings to search in</param>
        /// <param name="items">Items to search for</param>
        /// <param name="start">Starting index within <see cref="items"/></param>
        /// <param name="length">Amount to convert within <see cref="items"/></param>
        /// <returns>Index within <see cref="arr"/></returns>
        public static int[] GetIndexes(string[] arr, IReadOnlyList<string> items, int start, int length)
        {
            if (length < 0)
                length = items.Count - start;
            var result = new int[length];
            for (int i = 0; i < result.Length; i++)
                result[i] = FindIndexIgnoreCase(arr, items[start + i]);
            return result;
        }

        private static bool IsMatchIgnoreCase(string string1, string string2)
        {
            if (string1.Length != string2.Length)
                return false;
            const CompareOptions options = CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreWidth;
            var compare = CompareInfo.Compare(string1, string2, options);
            return compare == 0;
        }

        /// <summary>
        /// Gets the <see cref="nth"/> string entry within the input <see cref="line"/>, based on the <see cref="separator"/> and <see cref="start"/> position.
        /// </summary>
        public static string GetNthEntry(string line, int nth, int start, char separator = ',')
        {
            if (nth != 1)
                start = line.IndexOfNth(separator, nth - 1, start + 1);
            var end = line.IndexOfNth(separator, 1, start + 1);
            return end < 0 ? line.Substring(start + 1) : line.Substring(start + 1, end - start - 1);
        }

        private static int IndexOfNth(this string s, char t, int n, int start)
        {
            int count = 0;
            for (int i = start; i < s.Length; i++)
            {
                if (s[i] != t)
                    continue;
                if (++count == n)
                    return i;
            }
            return -1;
        }
    }
}
