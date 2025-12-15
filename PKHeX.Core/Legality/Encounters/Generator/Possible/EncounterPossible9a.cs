using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Iterates to find possible encounters for <see cref="EntityContext.Gen9a"/> encounters.
/// </summary>
public record struct EncounterPossible9a(EvoCriteria[] Chain, EncounterTypeGroup Flags) : IEnumerator<IEncounterable>
{
    public IEncounterable Current { get; private set; } = null!;

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
        Trade,

        StaticStart,

        SlotStart,
        Slot,
        Hyperspace,
        SlotEnd,

        StaticCapture,
        StaticGift,
        StaticEnd,

        End,
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
                if (TryGetNext(EncounterEvent.MGDB_G9A))
                    return true;
                Index = 0; State = YieldState.EventLocal; goto case YieldState.EventLocal;
            case YieldState.EventLocal:
                if (TryGetNext(EncounterEvent.EGDB_G9A))
                    return true;
                Index = 0; goto case YieldState.TradeStart;

            case YieldState.TradeStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Trade))
                    goto case YieldState.StaticStart;
                State = YieldState.Trade; goto case YieldState.Trade;
            case YieldState.Trade:
                if (TryGetNext(Encounters9a.Trades))
                    return true;
                Index = 0; goto case YieldState.StaticStart;

            case YieldState.StaticStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Static))
                    goto case YieldState.SlotStart;
                goto case YieldState.StaticCapture;
            case YieldState.StaticCapture:
                if (TryGetNext(Encounters9a.Static))
                    return true;
                Index = 0; State = YieldState.StaticGift; goto case YieldState.StaticGift;
            case YieldState.StaticGift:
                if (TryGetNext(Encounters9a.Gifts))
                    return true;
                Index = 0; goto case YieldState.StaticEnd;
            case YieldState.StaticEnd:
                goto case YieldState.SlotStart;

            case YieldState.SlotStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Slot))
                    goto case YieldState.End;
                State = YieldState.Slot;
                goto case YieldState.Slot;
            case YieldState.Slot:
                if (TryGetNext<EncounterArea9a, EncounterSlot9a>(Encounters9a.Slots))
                    return true;
                Index = 0; State = YieldState.Hyperspace;
                goto case YieldState.Hyperspace;
            case YieldState.Hyperspace:
                if (TryGetNext<EncounterArea9a, EncounterSlot9a>(Encounters9a.Hyperspace))
                    return true;
                goto case YieldState.SlotEnd;
            case YieldState.SlotEnd:
                goto case YieldState.End;

            case YieldState.End:
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

    private bool SetCurrent(IEncounterable match)
    {
        Current = match;
        return true;
    }
}
