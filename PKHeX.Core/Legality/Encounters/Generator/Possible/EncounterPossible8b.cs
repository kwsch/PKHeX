using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Iterates to find possible encounters for <see cref="GameVersion.BDSP"/> encounters.
/// </summary>
public record struct EncounterPossible8b(EvoCriteria[] Chain, EncounterTypeGroup Flags, GameVersion Version, PKM Entity) : IEnumerator<IEncounterable>
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
        Trade,

        StaticVersion,
        StaticVersionBD,
        StaticVersionSP,
        StaticShared,

        SlotStart,
        SlotBD,
        SlotSP,
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
                if (!EncounterGenerator8b.TryGetEgg(Chain, Version, out var egg))
                    goto case YieldState.EventStart;
                State = YieldState.BredSplit;
                return SetCurrent(egg);
            case YieldState.BredSplit:
                if (!EncounterGenerator8b.TryGetSplit((EncounterEgg)Current, Chain, out egg))
                    goto case YieldState.EventStart;
                State = YieldState.EventStart;
                return SetCurrent(egg);

            case YieldState.EventStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Mystery))
                    goto case YieldState.TradeStart;
                State = YieldState.Event; goto case YieldState.Event;
            case YieldState.Event:
                if (TryGetNext(EncounterEvent.MGDB_G8B))
                    return true;
                Index = 0; State = YieldState.EventLocal; goto case YieldState.EventLocal;
            case YieldState.EventLocal:
                if (TryGetNext(EncounterEvent.EGDB_G8B))
                    return true;
                Index = 0; goto case YieldState.TradeStart;

            case YieldState.TradeStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Trade))
                    goto case YieldState.StaticVersion;
                State = YieldState.Trade; goto case YieldState.Trade;
            case YieldState.Trade:
                if (TryGetNext(Encounters8b.TradeGift_BDSP))
                    return true;
                { Index = 0; goto case YieldState.StaticVersion; }

            case YieldState.StaticVersion:
                if (!Flags.HasFlag(EncounterTypeGroup.Static))
                    goto case YieldState.SlotStart;
                if (Version == GameVersion.BD)
                { State = YieldState.StaticVersionBD; goto case YieldState.StaticVersionBD; }
                if (Version == GameVersion.SP)
                { State = YieldState.StaticVersionSP; goto case YieldState.StaticVersionSP; }
                throw new ArgumentOutOfRangeException(nameof(Version));

            case YieldState.StaticVersionBD:
                if (TryGetNext(Encounters8b.StaticBD))
                    return true;
                Index = 0; State = YieldState.StaticShared; goto case YieldState.StaticShared;
            case YieldState.StaticVersionSP:
                if (TryGetNext(Encounters8b.StaticSP))
                    return true;
                Index = 0; State = YieldState.StaticShared; goto case YieldState.StaticShared;

            case YieldState.StaticShared:
                if (TryGetNext(Encounters8b.Encounter_BDSP))
                    return true;
                Index = 0; goto case YieldState.SlotStart;

            case YieldState.SlotStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Slot))
                    goto case YieldState.SlotEnd;
                if (Version is GameVersion.BD)
                { State = YieldState.SlotBD; goto case YieldState.SlotBD; }
                if (Version is GameVersion.SP)
                { State = YieldState.SlotSP; goto case YieldState.SlotSP; }
                throw new ArgumentOutOfRangeException(nameof(Version));
            case YieldState.SlotBD:
                if (TryGetNext(Encounters8b.SlotsBD))
                    return true;
                goto case YieldState.SlotEnd;
            case YieldState.SlotSP:
                if (TryGetNext(Encounters8b.SlotsSP))
                    return true;
                goto case YieldState.SlotEnd;
            case YieldState.SlotEnd:
                break;
        }
        return false;
    }

    private bool TryGetNext(EncounterArea8b[] areas)
    {
        for (; Index < areas.Length; Index++, SubIndex = 0)
        {
            var area = areas[Index];
            if (TryGetNextSub(area.Slots))
                return true;
        }
        return false;
    }

    private bool TryGetNextSub(EncounterSlot8b[] slots)
    {
        while (SubIndex < slots.Length)
        {
            var enc = slots[SubIndex++];
            foreach (var evo in Chain)
            {
                if (enc.Species != evo.Species)
                    continue;
                if (enc.IsInvalidMunchlaxTree(Entity))
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
