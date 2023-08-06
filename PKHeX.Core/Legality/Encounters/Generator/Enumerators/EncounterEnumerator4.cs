using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core;

public record struct EncounterEnumerator4(PKM Entity, EvoCriteria[] Chain, GameVersion Version) : IEnumerator<MatchedEncounter<IEncounterable>>
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
    private bool hasOriginalLocation;

    readonly object IEnumerator.Current => Current;

    public readonly void Reset() => throw new NotSupportedException();
    public readonly void Dispose() { }
    public readonly IEnumerator<MatchedEncounter<IEncounterable>> GetEnumerator() => this;

    private enum YieldState : byte
    {
        Start,
        EventStart,
        Event,
        Bred,
        BredSplit,

        TradeStart,
        TradeRanch,
        TradeDPPt,
        TradeHGSS,

        StartCaptures,

        StartSlot,
        SlotD,
        SlotP,
        SlotPt,
        SlotHG,
        SlotSS,
        EndSlot,

        StaticStart,
        StaticD,
        StaticP,
        StaticSharedDP,
        StaticPt,
        StaticSharedDPPt,

        StaticHG,
        StaticSS,
        StaticSharedHGSS,
        StaticPokewalker,

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

                if (Entity.FatefulEncounter)
                { State = YieldState.EventStart; goto case YieldState.EventStart; }
                State = YieldState.Bred; goto case YieldState.Bred;

            case YieldState.EventStart:
                State = YieldState.Event;
                if (PGT.IsRangerManaphy(Entity))
                    return SetCurrent(new(EncounterGenerator4.RangerManaphy, EncounterMatchRating.Match));
                goto case YieldState.Event;
            case YieldState.Event:
                if (TryGetNext(EncounterEvent.MGDB_G4, out var gift))
                    return SetCurrent(gift);
                if (Yielded)
                    break;
                Index = 0; State = YieldState.Bred; goto case YieldState.Bred;

            case YieldState.Bred:
                if (!Locations.IsEggLocationBred4(Entity.Egg_Location, Version))
                { State = YieldState.TradeStart; goto case YieldState.TradeStart; }
                if (!EncounterGenerator4.TryGetEgg(Chain, Version, out var egg))
                { State = YieldState.TradeStart; goto case YieldState.TradeStart; }
                State = YieldState.BredSplit;
                return SetCurrent(new(egg, EncounterMatchRating.Match));
            case YieldState.BredSplit:
                State = YieldState.End;
                if (!EncounterGenerator4.TryGetSplit((EncounterEgg)Current.Encounter, Chain, out egg))
                    break;
                return SetCurrent(new(egg, EncounterMatchRating.Match));

            case YieldState.TradeStart:
                if (Version == GameVersion.D)
                { State = YieldState.TradeRanch; goto case YieldState.TradeRanch; }
                if (Version is GameVersion.HG or GameVersion.SS)
                { State = YieldState.TradeHGSS; goto case YieldState.TradeHGSS; }
                State = YieldState.TradeDPPt; goto case YieldState.TradeDPPt;
            case YieldState.TradeRanch:
                if (TryGetNext(Encounters4DPPt.RanchGifts, out var ranch))
                    return SetCurrent(ranch);
                Index = 0; State = YieldState.TradeDPPt; goto case YieldState.TradeDPPt;
            case YieldState.TradeDPPt:
                if (TryGetNext(Encounters4DPPt.TradeGift_DPPtIngame, out var trDP))
                    return SetCurrent(trDP);
                Index = 0; State = YieldState.StartCaptures; goto case YieldState.StartCaptures;
            case YieldState.TradeHGSS:
                if (TryGetNext(Encounters4HGSS.TradeGift_HGSS, out var trHGSS))
                    return SetCurrent(trHGSS);
                Index = 0; State = YieldState.StartCaptures; goto case YieldState.StartCaptures;

            case YieldState.StartCaptures:
                InitializeWildLocationInfo();
                if (mustBeSlot)
                { State = YieldState.StartSlot; goto case YieldState.StartSlot; }
                State = YieldState.StaticStart; goto case YieldState.StaticStart;

            case YieldState.StartSlot:
                if (!EncounterStateUtil.CanBeWildEncounter(Entity))
                    goto case YieldState.EndSlot;
                if (Version == GameVersion.D)
                { State = YieldState.SlotD; goto case YieldState.SlotD; }
                if (Version == GameVersion.P)
                { State = YieldState.SlotP; goto case YieldState.SlotP; }
                if (Version == GameVersion.Pt)
                { State = YieldState.SlotPt; goto case YieldState.SlotPt; }
                if (Version == GameVersion.HG)
                { State = YieldState.SlotHG; goto case YieldState.SlotHG; }
                if (Version == GameVersion.SS)
                { State = YieldState.SlotSS; goto case YieldState.SlotSS; }
                throw new ArgumentOutOfRangeException(nameof(Version));
            case YieldState.SlotHG:
                if (TryGetNext<EncounterArea4, EncounterSlot4>(Encounters4HGSS.SlotsHG, out var slotHG))
                    return SetCurrentSlot(slotHG);
                goto case YieldState.EndSlot;
            case YieldState.SlotSS:
                if (TryGetNext<EncounterArea4, EncounterSlot4>(Encounters4HGSS.SlotsSS, out var slotSS))
                    return SetCurrentSlot(slotSS);
                goto case YieldState.EndSlot;
            case YieldState.SlotD:
                if (TryGetNext<EncounterArea4, EncounterSlot4>(Encounters4DPPt.SlotsD, out var slotD))
                    return SetCurrentSlot(slotD);
                goto case YieldState.EndSlot;
            case YieldState.SlotP:
                if (TryGetNext<EncounterArea4, EncounterSlot4>(Encounters4DPPt.SlotsP, out var slotP))
                    return SetCurrentSlot(slotP);
                goto case YieldState.EndSlot;
            case YieldState.SlotPt:
                if (TryGetNext<EncounterArea4, EncounterSlot4>(Encounters4DPPt.SlotsPt, out var slotPt))
                    return SetCurrentSlot(slotPt);
                goto case YieldState.EndSlot;
            case YieldState.EndSlot:
                if (mustBeSlot)
                    goto case YieldState.StaticStart; // be generous with bad balls
                goto case YieldState.Fallback; // already checked everything else

            case YieldState.StaticStart:
                if (Version == GameVersion.D)
                { State = YieldState.StaticD; goto case YieldState.StaticD; }
                if (Version == GameVersion.P)
                { State = YieldState.StaticP; goto case YieldState.StaticP; }
                if (Version == GameVersion.Pt)
                { State = YieldState.StaticPt; goto case YieldState.StaticPt; }
                if (Version == GameVersion.HG)
                { State = YieldState.StaticHG; goto case YieldState.StaticHG; }
                if (Version == GameVersion.SS)
                { State = YieldState.StaticSS; goto case YieldState.StaticSS; }
                throw new ArgumentOutOfRangeException(nameof(Version));

            case YieldState.StaticD:
                if (TryGetNext(Encounters4DPPt.StaticD, out var d))
                    return SetCurrent(d);
                Index = 0; State = YieldState.StaticSharedDP; goto case YieldState.StaticSharedDP;
            case YieldState.StaticP:
                if (TryGetNext(Encounters4DPPt.StaticP, out var p))
                    return SetCurrent(p);
                Index = 0; State = YieldState.StaticSharedDP; goto case YieldState.StaticSharedDP;
            case YieldState.StaticSharedDP:
                if (TryGetNext(Encounters4DPPt.StaticDP, out var dp))
                    return SetCurrent(dp);
                Index = 0; State = YieldState.StaticSharedDPPt; goto case YieldState.StaticSharedDPPt;
            case YieldState.StaticPt:
                if (TryGetNext(Encounters4DPPt.StaticPt, out var pt))
                    return SetCurrent(pt);
                Index = 0; State = YieldState.StaticSharedDPPt; goto case YieldState.StaticSharedDPPt;
            case YieldState.StaticSharedDPPt:
                if (TryGetNext(Encounters4DPPt.StaticDPPt, out var dpp))
                    return SetCurrent(dpp);
                Index = 0; goto case YieldState.StaticEnd;

            case YieldState.StaticHG:
                if (TryGetNext(Encounters4HGSS.StaticHG, out var hg))
                    return SetCurrent(hg);
                Index = 0; State = YieldState.StaticSharedHGSS; goto case YieldState.StaticSharedHGSS;
            case YieldState.StaticSS:
                if (TryGetNext(Encounters4HGSS.StaticSS, out var ss))
                    return SetCurrent(ss);
                Index = 0; State = YieldState.StaticSharedHGSS; goto case YieldState.StaticSharedHGSS;
            case YieldState.StaticSharedHGSS:
                if (TryGetNext(Encounters4HGSS.Encounter_HGSS, out var hgss))
                    return SetCurrent(hgss);
                Index = 0; State = YieldState.StaticPokewalker; goto case YieldState.StaticPokewalker;
            case YieldState.StaticPokewalker:
                if (TryGetNext(Encounters4HGSS.Encounter_PokeWalker, out var pw))
                    return SetCurrent(pw);
                Index = 0; State = YieldState.StaticEnd; goto case YieldState.StaticEnd;

            case YieldState.StaticEnd:
                if (mustBeSlot)
                    break;
                goto case YieldState.StartSlot;

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
        mustBeSlot = Entity.Ball is (int)Ball.Sport or (int)Ball.Safari; // never static
        hasOriginalLocation = Entity.Format == 4;
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

    private bool SetCurrentSlot(in MatchedEncounter<IEncounterable> match)
    {
        Current = match;
        Yielded = true;
        return true;
    }

    private bool SetCurrent(in MatchedEncounter<IEncounterable> match)
    {
        Current = match;
        Yielded = true;
        return true;
    }
}
