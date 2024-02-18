using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Iterates to find potentially matched encounters for <see cref="GameVersion.SWSH"/>.
/// </summary>
public record struct EncounterEnumerator8(PKM Entity, EvoCriteria[] Chain, GameVersion Version) : IEnumerator<MatchedEncounter<IEncounterable>>
{
    private IEncounterable? Deferred;
    private int Index;
    private int SubIndex;
    private EncounterMatchRating Rating = EncounterMatchRating.MaxNotMatch;
    private bool Yielded;
    public MatchedEncounter<IEncounterable> Current { get; private set; }
    private YieldState State;
    private ushort met;
    private bool mustBeSlot;
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
        TradeSW,
        TradeSH,
        TradeShared,

        StartCaptures,

        SlotStart,
        SlotSW,
        SlotSH,
        SlotSWHidden,
        SlotSHHidden,
        SlotEnd,

        StaticStart,
        NestSW, NestSH, DistSW, DistSH, DynamaxAdv, Crystal,
        StaticVersion,
        StaticVersionSW,
        StaticVersionSH,
        StaticShared,

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
                if (!Entity.FatefulEncounter)
                    goto case YieldState.Bred;
                State = YieldState.Event; goto case YieldState.Event;

            case YieldState.Event:
                if (TryGetNext(EncounterEvent.MGDB_G8))
                    return true;
                Index = 0; State = YieldState.EventLocal; goto case YieldState.EventLocal;
            case YieldState.EventLocal:
                if (TryGetNext(EncounterEvent.EGDB_G8))
                    return true;
                if (Yielded)
                    break;
                Index = 0; goto case YieldState.Bred;

            case YieldState.Bred:
                if (!Locations.IsEggLocationBred6(Entity.EggLocation))
                    goto case YieldState.StartCaptures;
                if (!EncounterGenerator8.TryGetEgg(Chain, Version, out var egg))
                    goto case YieldState.StartCaptures;
                State = YieldState.BredSplit;
                return SetCurrent(egg);
            case YieldState.BredSplit:
                State = YieldState.End;
                if (EncounterGenerator8.TryGetSplit((EncounterEgg)Current.Encounter, Chain, out egg))
                    return SetCurrent(egg);
                break;

            case YieldState.TradeStart:
                if (Version == GameVersion.SW)
                { State = YieldState.TradeSW; goto case YieldState.TradeSW; }
                if (Version == GameVersion.SH)
                { State = YieldState.TradeSH; goto case YieldState.TradeSH; }
                break;
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
                if (Yielded)
                    break;
                Index = 0; goto case YieldState.StartCaptures;

            case YieldState.StartCaptures:
                InitializeWildLocationInfo();
                if (mustBeSlot)
                    goto case YieldState.SlotStart;
                goto case YieldState.StaticStart;

            case YieldState.SlotStart:
                if (!EncounterStateUtil.CanBeWildEncounter(Entity))
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
                if (!mustBeSlot)
                    goto case YieldState.Fallback; // already checked everything else
                goto case YieldState.StaticStart;

            case YieldState.StaticStart:
                goto case YieldState.NestSW;

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
                Index = 0; State = YieldState.StaticVersion; goto case YieldState.StaticVersion;

            case YieldState.StaticVersion:
                if (Version == GameVersion.SW)
                { State = YieldState.StaticVersionSW; goto case YieldState.StaticVersionSW; }
                if (Version == GameVersion.SH)
                { State = YieldState.StaticVersionSH; goto case YieldState.StaticVersionSH; }
                goto case YieldState.Fallback; // already checked everything else

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
                if (mustBeSlot)
                    goto case YieldState.Fallback; // already checked everything else
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
        mustBeSlot = Entity is IRibbonIndex r && r.HasEncounterMark();
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

    private bool TryGetNextSub<T>(T[] slots)
        where T : class, IEncounterable, IEncounterMatch
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
