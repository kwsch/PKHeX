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
    /// Gets the <see cref="nth"/> string entry within the input <see cref="line"/>, based on the <see cref="separator"/>.
    /// </summary>
    public static ReadOnlySpan<char> GetNthEntry(ReadOnlySpan<char> line, int nth, char separator = '\t')
    {
        int start = 0;
        if (nth != 0)
            start = line.IndexOfNth(separator, nth) + 1; // 1 char after separator

        var tail = line[start..];
        var end = tail.IndexOf(separator);
        if (end == -1)
            return tail;
        return tail[..end];
    }

    private static int IndexOfNth(this ReadOnlySpan<char> s, char t, int n)
    {
        int count = 0;
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] != t)
                continue;
            if (++count == n)
                return i;
        }
        return -1;
    }

    /// <summary>
    /// Converts an all-caps encoded ASCII-Text hex string to a byte array.
    /// </summary>
    public static void LoadHexBytesTo(ReadOnlySpan<byte> str, Span<byte> dest, int tupleSize)
    {
        // The input string is 2-char hex values optionally separated.
        // The destination array should always be larger or equal than the bytes written. Let the runtime bounds check us.
        // Iterate through the string without allocating.
        for (int i = 0, j = 0; i < str.Length; i += tupleSize)
            dest[j++] = DecodeTuple((char)str[i + 0], (char)str[i + 1]);
    }

    /// <summary>
    /// Converts an all-caps hex string to a byte array.
    /// </summary>
    public static void LoadHexBytesTo(ReadOnlySpan<char> str, Span<byte> dest, int tupleSize)
    {
        // The input string is 2-char hex values optionally separated.
        // The destination array should always be larger or equal than the bytes written. Let the runtime bounds check us.
        // Iterate through the string without allocating.
        for (int i = 0, j = 0; i < str.Length; i += tupleSize)
            dest[j++] = DecodeTuple(str[i + 0], str[i + 1]);
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
