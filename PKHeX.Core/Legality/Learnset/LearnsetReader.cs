using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Unpacks <see cref="Learnset"/> data from legality binary inputs.
    /// </summary>
    public static class LearnsetReader
    {
        private static readonly Learnset EMPTY = new Learnset(Array.Empty<int>(), Array.Empty<int>());

        public static Learnset[] GetArray(byte[] input, int maxSpecies)
        {
            var data = new Learnset[maxSpecies + 1];

            int offset = 0;
            for (int s = 0; s < data.Length; s++)
                data[s] = ReadLearnset8(input, ref offset);

            return data;
        }

        public static Learnset[] GetArray(byte[][] entries)
        {
            Learnset[] data = new Learnset[entries.Length];
            for (int i = 0; i < data.Length; i++)
                data[i] = ReadLearnset16(entries[i]);
            return data;
        }

        /// <summary>
        /// Reads a Level up move pool definition from a contiguous chunk of GB era ROM data.
        /// </summary>
        /// <remarks>Moves and Levels are 8-bit</remarks>
        private static Learnset ReadLearnset8(byte[] data, ref int offset)
        {
            int end = offset; // scan for count
            if (data[end] == 0)
            {
                ++offset;
                return EMPTY;
            }
            do { end += 2; } while (data[end] != 0);

            var Count = (end - offset) / 2;
            var Moves = new int[Count];
            var Levels = new int[Count];
            for (int i = 0; i < Moves.Length; i++)
            {
                Levels[i] = data[offset++];
                Moves[i] = data[offset++];
            }
            ++offset;
            return new Learnset(Moves, Levels);
        }

        /// <summary>
        /// Reads a Level up move pool definition from a single move pool definition.
        /// </summary>
        /// <remarks>Count of moves, followed by Moves and Levels which are 16-bit</remarks>
        private static Learnset ReadLearnset16(byte[] data)
        {
            if (data.Length < 4 || data.Length % 4 != 0)
                return EMPTY;
            var Count = (data.Length / 4) - 1;
            var Moves = new int[Count];
            var Levels = new int[Count];
            for (int i = 0; i < Count; i++)
            {
                int ofs = i * 4;
                Moves[i] = BitConverter.ToInt16(data, ofs);
                Levels[i] = BitConverter.ToInt16(data, ofs + 2);
            }
            return new Learnset(Moves, Levels);
        }
    }
}