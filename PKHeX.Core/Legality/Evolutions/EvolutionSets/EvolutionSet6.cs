using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 6 Evolution Branch Entries
    /// </summary>
    public static class EvolutionSet6
    {
        internal static readonly HashSet<int> EvosWithArg = new() {6, 8, 16, 17, 18, 19, 20, 21, 22, 29};
        private const int SIZE = 6;

        private static EvolutionMethod[] GetMethods(ReadOnlySpan<byte> data)
        {
            var evos = new EvolutionMethod[data.Length / SIZE];
            for (int i = 0; i < data.Length; i += SIZE)
            {
                var entry = data.Slice(i, SIZE);
                evos[i / SIZE] = GetMethod(entry);
            }
            return evos;
        }

        private static EvolutionMethod GetMethod(ReadOnlySpan<byte> entry)
        {
            var method = ReadUInt16LittleEndian(entry);
            var arg = ReadUInt16LittleEndian(entry[2..]);
            var species = ReadUInt16LittleEndian(entry[4..]);

            // Argument is used by both Level argument and Item/Move/etc. Clear if appropriate.
            var lvl = EvosWithArg.Contains(method) ? 0 : arg;

            var evo = new EvolutionMethod(method, species, argument: arg, level: lvl);
            return evo;
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