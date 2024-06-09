using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Iterates to find possible encounters for <see cref="GameVersion.Gen3"/> encounters.
/// </summary>
public record struct EncounterPossible3(EvoCriteria[] Chain, EncounterTypeGroup Flags, GameVersion Version) : IEnumerator<IEncounterable>
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

        Bred,
        BredSplit,

        EventStart,
        EventColoR,
        EventColoS,
        Event,

        TradeStart,
        TradeRS,
        TradeE,
        TradeFR,
        TradeLG,
        TradeFRLG,

        SlotStart,
        SlotR,
        SlotS,
        SlotE,
        SwarmRSE,
        SlotFR,
        SlotLG,
        SlotEnd,

        StaticStart,
        StaticR,
        StaticS,
        StaticE,
        StaticSharedRSE,

        StaticFR,
        StaticLG,
        StaticSharedFRLG,
        StaticEnd,
    }

    public bool MoveNext()
    {
        switch (State)
        {
            case YieldState.Start:
                if (Chain.Length == 0)
                    break;
                if (!Flags.HasFlag(EncounterTypeGroup.Egg))
                    goto case YieldState.EventStart;
                goto case YieldState.Bred;

            case YieldState.Bred:
                if (!EncounterGenerator3.TryGetEgg(Chain, Version, out var egg))
                    goto case YieldState.EventStart;
                State = YieldState.BredSplit;
                return SetCurrent(egg);
            case YieldState.BredSplit:
                if (!EncounterGenerator3.TryGetSplit((EncounterEgg)Current, Chain, out egg))
                    goto case YieldState.EventStart;
                State = YieldState.EventStart;
                return SetCurrent(egg);

            case YieldState.EventStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Mystery))
                    goto case YieldState.TradeStart;
                State = YieldState.EventColoR; goto case YieldState.EventColoR;
            case YieldState.EventColoR:
                if (TryGetNext(Encounters3RSE.ColoGiftsR))
                    return true;
                Index = 0; State = YieldState.EventColoS; goto case YieldState.EventColoS;
            case YieldState.EventColoS:
                if (TryGetNext(Encounters3RSE.ColoGiftsS))
                    return true;
                Index = 0; State = YieldState.Event; goto case YieldState.Event;
            case YieldState.Event:
                if (TryGetNextEvent(EncountersWC3.Encounter_WC3))
                    return true;
                Index = 0; goto case YieldState.TradeStart;

            case YieldState.TradeStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Trade))
                    goto case YieldState.StaticStart;
                if (Version == GameVersion.E)
                { State = YieldState.TradeE; goto case YieldState.TradeE; }
                if (Version is GameVersion.FR)
                { State = YieldState.TradeFR; goto case YieldState.TradeFR; }
                if (Version is GameVersion.LG)
                { State = YieldState.TradeLG; goto case YieldState.TradeLG; }
                State = YieldState.TradeRS; goto case YieldState.TradeRS;
            case YieldState.TradeRS:
                if (TryGetNext(Encounters3RSE.TradeGift_RS))
                    return true;
                Index = 0; State = YieldState.TradeE; goto case YieldState.TradeE;
            case YieldState.TradeE:
                if (TryGetNext(Encounters3RSE.TradeGift_E))
                    return true;
                Index = 0; goto case YieldState.StaticStart;
            case YieldState.TradeFR:
                if (TryGetNext(Encounters3FRLG.TradeGift_FR))
                    return true;
                Index = 0; State = YieldState.TradeFRLG; goto case YieldState.TradeFRLG;
            case YieldState.TradeLG:
                if (TryGetNext(Encounters3FRLG.TradeGift_LG))
                    return true;
                Index = 0; State = YieldState.TradeFRLG; goto case YieldState.TradeFRLG;
            case YieldState.TradeFRLG:
                if (TryGetNext(Encounters3FRLG.TradeGift_FRLG))
                    return true;
                Index = 0; goto case YieldState.StaticStart;

            case YieldState.StaticStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Static))
                    goto case YieldState.SlotStart;
                if (Version == GameVersion.R)
                { State = YieldState.StaticR; goto case YieldState.StaticR; }
                if (Version == GameVersion.S)
                { State = YieldState.StaticS; goto case YieldState.StaticS; }
                if (Version == GameVersion.E)
                { State = YieldState.StaticE; goto case YieldState.StaticE; }
                if (Version == GameVersion.FR)
                { State = YieldState.StaticFR; goto case YieldState.StaticFR; }
                if (Version == GameVersion.LG)
                { State = YieldState.StaticLG; goto case YieldState.StaticLG; }
                throw new ArgumentOutOfRangeException(nameof(Version));

            case YieldState.StaticR:
                if (TryGetNext(Encounters3RSE.StaticR))
                    return true;
                Index = 0; State = YieldState.StaticSharedRSE; goto case YieldState.StaticSharedRSE;
            case YieldState.StaticS:
                if (TryGetNext(Encounters3RSE.StaticS))
                    return true;
                Index = 0; State = YieldState.StaticSharedRSE; goto case YieldState.StaticSharedRSE;
            case YieldState.StaticE:
                if (TryGetNext(Encounters3RSE.StaticE))
                    return true;
                Index = 0; State = YieldState.StaticSharedRSE; goto case YieldState.StaticSharedRSE;
            case YieldState.StaticSharedRSE:
                if (TryGetNext(Encounters3RSE.StaticRSE))
                    return true;
                Index = 0; goto case YieldState.StaticEnd;
            case YieldState.StaticFR:
                if (TryGetNext(Encounters3FRLG.StaticFR))
                    return true;
                Index = 0; State = YieldState.StaticSharedFRLG; goto case YieldState.StaticSharedFRLG;
            case YieldState.StaticLG:
                if (TryGetNext(Encounters3FRLG.StaticLG))
                    return true;
                Index = 0; State = YieldState.StaticSharedFRLG; goto case YieldState.StaticSharedFRLG;
            case YieldState.StaticSharedFRLG:
                if (TryGetNext(Encounters3FRLG.StaticFRLG))
                    return true;
                Index = 0; goto case YieldState.StaticEnd;
            case YieldState.StaticEnd:
                goto case YieldState.SlotStart;

            case YieldState.SlotStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Slot))
                    goto case YieldState.SlotEnd;
                if (Version == GameVersion.R)
                { State = YieldState.SlotR; goto case YieldState.SlotR; }
                if (Version == GameVersion.S)
                { State = YieldState.SlotS; goto case YieldState.SlotS; }
                if (Version == GameVersion.E)
                { State = YieldState.SlotE; goto case YieldState.SlotE; }
                if (Version == GameVersion.FR)
                { State = YieldState.SlotFR; goto case YieldState.SlotFR; }
                if (Version == GameVersion.LG)
                { State = YieldState.SlotLG; goto case YieldState.SlotLG; }
                throw new ArgumentOutOfRangeException(nameof(Version));

            case YieldState.SlotR:
                if (TryGetNext<EncounterArea3, EncounterSlot3>(Encounters3RSE.SlotsR))
                    return true;
                Index = 0; State = YieldState.SwarmRSE; goto case YieldState.SwarmRSE;
            case YieldState.SlotS:
                if (TryGetNext<EncounterArea3, EncounterSlot3>(Encounters3RSE.SlotsS))
                    return true;
                Index = 0; State = YieldState.SwarmRSE; goto case YieldState.SwarmRSE;
            case YieldState.SlotE:
                if (TryGetNext<EncounterArea3, EncounterSlot3>(Encounters3RSE.SlotsE))
                    return true;
                Index = 0; State = YieldState.SwarmRSE; goto case YieldState.SwarmRSE;
            case YieldState.SwarmRSE:
                if (TryGetNext<EncounterArea3, EncounterSlot3>(Encounters3RSE.SlotsSwarmRSE))
                    return true;
                Index = 0; goto case YieldState.SlotEnd;

            case YieldState.SlotFR:
                if (TryGetNext<EncounterArea3, EncounterSlot3>(Encounters3FRLG.SlotsFR))
                    return true;
                Index = 0; goto case YieldState.SlotEnd;
            case YieldState.SlotLG:
                if (TryGetNext<EncounterArea3, EncounterSlot3>(Encounters3FRLG.SlotsLG))
                    return true;
                Index = 0; goto case YieldState.SlotEnd;

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

    private bool TryGetNextEvent(WC3[] db)
    {
        for (; Index < db.Length;)
        {
            var enc = db[Index++];
            if (!enc.Version.Contains(Version))
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
