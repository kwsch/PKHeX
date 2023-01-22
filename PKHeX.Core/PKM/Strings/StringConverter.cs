using System;

namespace PKHeX.Core;

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
    /// <param name="isBigEndian">Encoding is Big Endian</param>
    /// <returns>Decoded string.</returns>
    public static string GetString(ReadOnlySpan<byte> data, int generation, bool jp, bool isBigEndian = false) => generation switch
    {
        3 when isBigEndian => StringConverter3GC.GetString(data),
        4 when isBigEndian => StringConverter4GC.GetString(data),

        1 or 2 => StringConverter12.GetString(data, jp),
        3 => StringConverter3.GetString(data, jp),
        4 => StringConverter4.GetString(data),
        5 => StringConverter5.GetString(data),
        6 => StringConverter6.GetString(data),
        7 => StringConverter7.GetString(data),
        8 => StringConverter8.GetString(data),
        9 => StringConverter8.GetString(data),
        _ => throw new ArgumentOutOfRangeException(nameof(generation)),
    };

    /// <summary>
    /// Converts bytes to a string according to the input parameters.
    /// </summary>
    /// <param name="data">Encoded data</param>
    /// <param name="result">Decoded character result buffer</param>
    /// <param name="generation">Generation string format</param>
    /// <param name="jp">Encoding is Japanese</param>
    /// <param name="isBigEndian">Encoding is Big Endian</param>
    /// <returns>Decoded string.</returns>
    public static int LoadString(ReadOnlySpan<byte> data, Span<char> result, int generation, bool jp, bool isBigEndian = false) => generation switch
    {
        3 when isBigEndian => StringConverter3GC.LoadString(data, result),
        4 when isBigEndian => StringConverter4GC.LoadString(data, result),

        1 or 2 => StringConverter12.LoadString(data, result, jp),
        3 => StringConverter3.LoadString(data, result, jp),
        4 => StringConverter4.LoadString(data, result),
        5 => StringConverter5.LoadString(data, result),
        6 => StringConverter6.LoadString(data, result),
        7 => StringConverter7.LoadString(data, result),
        8 => StringConverter8.LoadString(data, result),
        9 => StringConverter8.LoadString(data, result),
        _ => throw new ArgumentOutOfRangeException(nameof(generation)),
    };

    /// <summary>
    /// Gets the bytes for a Generation specific string according to the input parameters.
    /// </summary>
    /// <param name="destBuffer">Span of bytes to write encoded string data</param>
    /// <param name="value">Decoded string.</param>
    /// <param name="maxLength">Maximum length of the input <see cref="value"/></param>
    /// <param name="option">Buffer pre-formatting option</param>
    /// <param name="generation">Generation string format</param>
    /// <param name="jp">Encoding is Japanese</param>
    /// <param name="isBigEndian">Encoding is Big Endian</param>
    /// <param name="language">Language specific conversion (Chinese)</param>
    /// <returns>Count of bytes written to the <see cref="destBuffer"/>.</returns>
    public static int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option,
        int generation, bool jp, bool isBigEndian, int language = 0) => generation switch
    {
        3 when isBigEndian => StringConverter3GC.SetString(destBuffer, value, maxLength, option),
        4 when isBigEndian => StringConverter4GC.SetString(destBuffer, value, maxLength, option),

        1 or 2 => StringConverter12.SetString(destBuffer, value, maxLength, jp, option),
        3 => StringConverter3.SetString(destBuffer, value, maxLength, jp, option),
        4 => StringConverter4.SetString(destBuffer, value, maxLength, option),
        5 => StringConverter5.SetString(destBuffer, value, maxLength, option),
        6 => StringConverter6.SetString(destBuffer, value, maxLength, option),
        7 => StringConverter7.SetString(destBuffer, value, maxLength, language, option),
        8 => StringConverter8.SetString(destBuffer, value, maxLength, option),
        9 => StringConverter8.SetString(destBuffer, value, maxLength, option),
        _ => throw new ArgumentOutOfRangeException(nameof(generation)),
    };

    /// <summary>
    /// Converts full width to single width
    /// </summary>
    /// <param name="chr">Input character to sanitize.</param>
    internal static char SanitizeChar(char chr) => chr switch
    {
        '\uE08F' => '♀',
        '\uE08E' => '♂',
        '\u246E' => '♀',
        '\u246D' => '♂',
        _ => chr,
    };

    /// <summary>
    /// Converts full width to half width when appropriate
    /// </summary>
    /// <param name="chr">Input character to set back to data</param>
    /// <param name="fullWidth">Checks if the overall string is full-width</param>
    internal static char UnSanitizeChar(char chr, bool fullWidth = false)
    {
        if (fullWidth) // jp/ko/zh strings
            return chr; // keep as full width

        return chr switch
        {
            '\u2640' => '\uE08F',
            '\u2642' => '\uE08E',
            _ => chr,
        };
    }

    /// <summary>
    /// Converts full width to half width when appropriate, for Gen5 and prior.
    /// </summary>
    /// <param name="chr">Input character to set back to data</param>
    internal static char UnSanitizeChar5(char chr) => chr switch
    {
        '\u2640' => '\u246E',
        '\u2642' => '\u246D',
        _ => chr,
    };

    internal static bool GetIsFullWidthString(ReadOnlySpan<char> str)
    {
        foreach (var c in str)
        {
            if (c >> 12 is (0 or 0xE))
                continue;
            if (c is '\u2640' or '\u2642') // ♀♂
                continue;
            return true;
        }
        return false;
    }

    public static bool HasEastAsianScriptCharacters(ReadOnlySpan<char> str)
    {
        foreach (var c in str)
        {
            if (c is >= '\u4E00' and <= '\u9FFF')
                return true;
        }
        return false;
    }
}
