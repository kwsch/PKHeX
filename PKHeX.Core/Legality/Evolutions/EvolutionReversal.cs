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
    /// <param name="species">Species to devolve from</param>
    /// <param name="form">Form to devolve from</param>
    /// <param name="pk">Entity reference to sanity check evolutions with</param>
    /// <param name="levelMin">Minimum level the entity may exist as</param>
    /// <param name="levelMax">Maximum the entity may exist as</param>
    /// <param name="maxSpeciesID">Maximum species ID that may exist</param>
    /// <param name="skipChecks">Option to bypass some evolution criteria</param>
    /// <param name="stopSpecies">Species ID that should be the last node, if at all.</param>
    /// <returns>Reversed evolution lineage, with the lowest index being the current species-form.</returns>
    public static EvoCriteria[] Reverse(this IEvolutionLookup lineage, ushort species, byte form, PKM pk, byte levelMin, byte levelMax, int maxSpeciesID, bool skipChecks, int stopSpecies)
    {
        // Sometimes we have to sanitize the inputs.
        switch (species)
        {
            case (int)Species.Silvally:
                form = 0;
                break;
        }

        // Store our results -- trim at the end when we place it on the heap.
        const int maxEvolutions = 3;
        Span<EvoCriteria> evos = stackalloc EvoCriteria[maxEvolutions];

        var lvl = levelMax; // highest level, any level-up method will decrease.
        evos[0] = new EvoCriteria { Species = species, Form = form, LevelMax = lvl }; // current species-form

        // There aren't any circular evolution paths, and all lineages have at most 3 evolutions total.
        // There aren't any convergent evolution paths, so only yield the first connection.
        int ctr = 1; // count in the 'evos' span
        while (species != stopSpecies)
        {
            var key = EvolutionTree.GetLookupKey(species, form);
            var node = lineage[key];

            bool oneValid = false;
            for (int i = 0; i < 2; i++)
            {
                ref var link = ref i == 0 ? ref node.First : ref node.Second;
                if (link.IsEmpty)
                    break;

                if (link.IsEvolutionBanned(pk) && !skipChecks)
                    continue;

                var evo = link.Method;
                if (!evo.Valid(pk, lvl, skipChecks))
                    continue;

                if (evo.RequiresLevelUp && levelMin >= lvl)
                    break; // impossible evolution

                UpdateMinValues(evos[..ctr], evo, levelMin);

                species = link.Species;
                form = link.Form;
                evos[ctr++] = evo.GetEvoCriteria(species, form, lvl);
                if (evo.RequiresLevelUp)
                    lvl--;

                oneValid = true;
                break;
            }
            if (!oneValid)
                break;
        }

        // Remove future gen pre-evolutions; no Munchlax from a Gen3 Snorlax, no Pichu from a Gen1-only Raichu, etc
        ref var last = ref evos[ctr - 1];
        if (last.Species > maxSpeciesID)
        {
            for (int i = 0; i < ctr; i++)
            {
                if (evos[i].Species > maxSpeciesID)
                    continue;
                ctr--;
                break;
            }
        }

        // Last species is the wild/hatched species, the minimum level is because it has not evolved from previous species
        var result = evos[..ctr];
        last = ref result[^1];
        last = last with { LevelMin = levelMin, LevelUpRequired = 0 };

        // Rectify minimum levels
        RectifyMinimumLevels(result);

        return result.ToArray();
    }

    private static void RectifyMinimumLevels(Span<EvoCriteria> result)
    {
        for (int i = result.Length - 2; i >= 0; i--)
        {
            ref var evo = ref result[i];
            var prev = result[i + 1];
            var min = (byte)Math.Max(prev.LevelMin + evo.LevelUpRequired, evo.LevelMin);
            evo = evo with { LevelMin = min };
        }
    }

    private static void UpdateMinValues(Span<EvoCriteria> evos, EvolutionMethod evo, byte minLevel)
    {
        ref var last = ref evos[^1];
        if (!evo.RequiresLevelUp)
        {
            // Evolutions like elemental stones, trade, etc
            last = last with { LevelMin = minLevel };
            return;
        }
        if (evo.Level == 0)
        {
            // Friendship based Level Up Evolutions, Pichu -> Pikachu, Eevee -> Umbreon, etc
            last = last with { LevelMin = (byte)(minLevel + 1) };

            // Raichu from Pikachu would have a minimum level of 1; accounting for Pichu (level up required) results in a minimum level of 2
            if (evos.Length > 1)
            {
                ref var first = ref evos[0];
                if (!first.RequiresLvlUp)
                    first = first with { LevelMin = (byte)(minLevel + 1) };
            }
        }
        else // level up evolutions
        {
            last = last with { LevelMin = evo.Level };

            if (evos.Length > 1)
            {
                ref var first = ref evos[0];
                if (first.RequiresLvlUp)
                {
                    // Pokemon like Crobat, its minimum level is Golbat minimum level + 1
                    if (first.LevelMin <= evo.Level)
                        first = first with { LevelMin = (byte)(evo.Level + 1) };
                }
                else
                {
                    // Pokemon like Nidoqueen who evolve with an evolution stone, minimum level is prior evolution minimum level
                    if (first.LevelMin < evo.Level)
                        first = first with { LevelMin = evo.Level };
                }
            }
        }
        last = last with { LevelUpRequired = evo.RequiresLevelUp ? (byte)1 : (byte)0 };
    }
}
