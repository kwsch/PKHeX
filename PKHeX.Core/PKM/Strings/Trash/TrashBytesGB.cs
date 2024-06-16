using System;
using static PKHeX.Core.StringConverter1;

namespace PKHeX.Core;

/// <summary>
/// Generation 1 &amp; 2 Encoding Utility
/// </summary>
public static class TrashBytesGB
{
    /// <inheritdoc cref="TrashBytesUTF16.GetStringLength"/>
    public static int GetStringLength(ReadOnlySpan<byte> buffer)
    {
        int index = GetTerminatorIndex(buffer);
        return index == -1 ? buffer.Length : index;
    }

    /// <summary>
    /// Returns a 8-bit aligned index of the terminator.
    /// </summary>
    /// <param name="buffer">Backing buffer of the string.</param>
    /// <returns>Index of the terminator, or -1 if not found.</returns>
    public static int GetTerminatorIndex(ReadOnlySpan<byte> buffer)
        => buffer.IndexOfAny(TerminatorZero, TerminatorCode);
}
