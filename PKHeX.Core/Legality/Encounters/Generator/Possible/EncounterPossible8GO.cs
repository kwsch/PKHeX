using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Iterates to find possible encounters for <see cref="GameVersion.GO"/> encounters.
/// </summary>
public record struct EncounterPossible8GO(EvoCriteria[] Chain, EncounterTypeGroup Flags) : IEnumerator<IEncounterable>
{
    public IEncounterable Current { get; private set; }

    private int Index;
    private int SubIndex;
    private YieldState State;
    private int EvoIndex;
    readonly object IEnumerator.Current => Current;
    public readonly void Reset() => throw new NotSupportedException();
    public readonly void Dispose() { }
    public readonly IEnumerator<IEncounterable> GetEnumerator() => this;

    private enum YieldState : byte
    {
        Start,
        Seek,
        Slot,
    }

    public bool MoveNext()
    {
        switch (State)
        {
            case YieldState.Start:
                if (Chain.Length == 0)
                    break;
                if (!Flags.HasFlag(EncounterTypeGroup.Slot))
                    break;
                State = YieldState.Slot;
                goto case YieldState.Seek;
            case YieldState.Seek:
                if (!SeekNextArea(EncountersGO.SlotsGO))
                    break;
                goto case YieldState.Slot;
            case YieldState.Slot:
                var group = EncountersGO.SlotsGO[Index];
                if (TryGetNext(group.Slots))
                    return true;
                goto case YieldState.Seek;
        }
        return false;
    }

    private bool SeekNextArea<TArea>(TArea[] areas) where TArea : ISpeciesForm
    {
        for (; Index < areas.Length; Index++, EvoIndex = 0)
        {
            var area = areas[Index];
            do
            {
                if (area.Species != Chain[EvoIndex].Species)
                    continue;
                return true;
            }
            while (++EvoIndex < Chain.Length);
        }
        return false;
    }

    private bool TryGetNext<TSlot>(TSlot[] slots) where TSlot : IEncounterable, IEncounterMatch
    {
        var evo = Chain[EvoIndex];
        for (; SubIndex < slots.Length;)
        {
            var enc = slots[SubIndex++];
            if (enc.Species != evo.Species)
                continue;
            return SetCurrent(enc);
        }
        EvoIndex = 0; SubIndex = 0; Index++;
        return false;
    }

    private bool SetCurrent(in IEncounterable match)
    {
        Current = match;
        return true;
    }
}
