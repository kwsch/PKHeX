using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Evolution Branch Binary Unpacker
/// </summary>
public static class EvolutionSet
{
    private const int SIZE = 8;

    public static EvolutionMethod[][] GetArray(BinLinkerAccessor data, byte levelUp = 1)
    {
        var result = new EvolutionMethod[data.Length][];
        for (int i = 0; i < result.Length; i++)
            result[i] = GetEntry(data[i], levelUp);
        return result;
    }

    private static EvolutionMethod[] GetEntry(ReadOnlySpan<byte> data, byte levelUp)
    {
        if (data.Length == 0)
            return [];

        var result = new EvolutionMethod[data.Length / SIZE];
        for (int i = 0, offset = 0; i < result.Length; i++, offset += SIZE)
            result[i] = GetMethod(data.Slice(offset, SIZE), levelUp);
        return result;
    }

    private static EvolutionMethod GetMethod(ReadOnlySpan<byte> entry, byte levelUp)
    {
        var type = (EvolutionType)entry[0];
        var arg = ReadUInt16LittleEndian(entry[2..]);
        var species = ReadUInt16LittleEndian(entry[4..]);
        var form = entry[6];
        var level = entry[7];
        var lvlup = type.IsLevelUpRequired() ? levelUp : (byte)0;
        return new EvolutionMethod(species, arg, form, type, level, lvlup);
    }
}
