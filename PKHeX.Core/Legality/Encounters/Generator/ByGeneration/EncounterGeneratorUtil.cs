using System.Collections.Generic;

namespace PKHeX.Core;

internal static class EncounterGeneratorUtil
{
    public static IEnumerable<T> GetPossibleAll<T>(EvoCriteria[] chain, IReadOnlyList<T> table) where T : IEncounterTemplate
    {
        foreach (var enc in table)
        {
            foreach (var evo in chain)
            {
                if (evo.Species != enc.Species)
                    continue;
                yield return enc;
                break;
            }
        }
    }

    public static IEnumerable<TSlot> GetPossibleSlots<TArea, TSlot>(EvoCriteria[] chain, TArea[] areas) where TArea : IEncounterArea<TSlot> where TSlot : IEncounterable
    {
        foreach (var area in areas)
        {
            foreach (var enc in GetPossibleAll(chain, area.Slots))
                yield return enc;
        }
    }

    public static bool CheckYield(IEncounterable enc, EncounterMatchRating match, ref IEncounterable? prevEnc, ref EncounterMatchRating prevMatch)
    {
        if (match == EncounterMatchRating.Match)
            return true;
        if (match >= prevMatch)
            return false;

        prevMatch = match;
        prevEnc = enc;
        return false;
    }

    public static bool CheckYield<T>(MatchedEncounter<T> match)
        where T : IEncounterMatch, IEncounterable
    {
        var rating = match.Rating;
        return rating == EncounterMatchRating.Match;
    }

    public static bool CheckYield<T>(MatchedEncounter<T> match, ref IEncounterable? prevEnc)
        where T : IEncounterMatch, IEncounterable
    {
        var rating = match.Rating;
        if (rating == EncounterMatchRating.Match)
            return true;
        if (rating < EncounterMatchRating.PartialMatch)
            prevEnc = match.Encounter;
        return false;
    }

    public static bool CheckYield<T>(MatchedEncounter<T> match, ref IEncounterable? prevEnc, ref EncounterMatchRating prevMatch)
        where T : IEncounterMatch, IEncounterable
    {
        var rating = match.Rating;
        if (rating == EncounterMatchRating.Match)
            return true;
        if (rating >= prevMatch)
            return false;

        prevMatch = match.Rating;
        prevEnc = match.Encounter;
        return false;
    }
}
