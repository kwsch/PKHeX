using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 3 Evolution Branch Entries
    /// </summary>
    public static class EvolutionSet3
    {
        private static EvolutionMethod GetMethod(byte[] data, int offset)
        {
            int method = BitConverter.ToUInt16(data, offset + 0);
            int arg =  BitConverter.ToUInt16(data, offset + 2);
            int species = SpeciesConverter.GetG4Species(BitConverter.ToUInt16(data, offset + 4));
            //2 bytes padding

            switch (method)
            {
                case 1: /* Friendship*/
                case 2: /* Friendship day*/
                case 3: /* Friendship night*/
                case 5: /* Trade   */
                case 6: /* Trade while holding */
                    return new EvolutionMethod(method, species, argument: arg);
                case 4: /* Level Up */
                    return new EvolutionMethod(4, species, argument: arg, level:arg);
                case 7: /* Use item */
                case 15: /* Beauty evolution*/
                    return new EvolutionMethod(method + 1, species, argument: arg);
                case 8: /* Tyrogue -> Hitmonchan */
                case 9: /* Tyrogue -> Hitmonlee */
                case 10: /* Tyrogue -> Hitmontop*/
                case 11: /* Wurmple -> Silcoon evolution */
                case 12: /* Wurmple -> Cascoon evolution */
                case 13: /* Nincada -> Ninjask evolution */
                case 14: /* Shedinja spawn in Nincada -> Ninjask evolution */
                    return new EvolutionMethod(method + 1, species, argument: arg, level: arg);

                default:
                    throw new ArgumentException(nameof(method));
            }
        }

        public static IReadOnlyList<EvolutionMethod[]> GetArray(byte[] data)
        {
            var evos = new EvolutionMethod[Legal.MaxSpeciesID_3 + 1][];
            evos[0] = Array.Empty<EvolutionMethod>();
            for (int i = 1; i <= Legal.MaxSpeciesIndex_3; i++)
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
                    set[j] = GetMethod(data, offset + (j * size));
                evos[g4species] = set;
            }
            return evos;
        }
    }
}