using System.Collections.Generic;

namespace PKHeX.Core;

public readonly struct LeadEncounterQueue<TEnc>
{
    public readonly List<LeadEncounter<TEnc>> List = new(0);
    public LeadEncounterQueue() { }

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

public readonly record struct LeadEncounter<T>(LeadSeed Lead, T Encounter)
{
    public bool IsBetterThan(LeadEncounter<T> other) => Lead.IsBetterThan(other.Lead);
}
