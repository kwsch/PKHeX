namespace PKHeX.Core;

/// <summary>
/// <seealso cref="IEncounterTemplate"/> that contains information about what Index it is within the area's type-group of slots.
/// </summary>
/// <remarks>
/// Useful for checking legality (if the RNG can yield this slot).
/// </remarks>
public interface INumberedSlot
{
    /// <summary>
    /// Number Index of the <seealso cref="IEncounterTemplate"/>.
    /// </summary>
    byte SlotNumber { get; }
}
