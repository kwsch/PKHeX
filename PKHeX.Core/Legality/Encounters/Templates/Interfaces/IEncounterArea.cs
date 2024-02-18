namespace PKHeX.Core;

/// <summary>
/// Contains a collection of <see cref="Slots"/> for the area.
/// </summary>
/// <typeparam name="T">Encounter Slot type.</typeparam>
public interface IEncounterArea<out T> where T : IEncounterTemplate
{
    /// <summary>
    /// Slots in the area.
    /// </summary>
    T[] Slots { get; }
}

/// <summary>
/// Area contains matching location logic.
/// </summary>
public interface IAreaLocation
{
    bool IsMatchLocation(ushort location);
}
