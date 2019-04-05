using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 5 Evolution Branch Entries
    /// </summary>
    public static class EvolutionSet5
    {
        private static EvolutionMethod GetMethod(byte[] data, int offset)
        {
            var method = data[offset]; // other byte unnecessary
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

            if (EvolutionSet6.EvosWithArg.Contains(method))
                evo.Level = 0;
            return evo;
        }

        public static IReadOnlyList<EvolutionMethod[]> GetArray(byte[] data)
        {
            const int bpe = 6; // bytes per evolution entry
            const int entries = 7; // amount of entries per species
            const int size = entries * bpe; // bytes per species entry

            var evos = new EvolutionMethod[data.Length / size][];
            for (int i = 0; i < evos.Length; i++)
            {
                int offset = i * size;
                int count = 0;
                for (; count < entries; count++)
                {
                    var methodOffset = offset + (count * bpe);
                    var method = data[methodOffset];
                    if (method == 0)
                        break;
                }
                if (count == 0)
                {
                    evos[i] = Array.Empty<EvolutionMethod>();
                    continue;
                }

                var set = new EvolutionMethod[count];
                for (int j = 0; j < set.Length; j++)
                    set[j] = GetMethod(data, offset + (j * bpe));
                evos[i] = set;
            }
            return evos;
        }
    }
}