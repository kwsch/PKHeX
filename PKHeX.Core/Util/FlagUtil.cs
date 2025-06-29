using System;

namespace PKHeX.Core;

/// <summary>
/// Utility logic for dealing with bitflags in a byte array.
/// </summary>
public static class FlagUtil
{
    /// <summary>
    /// Gets the requested <see cref="bitIndex"/> from the byte at <see cref="offset"/>.
    /// </summary>
    /// <param name="arr">Buffer to read</param>
    /// <param name="offset">Offset of the byte</param>
    /// <param name="bitIndex">Bit to read</param>
    public static bool GetFlag(ReadOnlySpan<byte> arr, int offset, int bitIndex)
    {
        bitIndex &= 7; // ensure the bit access is 0-7
        return ((arr[offset] >> bitIndex) & 1) != 0;
    }

    /// <inheritdoc cref="GetFlag(ReadOnlySpan{byte}, int, int)"/>
    public static bool GetFlag(ReadOnlySpan<byte> arr, int bitIndex) => GetFlag(arr, bitIndex >> 3, bitIndex);

    /// <summary>
    /// Sets the requested <see cref="bitIndex"/> value to the byte at <see cref="offset"/>.
    /// </summary>
    /// <param name="arr">Buffer to modify</param>
    /// <param name="offset">Offset of the byte</param>
    /// <param name="bitIndex">Bit to write</param>
    /// <param name="value">Bit flag value to set</param>
    public static void SetFlag(Span<byte> arr, int offset, int bitIndex, bool value)
    {
        bitIndex &= 7; // ensure the bit access is 0-7
        var current = arr[offset] & ~(1 << bitIndex);
        var newValue = current | ((value ? 1 : 0) << bitIndex);
        arr[offset] = (byte)newValue;
    }

    /// <summary>
    /// Sets or clears a specific bit in the provided byte span at the specified index.
    /// </summary>
    /// <remarks>This method modifies the byte span in place.
    /// Ensure that the span has sufficient capacity to accommodate the specified bit index.
    /// </remarks>
    /// <param name="arr">The span of bytes where the bit will be set or cleared.</param>
    /// <param name="index">The zero-based index of the bit to modify. Must be within the bounds of the span.</param>
    /// <param name="value"><see langword="true"/> to set the bit to 1; <see langword="false"/> to clear the bit to 0.</param>
    public static void SetFlag(Span<byte> arr, int index, bool value) => SetFlag(arr, index >> 3, index, value);

    /// <inheritdoc cref="GetBitFlagArray(ReadOnlySpan{byte}, Span{bool})"/>
    /// <param name="data">The byte array containing the bit flags.</param>
    /// <param name="count">The number of bits to read from the byte array.</param>
    public static bool[] GetBitFlagArray(ReadOnlySpan<byte> data, int count)
    {
        var result = new bool[count];
        GetBitFlagArray(data, result);
        return result;
    }

    /// <summary>
    /// Converts a byte array into a boolean array, where each bit in the byte array corresponds to a boolean value in the result.
    /// </summary>
    /// <param name="data">The byte array containing the bit flags.</param>
    /// <param name="result">>The span to write the boolean values into.</param>
    public static void GetBitFlagArray(ReadOnlySpan<byte> data, Span<bool> result)
    {
        for (int i = 0; i < result.Length; i++)
            result[i] = (data[i >> 3] & (1 << (i & 7))) != 0;
    }

    /// <inheritdoc cref="GetBitFlagArray(ReadOnlySpan{byte}, Span{bool})"/>
    public static bool[] GetBitFlagArray(ReadOnlySpan<byte> data) => GetBitFlagArray(data, data.Length << 3);

    /// <summary>
    /// Sets the bit flags in the specified byte array based on the provided boolean values.
    /// </summary>
    /// <remarks>
    /// The method modifies the <paramref name="data"/> span in place.
    /// Each boolean value in <paramref name="value"/> corresponds to a bit in <paramref name="data"/>, with the first boolean value affecting the least significant bit of the first byte.
    /// Ensure that <paramref name="data"/> has enough bytes to store all bits from <paramref name="value"/>; otherwise, an <see cref="IndexOutOfRangeException"/> may occur.
    /// </remarks>
    /// <param name="data">A <see cref="Span{T}"/> of bytes where the bit flags will be set.</param>
    /// <param name="value">
    /// A <see cref="ReadOnlySpan{T}"/> of boolean values representing the bit flags to set.
    /// Each <see langword="true"/> value sets the corresponding bit to 1, and each <see langword="false"/> value sets it to 0.
    /// </param>
    public static void SetBitFlagArray(Span<byte> data, ReadOnlySpan<bool> value)
    {
        for (int i = 0; i < value.Length; i++)
        {
            var ofs = i >> 3;
            var mask = (1 << (i & 7));
            if (value[i])
                data[ofs] |= (byte)mask;
            else
                data[ofs] &= (byte)~mask;
        }
    }
}
