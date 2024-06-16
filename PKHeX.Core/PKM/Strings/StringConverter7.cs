using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Logic for converting a <see cref="string"/> for Generation 7 games.
/// </summary>
/// <remarks>Customized Unicode glyph remapping for visual display. Chinese Species names use a special range of characters -- <see cref="StringConverter7ZH"/>.</remarks>
public static class StringConverter7
{
    private const ushort TerminatorNull = 0;

    /// <summary>Converts Generation 7 encoded data to decoded string.</summary>
    /// <param name="data">Encoded data</param>
    /// <returns>Decoded string.</returns>
    public static string GetString(ReadOnlySpan<byte> data)
    {
        Span<char> result = stackalloc char[data.Length];
        int length = LoadString(data, result);
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
            var value = (char)ReadUInt16LittleEndian(data[i..]);
            if (value == TerminatorNull)
                break;

            result[ctr++] = StringConverter7ZH.IsPrivateChar(value)
                ? StringConverter7ZH.GetUnicodeChar(value)
                : StringConverter.NormalizeGenderSymbol(value);
        }
        return ctr;
    }

    /// <summary>Gets the bytes for a Generation 7 string.</summary>
    /// <param name="destBuffer">Span of bytes to write encoded string data</param>
    /// <param name="value">Decoded string.</param>
    /// <param name="maxLength">Maximum length of the input <see cref="value"/></param>
    /// <param name="language">Language specific conversion</param>
    /// <param name="option">Buffer pre-formatting option</param>
    /// <param name="chinese">Chinese string remapping should be attempted (only Pokémon names, without Nickname flag set)</param>
    /// <returns>Encoded data.</returns>
    public static int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, int language,
        StringConverterOption option = StringConverterOption.ClearZero, bool chinese = false)
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
                chr = StringConverter.UnNormalizeGenderSymbol(chr);
            if (chinese)
                chr = StringConverter7ZH.GetPrivateChar(chr, language == (int)LanguageID.ChineseT);
            WriteUInt16LittleEndian(destBuffer[(i * 2)..], chr);
        }

        int count = value.Length * 2;
        if (count == destBuffer.Length)
            return count;
        WriteUInt16LittleEndian(destBuffer[count..], TerminatorNull);
        return count + 2;
    }
}
