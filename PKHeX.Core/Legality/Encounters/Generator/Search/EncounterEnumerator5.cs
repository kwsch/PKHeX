using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Iterates to find potentially matched encounters for <see cref="GameVersion.Gen5"/>.
/// </summary>
public record struct EncounterEnumerator5(PKM Entity, EvoCriteria[] Chain, GameVersion Version) : IEnumerator<MatchedEncounter<IEncounterable>>
{
    private IEncounterable? Deferred;
    private int Index;
    private int SubIndex;
    private EncounterMatchRating Rating = EncounterMatchRating.MaxNotMatch;
    private bool Yielded;
    public MatchedEncounter<IEncounterable> Current { get; private set; }
    private YieldState State;
    private ushort met;
    readonly object IEnumerator.Current => Current;

    public readonly void Reset() => throw new NotSupportedException();
    public readonly void Dispose() { }
    public readonly IEnumerator<MatchedEncounter<IEncounterable>> GetEnumerator() => this;

    private enum YieldState : byte
    {
        Start,
        Event,
        EventLocal,
        Bred,
        BredSplit,

        TradeStart,
        TradeW,
        TradeB,
        TradeBW,
        TradeW2,
        TradeB2,
        TradeB2W2,

        StartCaptures,

        SlotStart,
        SlotW,
        SlotB,
        SlotW2,
        SlotB2,
        SlotEnd,

        StaticStart,
        StaticW,
        StaticB,
        StaticSharedBW,
        StaticEntreeBW,
        StaticW2,
        StaticB2,
        StaticN,
        StaticSharedB2W2,
        StaticEntreeB2W2,
        StaticRadar,
        StaticEntreeShared,

        Fallback,
        End,
    }

