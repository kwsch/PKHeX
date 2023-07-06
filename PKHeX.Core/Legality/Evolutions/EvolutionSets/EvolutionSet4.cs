using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 4 Evolution Branch Entries
/// </summary>
public static class EvolutionSet4
{
    private static EvolutionMethod GetMethod(ReadOnlySpan<byte> data)
    {
        var method = data[0]; // other byte unnecessary
        var arg = ReadUInt16LittleEndian(data[2..]);
        var species = ReadUInt16LittleEndian(data[4..]);

        if (method == 0)
            throw new ArgumentOutOfRangeException(nameof(method));

        // To have the same structure as gen 6
        // Gen 4 Method 6 is Gen 6 Method 7, G4 7 = G6 8, and so on
        if (method > 6)
            method++;

        var lvl = EvolutionSet6.IsMethodWithArg(method) ? 0 : arg;
        var type = (EvolutionType)method;
        var lvlup = type.IsLevelUpRequired() ? (byte)1 : (byte)0;
        return new EvolutionMethod(type, species, Argument: arg, Level: (byte)lvl, LevelUp: lvlup);
    }

    private const int bpe = 6; // bytes per evolution entry
    private const int entries = 7; // amount of entries per species
    private const int size = (entries * bpe) + 2; // bytes per species entry, + 2 alignment bytes

    public static EvolutionMethod[][] GetArray(ReadOnlySpan<byte> data)
    {
        var result = new EvolutionMethod[data.Length / size][];
        for (int i = 0, offset = 0; i < result.Length; i++, offset += size)
            result[i] = GetEntry(data.Slice(offset, size));
        return result;
    }

    private static EvolutionMethod[] GetEntry(ReadOnlySpan<byte> data)
    {
        int count = ScanCountEvolutions(data);
        if (count == 0)
            return Array.Empty<EvolutionMethod>();

        var result = new EvolutionMethod[count];
        for (int i = 0, offset = 0; i < result.Length; i++, offset += bpe)
            result[i] = GetMethod(data.Slice(offset, bpe));
        return result;
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
