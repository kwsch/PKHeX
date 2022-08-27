using System;
using System.Collections.Generic;
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
        var species = SpeciesConverter.GetG4Species(ReadUInt16LittleEndian(data[4..]));
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

    public static IReadOnlyList<EvolutionMethod[]> GetArray(ReadOnlySpan<byte> data)
    {
        var evos = new EvolutionMethod[Legal.MaxSpeciesID_3 + 1][];
        evos[0] = Array.Empty<EvolutionMethod>();
        for (ushort i = 1; i <= Legal.MaxSpeciesIndex_3; i++)
        {
            int g4species = SpeciesConverter.GetG4Species(i);
            if (g4species == 0)
                continue;

            const int maxCount = 5;
            const int size = 8;

            int offset = i * (maxCount * size);
            int count = 0;
            for (; count < maxCount; count++)
            {
                if (data[offset + (count * size)] == 0)
                    break;
            }
            if (count == 0)
            {
                evos[g4species] = Array.Empty<EvolutionMethod>();
                continue;
            }

            var set = new EvolutionMethod[count];
            for (int j = 0; j < set.Length; j++)
                set[j] = GetMethod(data.Slice(offset + (j * size), size));
            evos[g4species] = set;
        }
        return evos;
    }
}
