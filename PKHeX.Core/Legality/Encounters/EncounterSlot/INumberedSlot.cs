namespace PKHeX.Core
{
    /// <summary>
    /// <seealso cref="EncounterSlot"/> that contains information about what Index it is within the <seealso cref="EncounterArea"/>'s type-group of slots.
    /// </summary>
    /// <remarks>
    /// Useful for checking legality (if the RNG can yield this slot).
    /// </remarks>
    public interface INumberedSlot
    {
        /// <summary>
        /// Number Index of the <seealso cref="EncounterSlot"/>.
        /// </summary>
        int SlotNumber { get; }
    }
}
