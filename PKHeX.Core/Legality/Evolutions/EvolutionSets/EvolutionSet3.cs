using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 3 Evolution Branch Entries
    /// </summary>
    public sealed class EvolutionSet3 : EvolutionSet
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
                    return new EvolutionMethod { Method = method, Species = species, Argument = arg };
                case 4: /* Level Up */
                    return new EvolutionMethod { Method = 4, Species = species, Level = arg, Argument = arg };
                case 7: /* Use item */
                case 15: /* Beauty evolution*/
                    return new EvolutionMethod { Method = method + 1, Species = species, Argument = arg };
                case 8: /* Tyrogue -> Hitmonchan */
                case 9: /* Tyrogue -> Hitmonlee */
                case 10: /* Tyrogue -> Hitmontop*/
                case 11: /* Wurmple -> Silcoon evolution */
                case 12: /* Wurmple -> Cascoon evolution */
                case 13: /* Nincada -> Ninjask evolution */
                case 14: /* Shedinja spawn in Nincada -> Ninjask evolution */
                    return new EvolutionMethod { Method = method + 1, Species = species, Level = arg, Argument = arg };
            }
            return null;
        }

        public static List<EvolutionSet> GetArray(byte[] data)
        {
            EvolutionSet[] evos = new EvolutionSet[Legal.MaxSpeciesID_3 + 1];
            evos[0] = new EvolutionSet3 { PossibleEvolutions = new EvolutionMethod[0] };
            for (int i = 0; i <= Legal.MaxSpeciesIndex_3; i++)
            {
                int g4species = SpeciesConverter.GetG4Species(i);
                if (g4species == 0)
                    continue;

                int offset = i * 40;
                var m_list = new List<EvolutionMethod>();
                for (int j = 0; j < 5; j++)
                {
                    EvolutionMethod m = GetMethod(data,  offset);
                    if (m != null)
                        m_list.Add(m);
                    else
                        break;
                    offset += 8;
                }
                evos[g4species] = new EvolutionSet3 { PossibleEvolutions = m_list.ToArray() };
            }
            return evos.ToList();
        }
    }
}