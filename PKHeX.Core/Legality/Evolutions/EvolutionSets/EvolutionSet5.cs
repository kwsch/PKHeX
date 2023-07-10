using System;
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
        var lvl = EvolutionSet6.IsMethodWithArg(method) ? 0 : arg;
        var type = (EvolutionType)method;
        var lvlup = type.IsLevelUpRequired() ? (byte)1 : (byte)0;
        return new EvolutionMethod(type, species, Argument: arg, Level: (byte)lvl, LevelUp: lvlup);
    }

    private const int bpe = 6; // bytes per evolution entry

    public static EvolutionMethod[][] GetArray(BinLinkerAccessor data)
    {
        var result = new EvolutionMethod[data.Length][];
        for (int i = 0; i < result.Length; i++)
            result[i] = GetEntry(data[i]);
        return result;
    }

    private static EvolutionMethod[] GetEntry(ReadOnlySpan<byte> data)
    {
        if (data.Length == 0)
            return Array.Empty<EvolutionMethod>();

        var result = new EvolutionMethod[data.Length / bpe];
        for (int i = 0, offset = 0; i < result.Length; i++, offset += bpe)
            result[i] = GetMethod(data.Slice(offset, bpe));
        return result;
    }
}
