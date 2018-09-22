using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 1 Evolution Branch Entries
    /// </summary>
    public sealed class EvolutionSet1 : EvolutionSet
    {
        private static EvolutionMethod GetMethod(byte[] data, ref int offset)
        {
            switch (data[offset])
            {
                case 1: // Level
                    var m1 = new EvolutionMethod
                    {
                        Method = 1, // Level Up
                        Level = data[offset + 1],
                        Species = data[offset + 2]
                    };
                    offset += 3;
                    return m1;
                case 2: // Use Item
                    var m2 = new EvolutionMethod
                    {
                        Method = 8, // Use Item
                        Argument = data[offset + 1],
                        // 1
                        Species = data[offset + 3],
                    };
                    offset += 4;
                    return m2;
                case 3: // Trade
                    var m3 = new EvolutionMethod
                    {
                        Method = 5, // Trade
                        // 1
                        Species = data[offset + 2]
                    };
                    offset += 3;
                    return m3;
            }
            return null;
        }

        public static List<EvolutionSet> GetArray(byte[] data, int maxSpecies)
        {
            var evos = new List<EvolutionSet>();
            int offset = 0;
            for (int i = 0; i <= maxSpecies; i++)
            {
                var m = new List<EvolutionMethod>();
                while (data[offset] != 0)
                    m.Add(GetMethod(data, ref offset));
                ++offset;
                evos.Add(new EvolutionSet1 { PossibleEvolutions = m.ToArray() });
            }
            return evos;
        }
    }
}