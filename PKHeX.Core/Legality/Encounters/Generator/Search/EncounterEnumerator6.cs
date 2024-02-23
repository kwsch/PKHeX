using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Iterates to find potentially matched encounters for <see cref="GameVersion.Gen6"/>.
/// </summary>
public record struct EncounterEnumerator6(PKM Entity, EvoCriteria[] Chain, GameVersion Version) : IEnumerator<MatchedEncounter<IEncounterable>>
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
        BredTrade,
        BredSplit,
        BredSplitTrade,

        TradeStart,
        TradeXY,
        TradeAO,

        StartCaptures,

        SlotStart,
        SlotX,
        SlotY,
        SlotAS,
        SlotOR,
        SlotEnd,

        StaticStart,
        StaticAS,
        StaticOR,
        StaticSharedAO,
        StaticX,
        StaticY,
        StaticSharedXY,

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

                if (Entity.MetLocation == Locations.LinkTrade6NPC)
                    goto case YieldState.TradeStart;

                if (Entity.FatefulEncounter || Entity.MetLocation == Locations.LinkGift6)
                { State = YieldState.Event; goto case YieldState.Event; }
                goto case YieldState.Bred;

            case YieldState.Event:
                if (TryGetNext(EncounterEvent.MGDB_G6))
                    return true;
                Index = 0; State = YieldState.EventLocal; goto case YieldState.EventLocal;
            case YieldState.EventLocal:
                if (TryGetNext(EncounterEvent.EGDB_G6))
                    return true;
                if (Yielded)
                    break;
                Index = 0; goto case YieldState.Bred;

            case YieldState.Bred:
                if (!Locations.IsEggLocationBred6(Entity.EggLocation))
                    goto case YieldState.StartCaptures;
                if (!EncounterGenerator6.TryGetEgg(Chain, Version, out var egg))
                    break;
                State = YieldState.BredTrade;
                return SetCurrent(egg);
            case YieldState.BredTrade:
                State = YieldState.BredSplit;
                if (Entity.EggLocation != Locations.LinkTrade6)
                    goto case YieldState.BredSplit;
                egg = EncounterGenerator6.MutateEggTrade((EncounterEgg)Current.Encounter);
                return SetCurrent(egg);
            case YieldState.BredSplit:
                if (Chain[^1].Species is (int)Species.Togepi or (int)Species.Wynaut)
                    goto case YieldState.StartCaptures;
                State = YieldState.BredSplitTrade;
                if (!EncounterGenerator6.TryGetSplit((EncounterEgg)Current.Encounter, Chain, out egg))
                    break;
                return SetCurrent(egg);
            case YieldState.BredSplitTrade:
                State = YieldState.StartCaptures;
                if (Entity.EggLocation != Locations.LinkTrade6)
                    goto case YieldState.StartCaptures;
                egg = EncounterGenerator6.MutateEggTrade((EncounterEgg)Current.Encounter);
                return SetCurrent(egg);

            case YieldState.TradeStart:
                if (Version is GameVersion.X or GameVersion.Y)
                { State = YieldState.TradeXY; goto case YieldState.TradeXY; }
                if (Version is GameVersion.AS or GameVersion.OR)
                { State = YieldState.TradeAO; goto case YieldState.TradeAO; }
                throw new ArgumentOutOfRangeException(nameof(Version));
            case YieldState.TradeXY:
                if (TryGetNext(Encounters6XY.TradeGift_XY))
                    return true;
                if (Yielded)
                    break;
                Index = 0; goto case YieldState.StartCaptures;
            case YieldState.TradeAO:
                if (TryGetNext(Encounters6AO.TradeGift_AO))
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
                goto case YieldState.Fallback; // already checked everything else

            case YieldState.StaticStart:
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
        where TArea : class, IEncounterArea<TSlot>, IAreaLocation
        where TSlot : class, IEncounterable, IEncounterMatch
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

    private bool TryGetNextSub<T>(T[] slots) where T : class, IEncounterable, IEncounterMatch
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
