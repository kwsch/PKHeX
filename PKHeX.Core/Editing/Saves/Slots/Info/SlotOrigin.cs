namespace PKHeX.Core;

/// <summary>
/// Indicates where the slot data originated from.
/// </summary>
public enum SlotOrigin : byte
{
    /// <summary>
    /// Slot data originated from the Party, or follows "party format" data rules.
    /// </summary>
    /// <remarks>Some games do not permit forms to exist outside of the party.</remarks>
    Party = 0,

    /// <summary>
    /// Slot data originated from the Box, or follows "stored" data rules.
    /// </summary>
    Box = 1,
}
