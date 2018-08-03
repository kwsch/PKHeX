using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Levelup Learn Movepool Information
    /// </summary>
    public sealed class Learnset1 : Learnset
    {
        private Learnset1(byte[] data, ref int offset)
        {
            var moves = new List<int>();
            var levels = new List<int>();
            while (data[offset] != 0)
            {
                levels.Add(data[offset++]);
                moves.Add(data[offset++]);
            }
            ++offset;

            Moves = moves.ToArray();
            Levels = levels.ToArray();
            Count = Moves.Length;
        }

        public static Learnset[] GetArray(byte[] input, int maxSpecies)
        {
            var data = new Learnset[maxSpecies + 1];

            int offset = 0;
            for (int s = 0; s < data.Length; s++)
                data[s] = new Learnset1(input, ref offset);

            return data;
        }
    }
}