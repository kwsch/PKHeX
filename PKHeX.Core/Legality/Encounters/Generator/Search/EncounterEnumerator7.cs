using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Iterates to find potentially matched encounters for <see cref="GameVersion.Gen7"/>.
/// </summary>
public record struct EncounterEnumerator7(PKM Entity, EvoCriteria[] Chain, GameVersion Version) : IEnumerator<MatchedEncounter<IEncounterable>>
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
        Bred,
        BredTrade,
        BredSplit,
        BredSplitTrade,

        TradeStart,
        TradeSM,
        TradeUSUM,

        StartCaptures,

        SlotStart,
        SlotSN,
        SlotMN,
        SlotUS,
        SlotUM,
        SlotEnd,

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

                if (Entity.MetLocation == Locations.LinkTrade6NPC)
                    goto case YieldState.TradeStart;
                if (!Entity.FatefulEncounter)
                    goto case YieldState.Bred;
                State = YieldState.Event; goto case YieldState.Event;

            case YieldState.Event:
                if (TryGetNext(EncounterEvent.MGDB_G7))
                    return true;
                Index = 0; State = YieldState.EventLocal; goto case YieldState.EventLocal;
            case YieldState.EventLocal:
                if (TryGetNext(EncounterEvent.EGDB_G7))
                    return true;
                if (Yielded)
                    break;
                Index = 0; goto case YieldState.Bred;

            case YieldState.Bred:
                if (!Locations.IsEggLocationBred6(Entity.EggLocation))
                    goto case YieldState.StartCaptures;
                if (!EncounterGenerator7.TryGetEgg(Chain, Version, out var egg))
                    goto case YieldState.StartCaptures;
                State = YieldState.BredTrade;
                return SetCurrent(egg);
            case YieldState.BredTrade:
                State = YieldState.BredSplit;
                if (Entity.EggLocation != Locations.LinkTrade6)
                    goto case YieldState.BredSplit;
                egg = EncounterGenerator7.MutateEggTrade((EncounterEgg)Current.Encounter);
                return SetCurrent(egg);
            case YieldState.BredSplit:
                if (Chain[^1].Species == (int)Species.Eevee)
                { State = YieldState.StaticSharedUSUM; goto case YieldState.StaticSharedUSUM; }
                State = YieldState.BredSplitTrade;
                if (!EncounterGenerator7.TryGetSplit((EncounterEgg)Current.Encounter, Chain, out egg))
                    break;
                return SetCurrent(egg);
            case YieldState.BredSplitTrade:
                State = YieldState.End;
                if (Entity.EggLocation != Locations.LinkTrade6)
                    break;
                egg = EncounterGenerator7.MutateEggTrade((EncounterEgg)Current.Encounter);
                return SetCurrent(egg);

            case YieldState.TradeStart:
                if (Version is GameVersion.SN or GameVersion.MN)
                { State = YieldState.TradeSM; goto case YieldState.TradeSM; }
                if (Version is GameVersion.US or GameVersion.UM)
                { State = YieldState.TradeUSUM; goto case YieldState.TradeUSUM; }
                break;
            case YieldState.TradeSM:
                if (TryGetNext(Encounters7SM.TradeGift_SM))
                    return true;
                if (Yielded)
                    break;
                Index = 0; goto case YieldState.StartCaptures;
            case YieldState.TradeUSUM:
                if (TryGetNext(Encounters7USUM.TradeGift_USUM))
                    return true;
                if (Yielded)
                    break;
                Index = 0; goto case YieldState.StartCaptures;

            case YieldState.StartCaptures:
                InitializeWildLocationInfo();
                goto case YieldState.StaticStart;

            case YieldState.SlotStart:
                if (!EncounterStateUtil.CanBeWildEncounter(Entity))
                    goto case YieldState.SlotEnd;
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
                if (TryGetNext<EncounterArea7, EncounterSlot7>(Encounters7USUM.SlotsUS))
                    return true;
                goto case YieldState.SlotEnd;
            case YieldState.SlotUM:
                if (TryGetNext<EncounterArea7, EncounterSlot7>(Encounters7USUM.SlotsUM))
                    return true;
                goto case YieldState.SlotEnd;
            case YieldState.SlotSN:
                if (TryGetNext<EncounterArea7, EncounterSlot7>(Encounters7SM.SlotsSN))
                    return true;
                goto case YieldState.SlotEnd;
            case YieldState.SlotMN:
                if (TryGetNext<EncounterArea7, EncounterSlot7>(Encounters7SM.SlotsMN))
                    return true;
                goto case YieldState.SlotEnd;
            case YieldState.SlotEnd:
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
                if (TryGetNext(Encounters7USUM.StaticUS))
                    return true;
                Index = 0; State = YieldState.StaticSharedUSUM; goto case YieldState.StaticSharedUSUM;
            case YieldState.StaticUM:
                if (TryGetNext(Encounters7USUM.StaticUM))
                    return true;
                Index = 0; State = YieldState.StaticSharedUSUM; goto case YieldState.StaticSharedUSUM;
            case YieldState.StaticSharedUSUM:
                if (TryGetNext(Encounters7USUM.StaticUSUM))
                    return true;
                Index = 0; goto case YieldState.SlotStart;

            case YieldState.StaticSN:
                if (TryGetNext(Encounters7SM.StaticSN))
                    return true;
                Index = 0; State = YieldState.StaticSharedSM; goto case YieldState.StaticSharedSM;
            case YieldState.StaticMN:
                if (TryGetNext(Encounters7SM.StaticMN))
                    return true;
                Index = 0; State = YieldState.StaticSharedSM; goto case YieldState.StaticSharedSM;
            case YieldState.StaticSharedSM:
                if (TryGetNext(Encounters7SM.StaticSM))
                    return true;
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
