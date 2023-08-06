using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core;

public record struct EncounterEnumerator9(PKM Entity, EvoCriteria[] Chain, GameVersion Version) : IEnumerator<MatchedEncounter<IEncounterable>>
{
    private IEncounterable? Deferred;
    private int Index;
    private int SubIndex;
    private EncounterMatchRating Rating = EncounterMatchRating.MaxNotMatch;
    private bool Yielded;
    public MatchedEncounter<IEncounterable> Current { get; private set; }
    private YieldState State;
    private int met;
    private bool mustBeWild;
    readonly object IEnumerator.Current => Current;

    public readonly void Reset() => throw new NotSupportedException();
    public readonly void Dispose() { }
    public readonly IEnumerator<MatchedEncounter<IEncounterable>> GetEnumerator() => this;

    private enum YieldState : byte
    {
        Start,
        Event,
        Bred,
        TradeStart,
        Trade,

        StartCaptures,

        StartSlot,
        Slot,
        EndSlot,

        StaticVersion,
        StaticVersionSL,
        StaticVersionVL,
        StaticShared,
        StaticFixed,
        StaticTera,
        StaticDist,
        StaticMight,

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

                if (!Entity.FatefulEncounter)
                { State = YieldState.Bred; goto case YieldState.Bred; }
                State = YieldState.Event; goto case YieldState.Event;

            case YieldState.Event:
                if (TryGetNext(EncounterEvent.MGDB_G9, out var gift))
                    return SetCurrent(gift);
                if (Yielded)
                    break;
                Index = 0; State = YieldState.Bred; goto case YieldState.Bred;

            case YieldState.Bred:
                State = YieldState.TradeStart;
                if (EncounterGenerator9.TryGetEgg(Entity, Chain, Version, out var egg))
                    return SetCurrent(new(egg, EncounterMatchRating.Match));
                goto case YieldState.TradeStart;

            case YieldState.TradeStart:
                if (Entity.Met_Location == Locations.LinkTrade6NPC)
                    goto case YieldState.Trade;
                goto case YieldState.StartCaptures;
            case YieldState.Trade:
                if (TryGetNext(Encounters9.TradeGift_SV, out var trade))
                    return SetCurrent(trade);
                if (Yielded)
                    break;
                Index = 0; State = YieldState.StartCaptures; goto case YieldState.StartCaptures;

            case YieldState.StartCaptures:
                InitializeWildLocationInfo();
                if (mustBeWild)
                    goto case YieldState.StartSlot;
                goto case YieldState.StaticVersion;

            case YieldState.StartSlot:
                if (!EncounterStateUtil.CanBeWildEncounter(Entity))
                    goto case YieldState.EndSlot;
                goto case YieldState.Slot;
            case YieldState.Slot:
                if (TryGetNext<EncounterArea9, EncounterSlot9>(Encounters9.Slots, out var slot))
                    return SetCurrent(slot);
                goto case YieldState.EndSlot;
            case YieldState.EndSlot:
                if (!mustBeWild)
                    goto case YieldState.Fallback; // already checked everything else
                State = YieldState.StaticVersion; goto case YieldState.StaticVersion;

            case YieldState.StaticVersion:
                if (Version == GameVersion.SL)
                { State = YieldState.StaticVersionSL; goto case YieldState.StaticVersionSL; }
                if (Version == GameVersion.VL)
                { State = YieldState.StaticVersionVL; goto case YieldState.StaticVersionVL; }
                goto case YieldState.Fallback; // already checked everything else

            case YieldState.StaticVersionSL:
                if (TryGetNext(Encounters9.StaticSL, out var sl))
                    return SetCurrent(sl);
                Index = 0; State = YieldState.StaticShared; goto case YieldState.StaticShared;
            case YieldState.StaticVersionVL:
                if (TryGetNext(Encounters9.StaticVL, out var vl))
                    return SetCurrent(vl);
                Index = 0; State = YieldState.StaticShared; goto case YieldState.StaticShared;

            case YieldState.StaticShared:
                if (TryGetNext(Encounters9.Encounter_SV, out var ss))
                    return SetCurrent(ss);
                Index = 0; State = YieldState.StaticFixed; goto case YieldState.StaticFixed;

            case YieldState.StaticFixed:
                if (TryGetNext(Encounters9.Fixed, out var sf))
                    return SetCurrent(sf);
                Index = 0; State = YieldState.StaticTera; goto case YieldState.StaticTera;
            case YieldState.StaticTera:
                if (TryGetNext(Encounters9.Tera, out var st))
                    return SetCurrent(st);
                Index = 0; State = YieldState.StaticDist; goto case YieldState.StaticDist;
            case YieldState.StaticDist:
                if (TryGetNext(Encounters9.Dist, out var sd))
                    return SetCurrent(sd);
                Index = 0; State = YieldState.StaticMight; goto case YieldState.StaticMight;
            case YieldState.StaticMight:
                if (TryGetNext(Encounters9.Might, out var sm))
                    return SetCurrent(sm);
                if (mustBeWild)
                    goto case YieldState.Fallback; // already checked everything else
                Index = 0; State = YieldState.StartSlot; goto case YieldState.StartSlot;

            case YieldState.Fallback:
                State = YieldState.End;
                if (Deferred != null)
                    return SetCurrent(new MatchedEncounter<IEncounterable>(Deferred, Rating));
                break;
            case YieldState.End:
                return false;
        }
        return false;
    }

    private void InitializeWildLocationInfo()
    {
        mustBeWild = Entity is IRibbonIndex r && r.HasEncounterMark();
        met = Entity.Met_Location;
    }

    private bool TryGetNext<TArea, TSlot>(TArea[] areas, out MatchedEncounter<IEncounterable> match)
        where TArea : class, IEncounterArea<TSlot>, IAreaLocation
        where TSlot : class, IEncounterable, IEncounterMatch
    {
        for (; Index < areas.Length; Index++, SubIndex = 0)
        {
            var area = areas[Index];
            if (!area.IsMatchLocation(met))
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
