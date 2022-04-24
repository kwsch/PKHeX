using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 1 Evolution Branch Entries
    /// </summary>
    public static class EvolutionSet1
    {
        private static EvolutionMethod GetMethod(ReadOnlySpan<byte> data)
        {
            int method = data[0];
            int species = data[1];
            var arg = data[2];

            return (method == 1)
                ? new EvolutionMethod(method, species, level: arg)
                : new EvolutionMethod(method, species, argument: arg);
        }

        public static IReadOnlyList<EvolutionMethod[]> GetArray(ReadOnlySpan<byte> data, int maxSpecies)
        {
            var evos = new EvolutionMethod[maxSpecies + 1][];
            int ofs = 0;
            const int bpe = 3;
            for (int i = 0; i < evos.Length; i++)
            {
                int count = data[ofs];
                ofs++;
                if (count == 0)
                {
                    evos[i] = Array.Empty<EvolutionMethod>();
                    continue;
                }
                var m = new EvolutionMethod[count];
                for (int j = 0; j < m.Length; j++)
                {
                    m[j] = GetMethod(data.Slice(ofs, bpe));
                    ofs += bpe;
                }
                evos[i] = m;
            }
            return evos;
        }
    }
}
