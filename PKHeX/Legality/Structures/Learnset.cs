using System;
using System.Collections.Generic;
using System.IO;

namespace PKHeX.Core
{
    public abstract class Learnset
    {
        protected int Count;
        protected int[] Moves;
        protected int[] Levels;

        /// <summary>
        /// Returns the moves a Pokémon can learn between the specified level range.
        /// </summary>
        /// <param name="maxLevel">Maximum level</param>
        /// <param name="minLevel">Minimum level</param>
        /// <returns>Array of Move IDs</returns>
        public int[] getMoves(int maxLevel, int minLevel = 0)
        {
            if (minLevel <=1 && maxLevel >= 100)
                return Moves;
            int start = Array.FindIndex(Levels, z => z >= minLevel);
            if (start < 0)
                return new int[0];
            int end = Array.FindLastIndex(Levels, z => z <= maxLevel);
            if (end < 0)
                return new int[0];
            int[] result = new int[end - start + 1];
            Array.Copy(Moves, start, result, 0, result.Length);
            return result;
        }
        /// <summary>Returns the moves a Pokémon would have if it were encountered at the specified level.</summary>
        /// <remarks>In Generation 1, it is not possible to learn any moves lower than these encounter moves.</remarks>
        /// <param name="level">The level the Pokémon was encountered at.</param>
        /// <param name="count">The amount of move slots to return.</param>
        /// <returns>Array of Move IDs</returns>
        public int[] getEncounterMoves(int level, int count = 4)
        {
            if (count == 0 || Moves.Length == 0)
                return new int[0];
            int end = Array.FindLastIndex(Levels, z => z <= level);
            if (end < 0)
                return new int[0];
            count = Math.Min(count, 4);
            int start = Math.Max(end - count, 0);
            int[] result = new int[end - start + 1];
            Array.Copy(Moves, start, result, 0, result.Length);
            return result;
        }
        /// <summary>Returns the index of the lowest level move if the Pokémon were encountered at the specified level.</summary>
        /// <remarks>Helps determine the minimum level an encounter can be at.</remarks>
        /// <param name="level">The level the Pokémon was encountered at.</param>
        /// <returns>Array of Move IDs</returns>
        public int getMinMoveLevel(int level)
        {
            if (Levels.Length == 0)
                return 1;

            int end = Array.FindLastIndex(Levels, z => z <= level);
            return Math.Max(end - 4, 1);
        }

        /// <summary>Returns the level that a Pokémon can learn the specified move.</summary>
        /// <param name="move">Move ID</param>
        /// <returns>Level the move is learned at. If the result is below 0, it cannot be learned by levelup.</returns>
        public int getLevelLearnMove(int move)
        {
            int index = Array.IndexOf(Moves, move);
            return index < 0 ? 0 : Levels[index];
        }
    }

    public class Learnset1 : Learnset
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
        public static Learnset[] getArray(byte[] input, int maxSpecies)
        {
            var data = new Learnset[maxSpecies + 1];

            int offset = 0;
            for (int s = 0; s < data.Length; s++)
                data[s] = new Learnset1(input, ref offset);

            return data;
        }
    }
    public class Learnset6 : Learnset
    {
        private Learnset6(byte[] data)
        {
            if (data.Length < 4 || data.Length % 4 != 0)
            { Count = 0; Levels = new int[0]; Moves = new int[0]; return; }
            Count = data.Length / 4 - 1;
            Moves = new int[Count];
            Levels = new int[Count];
            using (BinaryReader br = new BinaryReader(new MemoryStream(data)))
                for (int i = 0; i < Count; i++)
                {
                    Moves[i] = br.ReadInt16();
                    Levels[i] = br.ReadInt16();
                }
        }
        public static Learnset[] getArray(byte[][] entries)
        {
            Learnset[] data = new Learnset[entries.Length];
            for (int i = 0; i < data.Length; i++)
                data[i] = new Learnset6(entries[i]);
            return data;
        }
    }
    public class Learnset7 : Learnset
    {
        private Learnset7(byte[] data)
        {
            if (data.Length < 4 || data.Length % 4 != 0)
            { Count = 0; Levels = new int[0]; Moves = new int[0]; return; }
            Count = data.Length / 4 - 1;
            Moves = new int[Count];
            Levels = new int[Count];
            using (BinaryReader br = new BinaryReader(new MemoryStream(data)))
                for (int i = 0; i < Count; i++)
                {
                    Moves[i] = br.ReadInt16();
                    Levels[i] = br.ReadInt16();
                }
        }
        public static Learnset[] getArray(byte[][] entries)
        {
            Learnset[] data = new Learnset[entries.Length];
            for (int i = 0; i < data.Length; i++)
                data[i] = new Learnset7(entries[i]);
            return data;
        }
    }
}
