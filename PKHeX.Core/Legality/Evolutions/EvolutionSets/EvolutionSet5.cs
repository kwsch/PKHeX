using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 5 Evolution Branch Entries
/// </summary>
public static class EvolutionSet5
{
    private static EvolutionMethod GetMethod(ReadOnlySpan<byte> data)
    {
        var method = data[0]; // other byte unnecessary
        var arg = ReadUInt16LittleEndian(data[2..]);
        var species = ReadUInt16LittleEndian(data[4..]);

        if (method == 0)
            throw new ArgumentOutOfRangeException(nameof(method));

        var lvl = EvolutionSet6.IsMethodWithArg(method) ? 0 : arg;
        var type = (EvolutionType)method;
        var lvlup = type.IsLevelUpRequired() ? (byte)1 : (byte)0;
        return new EvolutionMethod(type, species, Argument: arg, Level: (byte)lvl, LevelUp: lvlup);
    }

    private const int bpe = 6; // bytes per evolution entry
    private const int entries = 7; // amount of entries per species
    private const int size = entries * bpe; // bytes per species entry

    public static IReadOnlyList<EvolutionMethod[]> GetArray(ReadOnlySpan<byte> data)
    {
        var evos = new EvolutionMethod[data.Length / size][];
        for (int i = 0; i < evos.Length; i++)
        {
            int offset = i * size;
            var rawEntries = data.Slice(offset, size);
            var count = ScanCountEvolutions(rawEntries);
            if (count == 0)
            {
                evos[i] = Array.Empty<EvolutionMethod>();
                continue;
            }

            var set = new EvolutionMethod[count];
            for (int j = 0; j < set.Length; j++)
                set[j] = GetMethod(rawEntries.Slice(j * bpe, bpe));
            evos[i] = set;
        }
        return evos;
    }

    private static int ScanCountEvolutions(ReadOnlySpan<byte> data)
    {
        for (int count = 0; count < entries; count++)
        {
            var methodOffset = count * bpe;
            var method = data[methodOffset];
            if (method == 0)
                return count;
        }
        return entries;
    }
}
