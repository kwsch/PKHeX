using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core;

public record struct EncounterEnumerator8(PKM Entity, EvoCriteria[] Chain, GameVersion Version) : IEnumerator<MatchedEncounter<IEncounterable>>
{
    private IEncounterable? Deferred;
    private int Index;
    private int SubIndex;
    private EncounterMatchRating Rating = EncounterMatchRating.MaxNotMatch;
    private bool Yielded;
    public MatchedEncounter<IEncounterable> Current { get; private set; }
    private YieldState State;
    private int met;
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
        Bred,
        BredSplit,

        TradeStart,
        TradeSW,
        TradeSH,
        TradeShared,

        StartCaptures,

        SlotStart,
        SlotSW,
        SlotSH,
        SlotSWHidden,
        SlotSHHidden,
        SlotEnd,

        StaticVersion,
        StaticVersionSW,
        StaticVersionSH,
        StaticShared,
        NestSW, NestSH, DistSW, DistSH, DynamaxAdv, Crystal,

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
                if (TryGetNext(EncounterEvent.MGDB_G8, out var gift))
                    return SetCurrent(gift);
                Index = 0; State = YieldState.EventLocal; goto case YieldState.EventLocal;
            case YieldState.EventLocal:
                if (TryGetNext(EncounterEvent.EGDB_G8, out var local))
                    return SetCurrent(local);
                if (Yielded)
                    break;
                Index = 0; State = YieldState.Bred; goto case YieldState.Bred;

            case YieldState.Bred:
                if (!Locations.IsEggLocationBred6(Entity.Egg_Location))
                { State = YieldState.StartCaptures; goto case YieldState.StartCaptures; }
                if (!EncounterGenerator8.TryGetEgg(Chain, Version, out var egg))
                { State = YieldState.StartCaptures; goto case YieldState.StartCaptures; }
                State = YieldState.BredSplit;
                return SetCurrent(new(egg, EncounterMatchRating.Match));
            case YieldState.BredSplit:
                State = YieldState.End;
                if (EncounterGenerator8.TryGetSplit((EncounterEgg)Current.Encounter, Chain, out egg))
                    return SetCurrent(new(egg, EncounterMatchRating.Match));
                break;

            case YieldState.TradeStart:
                if (Version == GameVersion.SW)
                { State = YieldState.TradeSW; goto case YieldState.TradeSW; }
                if (Version == GameVersion.SH)
                { State = YieldState.TradeSH; goto case YieldState.TradeSH; }
                break;
            case YieldState.TradeSW:
                if (TryGetNext(Encounters8.TradeSW, out var trSW))
                    return SetCurrent(trSW);
                Index = 0; State = YieldState.TradeShared; goto case YieldState.TradeShared;
            case YieldState.TradeSH:
                if (TryGetNext(Encounters8.TradeSH, out var trSH))
                    return SetCurrent(trSH);
                Index = 0; State = YieldState.TradeShared; goto case YieldState.TradeShared;
            case YieldState.TradeShared:
                if (TryGetNext(Encounters8.TradeSWSH, out var trade))
                    return SetCurrent(trade);
                if (Yielded)
                    break;
                Index = 0; State = YieldState.StartCaptures; goto case YieldState.StartCaptures;

            case YieldState.StartCaptures:
                InitializeWildLocationInfo();
                if (mustBeSlot)
                    goto case YieldState.SlotStart;
                goto case YieldState.StaticVersion;

