using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class EvolutionReversePersonal : IEvolutionReverse
{
    public IEvolutionLookup Lineage { get; }

    public EvolutionReversePersonal(IPersonalTable t, EvolutionMethod[][] entries, ushort maxSpecies)
    {
        Lineage = GetLineage(t, entries, maxSpecies);
    }

    private static EvolutionReverseLookup GetLineage(IPersonalTable t, EvolutionMethod[][] entries, ushort maxSpecies)
    {
        var lineage = new EvolutionReverseLookup(maxSpecies);
        for (ushort sSpecies = 1; sSpecies <= maxSpecies; sSpecies++)
        {
            var fc = t[sSpecies].FormCount;
            for (byte sForm = 0; sForm < fc; sForm++)
            {
                var index = t.GetFormIndex(sSpecies, sForm);
                foreach (var evo in entries[index])
                {
                    var dSpecies = evo.Species;
                    if (dSpecies == 0)
                        break;

                    var dForm = evo.GetDestinationForm(sForm);
                    var link = new EvolutionLink(sSpecies, sForm, evo);
                    lineage.Register(link, dSpecies, dForm);
                }
            }
        }
        return lineage;
    }

    public EvolutionNode GetReverse(ushort species, byte form) => Lineage[species, form];

    public IEnumerable<(ushort Species, byte Form)> GetPreEvolutions(ushort species, byte form)
    {
        var node = Lineage[species, form];

        // No convergent evolutions; first method is enough.
        var s = node.First.Tuple;
        if (s.Species == 0)
            yield break;

        var preEvolutions = GetPreEvolutions(s.Species, s.Form);
        foreach (var preEvo in preEvolutions)
            yield return preEvo;
        yield return s;
    }

    public void BanEvo(ushort species, byte form, Func<PKM, bool> func)
    {
        ref var node = ref Lineage[species, form];
        node.Ban(func);
    }

    public int Devolve(Span<EvoCriteria> result, ushort species, byte form, PKM pk, byte levelMin, byte levelMax, ushort stopSpecies,
        bool skipChecks)
    {
        return Lineage.Devolve(result, species, form, pk, levelMin, levelMax, stopSpecies, skipChecks);
    }
}
