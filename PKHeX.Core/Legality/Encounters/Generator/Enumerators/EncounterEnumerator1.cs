using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core;

public record struct EncounterEnumerator1(PKM Entity, EvoCriteria[] Chain) : IEnumerator<MatchedEncounter<IEncounterable>>
{
    private IEncounterable? Deferred;
    private int Index;
    private int SubIndex;
    private EncounterMatchRating Rating = EncounterMatchRating.MaxNotMatch;
    public MatchedEncounter<IEncounterable> Current { get; private set; }
    private YieldState State;
    readonly object IEnumerator.Current => Current;

    public readonly void Reset() => throw new NotSupportedException();
    public readonly void Dispose() { }
    public readonly IEnumerator<MatchedEncounter<IEncounterable>> GetEnumerator() => this;

    private enum YieldState : byte
    {
        Start,

        TradeStart,
        TradeBU,
        TradeRB,
        TradeYW,

        SlotStart,
        SlotBU,
        SlotRD,
        SlotGN,
        SlotYW,

        StaticStart,
        StaticBU,
        StaticRB,
        StaticYW,
        StaticShared,

        EventStart,
        EventVC,
        EventGB,

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
                goto case YieldState.TradeStart;

            case YieldState.TradeStart:
                if (Entity.Japanese)
                { State = YieldState.TradeBU; goto case YieldState.TradeBU; }
                State = YieldState.TradeRB; goto case YieldState.TradeRB;
            case YieldState.TradeBU:
                if (TryGetNext(Encounters1.TradeGift_BU, out var trBU))
                    return SetCurrent(trBU);
                Index = 0; State = YieldState.TradeRB; goto case YieldState.TradeRB;
            case YieldState.TradeRB:
                if (TryGetNext(Encounters1.TradeGift_RB, out var trRB))
                    return SetCurrent(trRB);
                Index = 0; State = YieldState.TradeYW; goto case YieldState.TradeYW;
            case YieldState.TradeYW:
                if (TryGetNext(Encounters1.TradeGift_YW, out var trYW))
                    return SetCurrent(trYW);
                Index = 0; goto case YieldState.StaticStart;

            case YieldState.StaticStart:
                if (Entity.Japanese)
                { State = YieldState.StaticBU; goto case YieldState.StaticBU; }
                State = YieldState.StaticRB; goto case YieldState.StaticRB;

            case YieldState.StaticBU:
                if (TryGetNext(Encounters1.StaticBU, out var stBU))
                    return SetCurrent(stBU);
                Index = 0; State = YieldState.StaticRB; goto case YieldState.StaticRB;
            case YieldState.StaticRB:
                if (TryGetNext(Encounters1.StaticRB, out var stRB))
                    return SetCurrent(stRB);
                Index = 0; State = YieldState.StaticYW; goto case YieldState.StaticYW;
            case YieldState.StaticYW:
                if (TryGetNext(Encounters1.StaticYW, out var stYW))
                    return SetCurrent(stYW);
                Index = 0; State = YieldState.StaticShared; goto case YieldState.StaticShared;
            case YieldState.StaticShared:
                if (TryGetNext(Encounters1.StaticRBY, out var stSH))
                    return SetCurrent(stSH);
                Index = 0; goto case YieldState.SlotStart;

            case YieldState.SlotStart:
                if (Entity.Japanese)
                { State = YieldState.SlotBU; goto case YieldState.SlotBU; }
                State = YieldState.SlotRD; goto case YieldState.SlotRD;
            case YieldState.SlotBU:
                if (TryGetNext<EncounterArea1, EncounterSlot1>(Encounters1.SlotsBU, out var slBU))
                    return SetCurrent(slBU);
                Index = 0; State = YieldState.SlotRD; goto case YieldState.SlotRD;
            case YieldState.SlotRD:
                if (TryGetNext<EncounterArea1, EncounterSlot1>(Encounters1.SlotsRD, out var slRD))
                    return SetCurrent(slRD);
                Index = 0; State = YieldState.SlotGN; goto case YieldState.SlotGN;
            case YieldState.SlotGN:
                if (TryGetNext<EncounterArea1, EncounterSlot1>(Encounters1.SlotsGN, out var slGN))
                    return SetCurrent(slGN);
                Index = 0; State = YieldState.SlotYW; goto case YieldState.SlotYW;
            case YieldState.SlotYW:
                if (TryGetNext<EncounterArea1, EncounterSlot1>(Encounters1.SlotsYW, out var slYW))
                    return SetCurrent(slYW);
                Index = 0; goto case YieldState.EventStart;

            case YieldState.EventStart:
                if (ParseSettings.AllowGBVirtualConsole3DS)
                { State = YieldState.EventVC; goto case YieldState.EventVC; }
                State = YieldState.EventGB; goto case YieldState.EventGB;
            case YieldState.EventVC:
                if (TryGetNext(Encounters1VC.Gifts, out var evVC))
                    return SetCurrent(evVC);
                goto case YieldState.Fallback;
            case YieldState.EventGB:
                if (TryGetNext(Encounters1GBEra.Gifts, out var evGB))
                    return SetCurrent(evGB);
                goto case YieldState.Fallback;

            case YieldState.Fallback:
                State = YieldState.End;
                if (Deferred != null)
                    return SetCurrent(new MatchedEncounter<IEncounterable>(Deferred, Rating));
                break;
        }
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
