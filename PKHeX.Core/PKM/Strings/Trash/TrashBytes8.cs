using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 Encoding Utility
/// </summary>
public static class TrashBytes8
{
    /// <inheritdoc cref="TrashBytesUTF16.GetStringLength"/>
    public static int GetStringLength(ReadOnlySpan<byte> buffer)
    {
        int index = GetTerminatorIndex(buffer);
        return index == -1 ? buffer.Length : index;
    }

    /// <summary>
    /// Returns a 16-bit aligned index of the terminator.
    /// </summary>
    /// <param name="buffer">Backing buffer of the string.</param>
    /// <returns>Index of the terminator, or -1 if not found.</returns>
    public static int GetTerminatorIndex(ReadOnlySpan<byte> buffer)
        => buffer.IndexOf<byte>(0xFF);
}
