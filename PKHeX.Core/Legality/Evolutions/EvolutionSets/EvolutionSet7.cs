using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 7 Evolution Branch Entries
/// </summary>
public static class EvolutionSet7
{
    private const int SIZE = 8;

    public static EvolutionMethod[][] GetArray(BinLinkerAccessor data, bool LevelUpBypass)
    {
        var result = new EvolutionMethod[data.Length][];
        for (int i = 0; i < result.Length; i++)
            result[i] = GetEntry(data[i], LevelUpBypass);
        return result;
    }

    private static EvolutionMethod[] GetEntry(ReadOnlySpan<byte> data, bool LevelUpBypass)
    {
        if (data.Length == 0)
            return Array.Empty<EvolutionMethod>();

        var result = new EvolutionMethod[data.Length / SIZE];
        for (int i = 0, offset = 0; i < result.Length; i++, offset += SIZE)
            result[i] = GetMethod(data.Slice(offset, SIZE), LevelUpBypass);
        return result;
    }

    private static EvolutionMethod GetMethod(ReadOnlySpan<byte> entry, bool levelUpBypass)
    {
        var type = (EvolutionType)entry[0];
        var arg = ReadUInt16LittleEndian(entry[2..]);
        var species = ReadUInt16LittleEndian(entry[4..]);
        var form = entry[6];
        var level = entry[7];
        var lvlup = !levelUpBypass && type.IsLevelUpRequired() ? (byte)1 : (byte)0;
        return new EvolutionMethod(type, species, form, arg, level, lvlup);
    }
}
