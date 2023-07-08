using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class EvolutionReverseSpecies : IEvolutionReverse
{
    public EvolutionReverseLookup Lineage { get; }
    public EvolutionReverseSpecies(EvolutionMethod[][] entries, IPersonalTable t) => Lineage = GetLineage(t, entries);
    public ref readonly EvolutionNode GetReverse(ushort species, byte form) => ref Lineage[species, form];

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
                    var link = new EvolutionLink(evo, sSpecies, sForm);
                    lineage.Register(link, dSpecies, dForm);
                }
            }
        }

        return lineage;
    }

    public IEnumerable<(ushort Species, byte Form)> GetPreEvolutions(ushort species, byte form)
    {
        var node = Lineage[species, form];

        // No convergent evolutions; first method is enough.
        var s = node.First;
        if (s.Species == 0)
            yield break;

        var preEvolutions = GetPreEvolutions(s.Species, s.Form);
        foreach (var preEvo in preEvolutions)
            yield return preEvo;
        yield return (s.Species, s.Form);
    }

    public bool TryDevolve(ISpeciesForm head, PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks, out EvoCriteria result)
    {
        ref readonly var node = ref Lineage[head.Species, head.Form];
        return node.TryDevolve(pk, currentMaxLevel, levelMin, skipChecks, out result);
    }
}
