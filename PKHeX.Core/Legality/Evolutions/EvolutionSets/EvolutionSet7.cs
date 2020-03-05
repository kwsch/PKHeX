using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 7 Evolution Branch Entries
    /// </summary>
    public static class EvolutionSet7
    {
        private const int SIZE = 8;

        private static EvolutionMethod[] GetMethods(byte[] data)
        {
            var evos = new EvolutionMethod[data.Length / SIZE];
            for (int i = 0; i < data.Length; i += SIZE)
            {
                var method = BitConverter.ToUInt16(data, i + 0);
                var arg = BitConverter.ToUInt16(data, i + 2);
                var spec = BitConverter.ToUInt16(data, i + 4);
                var form = (sbyte) data[i + 6];
                var level = data[i + 7];
                evos[i / SIZE] = new EvolutionMethod(method, spec, argument: arg, level: level, form: form);
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