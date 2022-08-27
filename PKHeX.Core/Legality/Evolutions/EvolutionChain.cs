using System;

using static PKHeX.Core.Legal;

namespace PKHeX.Core;

/// <summary>
/// Logic to create an <see cref="EvolutionHistory"/>.
/// </summary>
internal static class EvolutionChain
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

    public static EvoCriteria[] GetValidPreEvolutions(PKM pk, int maxspeciesorigin = -1, int maxLevel = -1, int minLevel = 1, bool skipChecks = false)
    {
        if (maxLevel < 0)
            maxLevel = pk.CurrentLevel;

        if (maxspeciesorigin == -1 && ParseSettings.AllowGen1Tradeback && pk.Format <= 2 && pk.Generation == 1)
            maxspeciesorigin = MaxSpeciesID_2;

        var context = pk.Context;
        if (context < EntityContext.Gen2)
            context = EntityContext.Gen2;
        var et = EvolutionTree.GetEvolutionTree(context);
        return et.GetValidPreEvolutions(pk, levelMax: (byte)maxLevel, maxSpeciesOrigin: maxspeciesorigin, skipChecks: skipChecks, levelMin: (byte)minLevel);
    }
}
