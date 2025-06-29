using System;

namespace PKHeX.Core;

/// <summary>
/// Interface for converting between byte arrays and strings.
/// </summary>
/// <remarks>
/// Different implementations are used for different string encodings.
/// These vary across <see cref="EntityContext"/>.
/// </remarks>
public interface IStringConverter
{
    /// <summary>
    /// Reads a string from a byte array.
    /// </summary>
    /// <param name="data">Source data to decode.</param>
    string GetString(ReadOnlySpan<byte> data);

    /// <summary>
    /// Loads a string character by character into the <see cref="text"/> array until a terminator is found or the end of the buffer is reached.
    /// </summary>
    /// <param name="data">Source data to decode.</param>
    /// <param name="text">Resulting string buffer.</param>
    /// <returns>Count of characters written to <see cref="text"/>.</returns>
    int LoadString(ReadOnlySpan<byte> data, Span<char> text);

    /// <summary>
    /// Converts a string to a byte array.
    /// </summary>
    /// <param name="data">Destination data to store the encoded string.</param>
    /// <param name="text">Source string to encode.</param>
    /// <param name="length">Maximum length of the <see cref="text"/> to encode.</param>
    /// <param name="option">Buffer conditioning option.</param>
    /// <returns>Count of bytes written to <see cref="data"/>.</returns>
    int SetString(Span<byte> data, ReadOnlySpan<char> text, int length, StringConverterOption option);
}
