using System;
using System.Runtime.CompilerServices;

namespace PKHeX.Core;

/// <summary>
/// 4-bit decimal encoding used by some Generation 1 save file values.
/// </summary>
public static class BinaryCodedDecimal
{
    /// <summary>
    /// Returns a 32-bit signed integer converted from bytes in a Binary Coded Decimal format byte array.
    /// </summary>
    /// <param name="input">Input byte array to read from.</param>
    public static int ToInt32BE(ReadOnlySpan<byte> input)
    {
        int result = 0;
        foreach (var b in input)
            PushDigits(ref result, b);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void PushDigits(ref int result, byte b) => result = (result * 100) + (10 * (b >> 4)) + (b & 0xf);

    /// <summary>
    /// Writes the <see cref="value"/> to the <see cref="data"/> buffer.
    /// </summary>
    public static void WriteBytesBE(Span<byte> data, int value)
    {
        for (int i = data.Length - 1; i >= 0; i--, value /= 100)
            data[i] = (byte)((((value / 10) % 10) << 4) | (value % 10));
    }

    /// <inheritdoc cref="ToInt32BE(ReadOnlySpan{byte})"/>
    public static int ToInt32LE(ReadOnlySpan<byte> input)
    {
        int result = 0;
        for (int i = input.Length - 1; i >= 0; i--)
            PushDigits(ref result, input[i]);
        return result;
    }

    /// <summary>
    /// Writes the <see cref="value"/> to the <see cref="data"/> buffer.
    /// </summary>
    public static void WriteBytesLE(Span<byte> data, int value)
    {
        for (int i = 0; i < data.Length; i++, value /= 100)
            data[i] = (byte)((((value / 10) % 10) << 4) | (value % 10));
    }
}
