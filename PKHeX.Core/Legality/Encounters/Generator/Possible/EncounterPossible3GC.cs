using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Iterates to find possible encounters for <see cref="GameVersion.CXD"/> encounters.
/// </summary>
public record struct EncounterPossible3GC(EvoCriteria[] Chain, EncounterTypeGroup Flags) : IEnumerator<IEncounterable>
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

        TradeStart,
        Trade,

        StaticStart,
        StaticColo,
        StaticColoStarters,
        StaticColoGift,
        StaticXDShadow,
        StaticXDGift,
        StaticEReader,

        SlotStart,
        SlotXD,
    }

    public bool MoveNext()
    {
        switch (State)
        {
            case YieldState.Start:
                if (Chain.Length == 0)
                    break;
                goto case YieldState.TradeStart;

            case YieldState.TradeStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Trade))
                    goto case YieldState.StaticStart;
                State = YieldState.Trade; goto case YieldState.Trade;
            case YieldState.Trade:
                if (TryGetNext(Encounters3XD.Trades))
                    return true;
                Index = 0; goto case YieldState.StaticStart;

            case YieldState.StaticStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Static))
                    goto case YieldState.SlotStart;
                { State = YieldState.StaticColo; goto case YieldState.StaticColo; }
            case YieldState.StaticColo:
                if (TryGetNext(Encounters3Colo.Shadow))
                    return true;
                Index = 0; State = YieldState.StaticColoStarters; goto case YieldState.StaticColoStarters;
            case YieldState.StaticColoStarters:
                if (TryGetNext(Encounters3Colo.Starters))
                    return true;
                Index = 0; State = YieldState.StaticColoGift; goto case YieldState.StaticColoGift;
            case YieldState.StaticColoGift:
                if (TryGetNext(Encounters3Colo.Gifts))
                    return true;
                Index = 0; State = YieldState.StaticXDShadow; goto case YieldState.StaticXDShadow;
            case YieldState.StaticXDShadow:
                if (TryGetNext(Encounters3XD.Shadow))
                    return true;
                Index = 0; State = YieldState.StaticXDGift; goto case YieldState.StaticXDGift;
            case YieldState.StaticXDGift:
                if (TryGetNext(Encounters3XD.Gifts))
                    return true;
                Index = 0; State = YieldState.StaticEReader; goto case YieldState.StaticEReader;
            case YieldState.StaticEReader:
                if (TryGetNext(Encounters3Colo.EReader))
                    return true;
                Index = 0; goto case YieldState.SlotStart;

            case YieldState.SlotStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Slot))
                    break;
                State = YieldState.SlotXD; goto case YieldState.SlotXD;
            case YieldState.SlotXD:
                if (TryGetNext<EncounterArea3XD, EncounterSlot3XD>(Encounters3XD.Slots))
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
