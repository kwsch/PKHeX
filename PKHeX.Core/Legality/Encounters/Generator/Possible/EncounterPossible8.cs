using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Iterates to find possible encounters for <see cref="GameVersion.SWSH"/> encounters.
/// </summary>
public record struct EncounterPossible8(EvoCriteria[] Chain, EncounterTypeGroup Flags, GameVersion Version) : IEnumerator<IEncounterable>
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
        TradeSW,
        TradeSH,
        TradeShared,

        StaticStart,

        SlotStart,
        SlotSW,
        SlotSH,
        SlotSWHidden,
        SlotSHHidden,
        SlotEnd,

        StaticVersion,
        StaticVersionSW,
        StaticVersionSH,
        StaticShared,
        NestSW, NestSH, DistSW, DistSH, DynamaxAdv, Crystal,
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
                if (!EncounterGenerator8.TryGetEgg(Chain, Version, out var egg))
                    goto case YieldState.EventStart;
                State = YieldState.BredSplit;
                return SetCurrent(egg);
            case YieldState.BredSplit:
                if (!EncounterGenerator8.TryGetSplit((EncounterEgg)Current, Chain, out egg))
                    goto case YieldState.EventStart;
                State = YieldState.EventStart;
                return SetCurrent(egg);

            case YieldState.EventStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Mystery))
                    goto case YieldState.TradeStart;
                State = YieldState.Event; goto case YieldState.Event;
            case YieldState.Event:
                if (TryGetNext(EncounterEvent.MGDB_G8))
                    return true;
                Index = 0; State = YieldState.EventLocal; goto case YieldState.EventLocal;
            case YieldState.EventLocal:
                if (TryGetNext(EncounterEvent.EGDB_G8))
                    return true;
                Index = 0; goto case YieldState.TradeStart;

            case YieldState.TradeStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Trade))
                    goto case YieldState.StaticStart;
                if (Version == GameVersion.SW)
                { State = YieldState.TradeSW; goto case YieldState.TradeSW; }
                if (Version == GameVersion.SH)
                { State = YieldState.TradeSH; goto case YieldState.TradeSH; }
                throw new ArgumentOutOfRangeException(nameof(Version));
            case YieldState.TradeSW:
                if (TryGetNext(Encounters8.TradeSW))
                    return true;
                Index = 0; State = YieldState.TradeShared; goto case YieldState.TradeShared;
            case YieldState.TradeSH:
                if (TryGetNext(Encounters8.TradeSH))
                    return true;
                Index = 0; State = YieldState.TradeShared; goto case YieldState.TradeShared;
            case YieldState.TradeShared:
                if (TryGetNext(Encounters8.TradeSWSH))
                    return true;
                Index = 0; goto case YieldState.StaticStart;

            case YieldState.StaticStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Static))
                    goto case YieldState.SlotStart;
                goto case YieldState.StaticVersion;

            case YieldState.StaticVersion:
                if (Version == GameVersion.SW)
                { State = YieldState.StaticVersionSW; goto case YieldState.StaticVersionSW; }
                if (Version == GameVersion.SH)
                { State = YieldState.StaticVersionSH; goto case YieldState.StaticVersionSH; }
                throw new ArgumentOutOfRangeException(nameof(Version));

            case YieldState.StaticVersionSW:
                if (TryGetNext(Encounters8.StaticSW))
                    return true;
                Index = 0; State = YieldState.StaticShared; goto case YieldState.StaticShared;
            case YieldState.StaticVersionSH:
                if (TryGetNext(Encounters8.StaticSH))
                    return true;
                Index = 0; State = YieldState.StaticShared; goto case YieldState.StaticShared;

            case YieldState.StaticShared:
                if (TryGetNext(Encounters8.StaticSWSH))
                    return true;
                Index = 0; State = YieldState.NestSW; goto case YieldState.NestSW;

            case YieldState.NestSW:
                if (TryGetNext(Encounters8Nest.Nest_SW))
                    return true;
                Index = 0; State = YieldState.NestSH; goto case YieldState.NestSH;
            case YieldState.NestSH:
                if (TryGetNext(Encounters8Nest.Nest_SH))
                    return true;
                Index = 0; State = YieldState.DistSW; goto case YieldState.DistSW;
            case YieldState.DistSW:
                if (TryGetNext(Encounters8Nest.Dist_SW))
                    return true;
                Index = 0; State = YieldState.DistSH; goto case YieldState.DistSH;
            case YieldState.DistSH:
                if (TryGetNext(Encounters8Nest.Dist_SH))
                    return true;
                Index = 0; State = YieldState.DynamaxAdv; goto case YieldState.DynamaxAdv;
            case YieldState.DynamaxAdv:
                if (TryGetNext(Encounters8Nest.DynAdv_SWSH))
                    return true;
                Index = 0; State = YieldState.Crystal; goto case YieldState.Crystal;
            case YieldState.Crystal:
                if (TryGetNext(Encounters8Nest.Crystal_SWSH))
                    return true;
                Index = 0; goto case YieldState.SlotStart;

            case YieldState.SlotStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Slot))
                    goto case YieldState.SlotEnd;
                if (Version == GameVersion.SW)
                { State = YieldState.SlotSW; goto case YieldState.SlotSW; }
                if (Version == GameVersion.SH)
                { State = YieldState.SlotSH; goto case YieldState.SlotSH; }
                throw new ArgumentOutOfRangeException(nameof(Version));
            case YieldState.SlotSW:
                if (TryGetNext<EncounterArea8, EncounterSlot8>(Encounters8.SlotsSW_Symbol))
                    return true;
                Index = 0; State = YieldState.SlotSWHidden; goto case YieldState.SlotSWHidden;
            case YieldState.SlotSH:
                if (TryGetNext<EncounterArea8, EncounterSlot8>(Encounters8.SlotsSH_Symbol))
                    return true;
                Index = 0; State = YieldState.SlotSHHidden; goto case YieldState.SlotSHHidden;
            case YieldState.SlotSWHidden:
                if (TryGetNext<EncounterArea8, EncounterSlot8>(Encounters8.SlotsSW_Hidden))
                    return true;
                Index = 0; goto case YieldState.SlotEnd;
            case YieldState.SlotSHHidden:
                if (TryGetNext<EncounterArea8, EncounterSlot8>(Encounters8.SlotsSH_Hidden))
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
