using System;

namespace PKHeX.Core;

/// <summary>
/// Logic to create an <see cref="EvolutionHistory"/>.
/// </summary>
public static class EvolutionChain
{
    /// <summary>
    /// Build an <see cref="EvolutionHistory"/> for the given <paramref name="pk"/> and <paramref name="enc"/>.
    /// </summary>
    /// <param name="pk">Entity to search for.</param>
    /// <param name="enc">Evolution details.</param>
    public static EvolutionHistory GetEvolutionChainsAllGens(PKM pk, IEncounterTemplate enc)
    {
        var min = GetMinLevel(pk, enc);
        var origin = new EvolutionOrigin(pk.Species, enc.Version, enc.Generation, min, pk.CurrentLevel);
        if (!pk.IsEgg && enc is not EncounterInvalid)
            return GetEvolutionChainsSearch(pk, origin, enc.Context, enc.Species);

        return GetEvolutionChainsSearch(pk, origin, pk.Context, enc.Species);
    }

    /// <summary>
    /// Build an <see cref="EvolutionHistory"/> for the given <paramref name="pk"/> and <paramref name="enc"/>.
    /// </summary>
    /// <param name="pk">Entity to search for.</param>
    /// <param name="enc">Evolution details.</param>
    /// <param name="context">Starting (original) context of the <paramref name="pk"/>.</param>
    /// <param name="encSpecies">Encountered as species. If not known (search for all), set to 0.</param>
    public static EvolutionHistory GetEvolutionChainsSearch(PKM pk, EvolutionOrigin enc, EntityContext context, ushort encSpecies = 0)
    {
        Span<EvoCriteria> chain = stackalloc EvoCriteria[EvolutionTree.MaxEvolutions];
        return EvolutionChainsSearch(pk, enc, context, encSpecies, chain);
    }

    private static byte GetMinLevel(PKM pk, IEncounterTemplate enc) => enc.Generation switch
    {
        2 => pk is ICaughtData2 c2 ? Math.Max(c2.MetLevel, enc.LevelMin) : enc.LevelMin,
        <= 4 when pk.Format != enc.Generation => enc.LevelMin,
        _ => Math.Max(pk.MetLevel, enc.LevelMin),
    };

    private static EvolutionHistory EvolutionChainsSearch(PKM pk, EvolutionOrigin enc, EntityContext context, ushort encSpecies, Span<EvoCriteria> chain)
    {
        var history = new EvolutionHistory();
        var length = GetOriginChain(chain, pk, enc, encSpecies, enc.IsDiscardRequired(pk.Format));
        if (length == 0)
            return history;
        chain = chain[..length];

        // Update the chain to only include the current species, leave future evolutions as unknown
        if (encSpecies != 0)
            EvolutionUtil.ConditionBaseChainForward(chain, encSpecies);
        if (context == EntityContext.Gen2)
        {
            // Handle the evolution case for Gen2->Gen1
            EvolutionGroup2.Instance.Evolve(chain, pk, enc, history);
            EvolutionGroup1.Instance.Evolve(chain, pk, enc, history);
            if (pk.Format > 2) // Skip forward to Gen7
                context = EntityContext.Gen7;
            else // no more possible contexts; done.
                return history;
        }

        var group = EvolutionGroupUtil.GetGroup(context);
        while (true)
        {
            group.Evolve(chain, pk, enc, history);
            var previous = group.GetNext(pk, enc);
            if (previous is null)
                break;
            group = previous;
        }
        return history;
    }

    /// <summary>
    /// Gets a list of <see cref="EvoCriteria"/> that represent the possible original states of the <paramref name="pk"/>.
    /// </summary>
    /// <param name="pk">Entity to search for.</param>
    /// <param name="enc">Evolution details.</param>
    /// <param name="encSpecies">Encountered as species. If not known (search for all), set to 0.</param>
    /// <param name="discard">Discard evolutions that are not possible for the original context. Pass false to keep all evolutions.</param>
    public static EvoCriteria[] GetOriginChain(PKM pk, EvolutionOrigin enc, ushort encSpecies = 0, bool discard = true)
    {
        Span<EvoCriteria> result = stackalloc EvoCriteria[EvolutionTree.MaxEvolutions];
        int count = GetOriginChain(result, pk, enc, encSpecies, discard);
        if (count == 0)
            return [];

        var chain = result[..count];
        if (IsMetLost(pk, enc)) // Original met level lost, need to be more permissive on evos.
            EvolutionUtil.ConditionEncounterNoMet(chain);
        return chain.ToArray();
    }

    /// <summary>
    /// Gets a list of <see cref="EvoCriteria"/> that represent the possible original states of the <paramref name="pk"/>.
    /// </summary>
    /// <param name="result">Span to write results to.</param>
    /// <param name="pk">Entity to search for.</param>
    /// <param name="enc">Evolution details.</param>
    /// <param name="encSpecies">Encountered as species. If not known (search for all), set to 0.</param>
    /// <param name="discard">Discard evolutions that are not possible for the original context. Pass false to keep all evolutions.</param>
    /// <returns>Number of valid evolutions found.</returns>
    public static int GetOriginChain(Span<EvoCriteria> result, PKM pk, EvolutionOrigin enc, ushort encSpecies = 0, bool discard = true)
    {
        ushort species = enc.Species;
        byte form = pk.Form;
        if (pk.IsEgg && !enc.SkipChecks)
        {
            result[0] = new EvoCriteria { Species = species, Form = form, LevelMax = enc.LevelMax, LevelMin = enc.LevelMax };
            return 1;
        }

        result[0] = new EvoCriteria { Species = species, Form = form, LevelMax = enc.LevelMax };
        var count = DevolveFrom(result, pk, enc, pk.Context, encSpecies, discard);

        var chain = result[..count];
        EvolutionUtil.CleanDevolve(chain, enc.LevelMin);
        return count;
    }

    private static bool IsMetLost(PKM pk, EvolutionOrigin enc) => enc.Generation switch
    {
        >= 5 => false,
        <= 2 => pk is not ICaughtData2 { MetLevel: not 0 },
           _ => enc.Generation != pk.Format,
    };

    private static int DevolveFrom(Span<EvoCriteria> result, PKM pk, EvolutionOrigin enc, EntityContext context, ushort encSpecies, bool discard)
    {
        var group = EvolutionGroupUtil.GetGroup(context);
        while (true)
        {
            group.Devolve(result, pk, enc);
            var previous = group.GetPrevious(pk, enc);
            if (previous is null)
                break;
            group = previous;
        }

        if (discard)
            group.DiscardForOrigin(result, pk, enc);
        if (encSpecies != 0)
            return EvolutionUtil.IndexOf(result, encSpecies) + 1;
        return GetCount(result);
    }

    /// <summary>
    /// Gets the count of entries that are not empty (species == 0).
    /// </summary>
    private static int GetCount(in ReadOnlySpan<EvoCriteria> result)
    {
        int count = 0;
        foreach (ref readonly var evo in result)
        {
            if (evo.Species == 0)
                break;
            count++;
        }

        return count;
    }
}
