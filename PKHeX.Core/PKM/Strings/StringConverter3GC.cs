using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Logic for converting a <see cref="string"/> for Generation 3 GameCube games.
/// </summary>
public static class StringConverter3GC
{
    private const char TerminatorBigEndian = (char)0; // GC

    /// <summary>Converts Big Endian encoded data to decoded string.</summary>
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
        for (; i < data.Length; i += 2)
        {
            var value = (char)ReadUInt16BigEndian(data[i..]);
            if (value == TerminatorBigEndian)
                break;
            result[i/2] = value;
        }
        return i/2;
    }

    /// <summary>Gets the bytes for a Big Endian string.</summary>
    /// <param name="destBuffer">Span of bytes to write encoded string data</param>
    /// <param name="value">Decoded string.</param>
    /// <param name="maxLength">Maximum length of the input <see cref="value"/></param>
    /// <param name="option">Option to clear the buffer. Only <see cref="StringConverterOption.ClearZero"/> is recognized.</param>
    /// <returns>Encoded data.</returns>
    public static int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength,
        StringConverterOption option)
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

        if (destBuffer.Length == value.Length * 2)
            return value.Length * 2;
        WriteUInt16BigEndian(destBuffer[(value.Length * 2)..], TerminatorBigEndian);
        return (value.Length * 2) + 2;
    }
}
