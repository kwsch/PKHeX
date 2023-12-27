using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

/// <summary>
/// Interface for Accessing named blocks within a save file.
/// </summary>
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public interface ISaveBlockAccessor<out T>
{
    /// <summary>
    /// List of all known block details.
    /// </summary>
    IReadOnlyList<T> BlockInfo { get; }
}
