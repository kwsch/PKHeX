using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 6 Evolution Branch Entries
/// </summary>
public static class EvolutionSet6
{
    // 6, 8, 16, 17, 18, 19, 20, 21, 22, 29
    internal static bool IsMethodWithArg(byte method) => (0b100000011111110000000101000000 & (1 << method)) != 0;
    private const int SIZE = 6;

    private static EvolutionMethod[] GetMethods(ReadOnlySpan<byte> data)
    {
        var evos = new EvolutionMethod[data.Length / SIZE];
        for (int i = 0; i < data.Length; i += SIZE)
        {
            var entry = data.Slice(i, SIZE);
            evos[i/SIZE] = GetMethod(entry);
        }
        return evos;
    }

    private static EvolutionMethod GetMethod(ReadOnlySpan<byte> entry)
    {
        var method = entry[0];
        var arg = ReadUInt16LittleEndian(entry[2..]);
        var species = ReadUInt16LittleEndian(entry[4..]);

        // Argument is used by both Level argument and Item/Move/etc. Clear if appropriate.
        var lvl = IsMethodWithArg(method) ? 0 : arg;
        var type = (EvolutionType)method;
        var lvlup = type.IsLevelUpRequired() ? (byte)1 : (byte)0;
        return new EvolutionMethod(type, species, Argument: arg, Level: (byte)lvl, LevelUp: lvlup);
    }

    public static IReadOnlyList<EvolutionMethod[]> GetArray(BinLinkerAccessor data)
    {
        var evos = new EvolutionMethod[data.Length][];
        for (int i = 0; i < evos.Length; i++)
            evos[i] = GetMethods(data[i]);
        return evos;
    }
}
