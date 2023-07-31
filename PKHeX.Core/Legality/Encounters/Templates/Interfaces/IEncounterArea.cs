using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Contains a collection of <see cref="Slots"/> for the area.
/// </summary>
/// <typeparam name="T">Encounter Slot type.</typeparam>
public interface IEncounterArea<out T> where T : IEncounterTemplate, IVersion
{
    /// <summary>
    /// Slots in the area.
    /// </summary>
    T[] Slots { get; }

    /// <summary>
    /// Get matching slots for the given <see cref="PKM"/> and <see cref="EvoCriteria"/> chain.
    /// </summary>
    /// <param name="pk">Entity to check.</param>
    /// <param name="chain">Evolutions to check.</param>
    /// <returns>All possible slots that match the given criteria.</returns>
    IEnumerable<T> GetMatchingSlots(PKM pk, EvoCriteria[] chain);
}
