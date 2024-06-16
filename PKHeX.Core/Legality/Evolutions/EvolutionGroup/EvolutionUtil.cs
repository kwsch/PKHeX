using System;

namespace PKHeX.Core;

internal static class EvolutionUtil
{
    public static EvoCriteria[] SetHistory<T>(Span<EvoCriteria> result, T pt) where T : IPersonalTable
    {
        // Get the range of {result} that is present within pt.
        int start = 0;
        int count = 0;

        // Find first index that exists in the game, and take while they do.
        foreach (ref readonly var evo in result)
        {
            if (evo.Method == EvoCriteria.SentinelNotReached)
            {
                // Unable to evolve to this stage, skip everything before it
                start += count + 1;
                count = 0;
            }
            else if (pt.IsPresentInGame(evo.Species, evo.Form))
            {
                // Found a valid entry, increment count.
                count++;
            }
            else if (count == 0)
            {
                // No valid entries found yet, increment start.
                start++;
            }
            else
            {
                // Found an invalid entry, stop.
                break;
            }
        }

        return GetLocalEvolutionArray(result.Slice(start, count));
    }

    private static EvoCriteria[] GetLocalEvolutionArray(Span<EvoCriteria> result)
    {
        if (result.Length == 0)
            return [];

        var array = result.ToArray();
        var length = CleanEvolve(array);
        if (length != array.Length)
            Array.Resize(ref array, length);
        return array;
    }

    public static void Discard<T>(Span<EvoCriteria> result, T pt) where T : IPersonalTable
    {
        // Iterate through the result, and if an entry is not present in the game, shift other entries down and zero out the last entry.
        for (int i = 0; i < result.Length; i++)
        {
            var evo = result[i];
            if (evo.Species == 0)
                break;
            if (pt.IsPresentInGame(evo.Species, evo.Form))
                continue;

            ShiftDown(result[i..]);
        }
    }

    private static void ShiftDown(Span<EvoCriteria> result)
    {
        for (int i = 1; i < result.Length; i++)
            result[i - 1] = result[i];
        result[^1] = default; // zero out the last entry
    }

    public static int IndexOf(Span<EvoCriteria> result, ushort species)
    {
        for (int i = 0; i < result.Length; i++)
        {
            if (result[i].Species == species)
                return i;
        }
        return -1;
    }

    /// <summary>
    /// Revises the <see cref="result"/> to account for a new maximum <see cref="level"/>.
    /// </summary>
    public static void UpdateCeiling(Span<EvoCriteria> result, byte level)
    {
        foreach (ref var evo in result)
        {
            if (evo.Species == 0)
                break;
            evo = evo with { LevelMax = Math.Min(evo.LevelMax, level) };
        }
    }

    /// <summary>
    /// Revises the <see cref="result"/> to account for a new minimum <see cref="levelMin"/>.
    /// </summary>
    public static void UpdateFloor(Span<EvoCriteria> result, byte levelMin, byte levelMax)
    {
        // Reset the head to the new levelMax, then clamp every entry's minimum to the min.
        // Evolving non-head evolutions will pick up the new ranges later.
        ref var head = ref result[0];
        head = head with { LevelMax = levelMax };
        foreach (ref var evo in result)
        {
            if (evo.Species == 0)
                break;
            evo = evo with { LevelMin = Math.Max(evo.LevelMin, levelMin) };
        }
    }

    /// <summary>
    /// Mutates the result to leave placeholder data for the species that the <see cref="encSpecies"/> evolves into.
    /// </summary>
    /// <param name="result">All evolutions for the species.</param>
    /// <param name="encSpecies">Species encountered as.</param>
    public static void ConditionBaseChainForward(Span<EvoCriteria> result, ushort encSpecies)
    {
        foreach (ref var evo in result)
        {
            if (evo.Species == encSpecies)
                break;
            evo = evo with { Method = EvoCriteria.SentinelNotReached };
        }
    }

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
            evo = evo with { LevelMin = Math.Min(evo.LevelMax, levelMin), LevelUpRequired = req, Method = method };
            levelMin = Math.Max(nextMin, levelMin);
            req = nextReq;
            method = nextMethod;
        }
    }

    private static int CleanEvolve(Span<EvoCriteria> result)
    {
        // Rectify maximum levels.
        int i = 1;
        for (; i < result.Length; i++)
        {
            var next = result[i - 1];
            // Ignore LevelUp byte as it can learn moves prior to evolving.
            var newMax = next.LevelMax;
            ref var evo = ref result[i];
            if (evo.LevelMin > newMax - next.LevelUpRequired)
                break;
            evo = evo with { LevelMax = newMax };
        }
        return i;
    }

    public static void ConditionEncounterNoMet(Span<EvoCriteria> chain)
    {
        // Allow for under-leveled evolutions for purposes of finding an under-leveled evolved encounter.
        // e.g. a level 5 Silcoon encounter slot (normally needs level 7).
        for (int i = 0; i < chain.Length - 1; i++)
        {
            ref var evo = ref chain[i];
            if (evo.Method.IsLevelUpRequired())
                evo = evo with { LevelMin = (byte)(chain[i + 1].LevelMin + evo.LevelUpRequired) };
        }
    }
}