            case YieldState.SlotStart:
                if (!EncounterStateUtil.CanBeWildEncounter(Entity))
                    goto case YieldState.SlotEnd;
                if (Version == GameVersion.SW)
                { State = YieldState.SlotSW; goto case YieldState.SlotSW; }
                if (Version == GameVersion.SH)
                { State = YieldState.SlotSH; goto case YieldState.SlotSH; }
                throw new ArgumentOutOfRangeException(nameof(Version));
            case YieldState.SlotSW:
                if (TryGetNext<EncounterArea8, EncounterSlot8>(Encounters8.SlotsSW_Symbol, out var slotSW))
                    return SetCurrent(slotSW);
                Index = 0; State = YieldState.SlotSWHidden; goto case YieldState.SlotSWHidden;
            case YieldState.SlotSH:
                if (TryGetNext<EncounterArea8, EncounterSlot8>(Encounters8.SlotsSH_Symbol, out var slotSH))
                    return SetCurrent(slotSH);
                Index = 0; State = YieldState.SlotSHHidden; goto case YieldState.SlotSHHidden;
            case YieldState.SlotSWHidden:
                if (TryGetNext<EncounterArea8, EncounterSlot8>(Encounters8.SlotsSW_Hidden, out var hidSW))
                    return SetCurrent(hidSW);
                Index = 0; goto case YieldState.SlotEnd;
            case YieldState.SlotSHHidden:
                if (TryGetNext<EncounterArea8, EncounterSlot8>(Encounters8.SlotsSH_Hidden, out var hidSH))
                    return SetCurrent(hidSH);
                Index = 0; goto case YieldState.SlotEnd;
            case YieldState.SlotEnd:
                if (!mustBeSlot)
                    goto case YieldState.Fallback; // already checked everything else
                State = YieldState.StaticVersion; goto case YieldState.StaticVersion;

            case YieldState.StaticVersion:
                if (Version == GameVersion.SW)
                { State = YieldState.StaticVersionSW; goto case YieldState.StaticVersionSW; }
                if (Version == GameVersion.SH)
                { State = YieldState.StaticVersionSH; goto case YieldState.StaticVersionSH; }
                goto case YieldState.Fallback; // already checked everything else

            case YieldState.StaticVersionSW:
                if (TryGetNext(Encounters8.StaticSW, out var sw))
                    return SetCurrent(sw);
                Index = 0; State = YieldState.StaticShared; goto case YieldState.StaticShared;
            case YieldState.StaticVersionSH:
                if (TryGetNext(Encounters8.StaticSH, out var sh))
                    return SetCurrent(sh);
                Index = 0; State = YieldState.StaticShared; goto case YieldState.StaticShared;

            case YieldState.StaticShared:
                if (TryGetNext(Encounters8.StaticSWSH, out var ss))
                    return SetCurrent(ss);
                Index = 0; State = YieldState.NestSW; goto case YieldState.NestSW;

            case YieldState.NestSW:
                if (TryGetNext(Encounters8Nest.Nest_SW, out var nsw))
                    return SetCurrent(nsw);
                Index = 0; State = YieldState.NestSH; goto case YieldState.NestSH;
            case YieldState.NestSH:
                if (TryGetNext(Encounters8Nest.Nest_SH, out var nsh))
                    return SetCurrent(nsh);
                Index = 0; State = YieldState.DistSW; goto case YieldState.DistSW;
            case YieldState.DistSW:
                if (TryGetNext(Encounters8Nest.Dist_SW, out var dsw))
                    return SetCurrent(dsw);
                Index = 0; State = YieldState.DistSH; goto case YieldState.DistSH;
            case YieldState.DistSH:
                if (TryGetNext(Encounters8Nest.Dist_SH, out var dsh))
                    return SetCurrent(dsh);
                Index = 0; State = YieldState.DynamaxAdv; goto case YieldState.DynamaxAdv;
            case YieldState.DynamaxAdv:
                if (TryGetNext(Encounters8Nest.DynAdv_SWSH, out var dyn))
                    return SetCurrent(dyn);
                Index = 0; State = YieldState.Crystal; goto case YieldState.Crystal;
            case YieldState.Crystal:
                if (TryGetNext(Encounters8Nest.Crystal_SWSH, out var cry))
                    return SetCurrent(cry);
                if (mustBeSlot)
                    goto case YieldState.Fallback; // already checked everything else
                Index = 0; State = YieldState.SlotStart; goto case YieldState.SlotStart;

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
        mustBeSlot = Entity is IRibbonIndex r && r.HasEncounterMark();
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
