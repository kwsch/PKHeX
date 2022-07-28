using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 7 Evolution Branch Entries
/// </summary>
public static class EvolutionSet7
{
    private const int SIZE = 8;

    private static EvolutionMethod[] GetMethods(ReadOnlySpan<byte> data, bool LevelUpBypass)
    {
        var evos = new EvolutionMethod[data.Length / SIZE];
        for (int i = 0; i < data.Length; i += SIZE)
        {
            var entry = data.Slice(i, SIZE);
            evos[i / SIZE] = ReadEvolution(entry, LevelUpBypass);
        }
        return evos;
    }

    private static EvolutionMethod ReadEvolution(ReadOnlySpan<byte> entry, bool levelUpBypass)
    {
        var type = (EvolutionType)entry[0];
        var arg = ReadUInt16LittleEndian(entry[2..]);
        var species = ReadUInt16LittleEndian(entry[4..]);
        var form = entry[6];
        var level = entry[7];
        var lvlup = !levelUpBypass && type.IsLevelUpRequired() ? (byte)1 : (byte)0;
        return new EvolutionMethod(type, species, form, arg, level, lvlup);
    }

    public static IReadOnlyList<EvolutionMethod[]> GetArray(BinLinkerAccessor data, bool LevelUpBypass)
    {
        var evos = new EvolutionMethod[data.Length][];
        for (int i = 0; i < evos.Length; i++)
            evos[i] = GetMethods(data[i], LevelUpBypass);
        return evos;
    }
}
