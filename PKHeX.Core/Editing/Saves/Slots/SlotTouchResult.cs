namespace PKHeX.Core
{
    /// <summary>
    /// Result indicators for modifying a Slot within a <see cref="SaveFile"/> or other data location.
    /// </summary>
    public enum SlotTouchResult
    {
        /// <summary>
        /// Slot interaction was successful.
        /// </summary>
        Success,

        /// <summary>
        /// Slot interaction failed to do anything.
        /// </summary>
        FailNone,

        /// <summary>
        /// Slot interaction failed to apply the data.
        /// </summary>
        FailWrite,

        /// <summary>
        /// Slot interaction failed to delete the data.
        /// </summary>
        FailDelete,

        /// <summary>
        /// Slot interaction failed due to a bad/unmodifiable source.
        /// </summary>
        FailSource,

        /// <summary>
        /// Slot interaction failed due to a bad/unmodifiable destination.
        /// </summary>
        FailDestination,
    }
}