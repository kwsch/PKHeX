using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Iterates to find potentially matched encounters for <see cref="GameVersion.Gen2"/>.
/// </summary>
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
    private readonly ushort met;
    private readonly bool hasOriginalMet;
    private readonly bool canOriginateCrystal;

    public EncounterEnumerator2(PKM pk, EvoCriteria[] chain)
    {
        Entity = pk;
        Chain = chain;

        if (pk is ICaughtData2 { CaughtData: not 0 } c2)
        {
            canOriginateCrystal = true;
            hasOriginalMet = true;
            met = c2.MetLocation;
        }
        else
        {
            canOriginateCrystal = pk is { Format: >= 7, Korean: false } || pk.CanInhabitGen1();
        }
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
        StaticCOdd,
        StaticC,
        StaticGD,
        StaticSI,
        StaticGS,
        StaticShared,

        SlotStart,
        SlotC,
        SlotGD,
        SlotSI,
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
                State = YieldState.Trade; goto case YieldState.Trade;

            case YieldState.Trade:
                if (TryGetNext(Encounters2.TradeGift_GSC))
                    return true;
                Index = 0; goto case YieldState.Bred;

            case YieldState.Bred:
                State = Entity.Korean ? YieldState.StaticStart : YieldState.BredCrystal; // next state
                if (EncounterGenerator2.TryGetEgg(Chain, GameVersion.GS, out var egg))
                    return SetCurrent(egg);
                goto case YieldState.StaticStart;
            case YieldState.BredCrystal:
                State = YieldState.StaticStart;
                if (EncounterGenerator2.TryGetEggCrystal(Entity, (EncounterEgg)Current.Encounter, out egg))
                    return SetCurrent(egg);
                goto case YieldState.StaticStart;

            case YieldState.StaticStart:
                if (ParseSettings.AllowGen2OddEgg(Entity))
                { State = YieldState.StaticCOdd; goto case YieldState.StaticCOdd; }
                if (canOriginateCrystal)
                { State = YieldState.StaticC; goto case YieldState.StaticC; }
                State = YieldState.StaticGD; goto case YieldState.StaticGD;
            case YieldState.StaticCOdd:
                if (TryGetNext(Encounters2.StaticOddEggC))
                    return true;
                Index = 0;
                if (canOriginateCrystal)
                { State = YieldState.StaticC; goto case YieldState.StaticC; }
                State = YieldState.StaticGD; goto case YieldState.StaticGD;
            case YieldState.StaticC:
                if (TryGetNext(Encounters2.StaticC))
                    return true;
                if (hasOriginalMet || Entity.OriginalTrainerGender == 1)
                { Index = 0; State = YieldState.StaticShared; goto case YieldState.StaticShared; }
                Index = 0; State = YieldState.StaticGD; goto case YieldState.StaticGD;
            case YieldState.StaticGD:
                if (TryGetNext(Encounters2.StaticGD))
                    return true;
                Index = 0; State = YieldState.StaticSI; goto case YieldState.StaticSI;
            case YieldState.StaticSI:
                if (TryGetNext(Encounters2.StaticSI))
                    return true;
                Index = 0; State = YieldState.StaticGS; goto case YieldState.StaticGS;
            case YieldState.StaticGS:
                if (TryGetNext(Encounters2.StaticGS))
                    return true;
                Index = 0; State = YieldState.StaticShared; goto case YieldState.StaticShared;
            case YieldState.StaticShared:
                if (TryGetNext(Encounters2.StaticGSC))
                    return true;
                Index = 0; goto case YieldState.SlotStart;

            case YieldState.SlotStart:
                if (canOriginateCrystal)
                { State = YieldState.SlotC; goto case YieldState.SlotC; }
                State = YieldState.SlotGD; goto case YieldState.SlotGD;
            case YieldState.SlotC:
                if (TryGetNextLocation<EncounterArea2, EncounterSlot2>(Encounters2.SlotsC))
                    return true;
                if (hasOriginalMet || Entity.OriginalTrainerGender == 1)
                { Index = 0; goto case YieldState.SlotEnd; }
                Index = 0; State = YieldState.SlotGD; goto case YieldState.SlotGD;
            case YieldState.SlotGD:
                if (TryGetNext<EncounterArea2, EncounterSlot2>(Encounters2.SlotsGD))
                    return true;
                Index = 0; State = YieldState.SlotSI; goto case YieldState.SlotSI;
            case YieldState.SlotSI:
                if (TryGetNext<EncounterArea2, EncounterSlot2>(Encounters2.SlotsSI))
                    return true;
                Index = 0; goto case YieldState.SlotEnd;
            case YieldState.SlotEnd:
                goto case YieldState.EventStart;

            case YieldState.EventStart:
                if (Entity.Korean)
                { State = YieldState.Fallback; goto case YieldState.Fallback; }
                if (ParseSettings.AllowGBVirtualConsole3DS)
                { State = YieldState.EventVC; goto case YieldState.EventVC; }
                if (ParseSettings.AllowGBEraEvents)
                { State = YieldState.EventGB; goto case YieldState.EventGB; }
                throw new InvalidOperationException("No events allowed");
            case YieldState.EventVC:
                State = YieldState.Fallback;
                if (IsMatch(Encounters2.CelebiVC))
                    return SetCurrent(Encounters2.CelebiVC);
                goto case YieldState.Fallback;
            case YieldState.EventGB:
                if (TryGetNext(Encounters2GBEra.Gifts))
                    return true;
                State = YieldState.Fallback; goto case YieldState.Fallback;

            case YieldState.Fallback:
                State = YieldState.End;
                if (Deferred != null)
                    return SetCurrent(Deferred, Rating);
                break;
        }
        return false;
    }

    private bool TryGetNextLocation<TArea, TSlot>(TArea[] areas)
        where TArea : IEncounterArea<TSlot>, IAreaLocation
        where TSlot : IEncounterable, IEncounterMatch
    {
        for (; Index < areas.Length; Index++, SubIndex = 0)
        {
            var area = areas[Index];
            if (hasOriginalMet && !area.IsMatchLocation(met))
                continue;
            if (TryGetNextSub(area.Slots))
                return true;
        }
        return false;
    }

    private bool TryGetNext<TArea, TSlot>(TArea[] areas)
        where TArea : IEncounterArea<TSlot>
        where TSlot : IEncounterable, IEncounterMatch
    {
        for (; Index < areas.Length; Index++, SubIndex = 0)
        {
            var area = areas[Index];
            //if (!area.IsMatchLocation(met))
            //    continue;
            if (TryGetNextSub(area.Slots))
                return true;
        }
        return false;
    }

    private bool TryGetNextSub<T>(T[] slots) where T : IEncounterable, IEncounterMatch
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
        return true;
    }
}
