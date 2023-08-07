using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core;

public record struct EncounterEnumerator2 : IEnumerator<MatchedEncounter<IEncounterable>>
{
    private IEncounterable? Deferred;
    private int Index;
    private int SubIndex;
    private EncounterMatchRating Rating = EncounterMatchRating.MaxNotMatch;
    public MatchedEncounter<IEncounterable> Current { get; private set; }

    private YieldState State;
    private readonly PKM Entity;
    private readonly EvoCriteria[] Chain;
    private readonly int met;
    private readonly bool hasOriginalMet;
    private readonly bool canOriginateCrystal;

    public EncounterEnumerator2(PKM pk, EvoCriteria[] chain)
    {
        Entity = pk;
        Chain = chain;

        if (pk is not ICaughtData2 { CaughtData: not 0 } c2)
        {
            canOriginateCrystal = !pk.Korean && pk.CanInhabitGen1();
            return;
        }
        canOriginateCrystal = true;
        hasOriginalMet = true;
        met = c2.Met_Location;
    }

    readonly object IEnumerator.Current => Current;

    public readonly void Reset() => throw new NotSupportedException();
    public readonly void Dispose() { }
    public readonly IEnumerator<MatchedEncounter<IEncounterable>> GetEnumerator() => this;

    private enum YieldState : byte
    {
        Start,

        Trade,
        Bred,
        BredCrystal, // different egg moves

        EventStart,
        EventVC,
        EventGB,

        StaticStart,
        StaticC,
        StaticGD,
        StaticSV,
        StaticGS,
        StaticShared,

        SlotStart,
        SlotC,
        SlotGD,
        SlotSV,
        SlotEnd,

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
                goto case YieldState.EventStart;

            case YieldState.EventStart:
                if (Entity.Korean)
                { State = YieldState.Trade; goto case YieldState.Trade; }
                if (ParseSettings.AllowGBVirtualConsole3DS)
                { State = YieldState.EventVC; goto case YieldState.EventVC; }
                if (ParseSettings.AllowGBEraEvents)
                { State = YieldState.EventGB; goto case YieldState.EventGB; }
                throw new InvalidOperationException("No events allowed");
            case YieldState.EventVC:
                State = YieldState.Trade;
                if (IsMatch(Encounters2.CelebiVC))
                    return SetCurrent(new(Encounters2.CelebiVC, EncounterMatchRating.Match));
                goto case YieldState.Trade;
            case YieldState.EventGB:
                if (TryGetNext(Encounters2GBEra.StaticEventsGB, out var gift))
                    return SetCurrent(gift);
                Index = 0; State = YieldState.Trade; goto case YieldState.Trade;

            case YieldState.Trade:
                if (TryGetNext(Encounters2.TradeGift_GSC, out var trade))
                    return SetCurrent(trade);
                Index = 0; goto case YieldState.Bred;

            case YieldState.Bred:
                State = Entity.Korean ? YieldState.StaticStart : YieldState.BredCrystal; // next state
                if (EncounterGenerator2.TryGetEgg(Chain, GameVersion.GS, out var egg))
                    return SetCurrent(new(egg, EncounterMatchRating.Match));
                goto case YieldState.StaticStart;
            case YieldState.BredCrystal:
                State = YieldState.StaticStart;
                if (EncounterGenerator2.TryGetEggCrystal(Entity, (EncounterEgg)Current.Encounter, out var crystal))
                    return SetCurrent(new(crystal, EncounterMatchRating.Match));
                goto case YieldState.StaticStart;

