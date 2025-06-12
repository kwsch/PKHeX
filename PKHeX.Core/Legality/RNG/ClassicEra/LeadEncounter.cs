using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Simple priority queue for storing encounters based on their computed "best" or "most likely" lead party member.
/// </summary>
/// <typeparam name="TEnc">The type of the encounter associated with each lead.</typeparam>
public readonly struct LeadEncounterQueue<TEnc>
{
    /// <summary>
    /// Iterable list of encounters, ordered by <see cref="LeadSeed.Lead"/>.
    /// </summary>
    public readonly List<LeadEncounter<TEnc>> List = new(0);

    public LeadEncounterQueue() { }

    /// <summary>
    /// Inserts a new item into the collection using the specified lead and encounter.
    /// </summary>
    /// <param name="lead">Lead conditions.</param>
    /// <param name="encounter">Encounter obtained with the lead.</param>
    public void Insert(LeadSeed lead, TEnc encounter) => Insert(new(lead, encounter));

    public void Insert(LeadEncounter<TEnc> toInsert)
    {
        for (int i = 0; i < List.Count; i++)
        {
            if (!toInsert.IsBetterThan(List[i]))
                continue;
            List.Insert(i, toInsert);
            return;
        }
        List.Add(toInsert);
    }
}

/// <summary>
/// Wrapper for an encounter and its associated lead information.
/// </summary>
/// <param name="Lead">Lead party member required to trigger the encounter.</param>
/// <param name="Encounter">Encounter obtained.</param>
public readonly record struct LeadEncounter<T>(LeadSeed Lead, T Encounter)
{
    /// <summary>
    /// Better, i.e., more likely, lead condition required for the encounter.
    /// </summary>
    /// <param name="other">Other lead+encounter to compare against.</param>
    /// <returns><see langword="true"/> if it is more likely/preferable than the other.</returns>
    public bool IsBetterThan(LeadEncounter<T> other) => Lead.IsBetterThan(other.Lead);
}
