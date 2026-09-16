using System;

namespace PKHeX.Core;

/// <summary>
/// Logic for verifying Gen3/4 Effort Values present while still in Gen3/4 formats.
/// </summary>
public static class EffortExpLegality
{
    /// <summary>
    /// EVs obtained through vitamins (up to 100 per stat) are treated as free.
    /// </summary>
    private const byte VitaminEVLimit = EffortValues.MaxVitamins34;
    private const byte MaxEVs = EffortValues.Max255;

    /// <summary>
    /// Effort training step. Not worth tracking anything else; not relevant for calculations.
    /// </summary>
    /// <param name="EV">EV gain for the stat when defeating the Pokémon.</param>
    /// <param name="EXP">EXP gain when defeating the Pokémon.</param>
    /// <remarks>
    /// When we award EXP, we have the mon-to-train benched (does not battle), while the entire team holds an EXP share.
    /// This results in the defeated mon's EXP gains being diluted by (1/2)/6 (floored) when awarding the EVs.
    /// Multi-EV yield species are suboptimal for training as individual EV gain species are lower level thus more efficient (less EXP required).
    /// </remarks>
    private readonly record struct TrainingOption(byte EV, byte EXP);

    /// <summary>
    /// Gets the minimum amount of EXP that must have been gained after the encounter's initial state in order to obtain the requested EVs through EV training steps.
    /// </summary>
    /// <param name="evs">The EV values for each stat.</param>
    /// <param name="gainedEXP">The amount of EXP gained so far.</param>
    /// <param name="hasPokerus">Indicates whether the Pokémon has been infected with Pokerus.</param>
    /// <param name="originFormat">The format of the Pokémon's origin.</param>
    /// <param name="currentFormat">The current format of the Pokémon.</param>
    /// <returns>
    /// A negative return value indicates that <paramref name="gainedEXP"/> exceeds the minimum required EXP (legal).
    /// Zero indicates that no additional EXP is required (legal).
    /// A positive value indicates how much additional EXP is required (illegal).
    /// </returns>
    public static int GetRequiredEffortEXP(ReadOnlySpan<int> evs, uint gainedEXP, bool hasPokerus, byte originFormat, byte currentFormat)
    {
        // Gen 3 origin Pokémon can train in Gen 3 before transferring forward.
        // Gen 4 format can train in Gen 4.
        bool canTrainGen3 = originFormat == 3;
        bool canTrainGen4 = currentFormat >= 4 && originFormat <= 4;
        if (!canTrainGen3 && !canTrainGen4) // Shouldn't be calling this method.
            throw new ArgumentOutOfRangeException(nameof(currentFormat), currentFormat, null);

        Span<int> remaining = stackalloc int[6];
        var totalRemaining = 0;
        for (int i = 0; i < 6; i++)
        {
            // Vitamins provide EVs only in increments of 10, with a maximum of 100 EVs per stat.
            // Strip off the largest vitamin-compatible amount before checking training EXP.
            var ev = evs[i];
            var vitaminEV = Math.Min((ev / 10) * 10, VitaminEVLimit);
            var needed = ev - vitaminEV;

            remaining[i] = needed;
            totalRemaining += needed;
        }

        // If vitamins alone explain all EVs or all EVs are zero, then any EXP is legal.
        if (totalRemaining == 0)
            return gainedEXP > int.MaxValue ? 0 : -(int)gainedEXP; // can gain 0 exp / any (remove with berries).

        var need = 0;
        for (int stat = 0; stat < remaining.Length; stat++)
        {
            var needed = remaining[stat];
            if (needed == 0)
                continue;

            var exp = GetMinimumEXP(stat, needed, hasPokerus, canTrainGen3, canTrainGen4);
            need += exp;
        }
        return need - (int)Math.Min(gainedEXP, uint.MaxValue);
    }

    private static int GetMinimumEXP(int statIndex, int requiredEV, bool hasPokerus, bool canTrainGen3, bool canTrainGen4)
    {
        Span<TrainingOption> options = stackalloc TrainingOption[12];
        int count = 0;

        if (canTrainGen4)
            count = AddGen4Options(options, count, hasPokerus);
        if (canTrainGen3)
            count = AddGen3Options(options, count, hasPokerus, statIndex);

        // Count is always greater than 0.
        return SolveExact(options[..count], requiredEV);
    }

