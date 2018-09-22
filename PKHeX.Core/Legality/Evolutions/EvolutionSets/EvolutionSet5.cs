using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 5 Evolution Branch Entries
    /// </summary>
    public sealed class EvolutionSet5 : EvolutionSet
    {
        private static EvolutionMethod GetMethod(byte[] data, int offset)
        {
            int[] argEvos = { 6, 8, 16, 17, 18, 19, 20, 21, 22 };
            int method = BitConverter.ToUInt16(data, offset + 0);
            int arg = BitConverter.ToUInt16(data, offset + 2);
            int species = BitConverter.ToUInt16(data, offset + 4);

            if (method == 0)
                return null;

            var evo = new EvolutionMethod
            {
                Method = method,
                Argument = arg,
                Species = species,
                Level = arg,
            };

            if (argEvos.Contains(evo.Method))
                evo.Level = 0;
            return evo;
        }

        public static List<EvolutionSet> GetArray(byte[] data)
        {
            var evos = new List<EvolutionSet>();
            for (int i = 0; i <= Legal.MaxSpeciesIndex_5_B2W2; i++)
            {
                /* 42 bytes per species,
                 * for every species 7 evolutions with 6 bytes per evolution*/
                int offset = i * 42;
                var m_list = new List<EvolutionMethod>();
                for (int j = 0; j < 7; j++)
                {
                    EvolutionMethod m = GetMethod(data, offset);
                    if (m != null)
                        m_list.Add(m);
                    else
                        break;
                    offset += 6;
                }
                evos.Add(new EvolutionSet5 { PossibleEvolutions = m_list.ToArray() });
            }
            return evos;
        }
    }
}