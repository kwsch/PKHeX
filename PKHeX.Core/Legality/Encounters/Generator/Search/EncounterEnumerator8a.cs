using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core;

public record struct EncounterEnumerator8a(PKM Entity, EvoCriteria[] Chain) : IEnumerator<MatchedEncounter<IEncounterable>>
{
    private IEncounterable? Deferred;
    private int Index;
    private int SubIndex;
    private EncounterMatchRating Rating = EncounterMatchRating.MaxNotMatch;
    private bool Yielded;
    public MatchedEncounter<IEncounterable> Current { get; private set; }
    private YieldState State;
    private int met;
    private bool hasOriginalMet;
    readonly object IEnumerator.Current => Current;

    public readonly void Reset() => throw new NotSupportedException();
    public readonly void Dispose() { }
    public readonly IEnumerator<MatchedEncounter<IEncounterable>> GetEnumerator() => this;

    private enum YieldState : byte
    {
        Start,
        Event,
        EventLocal,
        Static,
        Slot,
        Fallback,
        End,
    }

    public bool MoveNext()
    {
        switch (State)
        {
            case YieldState.Start:
                if (Chain.Length == 0)
                    break;
                if (Entity is PK8 { SWSH: false })
                    break;
                if (Entity.IsEgg)
                    break;

                if (!Entity.FatefulEncounter)
                { State = YieldState.Static; goto case YieldState.Static; }
                State = YieldState.Event; goto case YieldState.Event;

            case YieldState.Event:
                if (TryGetNext(EncounterEvent.MGDB_G8A, out var g))
                    return SetCurrent(g);
                Index = 0; State = YieldState.EventLocal; goto case YieldState.EventLocal;
            case YieldState.EventLocal:
                if (TryGetNext(EncounterEvent.EGDB_G8A, out var local))
                    return SetCurrent(local);
                if (Yielded)
                    break;
                Index = 0; State = YieldState.Static; goto case YieldState.Static;

            case YieldState.Static:
                if (TryGetNextSub(Encounters8a.StaticLA, out var s))
                    return SetCurrent(s);

                // Static Encounters can collide with wild encounters (close match); don't break if a Static Encounter is yielded.
                if (!EncounterStateUtil.CanBeWildEncounter(Entity))
                    goto case YieldState.Fallback;

                InitializeWildLocationInfo();
                Index = 0; State = YieldState.Slot; goto case YieldState.Slot;

            case YieldState.Slot:
                if (TryGetNext<EncounterArea8a, EncounterSlot8a>(Encounters8a.SlotsLA, out var w))
                    return SetCurrent(w);
                goto case YieldState.Fallback;

            case YieldState.Fallback:
                State = YieldState.End;
                if (Deferred != null)
                    return SetCurrent(new MatchedEncounter<IEncounterable>(Deferred, Rating));
                break;
        }
        return false;
    }

    private void InitializeWildLocationInfo()
    {
        met = Entity.Met_Location;
        var remap = LocationsHOME.GetRemapState(EntityContext.Gen8a, Entity.Context);
        hasOriginalMet = true;
        if (remap.HasFlag(LocationRemapState.Remapped))
            hasOriginalMet = met != LocationsHOME.SWLA;
    }

    private bool TryGetNext<TArea, TSlot>(TArea[] areas, out MatchedEncounter<IEncounterable> match)
        where TArea : class, IEncounterArea<TSlot>, IAreaLocation
        where TSlot : class, IEncounterable, IEncounterMatch
    {
        for (; Index < areas.Length; Index++, SubIndex = 0)
        {
            var area = areas[Index];
            if (hasOriginalMet && !area.IsMatchLocation(met))
                continue;

            var slots = area.Slots;
            if (TryGetNextSub(slots, out match))
                return true;
        }
        match = default;
        return false;
    }

    private bool TryGetNextSub<T>(T[] slots, out MatchedEncounter<IEncounterable> match)
        where T : class, IEncounterable, IEncounterMatch
    {
        while (SubIndex < slots.Length)
        {
            var enc = slots[SubIndex++];
            foreach (var evo in Chain)
            {
                if (enc.Species != evo.Species)
                    continue;
                if (!enc.IsMatchExact(Entity, evo))
                    break;

                var rating = enc.GetMatchRating(Entity);
                if (rating == EncounterMatchRating.Match)
                {
                    match = new MatchedEncounter<IEncounterable>(enc, rating);
                    return true;
                }

                if (rating < Rating)
                {
                    Deferred = enc;
                    Rating = rating;
                }
                break;
            }
        }
        match = default;
        return false;
    }

    private bool TryGetNext<T>(T[] db, out MatchedEncounter<IEncounterable> match) where T : class, IEncounterable, IEncounterMatch
    {
        for (; Index < db.Length;)
        {
            var enc = db[Index++];
            foreach (var evo in Chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                if (!enc.IsMatchExact(Entity, evo))
                    break;
                var rating = enc.GetMatchRating(Entity);
                if (rating == EncounterMatchRating.Match)
                {
                    match = new MatchedEncounter<IEncounterable>(enc, rating);
                    return true;
                }
                if (rating < Rating)
                {
                    Deferred = enc;
                    Rating = rating;
                }
                break;
            }
        }
        match = default;
        return false;
    }

    private bool SetCurrent(in MatchedEncounter<IEncounterable> match)
    {
        Current = match;
        Yielded = true;
        return true;
    }
}
