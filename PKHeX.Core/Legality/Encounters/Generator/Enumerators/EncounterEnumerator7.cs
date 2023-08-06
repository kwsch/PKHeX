using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core;

public record struct EncounterEnumerator7(PKM Entity, EvoCriteria[] Chain, GameVersion Version) : IEnumerator<MatchedEncounter<IEncounterable>>
{
    private IEncounterable? Deferred;
    private int Index;
    private int SubIndex;
    private EncounterMatchRating Rating = EncounterMatchRating.MaxNotMatch;
    private bool Yielded;
    public MatchedEncounter<IEncounterable> Current { get; private set; }
    private YieldState State;
    private int met;
    readonly object IEnumerator.Current => Current;

    public readonly void Reset() => throw new NotSupportedException();
    public readonly void Dispose() { }
    public readonly IEnumerator<MatchedEncounter<IEncounterable>> GetEnumerator() => this;

    private enum YieldState : byte
    {
        Start,
        Event,
        Bred,
        BredTrade,
        BredSplit,
        BredSplitTrade,

        TradeStart,
        TradeSM,
        TradeUSUM,

        StartCaptures,

        StartSlot,
        SlotSN,
        SlotMN,
        SlotUS,
        SlotUM,
        EndSlot,

        StaticStart,
        StaticUS,
        StaticUM,
        StaticSharedUSUM,
        StaticSN,
        StaticMN,
        StaticSharedSM,

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

                if (Entity.Met_Location == Locations.LinkTrade6NPC)
                { State = YieldState.TradeStart; goto case YieldState.TradeStart; }
                if (!Entity.FatefulEncounter)
                { State = YieldState.Bred; goto case YieldState.Bred; }
                State = YieldState.Event; goto case YieldState.Event;

            case YieldState.Event:
                if (TryGetNext(EncounterEvent.MGDB_G7, out var gift))
                    return SetCurrent(gift);
                if (Yielded)
                    break;
                Index = 0; State = YieldState.Bred; goto case YieldState.Bred;

            case YieldState.Bred:
                if (!Locations.IsEggLocationBred6(Entity.Egg_Location))
                { State = YieldState.StartCaptures; goto case YieldState.StartCaptures; }
                if (!EncounterGenerator7.TryGetEgg(Chain, Version, out var egg))
                { State = YieldState.StartCaptures; goto case YieldState.StartCaptures; }
                State = YieldState.BredTrade;
                return SetCurrent(new(egg, EncounterMatchRating.Match));
            case YieldState.BredTrade:
                State = YieldState.BredSplit;
                if (Entity.Egg_Location != Locations.LinkTrade6)
                    goto case YieldState.BredSplit;
                egg = EncounterGenerator7.MutateEggTrade((EncounterEgg)Current.Encounter);
                return SetCurrent(new(egg, EncounterMatchRating.Match));
            case YieldState.BredSplit:
                if (Chain[^1].Species == (int)Species.Eevee)
                { State = YieldState.StaticSharedUSUM; goto case YieldState.StaticSharedUSUM; }
                State = YieldState.BredSplitTrade;
                if (!EncounterGenerator7.TryGetSplit((EncounterEgg)Current.Encounter, Chain, out egg))
                    break;
                return SetCurrent(new(egg, EncounterMatchRating.Match));
            case YieldState.BredSplitTrade:
                State = YieldState.End;
                if (Entity.Egg_Location != Locations.LinkTrade6)
                    break;
                egg = EncounterGenerator7.MutateEggTrade((EncounterEgg)Current.Encounter);
                return SetCurrent(new(egg, EncounterMatchRating.Match));

            case YieldState.TradeStart:
                if (Version is GameVersion.SN or GameVersion.MN)
                { State = YieldState.TradeSM; goto case YieldState.TradeSM; }
                if (Version is GameVersion.US or GameVersion.UM)
                { State = YieldState.TradeUSUM; goto case YieldState.TradeUSUM; }
                break;
            case YieldState.TradeSM:
                if (TryGetNext(Encounters7SM.TradeGift_SM, out var trSM))
                    return SetCurrent(trSM);
                if (Yielded)
                    break;
                Index = 0; State = YieldState.StartCaptures; goto case YieldState.StartCaptures;
            case YieldState.TradeUSUM:
                if (TryGetNext(Encounters7USUM.TradeGift_USUM, out var trUSUM))
                    return SetCurrent(trUSUM);
                if (Yielded)
                    break;
                Index = 0; State = YieldState.StartCaptures; goto case YieldState.StartCaptures;

