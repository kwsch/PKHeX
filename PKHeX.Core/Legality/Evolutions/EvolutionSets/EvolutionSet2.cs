using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 2 Evolution Branch Entries
    /// </summary>
    public sealed class EvolutionSet2 : EvolutionSet
    {
        private static EvolutionMethod GetMethod(byte[] data, ref int offset)
        {
            int method = data[offset];
            int arg = data[offset + 1];
            int species = data[offset + 2];
            offset += 3;

            switch (method)
            {
                case 1: /* Level Up */ return new EvolutionMethod { Method = 1, Species = species, Level = arg };
                case 2: /* Use Item */ return new EvolutionMethod { Method = 8, Species = species, Argument = arg };
                case 3: /*  Trade   */ return new EvolutionMethod { Method = 5, Species = species };
                case 4: /*Friendship*/ return new EvolutionMethod { Method = 1, Species = species };
                case 5: /*  Stats   */
                    // species is currently stat ID, we don't care about evo type as stats can be changed after evo
                    return new EvolutionMethod { Method = 1, Species = data[offset++], Level = arg }; // Tyrogue stats
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
                evos.Add(new EvolutionSet2 { PossibleEvolutions = m.ToArray() });
            }
            return evos;
        }
    }
}