using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Iterates to find possible encounters for <see cref="GameVersion.Gen6"/> encounters.
/// </summary>
public record struct EncounterPossible6(EvoCriteria[] Chain, EncounterTypeGroup Flags, GameVersion Version) : IEnumerator<IEncounterable>
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
        TradeXY,
        TradeAO,

        StaticStart,
        StaticAS,
        StaticOR,
        StaticSharedAO,
        StaticX,
        StaticY,
        StaticSharedXY,

        SlotStart,
        SlotX,
        SlotY,
        SlotAS,
        SlotOR,
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
                if (!EncounterGenerator6.TryGetEgg(Chain, Version, out var egg))
                    goto case YieldState.EventStart;
                State = YieldState.BredTrade;
                return SetCurrent(egg);
            case YieldState.BredTrade:
                State = YieldState.BredSplit;
                egg = EncounterGenerator6.MutateEggTrade((EncounterEgg)Current);
                return SetCurrent(egg);
            case YieldState.BredSplit:
                if (!EncounterGenerator6.TryGetSplit((EncounterEgg)Current, Chain, out egg))
                    goto case YieldState.EventStart;
                State = YieldState.BredSplitTrade;
                return SetCurrent(egg);
            case YieldState.BredSplitTrade:
                State = YieldState.EventStart;
                egg = EncounterGenerator6.MutateEggTrade((EncounterEgg)Current);
                return SetCurrent(egg);

            case YieldState.EventStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Mystery))
                    goto case YieldState.TradeStart;
                State = YieldState.Event; goto case YieldState.Event;
            case YieldState.Event:
                if (TryGetNextEvent(EncounterEvent.MGDB_G6))
                    return true;
                Index = 0; State = YieldState.EventLocal; goto case YieldState.EventLocal;
            case YieldState.EventLocal:
                if (TryGetNextEvent(EncounterEvent.EGDB_G6))
                    return true;
                Index = 0; goto case YieldState.TradeStart;

            case YieldState.TradeStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Trade))
                    goto case YieldState.StaticStart;
                if (Version is GameVersion.X or GameVersion.Y)
                { State = YieldState.TradeXY; goto case YieldState.TradeXY; }
                if (Version is GameVersion.AS or GameVersion.OR)
                { State = YieldState.TradeAO; goto case YieldState.TradeAO; }
                throw new ArgumentOutOfRangeException(nameof(Version));
            case YieldState.TradeXY:
                if (TryGetNext(Encounters6XY.TradeGift_XY))
                    return true;
                Index = 0; goto case YieldState.StaticStart;
            case YieldState.TradeAO:
                if (TryGetNext(Encounters6AO.TradeGift_AO))
                    return true;
                Index = 0; goto case YieldState.StaticStart;

            case YieldState.StaticStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Static))
                    goto case YieldState.SlotStart;
                if (Version == GameVersion.AS)
                { State = YieldState.StaticAS; goto case YieldState.StaticAS; }
                if (Version == GameVersion.OR)
                { State = YieldState.StaticOR; goto case YieldState.StaticOR; }
                if (Version == GameVersion.X)
                { State = YieldState.StaticX; goto case YieldState.StaticX; }
                if (Version == GameVersion.Y)
                { State = YieldState.StaticY; goto case YieldState.StaticY; }
                throw new ArgumentOutOfRangeException(nameof(Version));

            case YieldState.StaticAS:
                if (TryGetNext(Encounters6AO.StaticA))
                    return true;
                Index = 0; State = YieldState.StaticSharedAO; goto case YieldState.StaticSharedAO;
            case YieldState.StaticOR:
                if (TryGetNext(Encounters6AO.StaticO))
                    return true;
                Index = 0; State = YieldState.StaticSharedAO; goto case YieldState.StaticSharedAO;
            case YieldState.StaticSharedAO:
                if (TryGetNext(Encounters6AO.Encounter_AO))
                    return true;
                Index = 0; goto case YieldState.SlotStart;

            case YieldState.StaticX:
                if (TryGetNext(Encounters6XY.StaticX))
                    return true;
                Index = 0; State = YieldState.StaticSharedXY; goto case YieldState.StaticSharedXY;
            case YieldState.StaticY:
                if (TryGetNext(Encounters6XY.StaticY))
                    return true;
                Index = 0; State = YieldState.StaticSharedXY; goto case YieldState.StaticSharedXY;
            case YieldState.StaticSharedXY:
                if (TryGetNext(Encounters6XY.Encounter_XY))
                    return true;
                Index = 0; goto case YieldState.SlotStart;

            case YieldState.SlotStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Slot))
                    goto case YieldState.SlotEnd;
                if (Version == GameVersion.AS)
                { State = YieldState.SlotAS; goto case YieldState.SlotAS; }
                if (Version == GameVersion.OR)
                { State = YieldState.SlotOR; goto case YieldState.SlotOR; }
                if (Version == GameVersion.X)
                { State = YieldState.SlotX; goto case YieldState.SlotX; }
                if (Version == GameVersion.Y)
                { State = YieldState.SlotY; goto case YieldState.SlotY; }
                throw new ArgumentOutOfRangeException(nameof(Version));
            case YieldState.SlotAS:
                if (TryGetNext<EncounterArea6AO, EncounterSlot6AO>(Encounters6AO.SlotsA))
                    return true;
                goto case YieldState.SlotEnd;
            case YieldState.SlotOR:
                if (TryGetNext<EncounterArea6AO, EncounterSlot6AO>(Encounters6AO.SlotsO))
                    return true;
                goto case YieldState.SlotEnd;
            case YieldState.SlotX:
                if (TryGetNext<EncounterArea6XY, EncounterSlot6XY>(Encounters6XY.SlotsX))
                    return true;
                goto case YieldState.SlotEnd;
            case YieldState.SlotY:
                if (TryGetNext<EncounterArea6XY, EncounterSlot6XY>(Encounters6XY.SlotsY))
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
