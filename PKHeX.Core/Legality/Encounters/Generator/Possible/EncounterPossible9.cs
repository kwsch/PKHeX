using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Iterates to find possible encounters for <see cref="GameVersion.SV"/> encounters.
/// </summary>
public record struct EncounterPossible9(EvoCriteria[] Chain, EncounterTypeGroup Flags, GameVersion Version) : IEnumerator<IEncounterable>
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
        TradeStart,
        Trade,

        StaticStart,

        SlotStart,
        Slot,
        SlotEnd,

        StaticVersion,
        StaticVersionSL,
        StaticVersionVL,
        StaticShared,
        StaticFixed,
        StaticTeraBase,
        StaticTeraDLC1,
        StaticTeraDLC2,
        StaticDist,
        StaticOutbreak,
        StaticMight,
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
                if (TryGetNext(EncounterEvent.MGDB_G9))
                    return true;
                Index = 0; State = YieldState.EventLocal; goto case YieldState.EventLocal;
            case YieldState.EventLocal:
                if (TryGetNext(EncounterEvent.EGDB_G9))
                    return true;
                Index = 0; goto case YieldState.TradeStart;

            case YieldState.TradeStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Trade))
                    goto case YieldState.StaticStart;
                State = YieldState.Trade; goto case YieldState.Trade;
            case YieldState.Trade:
                if (TryGetNext(Encounters9.TradeGift_SV))
                    return true;
                Index = 0; goto case YieldState.StaticStart;

            case YieldState.StaticStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Static))
                    goto case YieldState.SlotStart;
                goto case YieldState.StaticVersion;
            case YieldState.StaticVersion:
                if (Version == GameVersion.SL)
                { State = YieldState.StaticVersionSL; goto case YieldState.StaticVersionSL; }
                if (Version == GameVersion.VL)
                { State = YieldState.StaticVersionVL; goto case YieldState.StaticVersionVL; }
                break;

            case YieldState.StaticVersionSL:
                if (TryGetNext(Encounters9.StaticSL))
                    return true;
                Index = 0; State = YieldState.StaticShared; goto case YieldState.StaticShared;
            case YieldState.StaticVersionVL:
                if (TryGetNext(Encounters9.StaticVL))
                    return true;
                Index = 0; State = YieldState.StaticShared; goto case YieldState.StaticShared;
            case YieldState.StaticShared:
                if (TryGetNext(Encounters9.Encounter_SV))
                    return true;
                Index = 0; State = YieldState.StaticFixed; goto case YieldState.StaticFixed;

            case YieldState.StaticFixed:
                if (TryGetNext(Encounters9.Fixed))
                    return true;
                Index = 0; State = YieldState.StaticTeraBase; goto case YieldState.StaticTeraBase;
            case YieldState.StaticTeraBase:
                if (TryGetNext(Encounters9.TeraBase))
                    return true;
                Index = 0; State = YieldState.StaticTeraDLC1; goto case YieldState.StaticTeraDLC1;
            case YieldState.StaticTeraDLC1:
                if (TryGetNext(Encounters9.TeraDLC1))
                    return true;
                Index = 0; State = YieldState.StaticTeraDLC2; goto case YieldState.StaticTeraDLC2;
            case YieldState.StaticTeraDLC2:
                if (TryGetNext(Encounters9.TeraDLC2))
                    return true;
                Index = 0; State = YieldState.StaticDist; goto case YieldState.StaticDist;
            case YieldState.StaticDist:
                if (TryGetNext(Encounters9.Dist))
                    return true;
                Index = 0; State = YieldState.StaticOutbreak; goto case YieldState.StaticOutbreak;
            case YieldState.StaticOutbreak:
                if (TryGetNext(Encounters9.Outbreak))
                    return true;
                Index = 0; State = YieldState.StaticMight; goto case YieldState.StaticMight;
            case YieldState.StaticMight:
                if (TryGetNext(Encounters9.Might))
                    return true;
                Index = 0; goto case YieldState.StaticEnd;
            case YieldState.StaticEnd:
                goto case YieldState.SlotStart;

            case YieldState.SlotStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Slot))
                    goto case YieldState.Bred;
                goto case YieldState.Slot;
            case YieldState.Slot:
                if (TryGetNext<EncounterArea9, EncounterSlot9>(Encounters9.Slots))
                    return true;
                goto case YieldState.SlotEnd;
            case YieldState.SlotEnd:
                goto case YieldState.Bred;

            case YieldState.Bred:
                if (!Flags.HasFlag(EncounterTypeGroup.Egg))
                    break;
                State = YieldState.End;
                if (EncounterGenerator9.TryGetEgg(Chain, Version, out var egg))
                    return SetCurrent(egg);
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
