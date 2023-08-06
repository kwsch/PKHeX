using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core;

public record struct EncounterEnumerator3(PKM Entity, EvoCriteria[] Chain, GameVersion Version) : IEnumerator<MatchedEncounter<IEncounterable>>
{
    private IEncounterable? Deferred;
    private int Index;
    private int SubIndex;
    private EncounterMatchRating Rating = EncounterMatchRating.MaxNotMatch;
    public MatchedEncounter<IEncounterable> Current { get; private set; }
    private YieldState State;
    private int met;
    private bool mustBeSlot;
    private bool hasOriginalLocation;

    readonly object IEnumerator.Current => Current;

    public readonly void Reset() => throw new NotSupportedException();
    public readonly void Dispose() { }
    public readonly IEnumerator<MatchedEncounter<IEncounterable>> GetEnumerator() => this;

    private enum YieldState : byte
    {
        Start,
        Event,
        Bred,
        BredSplit,

        TradeStart,
        TradeRS,
        TradeE,
        TradeFR,
        TradeLG,
        TradeFRLG,

        StartCaptures,

        StartSlot,
        SlotR,
        SlotS,
        SlotE,
        SlotFR,
        SlotLG,
        EndSlot,

        StaticStart,
        StaticR,
        StaticS,
        StaticE,
        StaticSharedRSE,

        StaticFR,
        StaticLG,
        StaticSharedFRLG,

        Fallback,
        End,
        StaticEnd,
    }

