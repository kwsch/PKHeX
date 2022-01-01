using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public static class StringConverter6
{
    private const ushort TerminatorNull = 0;

    /// <summary>Converts Generation 6 encoded data to decoded string.</summary>
    /// <param name="data">Encoded data</param>
    /// <returns>Decoded string.</returns>
    public static string GetString(ReadOnlySpan<byte> data)
    {
        Span<char> result = stackalloc char[data.Length];
        int length = LoadString(data, result);
        return new string(result[..length].ToArray());
    }

    private static int LoadString(ReadOnlySpan<byte> data, Span<char> result)
    {
        int i = 0;
        for (; i < data.Length; i += 2)
        {
            var value = ReadUInt16LittleEndian(data[i..]);
            if (value == TerminatorNull)
                break;
            result[i/2] = StringConverter.SanitizeChar((char)value);
        }
        return i/2;
    }

    /// <summary>Gets the bytes for a Generation 6 string.</summary>
    /// <param name="destBuffer"></param>
    /// <param name="value">Decoded string.</param>
    /// <param name="maxLength">Maximum length of the input <see cref="value"/></param>
    /// <param name="option"></param>
    /// <returns>Encoded data.</returns>
    public static int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength,
        StringConverterOption option = StringConverterOption.ClearZero)
    {
        if (value.Length > maxLength)
            value = value[..maxLength]; // Hard cap

        if (option is StringConverterOption.ClearZero)
            destBuffer.Clear();

        bool isFullWidth = StringConverter.GetIsFullWidthString(value);
        for (int i = 0; i < value.Length; i++)
        {
            char c = value[i];
            if (!isFullWidth)
                c = StringConverter.UnSanitizeChar(c);
            WriteUInt16LittleEndian(destBuffer[(i * 2)..], c);
        }

        int count = value.Length * 2;
        if (count == destBuffer.Length)
            return count;
        WriteUInt16LittleEndian(destBuffer[count..], TerminatorNull);
        return count + 2;
    }
}
