using System;
using System.Globalization;

namespace PKHeX.Core
{
    public static class StringUtil
    {
        private static readonly CultureInfo SearchCulture = CultureInfo.CurrentCulture;
        public static int FindIndexIgnoreCase(string[] arr, string value) => Array.FindIndex(arr, z => IsMatchIgnoreCase(z, value));

        private static bool IsMatchIgnoreCase(string string1, string string2)
        {
            if (string1.Length != string2.Length)
                return false;
            const CompareOptions options = CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase | CompareOptions.IgnoreSymbols;
            var compare = SearchCulture.CompareInfo.Compare(string1, string2, options);
            return compare == 0;
        }

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
