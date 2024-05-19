using System;
using System.Runtime.InteropServices;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// 16-bit encoded string utility
/// </summary>
public static class TrashBytes
{
    /// <summary>
    /// Gets the length of the string based on the terminator.
    /// </summary>
    /// <param name="buffer">Buffer to check the length of.</param>
    /// <param name="terminator">String terminator to search for.</param>
    /// <returns>Decoded index (char) of the terminator, or max length if not found.</returns>
    public static int GetStringLength(ReadOnlySpan<byte> buffer, ushort terminator = 0)
    {
        int index = GetTerminatorIndex(buffer, terminator);
        return index == -1 ? buffer.Length / 2 : index;
    }

    /// <summary>
    /// Returns a 16-bit aligned index of the terminator.
    /// </summary>
    /// <param name="buffer">Backing buffer of the string.</param>
    /// <param name="terminator">Terminator character to search for.</param>
    /// <returns>Decoded index (char) of the terminator, or -1 if not found.</returns>
    /// <remarks>When used on a raw string, returns the computed length of the string, assuming a terminator is present.</remarks>
    public static int GetTerminatorIndex(ReadOnlySpan<byte> buffer, ushort terminator = 0)
    {
        var u16 = MemoryMarshal.Cast<byte, ushort>(buffer);
        return u16.IndexOf(terminator);
    }

    public static TrashMatch IsUnderlayerPresent(ReadOnlySpan<char> under, ReadOnlySpan<byte> data, int charsUsed)
    {
        var input = MemoryMarshal.Cast<byte, char>(data);
        return IsUnderlayerPresent(under, input, charsUsed);
    }

    public static TrashMatch IsUnderlayerPresent(ReadOnlySpan<char> under, ReadOnlySpan<char> input, int charsUsed)
    {
        if (charsUsed >= under.Length)
            return TrashMatch.TooLongToTell;

        for (int i = charsUsed; i < under.Length; i++)
        {
            var c = input[i];
            if (!BitConverter.IsLittleEndian)
                c = (char)ReverseEndianness(c);
            if (c == under[i])
                continue;
            return TrashMatch.NotPresent;
        }
        return TrashMatch.Present;
    }
}
