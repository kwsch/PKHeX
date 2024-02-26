namespace PKHeX.Core;

/// <summary>
/// Contains information about the RNG restrictions for the slot, and any mutated RNG values.
/// </summary>
/// <remarks>
/// When encountering a Wild Pokémon with a lead that has an ability of <see cref="Ability.Static"/> or <see cref="Ability.MagnetPull"/>,
/// the game picks encounters from the <seealso cref="IEncounterArea{T}"/> that match the type.
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

    /// <summary>
    /// Indicates if the slot can be yielded by <see cref="Ability.Static"/>.
    /// </summary>
    bool IsStaticSlot => StaticCount != 0;

    /// <summary>
    /// Indicates if the slot can be yielded by <see cref="Ability.MagnetPull"/>.
    /// </summary>
    bool IsMagnetSlot => MagnetPullCount != 0;
}

public static class MagnetStaticExtensions
{
    /// <summary>
    /// Checks if the <see cref="enc"/> can be an encounter slot chosen via Static or Magnet Pull.
    /// </summary>
    /// <param name="enc">Encounter Slot</param>
    /// <param name="u16SlotRand">[0,65535]</param>
    /// <param name="lead">Which one it matched, if any.</param>
    /// <returns>False if not a matching slot.</returns>
    public static bool IsSlotValidStaticMagnet<T>(this T enc, uint u16SlotRand, out LeadRequired lead) where T : IMagnetStatic
    {
        if (enc.IsStaticSlot && u16SlotRand % enc.StaticCount == enc.StaticIndex)
        {
            lead = LeadRequired.Static;
            return true;
        }
        // Isn't checked for Fishing slots, but no fishing slots are steel type -- always false.
        if (enc.IsMagnetSlot && u16SlotRand % enc.MagnetPullCount == enc.MagnetPullIndex)
        {
            lead = LeadRequired.MagnetPull;
            return true;
        }

        lead = LeadRequired.None;
        return false;
    }
}