    /// <summary>
    /// Appends the available training options for Gen 4 to the <paramref name="options"/> span.
    /// </summary>
    /// <returns>The updated count of training options.</returns>
    private static int AddGen4Options(Span<TrainingOption> options, int count, bool hasPokerus)
    {
        // HGSS have wild level 2 Pokemon available with EV yields available for every stat to obtain 1 EV:1 EXP
        // (HP Hoothoot, Attack Sentret, Defense Geodude, SpA Chingling, SpD Ledyba, Speed Rattata)
        // The Power items are also available to increase the EV yield by +4 per EXP.
        options[count++] = new(1, 1); // Low level mon
        options[count++] = new(5, 1); // +Power item
        if (!hasPokerus) return count;
        options[count++] = new(02, 1);  // Low level mon
        options[count++] = new(10, 1); // +Power item
        return count;
    }

    /// <summary>
    /// Appends the available training options for Gen 3 to the <paramref name="options"/> span.
    /// </summary>
    /// <returns>The updated count of training options.</returns>
    private static int AddGen3Options(Span<TrainingOption> options, int count, bool hasPokerus, int statIndex)
    {
        // GBA games have HP/Atk/Spe EV donors that give less than 24 EXP (dilute to 1 EXP); other stats are less fortunate.
        switch (statIndex)
        {
            case 0 or 1 or 3: // HP / Attack / Speed
                options[count++] = new(1, 1); // Low level mon
                options[count++] = new(2, 1); // +Macho Brace
                if (hasPokerus) options[count++] = new(4, 1); // +Pokerus
                break;
            case 2: // Defense
                options[count++] = new(1, 4); // Geodude (worst case!)
                options[count++] = new(2, 3); // Metapod/Kakuna
                options[count++] = new(4, 3); // +Macho Brace
                if (hasPokerus) options[count++] = new(8, 3); // +Pokerus
                break;
            case 4: // Special Attack
                options[count++] = new(1, 3); // Ralts
                options[count++] = new(2, 3); // +Macho Brace
                if (hasPokerus) options[count++] = new(4, 3); // +Pokerus
                break;
            case 5: // Special Defense
                options[count++] = new(1, 2); // Lotad
                options[count++] = new(2, 2); // +Macho Brace
                if (hasPokerus) options[count++] = new(4, 2); // +Pokerus
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(statIndex));
        }
        return count;
    }

    // Sentinel value to represent unreachable states in the dynamic programming table.
    private const int Infinity = int.MaxValue / 4;

    /// <summary>
    /// Finds the minimum EXP cost to obtain exactly <paramref name="requiredEV"/> EVs from the supplied training options.
    /// </summary>
    private static int SolveExact(ReadOnlySpan<TrainingOption> options, int requiredEV)
    {
        if (requiredEV <= 0)
            return 0;

        // This is a small unbounded-knapsack / shortest-path style dynamic programming problem.
        // Each training option may be used any number of times, and we want the minimum EXP cost for an exact EV total.
        // Since every option increases EVs, states can be solved in ascending EV order without needing recursion or backtracking.

        // Dynamic programming table:
        //   minimumEXPByEV[x] = minimum EXP required to obtain exactly x EVs.
        //
        // Start with no EVs requiring zero EXP. All other states are initially unreachable.
        // We then build the table from low to high EV counts by considering each available training option
        // (for example, 5 EV for 1 EXP, or 10 EV for 1 EXP with Pokerus).
        //
        // Because every training option only increases the EV count,
        // when we reach minimumEXPByEV[ev] it already contains the cheapest known way to obtain exactly that many EVs.

        // We could store pre-computed tables for each stat/permutation... but a simple DP calc is fast enough and not worth bloating with cached tables.
        Span<int> minimumEXPByEV = stackalloc int[MaxEVs + 1];
        minimumEXPByEV.Fill(Infinity);
        minimumEXPByEV[0] = 0;

        for (int ev = 0; ev <= requiredEV; ev++)
        {
            var current = minimumEXPByEV[ev];
            if (current >= Infinity)
                continue;

            foreach (var option in options)
            {
                var next = ev + option.EV;
                if (next > requiredEV)
                    continue;

                // If we can obtain `ev` EVs for `current` EXP,
                // then applying this training option gives us `next` EVs for `current + option.EXP` EXP.
                // Keep whichever route is cheaper.
                var candidate = current + option.EXP;
                if (candidate < minimumEXPByEV[next])
                    minimumEXPByEV[next] = candidate;
            }
        }
        return minimumEXPByEV[requiredEV];
    }
}
