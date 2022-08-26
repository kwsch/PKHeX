using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core;

/// <summary>
/// Represents an Area where <see cref="PKM"/> can be encountered, which contains a Location ID and <see cref="EncounterSlot"/> data.
/// </summary>
public abstract record EncounterArea(GameVersion Version) : IVersion
{
    public int Location { get; protected init; }
    public SlotType Type { get; protected init; } = SlotType.Any;
    protected abstract IReadOnlyList<EncounterSlot> Raw { get; }

    /// <summary>
    /// Gets the slots contained in the area that match the provided data.
    /// </summary>
    /// <param name="pk">Pokémon Data</param>
    /// <param name="chain">Evolution lineage</param>
    /// <returns>Enumerable list of encounters</returns>
    public abstract IEnumerable<EncounterSlot> GetMatchingSlots(PKM pk, EvoCriteria[] chain);

    /// <summary>
    /// Checks if the provided met location ID matches the parameters for the area.
    /// </summary>
    /// <param name="location">Met Location ID</param>
    /// <returns>True if possibly originated from this area, false otherwise.</returns>
    public virtual bool IsMatchLocation(int location) => Location == location;

    public bool HasSpecies(ushort species) => Raw.Any(z => z.Species == species);
    public IEnumerable<EncounterSlot> GetSpecies(EvoCriteria[] chain) => Raw.Where(z => chain.Any(c => z.Species == c.Species));
}
