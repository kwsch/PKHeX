using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Iterates to find potentially matched encounters for <see cref="GameVersion.GG"/> encounters from GO Park.
/// </summary>
public record struct EncounterEnumerator7GO(PKM Entity, EvoCriteria[] Chain) : IEnumerator<MatchedEncounter<IEncounterable>>
{
    private IEncounterable? Deferred;
    private int Index;
    private int EvoIndex;
    private int SubIndex;
    private EncounterMatchRating Rating = EncounterMatchRating.MaxNotMatch;
    public MatchedEncounter<IEncounterable> Current { get; private set; }
    private YieldState State;
    readonly object IEnumerator.Current => Current;

    public readonly void Reset() => throw new NotSupportedException();
    public readonly void Dispose() { }
    public readonly IEnumerator<MatchedEncounter<IEncounterable>> GetEnumerator() => this;

    private enum YieldState : byte
    {
        Start,
        Seek,
        Slot,
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
                State = YieldState.Seek; goto case YieldState.Seek;

            case YieldState.Seek:
                if (!SeekNextArea(EncountersGO.SlotsGO_GG))
                    goto case YieldState.Fallback;
                State = YieldState.Slot; goto case YieldState.Slot;
            case YieldState.Slot:
                var group = EncountersGO.SlotsGO_GG[Index];
                if (TryGetNext(group.Slots))
                    return true;
                State = YieldState.Seek; goto case YieldState.Seek;

            case YieldState.Fallback:
                State = YieldState.End;
                if (Deferred != null)
                    return SetCurrent(Deferred, Rating);
                break;
        }
        return false;
    }

    private bool SeekNextArea<TArea>(TArea[] areas)
        where TArea : ISpeciesForm
    {
        for (; Index < areas.Length; Index++, EvoIndex = 0)
        {
            var area = areas[Index];
            do
            {
                if (IsCompatible(area, Chain[EvoIndex]))
                    return true;
            }
            while (++EvoIndex < Chain.Length);
        }
        return false;
    }

    private readonly bool IsCompatible<TArea>(TArea area, EvoCriteria evo) where TArea : ISpeciesForm
    {
        if (area.Species != evo.Species)
            return false;
        if (area.Form != evo.Form && !FormInfo.IsFormChangeable(area.Species, area.Form, evo.Form, EntityContext.Gen8, Entity.Context))
            return false;
        return true;
    }

    private bool TryGetNext<TSlot>(TSlot[] slots)
        where TSlot : IEncounterable, IEncounterMatch
    {
        var evo = Chain[EvoIndex];
        for (; SubIndex < slots.Length;)
        {
            var enc = slots[SubIndex++];
            if (enc.Species != evo.Species)
                continue;
            if (!enc.IsMatchExact(Entity, evo))
                continue;

            var rating = enc.GetMatchRating(Entity);
            if (rating == EncounterMatchRating.Match)
                return SetCurrent(enc);

            if (rating >= Rating)
                continue;
            Deferred = enc;
            Rating = rating;
        }
        EvoIndex = 0; SubIndex = 0; Index++;
        return false;
    }

    private bool SetCurrent<T>(T enc, EncounterMatchRating rating = EncounterMatchRating.Match) where T : IEncounterable
    {
        Current = new MatchedEncounter<IEncounterable>(enc, rating);
        return true;
    }
}
