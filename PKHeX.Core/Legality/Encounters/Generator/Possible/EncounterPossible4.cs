using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Iterates to find possible encounters for <see cref="GameVersion.Gen4"/> encounters.
/// </summary>
public record struct EncounterPossible4(EvoCriteria[] Chain, EncounterTypeGroup Flags, GameVersion Version, PKM Entity) : IEnumerator<IEncounterable>
{
    public IEncounterable Current { get; private set; }

    private int Index;
    private int SubIndex;
    private YieldState State;
    readonly object IEnumerator.Current => Current;
    public readonly void Reset() => throw new NotSupportedException();
    public readonly void Dispose() { }
    public readonly IEnumerator<IEncounterable> GetEnumerator() => this;

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

        SlotStart,
        SlotD,
        SlotP,
        SlotPt,
        SlotHG,
        SlotSS,
        SlotEnd,
    }

    public bool MoveNext()
    {
        switch (State)
        {
            case YieldState.Start:
                if (Chain.Length == 0)
                    break;
                if (Flags.HasFlag(EncounterTypeGroup.Egg))
                    goto case YieldState.Bred;
                goto case YieldState.EventStart;

            case YieldState.Bred:
                if (!EncounterGenerator4.TryGetEgg(Chain, Version, out var egg))
                    goto case YieldState.EventStart;
                State = YieldState.BredSplit;
                return SetCurrent(egg);
            case YieldState.BredSplit:
                State = YieldState.EventStart;
                if (EncounterGenerator4.TryGetSplit((EncounterEgg)Current, Chain, out egg))
                    return SetCurrent(egg);
                goto case YieldState.EventStart;

            case YieldState.EventStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Mystery))
                    goto case YieldState.TradeStart;
                State = YieldState.Event;
                if (Chain[^1].Species == (int)Species.Manaphy)
                    return SetCurrent(EncounterGenerator4.RangerManaphy);
                goto case YieldState.Event;
            case YieldState.Event:
                if (TryGetNextEvent(EncounterEvent.MGDB_G4))
                    return true;
                Index = 0; State = YieldState.EventLocal; goto case YieldState.EventLocal;
            case YieldState.EventLocal:
                if (TryGetNextEvent(EncounterEvent.EGDB_G4))
                    return true;
                Index = 0; goto case YieldState.TradeStart;

            case YieldState.TradeStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Trade))
                    goto case YieldState.StaticStart;
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
                Index = 0; goto case YieldState.StaticStart;
            case YieldState.TradeHGSS:
                if (TryGetNext(Encounters4HGSS.TradeGift_HGSS))
                    return true;
                Index = 0; goto case YieldState.StaticStart;

            case YieldState.StaticStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Static))
                    goto case YieldState.SlotStart;
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
                Index = 0; goto case YieldState.SlotStart;

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
                Index = 0; goto case YieldState.SlotStart;

            case YieldState.SlotStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Slot))
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
                if (TryGetNext(Encounters4HGSS.SlotsHG))
                    return true;
                goto case YieldState.SlotEnd;
            case YieldState.SlotSS:
                if (TryGetNext(Encounters4HGSS.SlotsSS))
                    return true;
                goto case YieldState.SlotEnd;
            case YieldState.SlotD:
                if (TryGetNext(Encounters4DPPt.SlotsD))
                    return true;
                goto case YieldState.SlotEnd;
            case YieldState.SlotP:
                if (TryGetNext(Encounters4DPPt.SlotsP))
                    return true;
                goto case YieldState.SlotEnd;
            case YieldState.SlotPt:
                if (TryGetNext(Encounters4DPPt.SlotsPt))
                    return true;
                goto case YieldState.SlotEnd;
            case YieldState.SlotEnd:
                break;
        }
        return false;
    }

    private bool TryGetNext(EncounterArea4[] areas)
    {
        for (; Index < areas.Length; Index++, SubIndex = 0)
        {
            var area = areas[Index];
            if (TryGetNextSub(area.Slots))
                return true;
        }
        return false;
    }

    private bool TryGetNextEvent<T>(T[] db) where T : class, IEncounterable, IRestrictVersion
    {
        for (; Index < db.Length;)
        {
            var enc = db[Index++];
            if (!enc.CanBeReceivedByVersion(Version))
                continue;
            foreach (var evo in Chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                return SetCurrent(enc);
            }
        }
        return false;
    }

    private bool TryGetNextSub(EncounterSlot4[] slots)
    {
        while (SubIndex < slots.Length)
        {
            var enc = slots[SubIndex++];
            foreach (var evo in Chain)
            {
                if (enc.Species != evo.Species)
                    continue;
                if (enc.IsInvalidMunchlaxTree(Entity))
                    continue;
                return SetCurrent(enc);
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
                return SetCurrent(enc);
            }
        }
        return false;
    }

    private bool SetCurrent(IEncounterable match)
    {
        Current = match;
        return true;
    }
}
