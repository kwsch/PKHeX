using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Iterates to find possible encounters for <see cref="GameVersion.PLA"/> encounters.
/// </summary>
public record struct EncounterPossible8a(EvoCriteria[] Chain, EncounterTypeGroup Flags) : IEnumerator<IEncounterable>
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

        StaticStart,
        Static,

        SlotStart,
        Slot,
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
                    goto case YieldState.StaticStart;
                State = YieldState.Event; goto case YieldState.Event;
            case YieldState.Event:
                if (TryGetNext(EncounterEvent.MGDB_G8A))
                    return true;
                Index = 0; State = YieldState.EventLocal; goto case YieldState.EventLocal;
            case YieldState.EventLocal:
                if (TryGetNext(EncounterEvent.EGDB_G8A))
                    return true;
                Index = 0; State = YieldState.Static; goto case YieldState.Static;

            case YieldState.StaticStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Static))
                    goto case YieldState.SlotStart;
                State = YieldState.Static; goto case YieldState.Static;
            case YieldState.Static:
                if (TryGetNextSub(Encounters8a.StaticLA))
                    return true;
                Index = 0; goto case YieldState.SlotStart;

            case YieldState.SlotStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Slot))
                    break;
                State = YieldState.Slot; goto case YieldState.Slot;
            case YieldState.Slot:
                if (TryGetNext<EncounterArea8a, EncounterSlot8a>(Encounters8a.SlotsLA))
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

    private bool SetCurrent(IEncounterable match)
    {
        Current = match;
        return true;
    }
}
