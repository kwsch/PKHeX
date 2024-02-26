using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Iterates to find possible encounters for <see cref="GameVersion.Gen1"/> encounters.
/// </summary>
public record struct EncounterPossible1(EvoCriteria[] Chain, EncounterTypeGroup Flags, GameVersion Version) : IEnumerator<IEncounterable>
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
        TradeBU,
        TradeRB,
        TradeYW,

        SlotStart,
        SlotBU,
        SlotRD,
        SlotGN,
        SlotYW,

        StaticStart,
        StaticBU,
        StaticRB,
        StaticYW,
        StaticShared,

        EventStart,
        EventVC,
        EventGB,
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
                if (Version is GameVersion.BU or GameVersion.RBY)
                { State = YieldState.TradeBU; goto case YieldState.TradeBU; }
                if (Version is GameVersion.YW)
                { State = YieldState.TradeYW; goto case YieldState.TradeYW; }
                State = YieldState.TradeRB; goto case YieldState.TradeRB;
            case YieldState.TradeBU:
                if (TryGetNext(Encounters1.TradeGift_BU))
                    return true;
                Index = 0;
                if (Version == GameVersion.BU)
                    goto case YieldState.StaticStart;
                State = YieldState.TradeYW; goto case YieldState.TradeYW;
            case YieldState.TradeYW:
                if (TryGetNext(Encounters1.TradeGift_YW))
                    return true;
                Index = 0;
                if (Version == GameVersion.YW)
                    goto case YieldState.StaticStart;
                State = YieldState.TradeRB; goto case YieldState.TradeRB;
            case YieldState.TradeRB:
                if (TryGetNext(Encounters1.TradeGift_RB))
                    return true;
                Index = 0; goto case YieldState.StaticStart;

            case YieldState.StaticStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Static))
                    goto case YieldState.SlotStart;
                if (Version is GameVersion.BU or GameVersion.RBY)
                { State = YieldState.StaticBU; goto case YieldState.StaticBU; }
                if (Version is GameVersion.YW)
                { State = YieldState.StaticYW; goto case YieldState.StaticYW; }
                State = YieldState.StaticRB; goto case YieldState.StaticRB;

            case YieldState.StaticBU:
                if (TryGetNext(Encounters1.StaticBU))
                    return true;
                Index = 0;
                if (Version == GameVersion.BU)
                { State = YieldState.StaticShared; goto case YieldState.StaticShared; }
                State = YieldState.StaticYW; goto case YieldState.StaticYW;
            case YieldState.StaticYW:
                if (TryGetNext(Encounters1.StaticYW))
                    return true;
                if (Version == GameVersion.YW)
                { State = YieldState.StaticShared; goto case YieldState.StaticShared; }
                State = YieldState.StaticRB; goto case YieldState.StaticRB;
            case YieldState.StaticRB:
                if (TryGetNext(Encounters1.StaticRB))
                    return true;
                Index = 0; State = YieldState.StaticShared; goto case YieldState.StaticShared;
            case YieldState.StaticShared:
                if (TryGetNext(Encounters1.StaticRBY))
                    return true;
                Index = 0; goto case YieldState.SlotStart;

            case YieldState.SlotStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Slot))
                    goto case YieldState.EventStart;
                if (Version is GameVersion.BU or GameVersion.RBY)
                { State = YieldState.SlotBU; goto case YieldState.SlotBU; }
                if (Version == GameVersion.YW)
                { State = YieldState.SlotYW; goto case YieldState.SlotYW; }
                if (Version is GameVersion.RD or GameVersion.RB)
                { State = YieldState.SlotRD; goto case YieldState.SlotRD; }
                if (Version == GameVersion.GN)
                { State = YieldState.SlotGN; goto case YieldState.SlotGN; }
                throw new ArgumentOutOfRangeException(nameof(Version));
            case YieldState.SlotBU:
                if (TryGetNext<EncounterArea1, EncounterSlot1>(Encounters1.SlotsBU))
                    return true;
                Index = 0;
                if (Version == GameVersion.BU)
                    goto case YieldState.EventStart;
                State = YieldState.SlotYW; goto case YieldState.SlotYW;
            case YieldState.SlotYW:
                if (TryGetNext<EncounterArea1, EncounterSlot1>(Encounters1.SlotsYW))
                    return true;
                Index = 0;
                if (Version == GameVersion.YW)
                    goto case YieldState.EventStart;
                State = YieldState.SlotRD; goto case YieldState.SlotRD;
            case YieldState.SlotRD:
                if (TryGetNext<EncounterArea1, EncounterSlot1>(Encounters1.SlotsRD))
                    return true;
                Index = 0;
                if (Version == GameVersion.RD)
                    goto case YieldState.EventStart;
                State = YieldState.SlotGN; goto case YieldState.SlotGN;
            case YieldState.SlotGN:
                if (TryGetNext<EncounterArea1, EncounterSlot1>(Encounters1.SlotsGN))
                    return true;
                Index = 0; goto case YieldState.EventStart;

            case YieldState.EventStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Mystery))
                    break;
                if (ParseSettings.AllowGBVirtualConsole3DS)
                { State = YieldState.EventVC; goto case YieldState.EventVC; }
                State = YieldState.EventGB; goto case YieldState.EventGB;
            case YieldState.EventVC:
                if (TryGetNext(Encounters1VC.Gift))
                    return true;
                break;
            case YieldState.EventGB:
                if (TryGetNext(Encounters1GBEra.Gifts))
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

    private bool TryGetNext<T>(T enc) where T : class, IEncounterable, IEncounterMatch
    {
        if (Index++ != 0)
            return false;
        foreach (var evo in Chain)
        {
            if (evo.Species != enc.Species)
                continue;
            return SetCurrent(enc);
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
