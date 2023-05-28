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
    /// <param name="stopSpecies">Species ID that should be the last node, if at all.</param>
    /// <param name="skipChecks">Option to bypass some evolution criteria</param>
    /// <returns>Reversed evolution lineage, with the lowest index being the current species-form.</returns>
    public static int Reverse(this IEvolutionLookup lineage, Span<EvoCriteria> result, ushort species, byte form,
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

            // Multiple methods can exist to devolve to the same species-form.
            // The first method is less restrictive (no LevelUp req), if two {level/other} methods exist.
            bool oneValid = false;
            for (int i = 0; i < 2; i++)
            {
                ref var link = ref i == 0 ? ref node.First : ref node.Second;
                if (link.IsEmpty)
                    return ctr;

                var evo = link.Method;
                if (evo.RequiresLevelUp && currentMaxLevel <= levelMin)
                    return ctr;

                if (link.IsEvolutionBanned(pk) && !skipChecks)
                    continue;

                var chk = evo.Check(pk, currentMaxLevel, skipChecks);
                if (chk != EvolutionCheckResult.Valid)
                    continue;

                head = result[ctr++] = new EvoCriteria
                {
                    Species = link.Species,
                    Form = link.Form,
                    LevelMax = currentMaxLevel,
                    Method = evo.Method,

                    // Temporarily store these and overwrite them when we clean the list.
                    LevelMin = evo.Level,
                    LevelUpRequired = evo.RequiresLevelUp ? (byte)1 : (byte)0,
                };
                if (evo.RequiresLevelUp)
                    currentMaxLevel--;

                oneValid = true;
                break;
            }

            if (!oneValid)
                return ctr;
        }
        return ctr;
    }

    private static void CleanDevolve(Span<EvoCriteria> result, byte levelMin)
    {
        // Rectify minimum levels -- trickle our two temp variables up the chain (essentially evolving from min).
        byte req = 0;
        for (int i = result.Length - 1; i >= 0; i--)
        {
            ref var evo = ref result[i];
            var nextMin = evo.LevelMin; // to evolve
            var nextReq = evo.LevelUpRequired;
            evo = evo with { LevelMin = levelMin, LevelUpRequired = req };
            levelMin = Math.Max(nextMin, levelMin);
            req = nextReq;
        }
    }
}
