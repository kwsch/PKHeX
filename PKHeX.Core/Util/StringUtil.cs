using System;
using System.Globalization;

namespace PKHeX.Core;

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
    public static int FindIndexIgnoreCase(ReadOnlySpan<string> arr, ReadOnlySpan<char> value)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            if (IsMatchIgnoreCase(arr[i], value))
                return i;
        }
        return -1;
    }

    public static bool IsMatchIgnoreCase(ReadOnlySpan<char> string1, ReadOnlySpan<char> string2)
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
    public static string GetNthEntry(ReadOnlySpan<char> line, int nth, int start, char separator = ',')
    {
        if (nth != 1)
            start = line.IndexOfNth(separator, nth - 1, start + 1);
        var end = line.IndexOfNth(separator, 1, start + 1);
        if (end == -1)
            return new string(line[(start + 1)..]);
        return new string(line[(start + 1)..end]);
    }

    private static int IndexOfNth(this ReadOnlySpan<char> s, char t, int n, int start)
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

    /// <summary>
    /// Converts an all-caps hex string to a byte array.
    /// </summary>
    public static byte[] ToByteArray(this string toTransform)
    {
        var result = new byte[toTransform.Length / 2];
        for (int i = 0; i < result.Length; i++)
        {
            var ofs = i << 1;
            var _0 = toTransform[ofs + 0];
            var _1 = toTransform[ofs + 1];
            result[i] = DecodeTuple(_0, _1);
        }
        return result;
    }

    private static byte DecodeTuple(char _0, char _1)
    {
        byte result;
        if (char.IsAsciiDigit(_0))
            result = (byte)((_0 - '0') << 4);
        else if (char.IsAsciiHexDigitUpper(_0))
            result = (byte)((_0 - 'A' + 10) << 4);
        else
            throw new ArgumentOutOfRangeException(nameof(_0));

        if (char.IsAsciiDigit(_1))
            result |= (byte)(_1 - '0');
        else if (char.IsAsciiHexDigitUpper(_1))
            result |= (byte)(_1 - 'A' + 10);
        else
            throw new ArgumentOutOfRangeException(nameof(_1));
        return result;
    }
}