    public bool MoveNext()
    {
        switch (State)
        {
            case YieldState.Start:
                if (Chain.Length == 0)
                    break;

                if (Entity.MetLocation == Locations.LinkTrade5NPC)
                    goto case YieldState.TradeStart;
                if (!Entity.FatefulEncounter)
                    goto case YieldState.Bred;
                State = YieldState.Event; goto case YieldState.Event;

            case YieldState.Event:
                if (TryGetNext(EncounterEvent.MGDB_G5))
                    return true;
                Index = 0; State = YieldState.EventLocal; goto case YieldState.EventLocal;
            case YieldState.EventLocal:
                if (TryGetNext(EncounterEvent.EGDB_G5))
                    return true;
                if (Yielded)
                    break;
                Index = 0; goto case YieldState.Bred;

            case YieldState.Bred:
                if (!Locations.IsEggLocationBred5(Entity.EggLocation))
                    goto case YieldState.StartCaptures;
                if (!EncounterGenerator5.TryGetEgg(Chain, Version, out var egg))
                    goto case YieldState.StartCaptures;
                State = YieldState.BredSplit;
                return SetCurrent(egg);
            case YieldState.BredSplit:
                bool daycare = Entity.EggLocation == Locations.Daycare5;
                State = daycare ? YieldState.End : YieldState.StartCaptures;
                if (EncounterGenerator5.TryGetSplit((EncounterEgg)Current.Encounter, Chain, out egg))
                    return SetCurrent(egg);
                if (daycare)
                    break; // no other encounters
                goto case YieldState.StartCaptures;

            case YieldState.TradeStart:
                if (Version == GameVersion.W)
                { State = YieldState.TradeW; goto case YieldState.TradeW; }
                if (Version == GameVersion.B)
                { State = YieldState.TradeB; goto case YieldState.TradeB; }
                if (Version == GameVersion.W2)
                { State = YieldState.TradeW2; goto case YieldState.TradeW2; }
                if (Version == GameVersion.B2)
                { State = YieldState.TradeB2; goto case YieldState.TradeB2; }

                throw new ArgumentOutOfRangeException(nameof(Version));
            case YieldState.TradeW:
                if (TryGetNext(Encounters5BW.TradeGift_W))
                    return true;
                if (Yielded)
                    break;
                Index = 0; State = YieldState.TradeBW; goto case YieldState.TradeBW;
            case YieldState.TradeB:
                if (TryGetNext(Encounters5BW.TradeGift_B))
                    return true;
                if (Yielded)
                    break;
                Index = 0; State = YieldState.TradeBW; goto case YieldState.TradeBW;
            case YieldState.TradeW2:
                if (TryGetNext(Encounters5B2W2.TradeGift_W2))
                    return true;
                if (Yielded)
                    break;
                Index = 0; State = YieldState.TradeB2W2; goto case YieldState.TradeB2W2;
            case YieldState.TradeB2:
                if (TryGetNext(Encounters5B2W2.TradeGift_B2))
                    return true;
                if (Yielded)
                    break;
                Index = 0; State = YieldState.TradeB2W2; goto case YieldState.TradeB2W2;

            case YieldState.TradeBW:
                if (TryGetNext(Encounters5BW.TradeGift_BW))
                    return true;
                if (Yielded)
                    break;
                Index = 0; goto case YieldState.StartCaptures;
            case YieldState.TradeB2W2:
                if (TryGetNext(Encounters5B2W2.TradeGift_B2W2))
                    return true;
                if (Yielded)
                    break;
                Index = 0; goto case YieldState.StartCaptures;

            case YieldState.StartCaptures:
                InitializeWildLocationInfo();
                goto case YieldState.StaticStart;

            case YieldState.SlotStart:
                if (!EncounterStateUtil.CanBeWildEncounter(Entity))
                    goto case YieldState.SlotEnd;
                if (Version == GameVersion.W)
                { State = YieldState.SlotW; goto case YieldState.SlotW; }
                if (Version == GameVersion.B)
                { State = YieldState.SlotB; goto case YieldState.SlotB; }
                if (Version == GameVersion.W2)
                { State = YieldState.SlotW2; goto case YieldState.SlotW2; }
                if (Version == GameVersion.B2)
                { State = YieldState.SlotB2; goto case YieldState.SlotB2; }
                throw new ArgumentOutOfRangeException(nameof(Version));
            case YieldState.SlotW2:
                if (TryGetNext<EncounterArea5, EncounterSlot5>(Encounters5B2W2.SlotsW2))
                    return true;
                goto case YieldState.SlotEnd;
            case YieldState.SlotB2:
                if (TryGetNext<EncounterArea5, EncounterSlot5>(Encounters5B2W2.SlotsB2))
                    return true;
                goto case YieldState.SlotEnd;
            case YieldState.SlotW:
                if (TryGetNext<EncounterArea5, EncounterSlot5>(Encounters5BW.SlotsW))
                    return true;
                goto case YieldState.SlotEnd;
            case YieldState.SlotB:
                if (TryGetNext<EncounterArea5, EncounterSlot5>(Encounters5BW.SlotsB))
                    return true;
                goto case YieldState.SlotEnd;
            case YieldState.SlotEnd:
                goto case YieldState.Fallback; // already checked everything else

            case YieldState.StaticStart:
                if (Version == GameVersion.W)
                { State = YieldState.StaticW; goto case YieldState.StaticW; }
                if (Version == GameVersion.B)
                { State = YieldState.StaticB; goto case YieldState.StaticB; }
                if (Version == GameVersion.W2)
                { State = YieldState.StaticW2; goto case YieldState.StaticW2; }
                if (Version == GameVersion.B2)
                { State = YieldState.StaticB2; goto case YieldState.StaticB2; }
                throw new ArgumentOutOfRangeException(nameof(Version));

            case YieldState.StaticW:
                if (TryGetNext(Encounters5BW.StaticW))
                    return true;
                Index = 0; State = YieldState.StaticSharedBW; goto case YieldState.StaticSharedBW;
            case YieldState.StaticB:
                if (TryGetNext(Encounters5BW.StaticB))
                    return true;
                Index = 0; State = YieldState.StaticSharedBW; goto case YieldState.StaticSharedBW;
            case YieldState.StaticSharedBW:
                if (TryGetNext(Encounters5BW.Encounter_BW))
                    return true;
                Index = 0; State = YieldState.StaticEntreeBW; goto case YieldState.StaticEntreeBW;
            case YieldState.StaticEntreeBW:
                if (TryGetNext(Encounters5BW.DreamWorld_BW))
                    return true;
                Index = 0; State = YieldState.StaticEntreeShared; goto case YieldState.StaticEntreeShared;

            case YieldState.StaticW2:
                if (TryGetNext(Encounters5B2W2.StaticW2))
                    return true;
                Index = 0; State = YieldState.StaticSharedB2W2; goto case YieldState.StaticSharedB2W2;
            case YieldState.StaticB2:
                if (TryGetNext(Encounters5B2W2.StaticB2))
                    return true;
                Index = 0; State = YieldState.StaticSharedB2W2; goto case YieldState.StaticSharedB2W2;
            case YieldState.StaticSharedB2W2:
                if (TryGetNext(Encounters5B2W2.Encounter_B2W2_Regular))
                    return true;
                Index = 0; State = YieldState.StaticN; goto case YieldState.StaticN;
            case YieldState.StaticN:
                if (TryGetNext(Encounters5B2W2.Encounter_B2W2_N))
                    return true;
                Index = 0; State = YieldState.StaticEntreeB2W2; goto case YieldState.StaticEntreeB2W2;
            case YieldState.StaticEntreeB2W2:
                if (TryGetNext(Encounters5B2W2.DreamWorld_B2W2))
                    return true;
                Index = 0; State = YieldState.StaticRadar; goto case YieldState.StaticRadar;
            case YieldState.StaticRadar:
                if (TryGetNext(Encounters5DR.Encounter_DreamRadar))
                    return true;
                Index = 0; State = YieldState.StaticEntreeShared; goto case YieldState.StaticEntreeShared;

            case YieldState.StaticEntreeShared:
                if (TryGetNext(Encounters5DR.DreamWorld_Common))
                    return true;
                Index = 0; goto case YieldState.SlotStart;

            case YieldState.Fallback:
                State = YieldState.End;
                if (Deferred != null)
                    return SetCurrent(Deferred, Rating);
                break;
        }
        return false;
    }

