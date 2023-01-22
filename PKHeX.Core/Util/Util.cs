using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PKHeX.Core;

public static partial class Util
{
    /// <summary>
    /// Parses the string into an <see cref="int"/>, skipping all characters except for valid digits.
    /// </summary>
    /// <param name="value">String to parse</param>
    /// <returns>Parsed value</returns>
    public static int ToInt32(ReadOnlySpan<char> value)
    {
        int result = 0;
        if (value.Length == 0)
            return result;

        bool negative = false;
        foreach (var c in value)
        {
            if (char.IsAsciiDigit(c))
            {
                result *= 10;
                result += c;
                result -= '0';
            }
            else if (c == '-' && result == 0)
            {
                negative = true;
            }
        }
        return negative ? -result : result;
    }

    /// <summary>
    /// Parses the string into a <see cref="uint"/>, skipping all characters except for valid digits.
    /// </summary>
    /// <param name="value">String to parse</param>
    /// <returns>Parsed value</returns>
    public static uint ToUInt32(ReadOnlySpan<char> value)
    {
        uint result = 0;
        if (value.Length == 0)
            return result;

        foreach (var c in value)
        {
            if (!char.IsAsciiDigit(c))
                continue;
            result *= 10;
            result += (uint)(c - '0');
        }
        return result;
    }

    /// <summary>
    /// Parses the hex string into a <see cref="uint"/>, skipping all characters except for valid digits.
    /// </summary>
    /// <param name="value">Hex String to parse</param>
    /// <returns>Parsed value</returns>
    public static uint GetHexValue(ReadOnlySpan<char> value)
    {
        uint result = 0;
        if (value.Length == 0)
            return result;

        foreach (var c in value)
        {
            if (char.IsAsciiDigit(c))
            {
                result <<= 4;
                result += (uint)(c - '0');
            }
            else if (char.IsAsciiHexDigitUpper(c))
            {
                result <<= 4;
                result += (uint)(c - 'A' + 10);
            }
            else if (char.IsAsciiHexDigitLower(c))
            {
                result <<= 4;
                result += (uint)(c - 'a' + 10);
            }
        }
        return result;
    }

    /// <summary>
    /// Parses the hex string into a <see cref="ulong"/>, skipping all characters except for valid digits.
    /// </summary>
    /// <param name="value">Hex String to parse</param>
    /// <returns>Parsed value</returns>
    public static ulong GetHexValue64(ReadOnlySpan<char> value)
    {
        ulong result = 0;
        if (value.Length == 0)
            return result;

        foreach (var c in value)
        {
            if (char.IsAsciiDigit(c))
            {
                result <<= 4;
                result += (uint)(c - '0');
            }
            else if (char.IsAsciiHexDigitUpper(c))
            {
                result <<= 4;
                result += (uint)(c - 'A' + 10);
            }
            else if (char.IsAsciiHexDigitLower(c))
            {
                result <<= 4;
                result += (uint)(c - 'a' + 10);
            }
        }
        return result;
    }

    public static byte[] GetBytesFromHexString(ReadOnlySpan<char> seed)
    {
        byte[] result = new byte[seed.Length / 2];
        for (int i = 0; i < result.Length; i++)
        {
            var slice = seed.Slice(i * 2, 2);
            result[^(i+1)] = (byte)GetHexValue(slice);
        }
        return result;
    }

    private const string HexChars = "0123456789ABCDEF";

    public static string GetHexStringFromBytes(ReadOnlySpan<byte> data)
    {
        System.Diagnostics.Debug.Assert(data.Length is (4 or 8 or 12 or 16));
        Span<char> result = stackalloc char[data.Length * 2];
        for (int i = 0; i < data.Length; i++)
        {
            // Write tuples from the opposite side of the result buffer.
            var offset = (data.Length - i - 1) * 2;
            result[offset + 0] = HexChars[data[i] >> 4];
            result[offset + 1] = HexChars[data[i] & 0xF];
        }
        return new string(result);
    }

    /// <summary>
    /// Filters the string down to only valid hex characters, returning a new string.
    /// </summary>
    /// <param name="str">Input string to filter</param>
    public static string GetOnlyHex(ReadOnlySpan<char> str)
    {
        if (str.IsWhiteSpace())
            return string.Empty;

        int ctr = 0;
        Span<char> result = stackalloc char[str.Length];
        foreach (var c in str)
        {
            if (char.IsAsciiHexDigit(c))
                result[ctr++] = c;
        }
        return new string(result[..ctr]);
    }

    /// <summary>
    /// Returns a new string with each word converted to its appropriate title case.
    /// </summary>
    /// <param name="span">Input string to modify</param>
    public static string ToTitleCase(ReadOnlySpan<char> span)
    {
        if (span.IsEmpty)
            return string.Empty;

        Span<char> result = stackalloc char[span.Length];
        // Add each word to the string builder. Continue from the first index that isn't a space.
        // Add the first character as uppercase, then add each successive character as lowercase.
        bool first = true;
        for (var i = 0; i < span.Length; i++)
        {
            char c = span[i];
            if (char.IsWhiteSpace(c))
            {
                first = true;
            }
            else if (first)
            {
                c = char.ToUpper(c);
                first = false;
            }
            else
            {
                c = char.ToLower(c);
            }
            result[i] = c;
        }
        return new string(result);
    }

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
        return index < 0 ? input : input[..index];
    }

    public static Dictionary<string, int>[] GetMultiDictionary(IReadOnlyList<IReadOnlyList<string>> nameArray, int start)
    {
        var result = new Dictionary<string, int>[nameArray.Count];
        for (int i = 0; i < result.Length; i++)
            result[i] = GetDictionary(nameArray[i], start);
        return result;
    }

    private static Dictionary<string, int> GetDictionary(IReadOnlyList<string> names, int start)
    {
        var result = new Dictionary<string, int>(names.Count - start);
        for (int i = start; i < names.Count; i++)
            result.Add(names[i], i);
        return result;
    }
}
