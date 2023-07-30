using System.Collections.Generic;

using static PKHeX.Core.EncounterStateUtil;
using static PKHeX.Core.EncounterTypeGroup;
using static PKHeX.Core.EncounterMatchRating;

namespace PKHeX.Core;

public sealed class EncounterGenerator8a : IEncounterGenerator
{
    public static readonly EncounterGenerator8a Instance = new();

    public IEnumerable<IEncounterable> GetPossible(PKM pk, EvoCriteria[] chain, GameVersion game, EncounterTypeGroup groups)
    {
        if (chain.Length == 0)
            yield break;

        if (groups.HasFlag(Mystery))
        {
            var table = EncounterEvent.MGDB_G8A;
            foreach (var enc in GetPossible(chain, table))
                yield return enc;
        }
        if (groups.HasFlag(Static))
        {
            var table = Encounters8a.StaticLA;
            foreach (var enc in GetPossible(chain, table))
                yield return enc;
        }

        if (groups.HasFlag(Slot))
        {
            var areas = Encounters8a.SlotsLA;
            foreach (var enc in GetPossibleSlots(chain, areas))
                yield return enc;
        }
    }

    private static IEnumerable<T> GetPossible<T>(EvoCriteria[] chain, IReadOnlyList<T> table) where T : IEncounterTemplate
    {
        foreach (var e in table)
        {
            foreach (var evo in chain)
            {
                if (evo.Species != e.Species)
                    continue;
                yield return e;
                break;
            }
        }
    }

    private static IEnumerable<IEncounterable> GetPossibleSlots(EvoCriteria[] chain, EncounterArea8a[] areas)
    {
        foreach (var area in areas)
        {
            foreach (var slot in area.Slots)
            {
                foreach (var evo in chain)
                {
                    if (evo.Species != slot.Species)
                        continue;
                    yield return slot;
                    break;
                }
            }
        }
    }

    public IEnumerable<IEncounterable> GetEncounters(PKM pk, EvoCriteria[] chain, LegalInfo info)
    {
        if (chain.Length == 0)
            yield break;
        if (pk is PK8 { SWSH: false })
            yield break;
        if (pk.IsEgg)
            yield break;

        // Mystery Gifts
        // All gifts are Fateful Encounter, but some Static Encounters are as well.
        if (pk.FatefulEncounter)
        {
            // If we yield any Mystery Gifts, we don't need to yield any other encounters.
            bool yielded = false;
            foreach (var mg in EncounterEvent.MGDB_G8A)
            {
                foreach (var evo in chain)
                {
                    if (evo.Species != mg.Species)
                        continue;

                    if (mg.IsMatchExact(pk, evo))
                    {
                        yield return mg;
                        yielded = true;
                    }
                    break;
                }
            }
            if (yielded)
                yield break;
        }

        IEncounterable? cache = null;
        EncounterMatchRating rating = MaxNotMatch;

        // Static Encounters can collide with wild encounters (close match); don't break if a Static Encounter is yielded.
        foreach (var e in Encounters8a.StaticLA)
        {
            foreach (var evo in chain)
            {
                if (evo.Species != e.Species)
                    continue;
                if (!e.IsMatchExact(pk, evo))
                    continue;

                var match = e.GetMatchRating(pk);
                if (match == Match)
                {
                    yield return e;
                }
                else if (match < rating)
                {
                    cache = e;
                    rating = match;
                }
            }
        }

        // Encounter Slots
        if (CanBeWildEncounter(pk))
        {
            var location = pk.Met_Location;
            var remap = LocationsHOME.GetRemapState(EntityContext.Gen8a, pk.Context);
            bool hasOriginalLocation = true;
            if (remap.HasFlag(LocationRemapState.Remapped))
                hasOriginalLocation = location != LocationsHOME.SWLA;
            foreach (var area in Encounters8a.SlotsLA)
            {
                if (hasOriginalLocation && !area.IsMatchLocation(location))
                    continue;

                var slots = area.GetMatchingSlots(pk, chain);
                foreach (var z in slots)
                {
                    var match = z.GetMatchRating(pk);
                    if (match == Match)
                    {
                        yield return z;
                    }
                    else if (match < rating)
                    {
                        cache = z;
                        rating = match;
                    }
                }
            }
        }

        if (cache != null)
            yield return cache;
    }
}
