namespace PKHeX.Core;

/// <summary>
/// Contains information about the slot types the object represents.
/// </summary>
public interface ISlotRNGType
{
    /// <summary>
    /// Encounter Slot Type
    /// </summary>
    SlotType Type { get; }
}
