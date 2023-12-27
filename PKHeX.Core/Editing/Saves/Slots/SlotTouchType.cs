namespace PKHeX.Core;

/// <summary>
/// Interaction types for stored entity slots.
/// </summary>
public enum SlotTouchType
{
    /// <summary> No action performed. </summary>
    None,

    /// <summary> Data read request </summary>
    Get,
    /// <summary> Data write request </summary>
    Set,
    /// <summary> Data delete/clear request </summary>
    Delete,
    /// <summary> Data swap/move request </summary>
    Swap,

    /// <summary> Request to be handled via external logic (atypical) </summary>
    External,
}

/// <summary>
/// Extension methods for <see cref="SlotTouchType"/>.
/// </summary>
public static class SlotTouchTypeUtil
{
    /// <summary>
    /// Indicates if the <see cref="SlotTouchType"/> happens after a write operation and the underlying data has been changed.
    /// </summary>
    public static bool IsContentChange(this SlotTouchType t) => t > SlotTouchType.Get;
}
