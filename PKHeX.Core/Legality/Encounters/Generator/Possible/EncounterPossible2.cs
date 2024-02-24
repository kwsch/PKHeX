using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Iterates to find possible encounters for <see cref="GameVersion.Gen2"/> encounters.
/// </summary>
public record struct EncounterPossible2(EvoCriteria[] Chain, EncounterTypeGroup Flags, GameVersion Version, PKM Entity) : IEnumerator<IEncounterable>
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
        BredCrystal,

        TradeStart,
        Trade,

        EventStart,
        EventVC,
        EventGB,

        StaticStart,
        StaticCOdd,
        StaticC,
        StaticGD,
        StaticSI,
        StaticGS,
        StaticShared,

        SlotStart,
        SlotC,
        SlotGD,
        SlotSI,
        End,
    }

    public bool MoveNext()
    {
        switch (State)
        {
            case YieldState.Start:
                if (Chain.Length == 0)
                    break;
                if (!Flags.HasFlag(EncounterTypeGroup.Egg))
                    goto case YieldState.TradeStart;
                goto case YieldState.Bred;

            case YieldState.Bred:
                // try with specific version, for yielded metadata purposes.
                var version = Version is GameVersion.GD or GameVersion.SI ? Version : GameVersion.GS;
                if (!EncounterGenerator2.TryGetEgg(Chain, version, out var egg))
                    goto case YieldState.TradeStart;
                State = ParseSettings.AllowGen2Crystal(Entity) ? YieldState.BredCrystal : YieldState.TradeStart;
                return SetCurrent(egg);
            case YieldState.BredCrystal:
                State = YieldState.TradeStart;
                if (!EncounterGenerator2.TryGetEgg(Chain, GameVersion.C, out egg))
                    goto case YieldState.TradeStart;
                return SetCurrent(egg);

            case YieldState.TradeStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Trade))
                    goto case YieldState.StaticStart;
                State = YieldState.Trade; goto case YieldState.Trade;
            case YieldState.Trade:
                if (TryGetNext(Encounters2.TradeGift_GSC))
                    return true;
                Index = 0; goto case YieldState.StaticStart;

            case YieldState.StaticStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Static))
                    goto case YieldState.SlotStart;
                if (Version is GameVersion.C or GameVersion.GSC)
                {
                    if (ParseSettings.AllowGen2OddEgg(Entity))
                    { State = YieldState.StaticCOdd; goto case YieldState.StaticCOdd; }
                    State = YieldState.StaticC; goto case YieldState.StaticC;
                }
                if (Version is GameVersion.GD or GameVersion.GS)
                { State = YieldState.StaticGD; goto case YieldState.StaticGD; }
                if (Version == GameVersion.SI)
                { State = YieldState.StaticSI; goto case YieldState.StaticSI; }
                throw new ArgumentOutOfRangeException(nameof(Version));
            case YieldState.StaticCOdd:
                if (TryGetNext(Encounters2.StaticOddEggC))
                    return true;
                Index = 0; State = YieldState.StaticC; goto case YieldState.StaticC;
            case YieldState.StaticC:
                if (TryGetNext(Encounters2.StaticC))
                    return true;
                Index = 0;
                if (Version == GameVersion.C)
                { State = YieldState.StaticShared; goto case YieldState.StaticShared; }
                State = YieldState.StaticGD; goto case YieldState.StaticGD;
            case YieldState.StaticGD:
                if (TryGetNext(Encounters2.StaticGD))
                    return true;
                Index = 0;
                if (Version == GameVersion.GD)
                { State = YieldState.StaticGS; goto case YieldState.StaticGS; }
                State = YieldState.StaticSI; goto case YieldState.StaticSI;
            case YieldState.StaticSI:
                if (TryGetNext(Encounters2.StaticSI))
                    return true;
                Index = 0; State = YieldState.StaticGS; goto case YieldState.StaticGS;
            case YieldState.StaticGS:
                if (TryGetNext(Encounters2.StaticGS))
                    return true;
                Index = 0; State = YieldState.StaticShared; goto case YieldState.StaticShared;
            case YieldState.StaticShared:
                if (TryGetNext(Encounters2.StaticGSC))
                    return true;
                Index = 0; goto case YieldState.SlotStart;

            case YieldState.SlotStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Slot))
                    goto case YieldState.EventStart;
                if (Version is GameVersion.C or GameVersion.GSC)
                { State = YieldState.SlotC; goto case YieldState.SlotC; }
                if (Version is GameVersion.GD or GameVersion.GS)
                { State = YieldState.SlotGD; goto case YieldState.SlotGD; }
                if (Version == GameVersion.SI)
                { State = YieldState.SlotSI; goto case YieldState.SlotSI; }
                throw new ArgumentOutOfRangeException(nameof(Version));
            case YieldState.SlotC:
                if (TryGetNextSlot(Encounters2.SlotsC))
                    return true;
                Index = 0;
                if (Version == GameVersion.C)
                    goto case YieldState.EventStart;
                State = YieldState.SlotGD; goto case YieldState.SlotGD;
            case YieldState.SlotGD:
                if (TryGetNextSlot(Encounters2.SlotsGD))
                    return true;
                Index = 0;
                if (Version == GameVersion.GD)
                    goto case YieldState.EventStart;
                State = YieldState.SlotSI; goto case YieldState.SlotSI;
            case YieldState.SlotSI:
                if (TryGetNextSlot(Encounters2.SlotsSI))
                    return true;
                Index = 0; goto case YieldState.EventStart;

            case YieldState.EventStart:
                if (!Flags.HasFlag(EncounterTypeGroup.Mystery))
                    break;
                if (ParseSettings.AllowGBVirtualConsole3DS)
                { State = YieldState.EventVC; goto case YieldState.EventVC; }
                if (ParseSettings.AllowGBEraEvents)
                { State = YieldState.EventGB; goto case YieldState.EventGB; }
                throw new InvalidOperationException("No events allowed");
            case YieldState.EventVC:
                State = YieldState.End;
                if (Chain[^1].Species == (int)Species.Celebi && Version == GameVersion.C)
                    return SetCurrent(Encounters2.CelebiVC);
                break;
            case YieldState.EventGB:
                if (TryGetNext(Encounters2GBEra.Gifts))
                    return true;
                break;
        }
        return false;
    }

    private bool TryGetNextSlot<TArea>(TArea[] areas)
        where TArea : class, IEncounterArea<EncounterSlot2>
    {
        for (; Index < areas.Length; Index++, SubIndex = 0)
        {
            var area = areas[Index];
            if (TryGetNextSub(area.Slots))
                return true;
        }
        return false;
    }

    private bool TryGetNextSub(EncounterSlot2[] slots)
    {
        while (SubIndex < slots.Length)
        {
            var enc = slots[SubIndex++];
            foreach (var evo in Chain)
            {
                if (enc.Species != evo.Species)
                    continue;

                if (enc.IsHeadbutt && !enc.IsTreeAvailable(Entity.TID16))
                    break;
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
