using System;
using System.Runtime.CompilerServices;

namespace PKHeX.Core;

/// <summary>
/// 4-bit decimal encoding used by some Generation 1 save file values.
/// </summary>
public static class BinaryCodedDecimal
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void PushDigits(ref uint result, uint b)
        => result = checked((result * 100) + (10 * (b >> 4)) + (b & 0xf));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte GetLowestTuple(uint value)
        => (byte)((((value / 10) % 10) << 4) | (value % 10));

    /// <summary>
    /// Returns a 32-bit signed integer converted from bytes in a Binary Coded Decimal format byte array.
    /// </summary>
    /// <param name="input">Input byte array to read from.</param>
    public static uint ReadUInt32BigEndian(ReadOnlySpan<byte> input)
    {
        uint result = 0;
        foreach (var b in input)
            PushDigits(ref result, b);
        return result;
    }

    /// <summary>
    /// Writes the <see cref="value"/> to the <see cref="data"/> buffer.
    /// </summary>
    public static void WriteUInt32BigEndian(Span<byte> data, uint value)
    {
        for (int i = data.Length - 1; i >= 0; i--, value /= 100)
            data[i] = GetLowestTuple(value);
    }

    /// <inheritdoc cref="ReadUInt32BigEndian"/>
    public static uint ReadUInt32LittleEndian(ReadOnlySpan<byte> input)
    {
        uint result = 0;
        for (int i = input.Length - 1; i >= 0; i--)
            PushDigits(ref result, input[i]);
        return result;
    }

    /// <summary>
    /// Writes the <see cref="value"/> to the <see cref="data"/> buffer.
    /// </summary>
    public static void WriteUInt32LittleEndian(Span<byte> data, uint value)
    {
        for (int i = 0; i < data.Length; i++, value /= 100)
            data[i] = GetLowestTuple(value);
    }
}