            case YieldState.StaticStart:
                if (canOriginateCrystal)
                { State = YieldState.StaticC; goto case YieldState.StaticC; }
                State = YieldState.SlotGD; goto case YieldState.SlotGD;
            case YieldState.StaticC:
                if (TryGetNext(Encounters2.StaticC, out var stC))
                    return SetCurrent(stC);
                if (hasOriginalMet || Entity.OT_Gender == 1)
                { Index = 0; State = YieldState.StaticShared; goto case YieldState.StaticShared; }
                Index = 0; State = YieldState.StaticGD; goto case YieldState.StaticGD;
            case YieldState.StaticGD:
                if (TryGetNext(Encounters2.StaticGD, out var stGD))
                    return SetCurrent(stGD);
                Index = 0; State = YieldState.StaticSV; goto case YieldState.StaticSV;
            case YieldState.StaticSV:
                if (TryGetNext(Encounters2.StaticSV, out var stSV))
                    return SetCurrent(stSV);
                Index = 0; State = YieldState.StaticGS; goto case YieldState.StaticGS;
            case YieldState.StaticGS:
                if (TryGetNext(Encounters2.StaticGS, out var stGS))
                    return SetCurrent(stGS);
                Index = 0; State = YieldState.StaticShared; goto case YieldState.StaticShared;
            case YieldState.StaticShared:
                if (TryGetNext(Encounters2.StaticGSC, out var stSH))
                    return SetCurrent(stSH);
                Index = 0; goto case YieldState.SlotStart;

            case YieldState.SlotStart:
                if (canOriginateCrystal)
                { State = YieldState.SlotC; goto case YieldState.SlotC; }
                State = YieldState.SlotGD; goto case YieldState.SlotGD;
            case YieldState.SlotC:
                if (TryGetNextLocation<EncounterArea2, EncounterSlot2>(Encounters2.SlotsC, out var slBU))
                    return SetCurrent(slBU);
                if (hasOriginalMet || Entity.OT_Gender == 1)
                { Index = 0; goto case YieldState.SlotEnd; }
                Index = 0; State = YieldState.SlotGD; goto case YieldState.SlotGD;
            case YieldState.SlotGD:
                if (TryGetNext<EncounterArea2, EncounterSlot2>(Encounters2.SlotsGD, out var slRD))
                    return SetCurrent(slRD);
                Index = 0; State = YieldState.SlotSV; goto case YieldState.SlotSV;
            case YieldState.SlotSV:
                if (TryGetNext<EncounterArea2, EncounterSlot2>(Encounters2.SlotsSV, out var slGN))
                    return SetCurrent(slGN);
                Index = 0; goto case YieldState.SlotEnd;
            case YieldState.SlotEnd:
                goto case YieldState.Fallback;

            case YieldState.Fallback:
                State = YieldState.End;
                if (Deferred != null)
                    return SetCurrent(new MatchedEncounter<IEncounterable>(Deferred, Rating));
                break;
        }
        return false;
    }

    private bool TryGetNextLocation<TArea, TSlot>(TArea[] areas, out MatchedEncounter<IEncounterable> match)
        where TArea : IEncounterArea<TSlot>, IAreaLocation
        where TSlot : IEncounterable, IEncounterMatch
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

    private bool TryGetNext<TArea, TSlot>(TArea[] areas, out MatchedEncounter<IEncounterable> match)
        where TArea : IEncounterArea<TSlot>
        where TSlot : IEncounterable, IEncounterMatch
    {
        for (; Index < areas.Length; Index++, SubIndex = 0)
        {
            var area = areas[Index];
            //if (!area.IsMatchLocation(met))
            //    continue;

            var slots = area.Slots;
            if (TryGetNextSub(slots, out match))
                return true;
        }
        match = default;
        return false;
    }

    private bool TryGetNextSub<T>(T[] slots, out MatchedEncounter<IEncounterable> match)
        where T : IEncounterable, IEncounterMatch
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

    private bool IsMatch<T>(T enc) where T : class, IEncounterable, IEncounterMatch
    {
        foreach (var evo in Chain)
        {
            if (evo.Species != enc.Species)
                continue;
            if (!enc.IsMatchExact(Entity, evo))
                break;
            var rating = enc.GetMatchRating(Entity);
            if (rating == EncounterMatchRating.Match)
                return true;
            if (rating < Rating)
            {
                Deferred = enc;
                Rating = rating;
            }
            break;
        }
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
        return true;
    }
}
