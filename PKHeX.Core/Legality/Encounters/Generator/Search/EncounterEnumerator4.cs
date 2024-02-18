using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Iterates to find potentially matched encounters for <see cref="GameVersion.Gen4"/>.
/// </summary>
public record struct EncounterEnumerator4(PKM Entity, EvoCriteria[] Chain, GameVersion Version) : IEnumerator<MatchedEncounter<IEncounterable>>
{
    private IEncounterable? Deferred;
    private int Index;
    private int SubIndex;
    private EncounterMatchRating Rating = EncounterMatchRating.MaxNotMatch;
    private bool Yielded;
    public MatchedEncounter<IEncounterable> Current { get; private set; }
    private YieldState State;
    private ushort met;
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
        EventLocal,
        Bred,
        BredSplit,

        TradeStart,
        TradeRanch,
        TradeDPPt,
        TradeHGSS,

        StartCaptures,

        SlotStart,
        SlotD,
        SlotP,
        SlotPt,
        SlotHG,
        SlotSS,
        SlotEnd,

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
                    goto case YieldState.EventStart;
                goto case YieldState.Bred;

            case YieldState.EventStart:
                if (PGT.IsRangerManaphy(Entity))
                {
                    State = YieldState.End;
                    return SetCurrent(EncounterGenerator4.RangerManaphy);
                }
                State = YieldState.Event; goto case YieldState.Event;
            case YieldState.Event:
                if (TryGetNext(EncounterEvent.MGDB_G4))
                    return true;
                Index = 0; State = YieldState.EventLocal; goto case YieldState.EventLocal;
            case YieldState.EventLocal:
                if (TryGetNext(EncounterEvent.EGDB_G4))
                    return true;
                if (Yielded)
                    break;
                Index = 0; goto case YieldState.Bred;

            case YieldState.Bred:
                if (!Locations.IsEggLocationBred4(Entity.EggLocation, Version))
                    goto case YieldState.TradeStart;
                if (!EncounterGenerator4.TryGetEgg(Chain, Version, out var egg))
                    goto case YieldState.TradeStart;
                State = YieldState.BredSplit;
                return SetCurrent(egg);
            case YieldState.BredSplit:
                if (!EncounterGenerator4.TryGetSplit((EncounterEgg)Current.Encounter, Chain, out egg))
                    goto case YieldState.TradeStart;
                State = YieldState.TradeStart;
                return SetCurrent(egg);

            case YieldState.TradeStart:
                if (Version == GameVersion.D)
                { State = YieldState.TradeRanch; goto case YieldState.TradeRanch; }
                if (Version is GameVersion.HG or GameVersion.SS)
                { State = YieldState.TradeHGSS; goto case YieldState.TradeHGSS; }
                State = YieldState.TradeDPPt; goto case YieldState.TradeDPPt;
            case YieldState.TradeRanch:
                if (TryGetNext(Encounters4DPPt.RanchGifts))
                    return true;
                Index = 0; State = YieldState.TradeDPPt; goto case YieldState.TradeDPPt;
            case YieldState.TradeDPPt:
                if (TryGetNext(Encounters4DPPt.TradeGift_DPPtIngame))
                    return true;
                Index = 0; goto case YieldState.StartCaptures;
            case YieldState.TradeHGSS:
                if (TryGetNext(Encounters4HGSS.TradeGift_HGSS))
                    return true;
                Index = 0; goto case YieldState.StartCaptures;

            case YieldState.StartCaptures:
                InitializeWildLocationInfo();
                if (mustBeSlot)
                    goto case YieldState.SlotStart;
                goto case YieldState.StaticStart;

