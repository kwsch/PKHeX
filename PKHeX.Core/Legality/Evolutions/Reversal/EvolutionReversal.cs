using System;

namespace PKHeX.Core;

/// <summary>
/// Evolution Reversal logic
/// </summary>
public static class EvolutionReversal
{
    /// <summary>
    /// Reverses from current state to see what evolutions the <see cref="pk"/> may have existed as.
    /// </summary>
    /// <param name="lineage">Evolution Method lineage reversal object</param>
    /// <param name="result">Result storage</param>
    /// <param name="species">Species to devolve from</param>
    /// <param name="form">Form to devolve from</param>
    /// <param name="pk">Entity reference to sanity check evolutions with</param>
    /// <param name="levelMin">Minimum level the entity may exist as</param>
    /// <param name="levelMax">Maximum the entity may exist as</param>
    /// <param name="stopSpecies">Species ID that should be the last node, if at all. Provide 0 to fully devolve.</param>
    /// <param name="skipChecks">Option to bypass some evolution criteria</param>
    /// <returns>Reversed evolution lineage, with the lowest index being the current species-form.</returns>
    public static int Devolve(this IEvolutionLookup lineage, Span<EvoCriteria> result, ushort species, byte form,
        PKM pk, byte levelMin, byte levelMax, ushort stopSpecies, bool skipChecks)
    {
        // Sometimes we have to sanitize the inputs.
        switch (species)
        {
            case (int)Species.Silvally:
                form = 0;
                break;
        }

        // Store our results -- trim at the end when we place it on the heap.
        var head = result[0] = new EvoCriteria { Species = species, Form = form, LevelMax = levelMax };
        int ctr = Devolve(lineage, result, head, pk, levelMax, levelMin, skipChecks, stopSpecies);
        CleanDevolve(result[..ctr], levelMin);
        return ctr;
    }

    private static int Devolve(IEvolutionLookup lineage, Span<EvoCriteria> result, EvoCriteria head,
        PKM pk, byte currentMaxLevel, byte levelMin, bool skipChecks, ushort stopSpecies)
    {
        // There aren't any circular evolution paths, and all lineages have at most 3 evolutions total.
        // There aren't any convergent evolution paths, so only yield the first connection.
        int ctr = 1; // count in the 'evos' span
        while (head.Species != stopSpecies)
        {
            var node = lineage[head.Species, head.Form];
            if (!node.TryDevolve(pk, currentMaxLevel, levelMin, skipChecks, out var x))
                return ctr;

            result[ctr++] = x;
            currentMaxLevel -= x.LevelUpRequired;
        }
        return ctr;
    }

    public static bool TryDevolve(this EvolutionNode node, PKM pk, byte currentMaxLevel, int levelMin, bool skipChecks, out EvoCriteria result)
    {
        // Multiple methods can exist to devolve to the same species-form.
        // The first method is less restrictive (no LevelUp req), if two {level/other} methods exist.
        for (int i = 0; i < 2; i++)
        {
            ref var link = ref i == 0 ? ref node.First : ref node.Second;
            if (link.IsEmpty)
                break;

            if (link.Method.RequiresLevelUp && currentMaxLevel <= levelMin)
                break;

            var chk = link.Method.Check(pk, currentMaxLevel, skipChecks);
            if (chk != EvolutionCheckResult.Valid)
                continue;

            result = Create(link, currentMaxLevel);
            return true;
        }

        result = default;
        return false;
    }

    private static EvoCriteria Create(EvolutionLink link, byte currentMaxLevel) => new()
    {
        Species = link.Species,
        Form = link.Form,
        LevelMax = currentMaxLevel,
        Method = link.Method.Method,

        // Temporarily store these and overwrite them when we clean the list.
        LevelMin = link.Method.Level,
        LevelUpRequired = link.Method.RequiresLevelUp ? (byte)1 : (byte)0,
    };

    public static void CleanDevolve(Span<EvoCriteria> result, byte levelMin)
    {
        // Rectify minimum levels.
        // trickle our two temp variables up the chain (essentially evolving from min).
        byte req = 0;
        EvolutionType method = EvolutionType.None;
        for (int i = result.Length - 1; i >= 0; i--)
        {
            ref var evo = ref result[i];
            var nextMin = evo.LevelMin; // to evolve
            var nextReq = evo.LevelUpRequired;
            var nextMethod = evo.Method;
            evo = evo with { LevelMin = levelMin, LevelUpRequired = req, Method = method };
            levelMin = Math.Max(nextMin, levelMin);
            req = nextReq;
            method = nextMethod;
        }
    }
}
