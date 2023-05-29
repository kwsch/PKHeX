using System;

namespace PKHeX.Core;

/// <summary>
/// Logic to create an <see cref="EvolutionHistory"/>.
/// </summary>
public static class EvolutionChain
{
    public static EvolutionHistory GetEvolutionChainsAllGens(PKM pk, IEncounterTemplate enc)
    {
        var origin = new EvolutionOrigin(enc.Species, (byte)enc.Version, (byte)enc.Generation, enc.LevelMin, (byte)pk.CurrentLevel);
        if (!pk.IsEgg && enc is not EncounterInvalid)
            return GetEvolutionChainsSearch(pk, origin);

        var history = new EvolutionHistory();
        var group = EvolutionGroupUtil.GetCurrentGroup(pk);
        var chain = group.GetInitialChain(pk, origin, pk.Species, pk.Form);
        history.Set(pk.Context, chain);
        return history;
    }

    public static EvolutionHistory GetEvolutionChainsSearch(PKM pk, EvolutionOrigin enc)
    {
        var group = EvolutionGroupUtil.GetCurrentGroup(pk);
        ReadOnlySpan<EvoCriteria> chain = group.GetInitialChain(pk, enc, pk.Species, pk.Form);

        var history = new EvolutionHistory();
        while (true)
        {
            var any = group.Append(pk, history, ref chain, enc);
            if (!any)
                break;
            var previous = group.GetPrevious(pk, enc);
            if (previous is null)
                break;
            group = previous;
        }
        return history;
    }

    public static EvoCriteria[] GetValidPreEvolutions(PKM pk, int maxLevel, int minLevel = 1, bool skipChecks = false)
    {
        var context = pk.Context;
        if (context < EntityContext.Gen2)
            context = EntityContext.Gen2;
        var et = EvolutionTree.GetEvolutionTree(context);
        return et.GetValidPreEvolutions(pk, levelMax: (byte)maxLevel, skipChecks: skipChecks, levelMin: (byte)minLevel);
    }
}

public static class EvolutionOrigins
{
    public static int GetOrigin(Span<EvoCriteria> result, PKM pk, bool skipChecks = false)
    {
        var origin = GetOrigin(pk, (byte)pk.Met_Level, (byte)pk.CurrentLevel, skipChecks);
        Seed(result, origin);
        var current = GetCurrent(pk);
        var initial = GetInitial(pk);
        Span<EvoCriteria> local = stackalloc EvoCriteria[EvolutionTree.MaxEvolutions];
        while (true)
        {
            var count = current.Reverse(local, result, pk, ref origin);
            if (count == 0)
                return 0;

            var success = current.Meld(result, local);
            if (!success)
                return 0;

            if (current == initial)
                return count;
            current = current.GetPrevious(origin);
            if (current is null)
                return 0;
        }
    }

    private static void Seed(Span<EvoCriteria> evos, EvolutionOrigin origin)
    {
        throw new NotImplementedException();
    }

    public static EvolutionOrigin GetOrigin(IEncounterTemplate pk, byte min, byte max, bool skipChecks = false)
    {
        return new EvolutionOrigin(0, (byte)pk.Version, (byte)pk.Generation, min, max, skipChecks);
    }

    private static EvolutionOrigin GetOrigin(PKM pk, byte min, byte max, bool skipChecks = false)
    {
        return new EvolutionOrigin(0, (byte)pk.Version, (byte)pk.Generation, min, max, skipChecks);
    }

    private static IEvolutionContext GetCurrent(PKM pk) => pk.Context switch
    {
        _ => throw new NotImplementedException(),
    };

    private static IEvolutionContext GetInitial(PKM pk)
    {
        throw new NotImplementedException();
    }
}
