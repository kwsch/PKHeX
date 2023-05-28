using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class EvolutionForwardPersonal : IEvolutionForward
{
    private readonly IPersonalTable Personal;
    private readonly EvolutionMethod[][] Entries;

    public EvolutionForwardPersonal(EvolutionMethod[][] entries, IPersonalTable personal)
    {
        Entries = entries;
        Personal = personal;
    }

    public ReadOnlyMemory<EvolutionMethod> GetForward(ushort species, byte form)
    {
        int index = Personal.GetFormIndex(species, form);
        return Entries[index];
    }

    public IEnumerable<(ushort Species, byte Form)> GetEvolutions(ushort species, byte form)
    {
        var methods  = GetForward(species, form);
        return GetEvolutions(methods, form);
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