            case YieldState.StartCaptures:
                InitializeWildLocationInfo();
                goto case YieldState.StaticStart;

            case YieldState.StartSlot:
                if (!EncounterStateUtil.CanBeWildEncounter(Entity))
                    goto case YieldState.EndSlot;
                if (Version == GameVersion.US)
                { State = YieldState.SlotUS; goto case YieldState.SlotUS; }
                if (Version == GameVersion.UM)
                { State = YieldState.SlotUM; goto case YieldState.SlotUM; }
                if (Version == GameVersion.SN)
                { State = YieldState.SlotSN; goto case YieldState.SlotSN; }
                if (Version == GameVersion.MN)
                { State = YieldState.SlotMN; goto case YieldState.SlotMN; }
                throw new ArgumentOutOfRangeException(nameof(Version));
            case YieldState.SlotUS:
                if (TryGetNext<EncounterArea7, EncounterSlot7>(Encounters7USUM.SlotsUS, out var slotUS))
                    return SetCurrent(slotUS);
                goto case YieldState.EndSlot;
            case YieldState.SlotUM:
                if (TryGetNext<EncounterArea7, EncounterSlot7>(Encounters7USUM.SlotsUM, out var slotUM))
                    return SetCurrent(slotUM);
                goto case YieldState.EndSlot;
            case YieldState.SlotSN:
                if (TryGetNext<EncounterArea7, EncounterSlot7>(Encounters7SM.SlotsSN, out var slotSN))
                    return SetCurrent(slotSN);
                goto case YieldState.EndSlot;
            case YieldState.SlotMN:
                if (TryGetNext<EncounterArea7, EncounterSlot7>(Encounters7SM.SlotsMN, out var slotMN))
                    return SetCurrent(slotMN);
                goto case YieldState.EndSlot;
            case YieldState.EndSlot:
                goto case YieldState.Fallback; // already checked everything else

            case YieldState.StaticStart:
                if (Version == GameVersion.US)
                { State = YieldState.StaticUS; goto case YieldState.StaticUS; }
                if (Version == GameVersion.UM)
                { State = YieldState.StaticUM; goto case YieldState.StaticUM; }
                if (Version == GameVersion.SN)
                { State = YieldState.StaticSN; goto case YieldState.StaticSN; }
                if (Version == GameVersion.MN)
                { State = YieldState.StaticMN; goto case YieldState.StaticMN; }
                throw new ArgumentOutOfRangeException(nameof(Version));

            case YieldState.StaticUS:
                if (TryGetNext(Encounters7USUM.StaticUS, out var us))
                    return SetCurrent(us);
                Index = 0; State = YieldState.StaticSharedUSUM; goto case YieldState.StaticSharedUSUM;
            case YieldState.StaticUM:
                if (TryGetNext(Encounters7USUM.StaticUM, out var um))
                    return SetCurrent(um);
                Index = 0; State = YieldState.StaticSharedUSUM; goto case YieldState.StaticSharedUSUM;
            case YieldState.StaticSharedUSUM:
                if (TryGetNext(Encounters7USUM.StaticUSUM, out var usum))
                    return SetCurrent(usum);
                Index = 0; State = YieldState.StartSlot; goto case YieldState.StartSlot;

            case YieldState.StaticSN:
                if (TryGetNext(Encounters7SM.StaticSN, out var sn))
                    return SetCurrent(sn);
                Index = 0; State = YieldState.StaticSharedSM; goto case YieldState.StaticSharedSM;
            case YieldState.StaticMN:
                if (TryGetNext(Encounters7SM.StaticMN, out var mn))
                    return SetCurrent(mn);
                Index = 0; State = YieldState.StaticSharedSM; goto case YieldState.StaticSharedSM;
            case YieldState.StaticSharedSM:
                if (TryGetNext(Encounters7SM.StaticSM, out var sm))
                    return SetCurrent(sm);
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
