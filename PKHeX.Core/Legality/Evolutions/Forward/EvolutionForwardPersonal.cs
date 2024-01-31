using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PKHeX.Core;

/// <summary>
/// Provides forward evolution pathways with reliance on personal table data for form branched evolutions.
/// </summary>
public sealed class EvolutionForwardPersonal(EvolutionMethod[][] Entries, IPersonalTable Personal) : IEvolutionForward
{
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

    public bool TryEvolve<T>(T head, ISpeciesForm next, PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks, EvolutionRuleTweak tweak, out EvoCriteria result) where T : ISpeciesForm
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

            result = Create(next.Species, next.Form, method, currentMaxLevel, levelMin, tweak);
            return true;
        }

        result = default;
        return false;
    }

    private static EvoCriteria Create(ushort species, byte form, EvolutionMethod method, byte currentMaxLevel, byte min, EvolutionRuleTweak tweak) => new()
    {
        Species = species,
        Form = form,
        LevelMax = currentMaxLevel,
        Method = method.Method,

        // Temporarily store these and overwrite them when we clean the list.
        LevelMin = Math.Max((byte)(min + method.LevelUp), method.Level),
        LevelUpRequired = GetLevelUp(method.LevelUp, currentMaxLevel, tweak),
    };

    /// <summary>
    /// Gets the level up requirement for the evolution.
    /// </summary>
    /// <seealso cref="EvolutionReversal.GetLevelUp"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte GetLevelUp(byte flag, byte currentMaxLevel, EvolutionRuleTweak tweak)
    {
        if (flag == 0)
            return 0;
        if (currentMaxLevel == 100 && tweak.AllowLevelUpEvolution100)
            return 0;
        return flag;
    }
}
