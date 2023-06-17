using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PKHeX.Core;

public sealed class EvolutionReverseSpecies : IEvolutionReverse
{
    public EvolutionReverseLookup Lineage { get; }

    public EvolutionReverseSpecies(EvolutionMethod[][] entries, IPersonalTable t)
    {
        Lineage = GetLineage(t, entries);
    }

    private static EvolutionReverseLookup GetLineage(IPersonalTable t, EvolutionMethod[][] entries)
    {
        var maxSpecies = t.MaxSpeciesID;
        var lineage = new EvolutionReverseLookup(maxSpecies);
        for (ushort sSpecies = 1; sSpecies <= maxSpecies; sSpecies++)
        {
            var fc = t[sSpecies].FormCount;
            for (byte sForm = 0; sForm < fc; sForm++)
            {
                foreach (var evo in entries[sSpecies])
                {
                    var dSpecies = evo.Species;
                    if (dSpecies == 0)
                        break;

                    var dForm = sSpecies == (int)Species.Espurr && evo.Method == EvolutionType.LevelUpFormFemale1
                        ? (byte)1
                        : sForm;
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
        throw new UnreachableException(); // You should never be here
    }

    public int Devolve(Span<EvoCriteria> result, ushort species, byte form, PKM pk, byte levelMin, byte levelMax, ushort stopSpecies,
        bool skipChecks)
    {
        return Lineage.Devolve(result, species, form, pk, levelMin, levelMax, stopSpecies, skipChecks);
    }

    public bool TryDevolve(ISpeciesForm head, PKM pk, byte currentMaxLevel, int levelMin, bool skipChecks, out EvoCriteria result)
    {
        var node = Lineage[head.Species, head.Form];
        return node.TryDevolve(pk, currentMaxLevel, levelMin, skipChecks, out result);
    }
}
