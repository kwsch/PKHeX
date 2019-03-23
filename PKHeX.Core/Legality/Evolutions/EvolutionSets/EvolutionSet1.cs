using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 1 Evolution Branch Entries
    /// </summary>
    public sealed class EvolutionSet1 : EvolutionSet
    {
        private static EvolutionMethod GetMethod(byte[] data, int offset)
        {
            int method = data[offset];
            int species = data[offset + 1];
            int arg = data[offset + 2];
            var obj = new EvolutionMethod {Method = method, Species = species};
            if (method == 1)
                obj.Level = arg;
            else
                obj.Argument = arg;
            return obj;
        }

        private static readonly EvolutionSet1 Blank = new EvolutionSet1 {PossibleEvolutions = Array.Empty<EvolutionMethod>()};

        public static IReadOnlyList<EvolutionSet> GetArray(byte[] data, int maxSpecies)
        {
            var evos = new EvolutionSet[maxSpecies + 1];
            int ofs = 0;
            const int bpe = 3;
            for (int i = 0; i < evos.Length; i++)
            {
                int count = data[ofs];
                ofs++;
                if (count == 0)
                {
                    evos[i] = Blank;
                    continue;
                }
                var m = new EvolutionMethod[count];
                for (int j = 0; j < m.Length; j++)
                {
                    m[j] = GetMethod(data, ofs);
                    ofs += bpe;
                }
                evos[i] = new EvolutionSet1 {PossibleEvolutions = m};
            }
            return evos;
        }
    }
}