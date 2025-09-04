using System;
using static PKHeX.Core.StringConverter4Util;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Logic for converting a <see cref="string"/> for Generation 4 GameCube games.
/// </summary>
public static class StringConverter4GC
{
    private const ushort Terminator = 0xFFFF;
    private const char TerminatorChar = (char)Terminator;

    /// <summary>
    /// Converts Generation 4 Big Endian encoded character data to string.
    /// </summary>
    /// <param name="data">Byte array containing encoded character data.</param>
    /// <returns>Converted string.</returns>
    public static string GetString(ReadOnlySpan<byte> data)
    {
        Span<char> result = stackalloc char[data.Length];
        var length = LoadString(data, result);
        return new string(result[..length]);
    }

    /// <inheritdoc cref="GetString(ReadOnlySpan{byte})"/>
    /// <param name="data">Encoded data</param>
    /// <param name="result">Decoded character result buffer</param>
    /// <returns>Character count loaded.</returns>
    public static int LoadString(ReadOnlySpan<byte> data, Span<char> result)
    {
        int i = 0;
        int ctr = 0;
        for (; i < data.Length; i += 2)
        {
            var value = ReadUInt16BigEndian(data[i..]);
            if (value == Terminator)
                break;
            char chr = (char)ConvertValue2CharG4(value);
            chr = NormalizeGenderSymbol(chr);
            result[ctr++] = chr;
        }
        return ctr;
    }

