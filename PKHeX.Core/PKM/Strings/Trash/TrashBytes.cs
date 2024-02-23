using System;
using System.Runtime.InteropServices;

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
    /// <returns>Index of the terminator, or max length if not found.</returns>
    public static int GetStringLength(ReadOnlySpan<byte> buffer, ushort terminator = 0)
    {
        int index = FindTerminatorIndex(buffer, terminator);
        return index == -1 ? buffer.Length / 2 : index;
    }

    /// <summary>
    /// Returns a 16-bit aligned index of the terminator.
    /// </summary>
    /// <param name="buffer">Backing buffer of the string.</param>
    /// <param name="terminator">Terminator character to search for.</param>
    /// <returns>Index of the terminator, or -1 if not found.</returns>
    /// <remarks>When used on a raw string, returns the computed length of the string, assuming a terminator is present.</remarks>
    public static int FindTerminatorIndex(ReadOnlySpan<byte> buffer, ushort terminator = 0)
    {
        var u16 = MemoryMarshal.Cast<byte, ushort>(buffer);
        return u16.IndexOf(terminator);
    }
}
