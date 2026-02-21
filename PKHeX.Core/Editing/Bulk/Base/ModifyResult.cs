using System;

namespace PKHeX.Core;

/// <summary>
/// Batch Editor Modification result for an individual processing operation.
/// </summary>
[Flags]
public enum ModifyResult
{
    /// <summary>
    /// No modifications were performed as a filter excluded it.
    /// </summary>
    Filtered,

    /// <summary>
    /// Not a suitable candidate for modification.
    /// </summary>
    Skipped,

    /// <summary>
    /// One or more modifications was successfully applied.
    /// </summary>
    Modified,

    /// <summary>
    /// An error was occurred while attempting modifications.
    /// </summary>
    Error = 0x80,
}
