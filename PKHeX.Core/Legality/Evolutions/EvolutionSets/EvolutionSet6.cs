using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 6 Evolution Branch Entries
    /// </summary>
    public static class EvolutionSet6
    {
        internal static readonly HashSet<int> EvosWithArg = new HashSet<int> {6, 8, 16, 17, 18, 19, 20, 21, 22, 29};
        private const int SIZE = 6;

        private static EvolutionMethod[] GetMethods(byte[] data)
        {
            var evos = new EvolutionMethod[data.Length / SIZE];
            for (int i = 0; i < data.Length; i += SIZE)
            {
                var evo = new EvolutionMethod
                {
                    Method = BitConverter.ToUInt16(data, i + 0),
                    Argument = BitConverter.ToUInt16(data, i + 2),
                    Species = BitConverter.ToUInt16(data, i + 4),

                    // Copy
                    Level = BitConverter.ToUInt16(data, i + 2),
                };

                // Argument is used by both Level argument and Item/Move/etc. Clear if appropriate.
                if (EvosWithArg.Contains(evo.Method))
                    evo.Level = 0;

                evos[i/SIZE] = evo;
            }
            return evos;
        }

        public static IReadOnlyList<EvolutionMethod[]> GetArray(IReadOnlyList<byte[]> data)
        {
            var evos = new EvolutionMethod[data.Count][];
            for (int i = 0; i < evos.Length; i++)
                evos[i] = GetMethods(data[i]);
            return evos;
        }
    }
}