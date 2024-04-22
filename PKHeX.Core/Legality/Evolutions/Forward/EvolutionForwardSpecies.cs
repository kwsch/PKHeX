using System;
using System.Collections.Generic;

namespace PKHeX.Core;

/// <summary>
/// Provides forward evolution pathways based only on species.
/// </summary>
public sealed class EvolutionForwardSpecies(EvolutionMethod[][] Entries) : IEvolutionForward
{
    public IEnumerable<(ushort Species, byte Form)> GetEvolutions(ushort species, byte form)
    {
        var methods = GetForward(species, form);
        return GetEvolutions(methods, form);
    }

    public ReadOnlyMemory<EvolutionMethod> GetForward(ushort species, byte form)
    {
        if (species >= Entries.Length)
            return ReadOnlyMemory<EvolutionMethod>.Empty;
        return Entries[species];
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

    public bool TryEvolve<T>(T head, ISpeciesForm next, PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks,
        EvolutionRuleTweak tweak, out EvoCriteria result) where T : ISpeciesForm
    {
        var methods = GetForward(head.Species, head.Form);
        foreach (var method in methods.Span)
        {
            if (method.Species != next.Species)
                continue;
            var expectForm = method.GetDestinationForm(head.Form);
            if (next.Form != expectForm)
                continue;

            var chk = method.Check(pk, currentMaxLevel, levelMin, skipChecks, tweak);
            if (chk != EvolutionCheckResult.Valid)
                continue;

            result = Create(next.Species, next.Form, method, currentMaxLevel, levelMin);
            return true;
        }

        result = default;
        return false;
    }

    private static EvoCriteria Create(ushort species, byte form, EvolutionMethod method, byte currentMaxLevel, byte min) => new()
    {
        Species = species,
        Form = form,
        LevelMax = currentMaxLevel,
        Method = method.Method,

        // Temporarily store these and overwrite them when we clean the list.
        LevelMin = Math.Max((byte)(min + method.LevelUp), method.Level),
        LevelUpRequired = method.LevelUp, // No need to tweak this, all games of this Type have the same default behavior.
    };
}
