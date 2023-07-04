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

        if (count == 0)
            return Array.Empty<EvoCriteria>();
        return result.Slice(start, count).ToArray();
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

    /// <summary>
    /// Revises the <see cref="result"/> to account for a new maximum <see cref="level"/>.
    /// </summary>
    public static void UpdateCeiling(Span<EvoCriteria> result, int level)
    {
        foreach (ref var evo in result)
        {
            if (evo.Species == 0)
                break;
            evo = evo with { LevelMax = (byte)Math.Min(evo.LevelMax, level) };
        }
    }

    /// <summary>
    /// Revises the <see cref="result"/> to account for a new minimum <see cref="level"/>.
    /// </summary>
    public static void UpdateFloor(Span<EvoCriteria> result, int level)
    {
        foreach (ref var evo in result)
        {
            if (evo.Species == 0)
                break;
            evo = evo with { LevelMin = (byte)Math.Max(evo.LevelMin, level) };
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
}
