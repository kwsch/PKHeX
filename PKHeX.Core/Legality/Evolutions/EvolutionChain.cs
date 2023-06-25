using System;

namespace PKHeX.Core;

/// <summary>
/// Logic to create an <see cref="EvolutionHistory"/>.
/// </summary>
public static class EvolutionChain
{
    public static EvolutionHistory GetEvolutionChainsAllGens(PKM pk, IEncounterTemplate enc)
    {
        var min = GetMinLevel(pk, enc);
        var origin = new EvolutionOrigin(pk.Species, (byte)enc.Version, (byte)enc.Generation, min, (byte)pk.CurrentLevel);
        if (!pk.IsEgg && enc is not EncounterInvalid)
            return GetEvolutionChainsSearch(pk, origin, enc.Context, enc.Species);

        return GetEvolutionChainsSearch(pk, origin, pk.Context, enc.Species);
    }

    private static byte GetMinLevel(PKM pk, IEncounterTemplate enc) => enc.Generation switch
    {
        2 => pk is ICaughtData2 c2 ? Math.Max((byte)c2.Met_Level, enc.LevelMin) : enc.LevelMin,
        < 4 when pk.Format != enc.Generation => enc.LevelMin,
        _ => Math.Max((byte)pk.Met_Level, enc.LevelMin),
    };

    public static EvolutionHistory GetEvolutionChainsSearch(PKM pk, EvolutionOrigin enc, EntityContext context, ushort encSpecies)
    {
        Span<EvoCriteria> chain = stackalloc EvoCriteria[EvolutionTree.MaxEvolutions];
        return EvolutionChainsSearch(pk, enc, context, encSpecies, chain);
    }

    private static EvolutionHistory EvolutionChainsSearch(PKM pk, EvolutionOrigin enc, EntityContext context, ushort encSpecies, Span<EvoCriteria> chain)
    {
        var history = new EvolutionHistory();
        var length = GetOriginChain(chain, pk, enc, false);
        if (length == 0)
            return history;
        chain = chain[..length];

        // Update the chain to only include the current species, leave future evolutions as unknown
        ConditionBaseChainForward(encSpecies, chain);

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

    private static void ConditionBaseChainForward(ushort encSpecies, Span<EvoCriteria> chain)
    {
        for (int i = 0; i < chain.Length; i++)
        {
            ref var evo = ref chain[i];
            if (evo.Species == encSpecies)
                break;
            evo = evo with { LevelMin = 0 };
        }
    }

    public static EvoCriteria[] GetOriginChain(PKM pk, EvolutionOrigin enc, bool discard = true)
    {
        Span<EvoCriteria> result = stackalloc EvoCriteria[EvolutionTree.MaxEvolutions];
        int count = GetOriginChain(result, pk, enc, discard);
        if (count == 0)
            return Array.Empty<EvoCriteria>();

        var chain = result[..count];
        return chain.ToArray();
    }

    public static int GetOriginChain(Span<EvoCriteria> result, PKM pk, EvolutionOrigin enc, bool discard = true)
    {
        ushort species = enc.Species;
        byte form = pk.Form;
        if (pk.IsEgg && !enc.SkipChecks)
        {
            result[0] = new EvoCriteria { Species = species, Form = form, LevelMax = enc.LevelMax, LevelMin = enc.LevelMax };
            return 1;
        }

        // Shedinja's evolution case can be a little tricky; hard-code handling.
        if (enc is { Species: (int)Species.Shedinja, LevelMax: >= 20 } && (!pk.HasOriginalMetLocation || enc.LevelMin < enc.LevelMax))
        {
            var min = Math.Max(enc.LevelMin, (byte)20);
            result[0] = new EvoCriteria { Species = (ushort)Species.Shedinja, LevelMax = enc.LevelMax, LevelMin = min, Method = EvolutionType.LevelUp };
            result[1] = new EvoCriteria { Species = (ushort)Species.Nincada, LevelMax = enc.LevelMax, LevelMin = enc.LevelMin };
            return 2;
        }

        // Sometimes we have to sanitize the inputs.
        switch (species)
        {
            case (int)Species.Silvally:
            case (int)Species.Palkia:
            case (int)Species.Dialga:
                form = 0;
                break;
        }

        result[0] = new EvoCriteria { Species = species, Form = form, LevelMax = enc.LevelMax };
        var count = DevolveFrom(result, pk, enc, pk.Context, discard);

        var chain = result[..count];
        EvolutionReversal.CleanDevolve(chain, enc.LevelMin);
        return count;
    }

    private static int DevolveFrom(Span<EvoCriteria> result, PKM pk, EvolutionOrigin enc, EntityContext context, bool discard)
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
            group.DiscardForOrigin(result, pk);

        return GetCount(result);
    }

    private static int GetCount(Span<EvoCriteria> result)
    {
        // return the count of species != 0
        int count = 0;
        foreach (var evo in result)
        {
            if (evo.Species == 0)
                break;
            count++;
        }

        return count;
    }
}
