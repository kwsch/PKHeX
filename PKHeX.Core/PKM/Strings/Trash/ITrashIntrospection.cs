using System;

namespace PKHeX.Core;

/// <summary>
/// Interface for introspection of trash data.
/// </summary>
/// <remarks>
/// Implementation abstractions for accessing results from
/// <see cref="TrashBytesUTF16"/>,
/// <see cref="TrashBytesGB"/>,
/// <see cref="TrashBytes8"/>
/// depending on the type's string encoding.
/// </remarks>
public interface ITrashIntrospection
{
    /// <summary>
    /// Gets the index of the string terminator in the given data.
    /// </summary>
    /// <param name="data">The data to search.</param>
    /// <returns>Character index of the string terminator.</returns>
    int GetStringTerminatorIndex(ReadOnlySpan<byte> data);

    /// <summary>
    /// Gets the length of the string based on the terminator or end of the data.
    /// </summary>
    /// <param name="data">Span of data to check the length of.</param>
    /// <returns>Count of characters in the string.</returns>
    int GetStringLength(ReadOnlySpan<byte> data);

    /// <summary>
    /// Gets the amount of bytes per character in the string encoding.
    /// </summary>
    int GetBytesPerChar();
}