            case YieldState.SlotStart:
                if (!EncounterStateUtil.CanBeWildEncounter(Entity))
                    goto case YieldState.SlotEnd;
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
                if (TryGetNext<EncounterArea4, EncounterSlot4>(Encounters4HGSS.SlotsHG))
                    return true;
                goto case YieldState.SlotEnd;
            case YieldState.SlotSS:
                if (TryGetNext<EncounterArea4, EncounterSlot4>(Encounters4HGSS.SlotsSS))
                    return true;
                goto case YieldState.SlotEnd;
            case YieldState.SlotD:
                if (TryGetNext<EncounterArea4, EncounterSlot4>(Encounters4DPPt.SlotsD))
                    return true;
                goto case YieldState.SlotEnd;
            case YieldState.SlotP:
                if (TryGetNext<EncounterArea4, EncounterSlot4>(Encounters4DPPt.SlotsP))
                    return true;
                goto case YieldState.SlotEnd;
            case YieldState.SlotPt:
                if (TryGetNext<EncounterArea4, EncounterSlot4>(Encounters4DPPt.SlotsPt))
                    return true;
                goto case YieldState.SlotEnd;
            case YieldState.SlotEnd:
                if (!mustBeSlot)
                    goto case YieldState.Fallback; // already checked everything else
                Index = 0; goto case YieldState.StaticStart; // be generous with bad balls

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
                if (TryGetNext(Encounters4DPPt.StaticD))
                    return true;
                Index = 0; State = YieldState.StaticSharedDP; goto case YieldState.StaticSharedDP;
            case YieldState.StaticP:
                if (TryGetNext(Encounters4DPPt.StaticP))
                    return true;
                Index = 0; State = YieldState.StaticSharedDP; goto case YieldState.StaticSharedDP;
            case YieldState.StaticSharedDP:
                if (TryGetNext(Encounters4DPPt.StaticDP))
                    return true;
                Index = 0; State = YieldState.StaticSharedDPPt; goto case YieldState.StaticSharedDPPt;
            case YieldState.StaticPt:
                if (TryGetNext(Encounters4DPPt.StaticPt))
                    return true;
                Index = 0; State = YieldState.StaticSharedDPPt; goto case YieldState.StaticSharedDPPt;
            case YieldState.StaticSharedDPPt:
                if (TryGetNext(Encounters4DPPt.StaticDPPt))
                    return true;
                Index = 0; goto case YieldState.StaticEnd;

            case YieldState.StaticHG:
                if (TryGetNext(Encounters4HGSS.StaticHG))
                    return true;
                Index = 0; State = YieldState.StaticSharedHGSS; goto case YieldState.StaticSharedHGSS;
            case YieldState.StaticSS:
                if (TryGetNext(Encounters4HGSS.StaticSS))
                    return true;
                Index = 0; State = YieldState.StaticSharedHGSS; goto case YieldState.StaticSharedHGSS;
            case YieldState.StaticSharedHGSS:
                if (TryGetNext(Encounters4HGSS.Encounter_HGSS))
                    return true;
                Index = 0; State = YieldState.StaticPokewalker; goto case YieldState.StaticPokewalker;
            case YieldState.StaticPokewalker:
                if (TryGetNext(Encounters4HGSS.Encounter_PokeWalker))
                    return true;
                Index = 0; goto case YieldState.StaticEnd;

            case YieldState.StaticEnd:
                if (mustBeSlot)
                    break;
                goto case YieldState.SlotStart;

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
        mustBeSlot = Entity is { EggLocation: 0, Ball: (int)Ball.Sport or (int)Ball.Safari }; // never static
        hasOriginalLocation = Entity.Format == 4;
    }

    private bool TryGetNext<TArea, TSlot>(TArea[] areas)
        where TArea : IEncounterArea<TSlot>, IAreaLocation
        where TSlot : IEncounterable, IEncounterMatch
    {
        for (; Index < areas.Length; Index++, SubIndex = 0)
        {
            var area = areas[Index];
            if (hasOriginalLocation && !area.IsMatchLocation(met))
                continue;
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

    private bool SetCurrent<T>(T enc, EncounterMatchRating rating = EncounterMatchRating.Match) where T : IEncounterable
    {
        Current = new MatchedEncounter<IEncounterable>(enc, rating);
        Yielded = true;
        return true;
    }
}
