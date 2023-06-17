using System;

namespace PKHeX.Core;

internal static class EvolutionUtil
{
    public static void SetHistory<T>(Span<EvoCriteria> result, T pt, out EvoCriteria[] destination) where T : IPersonalTable
    {
        // Get the range of {result} that is present within pt.
        int start = 0;
        int count = 0;

        // Find first index that exists in the game, and take while they do.
        foreach (var evo in result)
        {
            if (evo.Species == 0)
                break; // uninitialized struct, end of data
            if (evo.LevelMin == 0)
            {
                start++; // unable to evolve to this stage, skip
                continue;
            }

            if (pt.IsPresentInGame(evo.Species, evo.Form))
                count++;
            else if (count == 0)
                start++;
            else
                break;
        }

        if (count == 0)
        {
            destination = Array.Empty<EvoCriteria>();
            return;
        }
        destination = result.Slice(start, count).ToArray();
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

            // Shift down
            for (int j = i + 1; j < result.Length; j++)
            {
                var next = result[j];
                if (next.Species == 0)
                    break;
                result[j - 1] = next;
            }

            // Zero out last entry
            result[^1] = default;
        }
    }
}