    public bool MoveNext()
    {
        switch (State)
        {
            case YieldState.Start:
                if (Chain.Length == 0)
                    break;

                State = YieldState.Event;
                goto case YieldState.Event;

            case YieldState.Event:
                if (TryGetNext(EncountersWC3.Encounter_WC3, out var gift))
                    return SetCurrent(gift);
                Index = 0; goto case YieldState.TradeStart;

            case YieldState.TradeStart:
                if (Version == GameVersion.E)
                { State = YieldState.TradeE; goto case YieldState.TradeE; }
                if (Version is GameVersion.FR)
                { State = YieldState.TradeFR; goto case YieldState.TradeFR; }
                if (Version is GameVersion.LG)
                { State = YieldState.TradeLG; goto case YieldState.TradeLG; }
                State = YieldState.TradeRS; goto case YieldState.TradeRS;
            case YieldState.TradeRS:
                if (TryGetNext(Encounters3RSE.TradeGift_RS, out var trRS))
                    return SetCurrent(trRS);
                Index = 0; State = YieldState.TradeE; goto case YieldState.TradeE;
            case YieldState.TradeE:
                if (TryGetNext(Encounters3RSE.TradeGift_E, out var trE))
                    return SetCurrent(trE);
                Index = 0; goto case YieldState.StartCaptures;
            case YieldState.TradeFR:
                if (TryGetNext(Encounters3FRLG.TradeGift_FR, out var trFR))
                    return SetCurrent(trFR);
                Index = 0; State = YieldState.TradeFRLG; goto case YieldState.TradeFRLG;
            case YieldState.TradeLG:
                if (TryGetNext(Encounters3FRLG.TradeGift_LG, out var trLG))
                    return SetCurrent(trLG);
                Index = 0; State = YieldState.TradeFRLG; goto case YieldState.TradeFRLG;
            case YieldState.TradeFRLG:
                if (TryGetNext(Encounters3FRLG.TradeGift_FRLG, out var trFRLG))
                    return SetCurrent(trFRLG);
                Index = 0; goto case YieldState.StartCaptures;

            case YieldState.StartCaptures:
                InitializeWildLocationInfo();
                if (mustBeSlot)
                { State = YieldState.StartSlot; goto case YieldState.StartSlot; }
                State = YieldState.StaticStart; goto case YieldState.StaticStart;

            case YieldState.StartSlot:
                if (!EncounterStateUtil.CanBeWildEncounter(Entity))
                    goto case YieldState.EndSlot;
                if (Version == GameVersion.R)
                { State = YieldState.SlotR; goto case YieldState.SlotR; }
                if (Version == GameVersion.S)
                { State = YieldState.SlotS; goto case YieldState.SlotS; }
                if (Version == GameVersion.E)
                { State = YieldState.SlotE; goto case YieldState.SlotE; }
                if (Version == GameVersion.FR)
                { State = YieldState.SlotFR; goto case YieldState.SlotFR; }
                if (Version == GameVersion.LG)
                { State = YieldState.SlotLG; goto case YieldState.SlotLG; }
                throw new ArgumentOutOfRangeException(nameof(Version));
            case YieldState.SlotR:
                if (TryGetNext<EncounterArea3, EncounterSlot3>(Encounters3RSE.SlotsR, out var slotD))
                    return SetCurrent(slotD);
                Index = 0; goto case YieldState.EndSlot;
            case YieldState.SlotS:
                if (TryGetNext<EncounterArea3, EncounterSlot3>(Encounters3RSE.SlotsS, out var slotP))
                    return SetCurrent(slotP);
                Index = 0; goto case YieldState.EndSlot;
            case YieldState.SlotE:
                if (TryGetNext<EncounterArea3, EncounterSlot3>(Encounters3RSE.SlotsE, out var slotPt))
                    return SetCurrent(slotPt);
                Index = 0; goto case YieldState.EndSlot;
            case YieldState.SlotFR:
                if (TryGetNext<EncounterArea3, EncounterSlot3>(Encounters3FRLG.SlotsFR, out var slotHG))
                    return SetCurrent(slotHG);
                Index = 0; goto case YieldState.EndSlot;
            case YieldState.SlotLG:
                if (TryGetNext<EncounterArea3, EncounterSlot3>(Encounters3FRLG.SlotsLG, out var slotSS))
                    return SetCurrent(slotSS);
                Index = 0; goto case YieldState.EndSlot;
            case YieldState.EndSlot:
                if (mustBeSlot)
                    goto case YieldState.StaticStart; // be generous with bad balls
                goto case YieldState.Bred; // already checked everything else

            case YieldState.StaticStart:
                if (Version == GameVersion.R)
                { State = YieldState.StaticR; goto case YieldState.StaticR; }
                if (Version == GameVersion.S)
                { State = YieldState.StaticS; goto case YieldState.StaticS; }
                if (Version == GameVersion.E)
                { State = YieldState.StaticE; goto case YieldState.StaticE; }
                if (Version == GameVersion.FR)
                { State = YieldState.StaticFR; goto case YieldState.StaticFR; }
                if (Version == GameVersion.LG)
                { State = YieldState.StaticLG; goto case YieldState.StaticLG; }
                throw new ArgumentOutOfRangeException(nameof(Version));

            case YieldState.StaticR:
                if (TryGetNext(Encounters3RSE.StaticR, out var r))
                    return SetCurrent(r);
                Index = 0; State = YieldState.StaticSharedRSE; goto case YieldState.StaticSharedRSE;
            case YieldState.StaticS:
                if (TryGetNext(Encounters3RSE.StaticS, out var s))
                    return SetCurrent(s);
                Index = 0; State = YieldState.StaticSharedRSE; goto case YieldState.StaticSharedRSE;
            case YieldState.StaticE:
                if (TryGetNext(Encounters3RSE.StaticE, out var e))
                    return SetCurrent(e);
                Index = 0; State = YieldState.StaticSharedRSE; goto case YieldState.StaticSharedRSE;
            case YieldState.StaticSharedRSE:
                if (TryGetNext(Encounters3RSE.StaticRSE, out var rse))
                    return SetCurrent(rse);
                Index = 0; State = YieldState.StaticEnd; goto case YieldState.StaticEnd;
            case YieldState.StaticFR:
                if (TryGetNext(Encounters3FRLG.StaticFR, out var fr))
                    return SetCurrent(fr);
                Index = 0; State = YieldState.StaticSharedFRLG; goto case YieldState.StaticSharedFRLG;
            case YieldState.StaticLG:
                if (TryGetNext(Encounters3FRLG.StaticLG, out var lg))
                    return SetCurrent(lg);
                Index = 0; State = YieldState.StaticSharedFRLG; goto case YieldState.StaticSharedFRLG;
            case YieldState.StaticSharedFRLG:
                if (TryGetNext(Encounters3FRLG.StaticFRLG, out var frlg))
                    return SetCurrent(frlg);
                Index = 0; State = YieldState.StaticEnd; goto case YieldState.StaticEnd;

            case YieldState.StaticEnd:
                if (mustBeSlot)
                    goto case YieldState.Bred; // already checked slots
                goto case YieldState.StartSlot;

            case YieldState.Bred:
                if (!EncounterGenerator3.TryGetEgg(Chain, Version, out var egg))
                    goto case YieldState.Fallback;
                State = YieldState.BredSplit;
                return SetCurrent(new(egg, EncounterMatchRating.Match));
            case YieldState.BredSplit:
                State = YieldState.Fallback;
                if (!EncounterGenerator3.TryGetSplit((EncounterEgg)Current.Encounter, Chain, out egg))
                    goto case YieldState.Fallback;
                return SetCurrent(new(egg, EncounterMatchRating.Match));

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
        mustBeSlot = Entity.Ball is (int)Ball.Safari; // never static
        hasOriginalLocation = Entity.Format == 3;
    }

    private bool TryGetNext<TArea, TSlot>(TArea[] areas, out MatchedEncounter<IEncounterable> match)
        where TArea : IEncounterArea<TSlot>, IAreaLocation
        where TSlot : IEncounterable, IEncounterMatch
    {
        for (; Index < areas.Length; Index++, SubIndex = 0)
        {
            var area = areas[Index];
            if (hasOriginalLocation && !area.IsMatchLocation(met))
                continue;

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
