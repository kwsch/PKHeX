using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Iterates to find possible encounters for <see cref="GameVersion.GG"/> encounters.
/// </summary>
public record struct EncounterPossible7GG(EvoCriteria[] Chain, EncounterTypeGroup Flags, GameVersion Version) : IEnumerator<IEncounterable>
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

        TradeStart,
        TradeGP,
        TradeGE,
        TradeShared,

        StaticStart,
        StaticGP,
        StaticGE,
        StaticShared,

        SlotStart,
        SlotGP,
        SlotGE,
    }

    public bool MoveNext()
    {
        switch (State)
        {
            case YieldState.Start:
                if (Chain.Length == 0)
                    break;
                goto case YieldState.EventStart;

            case YieldState.EventStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Mystery))
                    goto case YieldState.TradeStart;
                State = YieldState.Event; goto case YieldState.Event;
            case YieldState.Event:
                if (TryGetNextEvent(EncounterEvent.MGDB_G7GG))
                    return true;
                Index = 0; State = YieldState.EventLocal; goto case YieldState.EventLocal;
            case YieldState.EventLocal:
                if (TryGetNextEvent(EncounterEvent.EGDB_G7GG))
                    return true;
                Index = 0; goto case YieldState.TradeStart;

            case YieldState.TradeStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Trade))
                    goto case YieldState.StaticStart;
                if (Version == GameVersion.GP)
                { State = YieldState.TradeGP; goto case YieldState.TradeGP; }
                if (Version == GameVersion.GE)
                { State = YieldState.TradeGE; goto case YieldState.TradeGE; }
                break;
            case YieldState.TradeGP:
                if (TryGetNext(Encounters7GG.TradeGift_GP))
                    return true;
                Index = 0; State = YieldState.TradeShared; goto case YieldState.TradeShared;
            case YieldState.TradeGE:
                if (TryGetNext(Encounters7GG.TradeGift_GE))
                    return true;
                Index = 0; State = YieldState.TradeShared; goto case YieldState.TradeShared;
            case YieldState.TradeShared:
                if (TryGetNext(Encounters7GG.TradeGift_GG))
                    return true;
                Index = 0; goto case YieldState.StaticStart;

            case YieldState.StaticStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Static))
                    goto case YieldState.SlotStart;
                if (Version == GameVersion.GP)
                { State = YieldState.StaticGP; goto case YieldState.StaticGP; }
                if (Version == GameVersion.GE)
                { State = YieldState.StaticGE; goto case YieldState.StaticGE; }
                throw new ArgumentOutOfRangeException(nameof(Version));

            case YieldState.StaticGP:
                if (TryGetNext(Encounters7GG.StaticGP))
                    return true;
                Index = 0; State = YieldState.StaticShared; goto case YieldState.StaticShared;
            case YieldState.StaticGE:
                if (TryGetNext(Encounters7GG.StaticGE))
                    return true;
                Index = 0; State = YieldState.StaticShared; goto case YieldState.StaticShared;
            case YieldState.StaticShared:
                if (TryGetNext(Encounters7GG.Encounter_GG))
                    return true;
                Index = 0; goto case YieldState.SlotStart;

            case YieldState.SlotStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Slot))
                    break;
                if (Version is GameVersion.GP)
                { State = YieldState.SlotGP; goto case YieldState.SlotGP; }
                if (Version is GameVersion.GE)
                { State = YieldState.SlotGE; goto case YieldState.SlotGE; }
                throw new ArgumentOutOfRangeException(nameof(Version));
            case YieldState.SlotGP:
                if (TryGetNext<EncounterArea7b, EncounterSlot7b>(Encounters7GG.SlotsGP))
                    return true;
                break;
            case YieldState.SlotGE:
                if (TryGetNext<EncounterArea7b, EncounterSlot7b>(Encounters7GG.SlotsGE))
                    return true;
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
