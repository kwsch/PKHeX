using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Iterates to find possible encounters for <see cref="GameVersion.Gen5"/> encounters.
/// </summary>
public record struct EncounterPossible5(EvoCriteria[] Chain, EncounterTypeGroup Flags, GameVersion Version) : IEnumerator<IEncounterable>
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
        TradeW,
        TradeB,
        TradeBW,
        TradeW2,
        TradeB2,
        TradeB2W2,

        StaticStart,
        StaticW,
        StaticB,
        StaticSharedBW,
        StaticEntreeBW,
        StaticW2,
        StaticB2,
        StaticN,
        StaticSharedB2W2,
        StaticEntreeB2W2,
        StaticRadar,
        StaticEntreeShared,

        SlotStart,
        SlotW,
        SlotB,
        SlotW2,
        SlotB2,
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
                if (!EncounterGenerator5.TryGetEgg(Chain, Version, out var egg))
                    goto case YieldState.EventStart;
                State = YieldState.BredSplit;
                return SetCurrent(egg);
            case YieldState.BredSplit:
                if (!EncounterGenerator5.TryGetSplit((EncounterEgg)Current, Chain, out egg))
                    goto case YieldState.EventStart;
                State = YieldState.EventStart;
                return SetCurrent(egg);

            case YieldState.EventStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Mystery))
                    goto case YieldState.TradeStart;
                State = YieldState.Event; goto case YieldState.Event;
            case YieldState.Event:
                if (TryGetNextEvent(EncounterEvent.MGDB_G5))
                    return true;
                Index = 0; State = YieldState.EventLocal; goto case YieldState.EventLocal;
            case YieldState.EventLocal:
                if (TryGetNextEvent(EncounterEvent.EGDB_G5))
                    return true;
                Index = 0; goto case YieldState.TradeStart;

            case YieldState.TradeStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Trade))
                    goto case YieldState.StaticStart;
                if (Version == GameVersion.W)
                { State = YieldState.TradeW; goto case YieldState.TradeW; }
                if (Version == GameVersion.B)
                { State = YieldState.TradeB; goto case YieldState.TradeB; }
                if (Version == GameVersion.W2)
                { State = YieldState.TradeW2; goto case YieldState.TradeW2; }
                if (Version == GameVersion.B2)
                { State = YieldState.TradeB2; goto case YieldState.TradeB2; }
                throw new ArgumentOutOfRangeException(nameof(Version));
            case YieldState.TradeW:
                if (TryGetNext(Encounters5BW.TradeGift_W))
                    return true;
                Index = 0; State = YieldState.TradeBW; goto case YieldState.TradeBW;
            case YieldState.TradeB:
                if (TryGetNext(Encounters5BW.TradeGift_B))
                    return true;
                Index = 0; State = YieldState.TradeBW; goto case YieldState.TradeBW;
            case YieldState.TradeW2:
                if (TryGetNext(Encounters5B2W2.TradeGift_W2))
                    return true;
                Index = 0; State = YieldState.TradeB2W2; goto case YieldState.TradeB2W2;
            case YieldState.TradeB2:
                if (TryGetNext(Encounters5B2W2.TradeGift_B2))
                    return true;
                Index = 0; State = YieldState.TradeB2W2; goto case YieldState.TradeB2W2;

            case YieldState.TradeBW:
                if (TryGetNext(Encounters5BW.TradeGift_BW))
                    return true;
                Index = 0; goto case YieldState.StaticStart;
            case YieldState.TradeB2W2:
                if (TryGetNext(Encounters5B2W2.TradeGift_B2W2))
                    return true;
                Index = 0; goto case YieldState.StaticStart;

            case YieldState.StaticStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Static))
                    goto case YieldState.SlotStart;
                if (Version == GameVersion.W)
                { State = YieldState.StaticW; goto case YieldState.StaticW; }
                if (Version == GameVersion.B)
                { State = YieldState.StaticB; goto case YieldState.StaticB; }
                if (Version == GameVersion.W2)
                { State = YieldState.StaticW2; goto case YieldState.StaticW2; }
                if (Version == GameVersion.B2)
                { State = YieldState.StaticB2; goto case YieldState.StaticB2; }
                throw new ArgumentOutOfRangeException(nameof(Version));

            case YieldState.StaticW:
                if (TryGetNext(Encounters5BW.StaticW))
                    return true;
                Index = 0; State = YieldState.StaticSharedBW; goto case YieldState.StaticSharedBW;
            case YieldState.StaticB:
                if (TryGetNext(Encounters5BW.StaticB))
                    return true;
                Index = 0; State = YieldState.StaticSharedBW; goto case YieldState.StaticSharedBW;
            case YieldState.StaticSharedBW:
                if (TryGetNext(Encounters5BW.Encounter_BW))
                    return true;
                Index = 0; State = YieldState.StaticEntreeBW; goto case YieldState.StaticEntreeBW;
            case YieldState.StaticEntreeBW:
                if (TryGetNext(Encounters5BW.DreamWorld_BW))
                    return true;
                Index = 0; State = YieldState.StaticEntreeShared; goto case YieldState.StaticEntreeShared;

            case YieldState.StaticW2:
                if (TryGetNext(Encounters5B2W2.StaticW2))
                    return true;
                Index = 0; State = YieldState.StaticSharedB2W2; goto case YieldState.StaticSharedB2W2;
            case YieldState.StaticB2:
                if (TryGetNext(Encounters5B2W2.StaticB2))
                    return true;
                Index = 0; State = YieldState.StaticSharedB2W2; goto case YieldState.StaticSharedB2W2;
            case YieldState.StaticSharedB2W2:
                if (TryGetNext(Encounters5B2W2.Encounter_B2W2_Regular))
                    return true;
                Index = 0; State = YieldState.StaticN; goto case YieldState.StaticN;
            case YieldState.StaticN:
                if (TryGetNext(Encounters5B2W2.Encounter_B2W2_N))
                    return true;
                Index = 0; State = YieldState.StaticEntreeB2W2; goto case YieldState.StaticEntreeB2W2;
            case YieldState.StaticEntreeB2W2:
                if (TryGetNext(Encounters5B2W2.DreamWorld_B2W2))
                    return true;
                Index = 0; State = YieldState.StaticRadar; goto case YieldState.StaticRadar;
            case YieldState.StaticRadar:
                if (TryGetNext(Encounters5DR.Encounter_DreamRadar))
                    return true;
                Index = 0; State = YieldState.StaticEntreeShared; goto case YieldState.StaticEntreeShared;

            case YieldState.StaticEntreeShared:
                if (TryGetNext(Encounters5DR.DreamWorld_Common))
                    return true;
                Index = 0; goto case YieldState.SlotStart;

            case YieldState.SlotStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Slot))
                    goto case YieldState.SlotEnd;
                if (Version == GameVersion.W)
                { State = YieldState.SlotW; goto case YieldState.SlotW; }
                if (Version == GameVersion.B)
                { State = YieldState.SlotB; goto case YieldState.SlotB; }
                if (Version == GameVersion.W2)
                { State = YieldState.SlotW2; goto case YieldState.SlotW2; }
                if (Version == GameVersion.B2)
                { State = YieldState.SlotB2; goto case YieldState.SlotB2; }
                throw new ArgumentOutOfRangeException(nameof(Version));
            case YieldState.SlotW2:
                if (TryGetNext<EncounterArea5, EncounterSlot5>(Encounters5B2W2.SlotsW2))
                    return true;
                goto case YieldState.SlotEnd;
            case YieldState.SlotB2:
                if (TryGetNext<EncounterArea5, EncounterSlot5>(Encounters5B2W2.SlotsB2))
                    return true;
                goto case YieldState.SlotEnd;
            case YieldState.SlotW:
                if (TryGetNext<EncounterArea5, EncounterSlot5>(Encounters5BW.SlotsW))
                    return true;
                goto case YieldState.SlotEnd;
            case YieldState.SlotB:
                if (TryGetNext<EncounterArea5, EncounterSlot5>(Encounters5BW.SlotsB))
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
