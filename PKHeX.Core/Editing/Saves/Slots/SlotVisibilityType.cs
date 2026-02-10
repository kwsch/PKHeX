using System;

namespace PKHeX.Core;

/// <summary>
/// Specifies the visibility options for a slot when it is displayed in the user interface.
/// </summary>
/// <remarks>This enumeration supports bitwise combination of its values to allow multiple visibility behaviors to be applied simultaneously.</remarks>
[Flags]
public enum SlotVisibilityType
{
    /// <summary>
    /// No special visibility handling.
    /// </summary>
    None,

    /// <summary>
    /// Check the legality of the slot when displaying it.
    /// </summary>
    CheckLegalityIndicate = 1 << 0,

    /// <summary>
    /// Fade-out the slot if it does not match the current filter.
    /// </summary>
    FilterMismatch = 1 << 1,
}
