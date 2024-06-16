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
    /// <param name="language">Language specific conversion</param>
    /// <returns>Decoded string.</returns>
    public static string GetString(ReadOnlySpan<byte> data, byte generation, bool jp, bool isBigEndian = false, int language = 0) => generation switch
    {
        3 when isBigEndian => StringConverter3GC.GetString(data),
        4 when isBigEndian => StringConverter4GC.GetString(data),

        1 => StringConverter1.GetString(data, jp),
        2 => StringConverter2.GetString(data, language),
        3 => StringConverter3.GetString(data, language),
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
    /// <param name="language">Language specific conversion</param>
    /// <returns>Decoded string.</returns>
    public static int LoadString(ReadOnlySpan<byte> data, Span<char> result, byte generation, bool jp, bool isBigEndian = false, int language = 0) => generation switch
    {
        3 when isBigEndian => StringConverter3GC.LoadString(data, result),
        4 when isBigEndian => StringConverter4GC.LoadString(data, result),

        1 => StringConverter1.LoadString(data, result, jp),
        2 => StringConverter2.LoadString(data, result, language),
        3 => StringConverter3.LoadString(data, result, language),
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
    /// <param name="language">Language specific conversion</param>
    /// <returns>Count of bytes written to the <see cref="destBuffer"/>.</returns>
    public static int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option,
        byte generation, bool jp, bool isBigEndian, int language = 0) => generation switch
    {
        3 when isBigEndian => StringConverter3GC.SetString(destBuffer, value, maxLength, option),
        4 when isBigEndian => StringConverter4GC.SetString(destBuffer, value, maxLength, language, option),

        1 => StringConverter1.SetString(destBuffer, value, maxLength, jp, option),
        2 => StringConverter2.SetString(destBuffer, value, maxLength, language, option),
        3 => StringConverter3.SetString(destBuffer, value, maxLength, language, option),
        4 => StringConverter4.SetString(destBuffer, value, maxLength, language, option),
        5 => StringConverter5.SetString(destBuffer, value, maxLength, language, option),
        6 => StringConverter6.SetString(destBuffer, value, maxLength, language, option),
        7 => StringConverter7.SetString(destBuffer, value, maxLength, language, option),
        8 => StringConverter8.SetString(destBuffer, value, maxLength, option),
        9 => StringConverter8.SetString(destBuffer, value, maxLength, option),
        _ => throw new ArgumentOutOfRangeException(nameof(generation)),
    };

    /// <summary>
    /// Full-width gender 16-bit char representation.
    /// </summary>
    public const char FGF = '\u2640'; // '♀'
    /// <inheritdoc cref="FGM"/>
    public const char FGM = '\u2642'; // '♂'

    /// <summary>
    /// Half-width gender 16-bit char representation.
    /// </summary>
    /// <remarks>
    /// Exact value is the value when converted to Generation 6 &amp; 7 encoding.
    /// Once transferred to the Nintendo Switch era, the value is converted to full-width.
    /// </remarks>
    public const char HGM = '\uE08E'; // '♂'
    /// <inheritdoc cref="HGM"/>
    public const char HGF = '\uE08F'; // '♀'

    /// <summary>
    /// Converts full width to single width
    /// </summary>
    /// <param name="chr">Input character to sanitize.</param>
    public static char NormalizeGenderSymbol(char chr) => chr switch
    {
        HGM => FGM, // '♂'
        HGF => FGF, // '♀'
        _ => chr,
    };

    /// <summary>
    /// Converts full width to half width when appropriate
    /// </summary>
    /// <param name="chr">Input character to set back to data</param>
    /// <param name="fullWidth">Checks if the overall string is full-width</param>
    public static char UnNormalizeGenderSymbol(char chr, bool fullWidth = false) => fullWidth ? chr : chr switch
    {
        FGM => HGM, // '♂'
        FGF => HGF, // '♀'
        _ => chr,
    };

    /// <summary>
    /// Determines if a string contains full-width characters.
    /// </summary>
    /// <param name="str">The input string to check for full-width characters.</param>
    /// <returns>True if the input string contains full-width characters; otherwise, false.</returns>
    internal static bool GetIsFullWidthString(ReadOnlySpan<char> str)
    {
        foreach (var c in str)
        {
            if (c >> 12 is (0 or 0xE))
                continue;
            if (c is FGF or FGM) // ♀♂
                continue;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Determines if a string contains East Asian script characters.
    /// </summary>
    /// <param name="str">The input string to check for East Asian script characters.</param>
    /// <returns>True if the input string contains East Asian script characters; otherwise, false.</returns>
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
