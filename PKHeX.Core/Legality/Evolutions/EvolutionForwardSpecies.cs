using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class EvolutionForwardSpecies : IEvolutionForward
{
    private readonly EvolutionMethod[][] Entries;

    public EvolutionForwardSpecies(EvolutionMethod[][] entries) => Entries = entries;

    public IEnumerable<(ushort Species, byte Form)> GetEvolutions(ushort species, byte form)
    {
        var methods = GetForward(species, form);
        return GetEvolutions(methods, form);
    }

    public ReadOnlyMemory<EvolutionMethod> GetForward(ushort species, byte form)
    {
        var arr = Entries;
        if (species >= arr.Length)
            return Array.Empty<EvolutionMethod>();
        return arr[species];
    }

    private IEnumerable<(ushort Species, byte Form)> GetEvolutions(ReadOnlyMemory<EvolutionMethod> evos, byte form)
    {
        for (int i = 0; i < evos.Length; i++)
        {
            var method = evos.Span[i];
            var s = method.Species;
            if (s == 0)
                continue;
            var f = method.GetDestinationForm(form);
            yield return (s, f);
            var nextEvolutions = GetEvolutions(s, f);
            foreach (var nextEvo in nextEvolutions)
                yield return nextEvo;
        }
    }
}
