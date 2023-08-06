using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace PKHeX.Core;

public record struct EncounterEnumerator8b(PKM Entity, EvoCriteria[] Chain, GameVersion Version) : IEnumerator<MatchedEncounter<IEncounterable>>
{
    private IEncounterable? Deferred;
    private int Index;
    private int SubIndex;
    private EncounterMatchRating Rating = EncounterMatchRating.MaxNotMatch;
    private bool Yielded;
    public MatchedEncounter<IEncounterable> Current { get; private set; }
    private YieldState State;
    private int met;
    private bool hasOriginalLocation;
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
        BredSplit,
        TradeStart,
        Trade,

        StartCaptures,

        StartSlot,
        SlotBD,
        SlotSP,
        EndSlot,

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
                Debug.Assert(Entity is not PK8);
                if (Chain.Length == 0)
                    break;

                if (!Entity.FatefulEncounter)
                { State = YieldState.Bred; goto case YieldState.Bred; }
                State = YieldState.Event; goto case YieldState.Event;

            case YieldState.Event:
                if (TryGetNext(EncounterEvent.MGDB_G8B, out var gift))
                    return SetCurrent(gift);
                if (Yielded)
                    break;
                Index = 0; State = YieldState.Bred; goto case YieldState.Bred;

            case YieldState.Bred:
                if (!Locations.IsEggLocationBred8b(Entity.Egg_Location))
                { State = YieldState.TradeStart; goto case YieldState.TradeStart; }
                if (!EncounterGenerator8b.TryGetEgg(Chain, Version, out var egg))
                { State = YieldState.TradeStart; goto case YieldState.TradeStart; }
                State = YieldState.BredSplit;
                return SetCurrent(new(egg, EncounterMatchRating.Match));
            case YieldState.BredSplit:
                State = YieldState.End;
                if (EncounterGenerator8b.TryGetSplit((EncounterEgg)Current.Encounter, Chain, out egg))
                    return SetCurrent(new(egg, EncounterMatchRating.Match));
                break;

            case YieldState.TradeStart:
                if (Entity.Met_Location == Locations.LinkTrade6NPC)
                    goto case YieldState.Trade;
                goto case YieldState.StartCaptures;
            case YieldState.Trade:
                if (!TryGetNext(Encounters8b.TradeGift_BDSP, out var trade))
                { Index = 0; State = YieldState.StartCaptures; goto case YieldState.StartCaptures; }
                State = YieldState.End;
                return SetCurrent(trade);

            case YieldState.StartCaptures:
                InitializeWildLocationInfo();
                if (mustBeSlot)
                    goto case YieldState.StartSlot;
                goto case YieldState.StaticVersion;

            case YieldState.StartSlot:
                if (!EncounterStateUtil.CanBeWildEncounter(Entity))
                    goto case YieldState.EndSlot;
                if (Version is GameVersion.BD)
                { State = YieldState.SlotBD; goto case YieldState.SlotBD; }
                if (Version is GameVersion.SP)
                { State = YieldState.SlotSP; goto case YieldState.SlotSP; }
                throw new ArgumentOutOfRangeException(nameof(Version));
            case YieldState.SlotBD:
                if (TryGetNext<EncounterArea8b, EncounterSlot8b>(Encounters8b.SlotsBD, out var slotBD))
                    return SetCurrent(slotBD);
                goto case YieldState.EndSlot;
            case YieldState.SlotSP:
                if (TryGetNext<EncounterArea8b, EncounterSlot8b>(Encounters8b.SlotsSP, out var slotSP))
                    return SetCurrent(slotSP);
                goto case YieldState.EndSlot;
            case YieldState.EndSlot:
                if (!mustBeSlot)
                    goto case YieldState.Fallback; // already checked everything else
                State = YieldState.StaticVersion; goto case YieldState.StaticVersion;

            case YieldState.StaticVersion:
                if (Version == GameVersion.BD)
                { State = YieldState.StaticVersionBD; goto case YieldState.StaticVersionBD; }
                if (Version == GameVersion.SP)
                { State = YieldState.StaticVersionSP; goto case YieldState.StaticVersionSP; }
                goto case YieldState.Fallback; // already checked everything else

            case YieldState.StaticVersionBD:
                if (TryGetNext(Encounters8b.StaticBD, out var staticBD))
                    return SetCurrent(staticBD);
                Index = 0; State = YieldState.StaticShared; goto case YieldState.StaticShared;
            case YieldState.StaticVersionSP:
                if (TryGetNext(Encounters8b.StaticSP, out var staticSP))
                    return SetCurrent(staticSP);
                Index = 0; State = YieldState.StaticShared; goto case YieldState.StaticShared;

            case YieldState.StaticShared:
                if (TryGetNext(Encounters8b.Encounter_BDSP, out var shared))
                    return SetCurrent(shared);
                if (mustBeSlot)
                    goto case YieldState.Fallback; // already checked everything else
                Index = 0; State = YieldState.StartSlot; goto case YieldState.StartSlot;

            case YieldState.Fallback:
                State = YieldState.End;
                if (Deferred != null)
                    return SetCurrent(new MatchedEncounter<IEncounterable>(Deferred, Rating));
                break;
            case YieldState.End:
                return false;
        }
        return false;
    }

    private void InitializeWildLocationInfo()
    {
        mustBeSlot = Entity.Ball == (byte)Ball.Safari;
        met = Entity.Met_Location;
        var location = met;
        var remap = LocationsHOME.GetRemapState(EntityContext.Gen8b, Entity.Context);
        hasOriginalLocation = true;
        if (remap.HasFlag(LocationRemapState.Remapped))
            hasOriginalLocation = location != LocationsHOME.GetMetSWSH((ushort)location, (int)Version);
    }

    private bool TryGetNext<TArea, TSlot>(TArea[] areas, out MatchedEncounter<IEncounterable> match)
        where TArea : class, IEncounterArea<TSlot>, IAreaLocation
        where TSlot : class, IEncounterable, IEncounterMatch
    {
        for (; Index < areas.Length; Index++, SubIndex = 0)
        {
            var area = areas[Index];
            if (hasOriginalLocation && !area.IsMatchLocation(met))
                continue;

            var slots = area.Slots;
            if (TryGetNextSub(slots, out match))
                return true;
        }
        match = default;
        return false;
    }

    private bool TryGetNextSub<T>(T[] slots, out MatchedEncounter<IEncounterable> match)
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
                {
                    match = new MatchedEncounter<IEncounterable>(enc, rating);
                    return true;
                }

                if (rating < Rating)
                {
                    Deferred = enc;
                    Rating = rating;
                }
                break;
            }
        }
        match = default;
        return false;
    }

    private bool TryGetNext<T>(T[] db, out MatchedEncounter<IEncounterable> match) where T : class, IEncounterable, IEncounterMatch
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
                {
                    match = new MatchedEncounter<IEncounterable>(enc, rating);
                    return true;
                }
                if (rating < Rating)
                {
                    Deferred = enc;
                    Rating = rating;
                }
                break;
            }
        }
        match = default;
        return false;
    }

    private bool SetCurrent(in MatchedEncounter<IEncounterable> match)
    {
        Current = match;
        Yielded = true;
        return true;
    }
}
