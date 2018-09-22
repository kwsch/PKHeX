using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 4 Evolution Branch Entries
    /// </summary>
    public sealed class EvolutionSet4 : EvolutionSet
    {
        private static EvolutionMethod GetMethod(byte[] data, int offset)
        {
            int[] argEvos = { 6, 8, 16, 17, 18, 19, 20, 21, 22 };
            int method = BitConverter.ToUInt16(data, offset + 0);
            int arg = BitConverter.ToUInt16(data, offset + 2);
            int species = BitConverter.ToUInt16(data, offset + 4);

            if (method == 0)
                return null;
            // To have the same estructure as gen 6
            // Gen 4 Method 6 is Gen 6 Method 7, G4 7 = G6 8, and so on
            if (method > 6)
                method++;

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
            const int bpe = 6; // bytes per evolution entry
            const int entries = 7; // 7 * 6 = 42, + 2 alignment bytes
            const int size = 44; // bytes per species entry

            int count = data.Length / size;
            for (int i = 0; i < count; i++)
            {
                int offset = i * size;
                var m_list = new List<EvolutionMethod>();
                for (int j = 0; j < entries; j++)
                {
                    EvolutionMethod m = GetMethod(data, offset);
                    if (m != null)
                        m_list.Add(m);
                    else
                        break;
                    offset += bpe;
                }
                evos.Add(new EvolutionSet4 { PossibleEvolutions = m_list.ToArray() });
            }
            return evos;
        }
    }
}