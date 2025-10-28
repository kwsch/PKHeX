using System;
using System.Diagnostics.CodeAnalysis;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Evolution Branch Binary Unpacker
/// </summary>
public static class EvolutionSet
{
    private const int SIZE = 8;

    /// <summary>
    /// Retrieves a two-dimensional array of <see cref="EvolutionMethod"/> objects based on the provided data.
    /// </summary>
    /// <param name="data">Container data to unpack.</param>
    /// <param name="levelUp">Level up amount required to trigger a level up evolution. Is 0 for games like <see cref="GameVersion.PLA"/> which can trigger manually when satisfied.</param>
    public static EvolutionMethod[][] GetArray(BinLinkerAccessor16 data, [ConstantExpected] byte levelUp = 1)
    {
        var result = new EvolutionMethod[data.Length][];
        for (int i = 0; i < result.Length; i++)
            result[i] = GetEntry(data[i], levelUp);
        return result;
    }

    private static EvolutionMethod[] GetEntry(ReadOnlySpan<byte> data, [ConstantExpected] byte levelUp)
    {
        if (data.Length == 0)
            return [];

        var result = new EvolutionMethod[data.Length / SIZE];
        for (int i = 0, offset = 0; i < result.Length; i++, offset += SIZE)
            result[i] = GetMethod(data.Slice(offset, SIZE), levelUp);
        return result;
    }

    private static EvolutionMethod GetMethod(ReadOnlySpan<byte> entry, [ConstantExpected] byte levelUp)
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
