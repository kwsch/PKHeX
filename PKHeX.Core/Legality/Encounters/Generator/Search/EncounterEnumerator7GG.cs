using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Iterates to find potentially matched encounters for <see cref="GameVersion.GG"/>.
/// </summary>
public record struct EncounterEnumerator7GG(PKM Entity, EvoCriteria[] Chain, GameVersion Version) : IEnumerator<MatchedEncounter<IEncounterable>>
{
    private IEncounterable? Deferred;
    private int Index;
    private int SubIndex;
    private EncounterMatchRating Rating = EncounterMatchRating.MaxNotMatch;
    private bool Yielded;
    public MatchedEncounter<IEncounterable> Current { get; private set; }
    private YieldState State;
    private ushort met;
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
        TradeGP,
        TradeGE,
        TradeShared,

        StaticStart,
        StaticGP,
        StaticGE,
        StaticShared,

        SlotStart,
        SlotGP,
        SlotGE,

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
                if (Entity.IsEgg)
                    break;
                InitializeWildLocationInfo();
                if (met == Locations.LinkTrade6NPC)
                    goto case YieldState.TradeStart;
                if (!Entity.FatefulEncounter)
                    goto case YieldState.StaticStart;
                State = YieldState.Event; goto case YieldState.Event;

            case YieldState.TradeStart:
                if (Version == GameVersion.GP)
                { State = YieldState.TradeGP; goto case YieldState.TradeGP; }
                if (Version == GameVersion.GE)
                { State = YieldState.TradeGE; goto case YieldState.TradeGE; }
                break;
            case YieldState.TradeGP:
                if (TryGetNext(Encounters7GG.TradeGift_GP))
                    return true;
                Index = 0; State = YieldState.TradeShared; goto case YieldState.TradeShared;
            case YieldState.TradeGE:
                if (TryGetNext(Encounters7GG.TradeGift_GE))
                    return true;
                Index = 0; State = YieldState.TradeShared; goto case YieldState.TradeShared;
            case YieldState.TradeShared:
                if (TryGetNext(Encounters7GG.TradeGift_GG))
                    return true;
                break;

            case YieldState.Event:
                if (TryGetNext(EncounterEvent.MGDB_G7GG))
                    return true;
                Index = 0; State = YieldState.EventLocal; goto case YieldState.EventLocal;
            case YieldState.EventLocal:
                if (TryGetNext(EncounterEvent.EGDB_G7GG))
                    return true;
                if (Yielded)
                    break;
                Index = 0; goto case YieldState.StaticStart;

            case YieldState.StaticStart:
                if (Version == GameVersion.GP)
                { State = YieldState.StaticGP; goto case YieldState.StaticGP; }
                if (Version == GameVersion.GE)
                { State = YieldState.StaticGE; goto case YieldState.StaticGE; }
                goto case YieldState.Fallback; // already checked everything else

            case YieldState.StaticGP:
                if (TryGetNext(Encounters7GG.StaticGP))
                    return true;
                Index = 0; State = YieldState.StaticShared; goto case YieldState.StaticShared;
            case YieldState.StaticGE:
                if (TryGetNext(Encounters7GG.StaticGE))
                    return true;
                Index = 0; State = YieldState.StaticShared; goto case YieldState.StaticShared;
            case YieldState.StaticShared:
                if (TryGetNext(Encounters7GG.Encounter_GG))
                    return true;
                if (Yielded)
                    break;
                Index = 0; goto case YieldState.SlotStart;

            case YieldState.SlotStart:
                if (!EncounterStateUtil.CanBeWildEncounter(Entity))
                    break;
                if (Version is GameVersion.GP)
                { State = YieldState.SlotGP; goto case YieldState.SlotGP; }
                if (Version is GameVersion.GE)
                { State = YieldState.SlotGE; goto case YieldState.SlotGE; }
                throw new ArgumentOutOfRangeException(nameof(Version));
            case YieldState.SlotGP:
                if (TryGetNext<EncounterArea7b, EncounterSlot7b>(Encounters7GG.SlotsGP))
                    return true;
                goto case YieldState.Fallback; // already checked everything else
            case YieldState.SlotGE:
                if (TryGetNext<EncounterArea7b, EncounterSlot7b>(Encounters7GG.SlotsGE))
                    return true;
                if (Yielded)
                    break;
                goto case YieldState.Fallback; // already checked everything else

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
