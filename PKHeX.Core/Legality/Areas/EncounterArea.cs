namespace PKHeX.Core;

/// <summary>
/// Represents an Area where <see cref="PKM"/> can be encountered, which contains a Location ID and <see cref="EncounterSlot"/> data.
/// </summary>
public abstract record EncounterArea(GameVersion Version) : IVersion
{
    public int Location { get; protected init; }
    public SlotType Type { get; protected init; }

    /// <summary>
    /// Checks if the provided met location ID matches the parameters for the area.
    /// </summary>
    /// <param name="location">Met Location ID</param>
    /// <returns>True if possibly originated from this area, false otherwise.</returns>
    public virtual bool IsMatchLocation(int location) => Location == location;
}

internal interface IMemorySpeciesArea
{
    bool HasSpecies(ushort species);
}