    private void InitializeWildLocationInfo()
    {
        met = Entity.MetLocation;
    }

    private bool TryGetNext<TArea, TSlot>(TArea[] areas)
        where TArea : IEncounterArea<TSlot>, IAreaLocation
        where TSlot : IEncounterable, IEncounterMatch
    {
        for (; Index < areas.Length; Index++, SubIndex = 0)
        {
            var area = areas[Index];
            if (!area.IsMatchLocation(met))
                continue;
            if (TryGetNextSub(area.Slots))
                return true;
        }
        return false;
    }

    private bool TryGetNextSub<T>(T[] slots)
        where T : IEncounterable, IEncounterMatch
    {
        while (SubIndex < slots.Length)
        {
            var enc = slots[SubIndex++];
            foreach (var evo in Chain)
            {
                if (enc.Species != evo.Species)
                    continue;
                if (!enc.IsMatchExact(Entity, evo))
                    break;

                var rating = enc.GetMatchRating(Entity);
                if (rating == EncounterMatchRating.Match)
                    return SetCurrent(enc);

                if (rating < Rating)
                {
                    Deferred = enc;
                    Rating = rating;
                }
                break;
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
                if (!enc.IsMatchExact(Entity, evo))
                    break;
                var rating = enc.GetMatchRating(Entity);
                if (rating == EncounterMatchRating.Match)
                    return SetCurrent(enc);

                if (rating < Rating)
                {
                    Deferred = enc;
                    Rating = rating;
                }
                break;
            }
        }
        return false;
    }

    private bool SetCurrent<T>(T enc, EncounterMatchRating rating = EncounterMatchRating.Match) where T : IEncounterable
    {
        Current = new MatchedEncounter<IEncounterable>(enc, rating);
        Yielded = true;
        return true;
    }
}
