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

#if NET9_0_OR_GREATER
    REPLACE WITH TryFromHexString / TryToHexString
#endif

    /// <summary>
    /// Parses a variable length hex string (non-spaced, bytes in reverse order).
    /// </summary>
    public static byte[] GetBytesFromHexString(ReadOnlySpan<char> input)
    {
        byte[] result = new byte[input.Length / 2];
        GetBytesFromHexString(input, result);
        return result;
    }

    /// <inheritdoc cref="GetBytesFromHexString(ReadOnlySpan{char})"/>
    public static void GetBytesFromHexString(ReadOnlySpan<char> input, Span<byte> result)
    {
        for (int i = 0; i < result.Length; i++)
        {
            var slice = input.Slice(i * 2, 2);
            result[^(i + 1)] = (byte)GetHexValue(slice);
        }
    }

    private const string HexChars = "0123456789ABCDEF";

    /// <summary>
    /// Converts the byte array into a hex string (non-spaced, bytes in reverse order).
    /// </summary>
    public static string GetHexStringFromBytes(ReadOnlySpan<byte> data)
    {
        System.Diagnostics.Debug.Assert(data.Length is (4 or 8 or 12 or 16));
        Span<char> result = stackalloc char[data.Length * 2];
        GetHexStringFromBytes(data, result);
        return new string(result);
    }

    /// <inheritdoc cref="GetHexStringFromBytes(ReadOnlySpan{byte})"/>
    public static void GetHexStringFromBytes(ReadOnlySpan<byte> data, Span<char> result)
    {
        if (result.Length != data.Length * 2)
            throw new ArgumentException("Result buffer must be twice the size of the input buffer.");
        for (int i = 0; i < data.Length; i++)
        {
            // Write tuples from the opposite side of the result buffer.
            var offset = (data.Length - i - 1) * 2;
            result[offset + 0] = HexChars[data[i] >> 4];
            result[offset + 1] = HexChars[data[i] & 0xF];
        }
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

    /// <summary>
    /// Returns a new string with each word converted to its appropriate title case.
    /// </summary>
    /// <param name="span">Input string to modify</param>
    public static string ToTitleCase(ReadOnlySpan<char> span)
    {
        if (span.IsEmpty)
            return string.Empty;

        Span<char> result = stackalloc char[span.Length];
        ToTitleCase(span, result);
        return new string(result);
    }

    /// <inheritdoc cref="ToTitleCase(ReadOnlySpan{char})"/>
    public static void ToTitleCase(ReadOnlySpan<char> span, Span<char> result)
    {
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
    }
}
