using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Iterates to find potentially matched encounters for <see cref="GameVersion.ZA"/>.
/// </summary>
public record struct EncounterEnumerator9a(PKM Entity, EvoCriteria[] Chain) : IEnumerator<MatchedEncounter<IEncounterable>>
{
    private IEncounterable? Deferred;
    private int Index;
    private int SubIndex;
    private EncounterMatchRating Rating = EncounterMatchRating.MaxNotMatch;
    private bool Yielded;
    public MatchedEncounter<IEncounterable> Current { get; private set; }
    private YieldState State;
    private ushort met;
    private bool mustBeSlot;
    readonly object IEnumerator.Current => Current;

    public readonly void Reset() => throw new NotSupportedException();
    public readonly void Dispose() { }
    public readonly IEnumerator<MatchedEncounter<IEncounterable>> GetEnumerator() => this;

    private enum YieldState : byte
    {
        Start,

        Event,
        EventLocal,
        TradeStart,
        Trade,

        StartCaptures,
        StaticStart,

        SlotStart,
        Slot,
        Hyperspace,
        SlotEnd,

        StaticCapture,
        StaticGift,
        StaticEnd,

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
                    goto case YieldState.TradeStart;
                State = YieldState.Event; goto case YieldState.Event;

            case YieldState.Event:
                if (TryGetNext(EncounterEvent.MGDB_G9A))
                    return true;
                Index = 0; State = YieldState.EventLocal; goto case YieldState.EventLocal;
            case YieldState.EventLocal:
                if (TryGetNext(EncounterEvent.EGDB_G9A))
                    return true;
                if (Yielded)
                    break;
                Index = 0; goto case YieldState.TradeStart;

            case YieldState.TradeStart:
                if (Entity.MetLocation != Locations.LinkTrade6NPC)
                    goto case YieldState.StartCaptures;
                State = YieldState.Trade; goto case YieldState.Trade;
            case YieldState.Trade:
                if (TryGetNext(Encounters9a.Trades))
                    return true;
                if (Yielded)
                    break;
                Index = 0; goto case YieldState.StartCaptures;

            case YieldState.StartCaptures:
                InitializeWildLocationInfo();
                if (mustBeSlot)
                    goto case YieldState.SlotStart;
                goto case YieldState.StaticStart;

            case YieldState.SlotStart:
                if (!EncounterStateUtil.CanBeWildEncounter(Entity))
                    goto case YieldState.SlotEnd;
                State = YieldState.Slot; goto case YieldState.Slot;
            case YieldState.Slot:
                if (TryGetNext<EncounterArea9a, EncounterSlot9a>(Encounters9a.Slots))
                    return true;
                Index = 0; State = YieldState.Hyperspace; goto case YieldState.Hyperspace;
            case YieldState.Hyperspace:
                if (TryGetNext<EncounterArea9a, EncounterSlot9a>(Encounters9a.Hyperspace))
                    return true;
                Index = 0; goto case YieldState.SlotEnd;
            case YieldState.SlotEnd:
                if (!mustBeSlot)
                    goto case YieldState.Fallback; // already checked everything else
                goto case YieldState.StaticStart;

            case YieldState.StaticStart:
                State = YieldState.StaticCapture; goto case YieldState.StaticCapture;

            case YieldState.StaticCapture:
                if (TryGetNext(Encounters9a.Static))
                    return true;
                Index = 0; State = YieldState.StaticGift; goto case YieldState.StaticGift;
            case YieldState.StaticGift:
                if (TryGetNext(Encounters9a.Gifts))
                    return true;
                Index = 0; State = YieldState.StaticEnd; goto case YieldState.StaticEnd;

            case YieldState.StaticEnd:
                if (mustBeSlot)
                    goto case YieldState.Fallback; // already checked everything else
                Index = 0; goto case YieldState.SlotStart;

            case YieldState.Fallback:
                State = YieldState.End;
                if (Deferred != null)
                    return SetCurrent(Deferred, Rating);
                break;
        }
        return false;
    }

    private void InitializeWildLocationInfo()
    {
        met = Entity.MetLocation;
        mustBeSlot = Entity is IRibbonIndex r && r.HasEncounterMark();
    }

    private bool TryGetNext<TArea, TSlot>(TArea[] areas)
        where TArea : class, IEncounterArea<TSlot>, IAreaLocation
        where TSlot : class, IEncounterable, IEncounterMatch
    {
        for (; Index < areas.Length; Index++, SubIndex = 0)
        {
            var area = areas[Index];
            if (!area.IsMatchLocation(met))
                continue;
            if (TryGetNextSub(area.Slots))
                return true;
        }
        return false;
    }

    private bool TryGetNextSub<T>(T[] slots) where T : class, IEncounterable, IEncounterMatch
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
                    return SetCurrent(enc);

                if (rating < Rating)
                {
                    Deferred = enc;
                    Rating = rating;
                }
                break;
            }
        }
        return false;
    }

    private bool TryGetNext<T>(T[] db) where T : class, IEncounterable, IEncounterMatch
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
                    return SetCurrent(enc);

                if (rating < Rating)
                {
                    Deferred = enc;
                    Rating = rating;
                }
                break;
            }
        }
        return false;
    }

    private bool SetCurrent<T>(T enc, EncounterMatchRating rating = EncounterMatchRating.Match) where T : IEncounterable
    {
        Current = new MatchedEncounter<IEncounterable>(enc, rating);
        Yielded = true;
        return true;
    }
}
