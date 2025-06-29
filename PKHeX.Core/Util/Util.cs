using System;

namespace PKHeX.Core;

public static partial class Util
{
    /// <summary>
    /// Parses the string into a 32-bit integer, skipping all characters except for valid digits.
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
                result |= (uint)(c - '0');
            }
            else if (char.IsAsciiHexDigitUpper(c))
            {
                result <<= 4;
                result |= (uint)(c - 'A' + 10);
            }
            else if (char.IsAsciiHexDigitLower(c))
            {
                result <<= 4;
                result |= (uint)(c - 'a' + 10);
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
                result |= (uint)(c - '0');
            }
            else if (char.IsAsciiHexDigitUpper(c))
            {
                result <<= 4;
                result |= (uint)(c - 'A' + 10);
            }
            else if (char.IsAsciiHexDigitLower(c))
            {
                result <<= 4;
                result |= (uint)(c - 'a' + 10);
            }
        }
        return result;
    }

    /// <inheritdoc cref="GetBytesFromHexString(ReadOnlySpan{char}, Span{byte})"/>
    public static byte[] GetBytesFromHexString(ReadOnlySpan<char> input)
        => Convert.FromHexString(input);

    /// <summary>
    /// Parses a variable length hex string (non-spaced, bytes in order).
    /// </summary>
    /// <param name="input">Hex string to parse</param>
    /// <param name="result">Buffer to write the result to</param>
    public static void GetBytesFromHexString(ReadOnlySpan<char> input, Span<byte> result)
        => Convert.FromHexString(input, result, out _, out _);

    /// <summary>
    /// Converts the byte array into a hex string (non-spaced, bytes in order).
    /// </summary>
    public static string GetHexStringFromBytes(ReadOnlySpan<byte> data)
    {
        System.Diagnostics.Debug.Assert(data.Length is (4 or 8 or 12 or 16));
        return Convert.ToHexString(data);
    }

    /// <summary>
    /// Filters the string down to only valid hex characters, returning a new string.
    /// </summary>
    /// <param name="str">Input string to filter</param>
    public static string GetOnlyHex(ReadOnlySpan<char> str)
    {
        if (str.IsWhiteSpace())
            return string.Empty;

        Span<char> result = stackalloc char[str.Length];
        int ctr = GetOnlyHex(str, result);
        return new string(result[..ctr]);
    }

    /// <inheritdoc cref="GetOnlyHex(ReadOnlySpan{char})"/>
    public static int GetOnlyHex(ReadOnlySpan<char> str, Span<char> result)
    {
        int ctr = 0;
        foreach (var c in str)
        {
            if (char.IsAsciiHexDigit(c))
                result[ctr++] = c;
        }
        return ctr;
    }

    /// <inheritdoc cref="ToTitleCase(ReadOnlySpan{char}, Span{char})"/>
    public static string ToTitleCase(ReadOnlySpan<char> span)
    {
        if (span.IsEmpty)
            return string.Empty;

        Span<char> result = stackalloc char[span.Length];
        ToTitleCase(span, result);
        return new string(result);
    }

    /// <summary>
    /// Returns a new string with each word converted to its appropriate title case.
    /// </summary>
    /// <remarks>
    /// Assumes that words are separated by whitespace characters. Duplicate whitespace are not skipped.
    /// </remarks>
    /// <param name="span">Input string to modify</param>
    /// <param name="result">>Buffer to write the result to</param>
    public static void ToTitleCase(ReadOnlySpan<char> span, Span<char> result)
    {
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
    }
}
