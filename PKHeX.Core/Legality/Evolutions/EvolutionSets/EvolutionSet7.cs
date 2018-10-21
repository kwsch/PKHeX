using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 7 Evolution Branch Entries
    /// </summary>
    public sealed class EvolutionSet7 : EvolutionSet
    {
        private const int SIZE = 8;

        public EvolutionSet7(byte[] data)
        {
            PossibleEvolutions = new EvolutionMethod[data.Length / SIZE];
            for (int i = 0; i < data.Length; i += SIZE)
            {
                PossibleEvolutions[i / SIZE] = new EvolutionMethod
                {
                    Method = BitConverter.ToUInt16(data, i + 0),
                    Argument = BitConverter.ToUInt16(data, i + 2),
                    Species = BitConverter.ToUInt16(data, i + 4),
                    Form = (sbyte)data[i + 6],
                    Level = data[i + 7],
                };
            }
        }
    }
}