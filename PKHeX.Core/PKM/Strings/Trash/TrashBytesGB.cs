using System;
using static PKHeX.Core.StringConverter12;

namespace PKHeX.Core;

/// <summary>
/// Generation 1 &amp; 2 Encoding Utility
/// </summary>
public static class TrashBytesGB
{
    /// <inheritdoc cref="TrashBytes.GetStringLength"/>
    public static int GetStringLength(ReadOnlySpan<byte> buffer)
    {
        int index = FindTerminatorIndex(buffer);
        return index == -1 ? buffer.Length : index;
    }

    /// <summary>
    /// Returns a 16-bit aligned index of the terminator.
    /// </summary>
    /// <param name="buffer">Backing buffer of the string.</param>
    /// <returns>Index of the terminator, or -1 if not found.</returns>
    public static int FindTerminatorIndex(ReadOnlySpan<byte> buffer)
        => buffer.IndexOfAny(G1TerminatorZero, G1TerminatorCode);
}
