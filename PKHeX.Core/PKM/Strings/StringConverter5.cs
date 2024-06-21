using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Logic for converting a <see cref="string"/> for Generation 5 games.
/// </summary>
public static class StringConverter5
{
    public const char Terminator = (char)0xFFFF;

    /// <summary>Converts Generation 5 encoded data to decoded string.</summary>
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
            var value = ReadUInt16LittleEndian(data[i..]);
            if (value == Terminator)
                break;
            result[ctr++] = StringConverter4Util.NormalizeGenderSymbol((char)value);
        }
        return ctr;
    }

    /// <summary>Gets the bytes for a Generation 5 string.</summary>
    /// <param name="destBuffer">Span of bytes to write encoded string data</param>
    /// <param name="value">Decoded string.</param>
    /// <param name="maxLength">Maximum length of the input <see cref="value"/></param>
    /// <param name="language">Language specific conversion</param>
    /// <param name="option">Buffer pre-formatting option</param>
    /// <returns>Encoded data.</returns>
    public static int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, int language,
        StringConverterOption option = StringConverterOption.ClearZero)
    {
        if (value.Length > maxLength)
            value = value[..maxLength]; // Hard cap

        if (option is StringConverterOption.ClearZero)
            destBuffer.Clear();
        else if (option is StringConverterOption.ClearFF)
            destBuffer.Fill(0xFF);

        bool isHalfWidth = language == (int)LanguageID.Korean || !StringConverter.GetIsFullWidthString(value);
        for (int i = 0; i < value.Length; i++)
        {
            var chr = value[i];
            if (isHalfWidth)
                chr = StringConverter4Util.UnNormalizeGenderSymbol(chr);
            WriteUInt16LittleEndian(destBuffer[(i * 2)..], chr);
        }

        int count = value.Length * 2;
        if (count == destBuffer.Length)
            return count;
        WriteUInt16LittleEndian(destBuffer[count..], Terminator);
        return count + 2;
    }
}