    /// <summary>
    /// Converts a string to Generation 4 Big Endian encoded character data.
    /// </summary>
    /// <param name="destBuffer">Span of bytes to write encoded string data</param>
    /// <param name="value">String to be converted.</param>
    /// <param name="maxLength">Maximum length of string</param>
    /// <param name="language">Language specific conversion</param>
    /// <param name="option">Buffer pre-formatting option</param>
    /// <returns>Byte array containing encoded character data</returns>
    public static int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, int language,
        StringConverterOption option = StringConverterOption.ClearZero)
    {
        if (value.Length > maxLength)
            value = value[..maxLength]; // Hard cap

        if (option is StringConverterOption.ClearZero)
            destBuffer.Clear();

        bool isHalfWidth = language == (int)LanguageID.Korean || !StringConverter.GetIsFullWidthString(value);
        for (int i = 0; i < value.Length; i++)
        {
            var chr = value[i];
            if (isHalfWidth)
                chr = UnNormalizeGenderSymbol(chr);
            ushort val = ConvertChar2ValueG4(chr);
            WriteUInt16BigEndian(destBuffer[(i * 2)..], val);
        }

        var count = value.Length * 2;
        if (count == destBuffer.Length)
            return count;
        WriteUInt16BigEndian(destBuffer[count..], Terminator);
        return count + 2;
    }

    #region Ranch
    /// <summary>
    /// Converts Generation 4 Big Endian encoded character data to string, with direct Unicode characters.
    /// </summary>
    /// <remarks>Used by the Save File's internal strings.</remarks>
    /// <param name="data">Byte array containing encoded character data.</param>
    /// <returns>Converted string.</returns>
    public static string GetStringUnicode(ReadOnlySpan<byte> data)
    {
        Span<char> result = stackalloc char[data.Length];
        var length = LoadStringUnicode(data, result);
        return new string(result[..length]);
    }

    /// <inheritdoc cref="GetStringUnicode(System.ReadOnlySpan{byte})"/>
    /// <param name="data">Encoded data</param>
    /// <param name="result">Decoded character result buffer</param>
    /// <returns>Character count loaded.</returns>
    public static int LoadStringUnicode(ReadOnlySpan<byte> data, Span<char> result)
    {
        int i = 0;
        int ctr = 0;
        for (; i < data.Length; i += 2)
        {
            char chr = (char)ReadUInt16BigEndian(data[i..]);
            if (chr == TerminatorChar)
                break;
            result[ctr++] = chr;
        }
        return ctr;
    }

    /// <summary>
    /// Converts a string to Generation 4 Big Endian encoded character data, with direct Unicode characters.
    /// </summary>
    /// <remarks>Used by the Save File's internal strings.</remarks>
    /// <param name="value">String to be converted.</param>
    /// <param name="destBuffer">Span of bytes to write encoded string data</param>
    /// <param name="maxLength">Maximum length of string</param>
    /// <param name="option">Buffer pre-formatting option</param>
    /// <returns>Byte array containing encoded character data</returns>
    public static int SetStringUnicode(ReadOnlySpan<char> value, Span<byte> destBuffer, int maxLength, StringConverterOption option = StringConverterOption.ClearZero)
    {
        if (value.Length > maxLength)
            value = value[..maxLength]; // Hard cap

        if (option is StringConverterOption.ClearZero)
            destBuffer.Clear();

        for (int i = 0; i < value.Length; i++)
        {
            var c = value[i];
            WriteUInt16BigEndian(destBuffer[(i * 2)..], c);
        }

        var count = value.Length * 2;
        if (count == destBuffer.Length)
            return count;
        WriteUInt16BigEndian(destBuffer[count..], 0);
        return count + 2;
    }
    #endregion

    #region PBR
    // Native PBR strings are terminated by \xFFFF\xFFFF.
    // User-input strings are terminated by \x0000.
    // These three variables can be present in some user-input strings.

    private const ushort Null = 0x0000;
    private const char NullChar = (char)Null;

    private const char VariableChar = '\xFFFF';
    private const char ProportionalChar = '\x0013';
    private const char PokemonNameChar = '\x0015';
    private const char LineBreakChar = '\xFFFE';

    public const char Proportional = '￼';
    public const char PokemonName = 'Ⓟ';
    public const char LineBreak = '⏎';

    /// <inheritdoc cref="GetStringUnicode(System.ReadOnlySpan{byte})"/>
    public static string GetStringUnicodeBR(ReadOnlySpan<byte> data)
    {
        Span<char> result = stackalloc char[data.Length];
        var length = LoadStringUnicodeBR(data, result);
        return new string(result[..length]);
    }

    /// <inheritdoc cref="LoadStringUnicode(System.ReadOnlySpan{byte}, System.Span{char})"/>
    public static int LoadStringUnicodeBR(ReadOnlySpan<byte> data, Span<char> result)
    {
        int ctr = 0;
        for (int i = 0; i < data.Length; i += 2)
        {
            char chr = (char)ReadUInt16BigEndian(data[i..]);
            if (chr == NullChar)
                break;
            if (chr == VariableChar)
            {
                i += 2;
                if (i >= data.Length)
                    break;
                char nextChr = (char)ReadUInt16BigEndian(data[i..]);
                if (nextChr == TerminatorChar)
                    break;
                chr = nextChr switch
                {
                    LineBreakChar => LineBreak,
                    ProportionalChar => Proportional,
                    PokemonNameChar => PokemonName,
                    _ => chr
                };
                if (chr == VariableChar)
                {
                    // Unhandled variable, copy directly
                    result[ctr++] = chr;
                    result[ctr++] = nextChr;
                    continue;
                }
            }
            result[ctr++] = chr;
        }
        return ctr;
    }

    /// <inheritdoc cref="SetStringUnicode(ReadOnlySpan{char}, Span{byte}, int, StringConverterOption)"/>
    public static int SetStringUnicodeBR(ReadOnlySpan<char> value, Span<byte> destBuffer, int maxLength = -1, StringConverterOption option = StringConverterOption.ClearZero)
    {
        if (maxLength < 0)
            maxLength = destBuffer.Length - 2;
        if (option is StringConverterOption.ClearZero)
            destBuffer.Clear();

        int count = 0;
        for (int i = 0; i < value.Length && count < maxLength; i++)
        {
            var c = value[i];
            if (c is LineBreak or Proportional or PokemonName)
            {
                if (count + 2 >= maxLength)
                    break;
                WriteUInt16BigEndian(destBuffer[count..], VariableChar);
                count += 2;
                WriteUInt16BigEndian(destBuffer[count..], c switch
                {
                    LineBreak => LineBreakChar,
                    Proportional => ProportionalChar,
                    PokemonName => PokemonNameChar,
                    _ => c,
                });
                count += 2;
            }
            else
            {
                WriteUInt16BigEndian(destBuffer[count..], c);
                count += 2;
            }
        }

        if (count == destBuffer.Length)
            return count;
        WriteUInt16BigEndian(destBuffer[count..], 0);
        return count + 2;
    }
    #endregion
}
