using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Exposes useful <see cref="SCBlock"/> access information useful for more advanced data requests.
/// </summary>
public interface ISCBlockArray
{
    /// <summary>
    /// Gets the list of all data blocks the implementing object has.
    /// </summary>
    public IReadOnlyList<SCBlock> AllBlocks { get; }

    /// <summary>
    /// Gets the <see cref="SCBlockAccessor"/> for the implementing object, allowing for looking up specific blocks by key.
    /// </summary>
    SCBlockAccessor Accessor { get; }
}
