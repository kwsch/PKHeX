using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Iterates to find potentially matched encounters for <see cref="GameVersion.Gen1"/>.
/// </summary>
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
                if (TryGetNext(Encounters1.TradeGift_BU))
                    return true;
                Index = 0; State = YieldState.TradeRB; goto case YieldState.TradeRB;
            case YieldState.TradeRB:
                if (TryGetNext(Encounters1.TradeGift_RB))
                    return true;
                Index = 0; State = YieldState.TradeYW; goto case YieldState.TradeYW;
            case YieldState.TradeYW:
                if (TryGetNext(Encounters1.TradeGift_YW))
                    return true;
                Index = 0; goto case YieldState.StaticStart;

            case YieldState.StaticStart:
                if (Entity.Japanese)
                { State = YieldState.StaticBU; goto case YieldState.StaticBU; }
                State = YieldState.StaticRB; goto case YieldState.StaticRB;

            case YieldState.StaticBU:
                if (TryGetNext(Encounters1.StaticBU))
                    return true;
                Index = 0; State = YieldState.StaticRB; goto case YieldState.StaticRB;
            case YieldState.StaticRB:
                if (TryGetNext(Encounters1.StaticRB))
                    return true;
                Index = 0; State = YieldState.StaticYW; goto case YieldState.StaticYW;
            case YieldState.StaticYW:
                if (TryGetNext(Encounters1.StaticYW))
                    return true;
                Index = 0; State = YieldState.StaticShared; goto case YieldState.StaticShared;
            case YieldState.StaticShared:
                if (TryGetNext(Encounters1.StaticRBY))
                    return true;
                Index = 0; goto case YieldState.SlotStart;

            case YieldState.SlotStart:
                if (Entity.Japanese)
                { State = YieldState.SlotBU; goto case YieldState.SlotBU; }
                State = YieldState.SlotRD; goto case YieldState.SlotRD;
            case YieldState.SlotBU:
                if (TryGetNext<EncounterArea1, EncounterSlot1>(Encounters1.SlotsBU))
                    return true;
                Index = 0; State = YieldState.SlotRD; goto case YieldState.SlotRD;
            case YieldState.SlotRD:
                if (TryGetNext<EncounterArea1, EncounterSlot1>(Encounters1.SlotsRD))
                    return true;
                Index = 0; State = YieldState.SlotGN; goto case YieldState.SlotGN;
            case YieldState.SlotGN:
                if (TryGetNext<EncounterArea1, EncounterSlot1>(Encounters1.SlotsGN))
                    return true;
                Index = 0; State = YieldState.SlotYW; goto case YieldState.SlotYW;
            case YieldState.SlotYW:
                if (TryGetNext<EncounterArea1, EncounterSlot1>(Encounters1.SlotsYW))
                    return true;
                Index = 0; goto case YieldState.EventStart;

            case YieldState.EventStart:
                if (ParseSettings.AllowGBVirtualConsole3DS)
                { State = YieldState.EventVC; goto case YieldState.EventVC; }
                State = YieldState.EventGB; goto case YieldState.EventGB;
            case YieldState.EventVC:
                if (TryGetNext(Encounters1VC.Gift))
                    return true;
                goto case YieldState.Fallback;
            case YieldState.EventGB:
                if (TryGetNext(Encounters1GBEra.Gifts))
                    return true;
                goto case YieldState.Fallback;

            case YieldState.Fallback:
                State = YieldState.End;
                if (Deferred != null)
                    return SetCurrent(Deferred, Rating);
                break;
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

    private bool TryGetNextSub<T>(T[] slots)
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

    private bool TryGetNext<T>(T enc) where T : class, IEncounterable, IEncounterMatch
    {
        if (Index++ != 0)
            return false;
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
        return false;
    }

    private bool SetCurrent<T>(T enc, EncounterMatchRating rating = EncounterMatchRating.Match) where T : IEncounterable
    {
        Current = new MatchedEncounter<IEncounterable>(enc, rating);
        return true;
    }
}
