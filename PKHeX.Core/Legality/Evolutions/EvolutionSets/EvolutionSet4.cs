using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 4 Evolution Branch Entries
    /// </summary>
    public sealed class EvolutionSet4 : EvolutionSet
    {
        private static EvolutionMethod GetMethod(byte[] data, int offset)
        {
            var method = data[offset]; // other byte unnecessary
            int arg = BitConverter.ToUInt16(data, offset + 2);
            int species = BitConverter.ToUInt16(data, offset + 4);

            if (method == 0)
                return null;

            // To have the same structure as gen 6
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

            if (EvolutionSet6.EvosWithArg.Contains(method))
                evo.Level = 0;
            return evo;
        }

        private static readonly EvolutionSet Blank = new EvolutionSet4 { PossibleEvolutions = Array.Empty<EvolutionMethod>() };

        public static IReadOnlyList<EvolutionSet> GetArray(byte[] data)
        {
            const int bpe = 6; // bytes per evolution entry
            const int entries = 7; // amount of entries per species
            const int size = (entries * bpe) + 2; // bytes per species entry, + 2 alignment bytes

            var evos = new EvolutionSet[data.Length / size];
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
                    evos[i] = Blank;
                    continue;
                }

                var set = new EvolutionMethod[count];
                for (int j = 0; j < set.Length; j++)
                    set[j] = GetMethod(data, offset + (j * bpe));
                evos[i] = new EvolutionSet4 {PossibleEvolutions = set};
            }
            return evos;
        }
    }
}