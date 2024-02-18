using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace PKHeX.Core;

/// <summary>
/// Iterates to find potentially matched encounters for <see cref="GameVersion.BDSP"/> encounters while in the <see cref="PK8"/> format.
/// </summary>
public record struct EncounterEnumerator8bSWSH(PKM Entity, EvoCriteria[] Chain, GameVersion Version) : IEnumerator<MatchedEncounter<IEncounterable>>
{
    private IEncounterable? Deferred;
    private int Index;
    private int SubIndex;
    private EncounterMatchRating Rating = EncounterMatchRating.MaxNotMatch;
    private bool Yielded;
    public MatchedEncounter<IEncounterable> Current { get; private set; }
    private YieldState State;
    private ushort met;
    private bool hasOriginalLocation;
    private bool mustBeWild;
    readonly object IEnumerator.Current => Current;

    public readonly void Reset() => throw new NotSupportedException();
    public readonly void Dispose() { }
    public readonly IEnumerator<MatchedEncounter<IEncounterable>> GetEnumerator() => this;

    private enum YieldState : byte
    {
        Start,
        Event,
        Bred,
        BredSplit,
        TradeStart,
        Trade,

        StartCaptures,

        SlotStart,
        SlotBD,
        SlotSP,
        SlotEnd,

        StaticVersion,
        StaticVersionBD,
        StaticVersionSP,
        StaticShared,

        Fallback,
        End,
    }

    public bool MoveNext()
    {
        switch (State)
        {
            case YieldState.Start:
                Debug.Assert(Entity is PK8);
                if (Chain.Length == 0)
                    break;

                if (!Entity.FatefulEncounter)
                    goto case YieldState.Bred;
                State = YieldState.Event; goto case YieldState.Event;

            case YieldState.Event:
                if (TryGetNext(EncounterEvent.MGDB_G8B))
                    return true;
                if (Yielded)
                    break;
                Index = 0; goto case YieldState.Bred;

            case YieldState.Bred:
                if (!WasBredEggBDSP())
                    goto case YieldState.TradeStart;
                if (!EncounterGenerator8b.TryGetEgg(Chain, Version, out var egg))
                    goto case YieldState.TradeStart;
                State = YieldState.BredSplit;
                return SetCurrent(egg);
            case YieldState.BredSplit:
                if (!EncounterGenerator8b.TryGetSplit((EncounterEgg)Current.Encounter, Chain, out egg))
                    goto case YieldState.TradeStart;
                State = YieldState.End;
                return SetCurrent(egg);

            case YieldState.TradeStart:
                goto case YieldState.Trade;
            case YieldState.Trade:
                if (TryGetNext(Encounters8b.TradeGift_BDSP))
                { State = YieldState.End; return true; }
                Index = 0; goto case YieldState.StartCaptures;

            case YieldState.StartCaptures:
                InitializeWildLocationInfo();
                if (mustBeWild)
                    goto case YieldState.SlotStart;
                goto case YieldState.StaticVersion;

            case YieldState.SlotStart:
                if (!EncounterStateUtil.CanBeWildEncounter(Entity))
                    goto case YieldState.SlotEnd;
                if (Version is GameVersion.BD)
                { State = YieldState.SlotBD; goto case YieldState.SlotBD; }
                if (Version is GameVersion.SP)
                { State = YieldState.SlotSP; goto case YieldState.SlotSP; }
                throw new ArgumentOutOfRangeException(nameof(Version));
            case YieldState.SlotBD:
                if (TryGetNext<EncounterArea8b, EncounterSlot8b>(Encounters8b.SlotsBD))
                    return true;
                goto case YieldState.SlotEnd;
            case YieldState.SlotSP:
                if (TryGetNext<EncounterArea8b, EncounterSlot8b>(Encounters8b.SlotsSP))
                    return true;
                goto case YieldState.SlotEnd;
            case YieldState.SlotEnd:
                if (!mustBeWild)
                    goto case YieldState.Fallback; // already checked everything else
                goto case YieldState.StaticVersion;

            case YieldState.StaticVersion:
                if (Version == GameVersion.BD)
                { State = YieldState.StaticVersionBD; goto case YieldState.StaticVersionBD; }
                if (Version == GameVersion.SP)
                { State = YieldState.StaticVersionSP; goto case YieldState.StaticVersionSP; }
                goto case YieldState.Fallback; // already checked everything else

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
                if (mustBeWild)
                    goto case YieldState.Fallback; // already checked everything else
                Index = 0; goto case YieldState.SlotStart;

            case YieldState.Fallback:
                State = YieldState.End;
                if (Deferred != null)
                    return SetCurrent(Deferred, Rating);
                break;
            case YieldState.End:
                return false;
        }
        return false;
    }

    private readonly bool WasBredEggBDSP() => Entity.MetLevel == EggStateLegality.EggMetLevel && Entity.EggLocation switch
    {
        LocationsHOME.SWSHEgg => true, // Regular hatch location (not link trade)
        LocationsHOME.SWBD => Entity.MetLocation == LocationsHOME.SWBD, // Link Trade transferred over must match Met Location
        LocationsHOME.SHSP => Entity.MetLocation == LocationsHOME.SHSP, // Link Trade transferred over must match Met Location
        _ => false,
    };

    private void InitializeWildLocationInfo()
    {
        mustBeWild = Entity.Ball == (byte)Ball.Safari;
        met = Entity.MetLocation;
        var location = met;
        var remap = LocationsHOME.GetRemapState(EntityContext.Gen8b, Entity.Context);
        hasOriginalLocation = true;
        if (remap.HasFlag(LocationRemapState.Remapped))
            hasOriginalLocation = location != LocationsHOME.GetMetSWSH(location, Version);
    }

    private bool TryGetNext<TArea, TSlot>(TArea[] areas)
        where TArea : class, IEncounterArea<TSlot>, IAreaLocation
        where TSlot : class, IEncounterable, IEncounterMatch
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
