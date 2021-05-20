namespace PKHeX.Core
{
    /// <summary>
    /// Contains information about the RNG restrictions for the slot, and any mutated RNG values.
    /// </summary>
    /// <remarks>
    /// When encountering a Wild Pokémon with a lead that has an ability of <see cref="Ability.Static"/> or <see cref="Ability.MagnetPull"/>,
    /// the game picks encounters from the <seealso cref="EncounterArea"/> that match the type.
    /// The values in this interface change the <seealso cref="INumberedSlot"/> to allow different values for slot yielding.
    /// </remarks>
    public interface IMagnetStatic
    {
        /// <summary> <see cref="INumberedSlot.SlotNumber"/> if the lead is <see cref="Ability.Static"/> </summary>
        int StaticIndex { get; }
        /// <summary> <see cref="INumberedSlot.SlotNumber"/> if the lead is <see cref="Ability.MagnetPull"/> </summary>
        int MagnetPullIndex { get; }

        /// <summary> Count of slots in the parent area that can be yielded by <see cref="Ability.Static"/> </summary>
        int StaticCount { get; }
        /// <summary> Count of slots in the parent area that can be yielded by <see cref="Ability.MagnetPull"/> </summary>
        int MagnetPullCount { get; }
    }

    public static class MagnetStaticExtensions
    {
        public static bool IsMatchStatic(this IMagnetStatic slot,  int index, int count) => index == slot.StaticIndex && count == slot.StaticCount;
        public static bool IsMatchMagnet(this IMagnetStatic slot,  int index, int count) => index == slot.MagnetPullIndex && count == slot.MagnetPullCount;
    }
}
