using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Iterates to find potentially matched encounters for <see cref="GameVersion.CXD"/>.
/// </summary>
public record struct EncounterEnumerator3GC(PKM Entity, EvoCriteria[] Chain) : IEnumerator<MatchedEncounter<IEncounterable>>
{
    private IEncounterable? Deferred;
    private int Index;
    private int SubIndex;
    private EncounterMatchRating Rating = EncounterMatchRating.MaxNotMatch;
    public MatchedEncounter<IEncounterable> Current { get; private set; }
    private YieldState State;
    private ushort met;
    private bool hasOriginalLocation;

    readonly object IEnumerator.Current => Current;

    public readonly void Reset() => throw new NotSupportedException();
    public readonly void Dispose() { }
    public readonly IEnumerator<MatchedEncounter<IEncounterable>> GetEnumerator() => this;

    private enum YieldState : byte
    {
        Start,

        Trade,

        StaticColo,
        StaticColoStarters,
        StaticColoGift,
        StaticXDShadow,
        StaticXDGift,

        SlotXD,
        StaticEReader,

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
                if (!EncounterStateUtil.CanBeWildEncounter(Entity))
                    break;
                State = YieldState.Trade; goto case YieldState.Trade;

            case YieldState.Trade:
                if (TryGetNext(Encounters3XD.Trades))
                    return true;
                Index = 0; State = YieldState.StaticColo; goto case YieldState.StaticColo;

            case YieldState.StaticColo:
                if (TryGetNext(Encounters3Colo.Shadow))
                    return true;
                Index = 0; State = YieldState.StaticColoStarters; goto case YieldState.StaticColoStarters;
            case YieldState.StaticColoStarters:
                if (TryGetNext(Encounters3Colo.Starters))
                    return true;
                Index = 0; State = YieldState.StaticColoGift; goto case YieldState.StaticColoGift;
            case YieldState.StaticColoGift:
                if (TryGetNext(Encounters3Colo.Gifts))
                    return true;
                Index = 0; State = YieldState.StaticXDShadow; goto case YieldState.StaticXDShadow;

            case YieldState.StaticXDShadow:
                if (TryGetNext(Encounters3XD.Shadow))
                    return true;
                Index = 0; State = YieldState.StaticXDGift; goto case YieldState.StaticXDGift;
            case YieldState.StaticXDGift:
                if (TryGetNext(Encounters3XD.Gifts))
                    return true;
                Index = 0; State = YieldState.StaticEReader; goto case YieldState.StaticEReader;
            case YieldState.StaticEReader:
                State = YieldState.Fallback;
                if (Entity.Japanese && TryGetNext(Encounters3Colo.EReader))
                    return true;
                Index = 0; State = YieldState.SlotXD; goto case YieldState.SlotXD;

            case YieldState.SlotXD:
                InitializeWildLocationInfo();
                if (TryGetNext<EncounterArea3XD, EncounterSlot3XD>(Encounters3XD.Slots))
                    return true;
                goto case YieldState.Fallback;

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
        hasOriginalLocation = Entity.Format == 3;
    }

    private bool TryGetNext<TArea, TSlot>(TArea[] areas)
        where TArea : IEncounterArea<TSlot>, IAreaLocation
        where TSlot : IEncounterable, IEncounterMatch
    {
        for (; Index < areas.Length; Index++, SubIndex = 0)
        {
            var area = areas[Index];
            if (hasOriginalLocation && !area.IsMatchLocation(met))
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
        return true;
    }
}
