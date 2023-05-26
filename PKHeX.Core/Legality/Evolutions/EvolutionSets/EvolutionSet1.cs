using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 1 Evolution Branch Entries
/// </summary>
public static class EvolutionSet1
{
    private static EvolutionMethod GetMethod(ReadOnlySpan<byte> data)
    {
        var method = data[0];
        var species = data[1];
        var arg = data[2];

        return method == 1
            ? new EvolutionMethod(EvolutionType.LevelUp, species, Level: arg, LevelUp: 1)
            : new EvolutionMethod((EvolutionType)method, species, Argument: arg);
    }

    public static EvolutionMethod[][] GetArray(ReadOnlySpan<byte> data, int maxSpecies)
    {
        var result = new EvolutionMethod[maxSpecies + 1][];
        for (int i = 0, offset = 0; i < result.Length; i++)
            result[i] = GetEntry(data, ref offset);
        return result;
    }

    private static EvolutionMethod[] GetEntry(ReadOnlySpan<byte> data, ref int offset)
    {
        var count = data[offset++];
        if (count == 0)
            return Array.Empty<EvolutionMethod>();

        const int bpe = 3;
        var result = new EvolutionMethod[count];
        for (int i = 0; i < result.Length; i++, offset += bpe)
            result[i] = GetMethod(data.Slice(offset, bpe));
        return result;
    }
}
