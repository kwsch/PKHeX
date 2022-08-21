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

public static class SlotTouchTypeUtil
{
    public static bool IsContentChange(this SlotTouchType t) => t > SlotTouchType.Get;
}
