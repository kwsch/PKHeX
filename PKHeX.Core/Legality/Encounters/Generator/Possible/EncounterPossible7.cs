using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Iterates to find possible encounters for <see cref="GameVersion.Gen7"/> encounters.
/// </summary>
public record struct EncounterPossible7(EvoCriteria[] Chain, EncounterTypeGroup Flags, GameVersion Version) : IEnumerator<IEncounterable>
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
                if (!EncounterGenerator7.TryGetEgg(Chain, Version, out var egg))
                    goto case YieldState.EventStart;
                State = YieldState.BredTrade;
                return SetCurrent(egg);
            case YieldState.BredTrade:
                State = YieldState.BredSplit;
                egg = EncounterGenerator7.MutateEggTrade((EncounterEgg)Current);
                return SetCurrent(egg);
            case YieldState.BredSplit:
                if (!EncounterGenerator7.TryGetSplit((EncounterEgg)Current, Chain, out egg))
                    goto case YieldState.EventStart;
                State = YieldState.BredSplitTrade;
                return SetCurrent(egg);
            case YieldState.BredSplitTrade:
                State = YieldState.EventStart;
                egg = EncounterGenerator7.MutateEggTrade((EncounterEgg)Current);
                return SetCurrent(egg);

            case YieldState.EventStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Mystery))
                    goto case YieldState.TradeStart;
                State = YieldState.Event; goto case YieldState.Event;
            case YieldState.Event:
                if (TryGetNextEvent(EncounterEvent.MGDB_G7))
                    return true;
                Index = 0; State = YieldState.EventLocal; goto case YieldState.EventLocal;
            case YieldState.EventLocal:
                if (TryGetNextEvent(EncounterEvent.EGDB_G7))
                    return true;
                Index = 0; goto case YieldState.TradeStart;

            case YieldState.TradeStart:
                if (Version is GameVersion.SN or GameVersion.MN)
                { State = YieldState.TradeSM; goto case YieldState.TradeSM; }
                if (Version is GameVersion.US or GameVersion.UM)
                { State = YieldState.TradeUSUM; goto case YieldState.TradeUSUM; }
                throw new ArgumentOutOfRangeException(nameof(Version));
            case YieldState.TradeSM:
                if (TryGetNext(Encounters7SM.TradeGift_SM))
                    return true;
                Index = 0; goto case YieldState.StartCaptures;
            case YieldState.TradeUSUM:
                if (TryGetNext(Encounters7USUM.TradeGift_USUM))
                    return true;
                Index = 0; goto case YieldState.StartCaptures;

            case YieldState.StartCaptures:
                goto case YieldState.StaticStart;

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

            case YieldState.SlotStart:
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
                break;
        }
        return false;
    }

    private bool TryGetNext<TArea, TSlot>(TArea[] areas)
        where TArea : class, IEncounterArea<TSlot>
        where TSlot : class, IEncounterable, IEncounterMatch
    {
        for (; Index < areas.Length; Index++, SubIndex = 0)
        {
            var area = areas[Index];
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

    private bool SetCurrent(IEncounterable match)
    {
        Current = match;
        return true;
    }
}
