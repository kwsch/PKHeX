namespace PKHeX.Core;

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
    byte StaticIndex { get; }
    /// <summary> <see cref="INumberedSlot.SlotNumber"/> if the lead is <see cref="Ability.MagnetPull"/> </summary>
    byte MagnetPullIndex { get; }

    /// <summary> Count of slots in the parent area that can be yielded by <see cref="Ability.Static"/> </summary>
    byte StaticCount { get; }
    /// <summary> Count of slots in the parent area that can be yielded by <see cref="Ability.MagnetPull"/> </summary>
    byte MagnetPullCount { get; }

    bool IsStaticSlot => StaticCount != 0 && StaticIndex != byte.MaxValue;
    bool IsMagnetSlot => MagnetPullCount != 0 && MagnetPullIndex != byte.MaxValue;

    bool IsMatchStatic(int index, int count) => index == StaticIndex && count == StaticCount;
    bool IsMatchMagnet(int index, int count) => index == MagnetPullIndex && count == MagnetPullCount;
}
