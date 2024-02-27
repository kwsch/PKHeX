using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Iterates to find potentially matched encounters for <see cref="GameVersion.SV"/> encounters while in the <see cref="PK8"/> format.
/// </summary>
public record struct EncounterEnumerator9SWSH(PKM Entity, EvoCriteria[] Chain, GameVersion Version) : IEnumerator<MatchedEncounter<IEncounterable>>
{
    private IEncounterable? Deferred;
    private int Index;
    private int SubIndex;
    private EncounterMatchRating Rating = EncounterMatchRating.MaxNotMatch;
    private bool Yielded;
    public MatchedEncounter<IEncounterable> Current { get; private set; }
    private YieldState State;
    //private ushort met;
    private bool mustBeSlot;
    readonly object IEnumerator.Current => Current;

    public readonly void Reset() => throw new NotSupportedException();
    public readonly void Dispose() { }
    public readonly IEnumerator<MatchedEncounter<IEncounterable>> GetEnumerator() => this;

    private enum YieldState : byte
    {
        Start,
        Event,
        Bred,
        TradeStart,
        Trade,

        StartCaptures,

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

                if (!Entity.FatefulEncounter)
                    goto case YieldState.Bred;
                State = YieldState.Event; goto case YieldState.Event;

            case YieldState.Event:
                if (TryGetNext(EncounterEvent.MGDB_G9))
                    return true;
                if (Yielded)
                    break;
                Index = 0; goto case YieldState.Bred;

            case YieldState.Bred:
                State = YieldState.TradeStart;
                if (WasBredEggSWSH() && EncounterGenerator9.TryGetEgg(Entity, Chain, Version, out var egg))
                    return SetCurrent(egg);
                goto case YieldState.TradeStart;

            case YieldState.TradeStart:
                //if (Entity.MetLocation == Locations.LinkTrade6NPC)
                //    goto case YieldState.Trade;
                goto case YieldState.StartCaptures;
            case YieldState.Trade:
                if (TryGetNext(Encounters9.TradeGift_SV))
                    return true;
                if (Yielded)
                    break;
                Index = 0; goto case YieldState.StartCaptures;

            case YieldState.StartCaptures:
                InitializeWildLocationInfo();
                if (mustBeSlot)
                    goto case YieldState.SlotStart;
                goto case YieldState.StaticVersion;

            case YieldState.SlotStart:
                if (!EncounterStateUtil.CanBeWildEncounter(Entity))
                    goto case YieldState.SlotEnd;
                State = YieldState.Slot; goto case YieldState.Slot;
            case YieldState.Slot:
                if (TryGetNext<EncounterArea9, EncounterSlot9>(Encounters9.Slots))
                    return true;
                goto case YieldState.SlotEnd;
            case YieldState.SlotEnd:
                if (!mustBeSlot)
                    goto case YieldState.Fallback; // already checked everything else
                goto case YieldState.StaticVersion;

            case YieldState.StaticVersion:
                if (Version == GameVersion.SL)
                { State = YieldState.StaticVersionSL; goto case YieldState.StaticVersionSL; }
                if (Version == GameVersion.VL)
                { State = YieldState.StaticVersionVL; goto case YieldState.StaticVersionVL; }
                goto case YieldState.Fallback; // already checked everything else

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

    private readonly bool WasBredEggSWSH() => Entity.MetLevel == EggStateLegality.EggMetLevel && Entity.EggLocation switch
    {
        LocationsHOME.SWSHEgg => true, // Regular hatch location (not link trade)
        LocationsHOME.SWSL => Entity.MetLocation == LocationsHOME.SWSL, // Link Trade transferred over must match Met Location
        LocationsHOME.SHVL => Entity.MetLocation == LocationsHOME.SHVL, // Link Trade transferred over must match Met Location
        _ => false,
    };

    private void InitializeWildLocationInfo()
    {
        mustBeSlot = Entity is IRibbonIndex r && r.HasEncounterMark();
        //met = Entity.MetLocation;
    }

    private bool TryGetNext<TArea, TSlot>(TArea[] areas)
        where TArea : class, IEncounterArea<TSlot>, IAreaLocation
        where TSlot : class, IEncounterable, IEncounterMatch
    {
        for (; Index < areas.Length; Index++, SubIndex = 0)
        {
            var area = areas[Index];
            //if (!area.IsMatchLocation(met))
            //    continue;
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
