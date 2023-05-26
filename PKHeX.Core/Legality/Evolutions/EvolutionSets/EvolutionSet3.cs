using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 Evolution Branch Entries
/// </summary>
public static class EvolutionSet3
{
    private static EvolutionMethod GetMethod(ReadOnlySpan<byte> data)
    {
        var method = data[0];
        var arg =  ReadUInt16LittleEndian(data[2..]);
        var species = SpeciesConverter.GetNational3(ReadUInt16LittleEndian(data[4..]));
        //2 bytes padding

        switch (method)
        {
            case 1: /* Friendship*/
            case 2: /* Friendship day*/
            case 3: /* Friendship night*/
                return new EvolutionMethod((EvolutionType)method, species, Argument: arg, LevelUp: 1);
            case 5: /* Trade   */
            case 6: /* Trade while holding */
                return new EvolutionMethod((EvolutionType)method, species, Argument: arg);
            case 4: /* Level Up */
                return new EvolutionMethod(EvolutionType.LevelUp, species, Argument: arg, Level: (byte)arg, LevelUp: 1);
            case 7: /* Use item */
                return new EvolutionMethod((EvolutionType)(method + 1), species, Argument: arg);
            case 15: /* Beauty evolution*/
                return new EvolutionMethod((EvolutionType)(method + 1), species, Argument: arg, LevelUp: 1);
            case 8: /* Tyrogue -> Hitmonchan */
            case 9: /* Tyrogue -> Hitmonlee */
            case 10: /* Tyrogue -> Hitmontop*/
            case 11: /* Wurmple -> Silcoon evolution */
            case 12: /* Wurmple -> Cascoon evolution */
            case 13: /* Nincada -> Ninjask evolution */
            case 14: /* Shedinja spawn in Nincada -> Ninjask evolution */
                return new EvolutionMethod((EvolutionType)(method + 1), species, Argument: arg, Level: (byte)arg, LevelUp: 1);

            default:
                throw new ArgumentOutOfRangeException(nameof(method));
        }
    }

    public static EvolutionMethod[][] GetArray(ReadOnlySpan<byte> data)
    {
        var result = new EvolutionMethod[Legal.MaxSpeciesID_3 + 1][];
        result[0] = Array.Empty<EvolutionMethod>();
        for (ushort i = 1; i <= Legal.MaxSpeciesIndex_3; i++)
        {
            int g4species = SpeciesConverter.GetNational3(i);
            if (g4species != 0)
                result[g4species] = GetEntry(data, i);
        }
        return result;
    }

    private const int maxCount = 5;
    private const int size = 8;

    private static EvolutionMethod[] GetEntry(ReadOnlySpan<byte> data, ushort index)
    {
        var span = data.Slice(index * (maxCount * size), maxCount * size);
        int count = ScanCountEvolutions(span);

        if (count == 0)
            return Array.Empty<EvolutionMethod>();

        var result = new EvolutionMethod[count];
        for (int i = 0, offset = 0; i < result.Length; i++, offset += size)
            result[i] = GetMethod(span.Slice(offset, size));
        return result;
    }

    private static int ScanCountEvolutions(ReadOnlySpan<byte> span)
    {
        for (int count = 0; count < maxCount; count++)
        {
            if (span[count * size] == 0)
                return count;
        }
        return maxCount;
    }
}
