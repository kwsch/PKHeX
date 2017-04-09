using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PKHeX.Core
{
    public abstract class Learnset
    {
        protected int Count;
        protected int[] Moves;
        protected int[] Levels;

        public int[] getMoves(int level)
        {
            if (level >= 100)
                return Moves;
            for (int i = 0; i < Levels.Length; i++)
                if (Levels[i] > level)
                    return Moves.Take(i).ToArray();
            return Moves;
        }

        public int[] getMoves(int minLevel, int maxLevel)
        {
            if (minLevel > Levels.LastOrDefault()) return new int[0];
            var skip = Levels.Select((m, i) => i).SkipWhile(i => Levels[i] < minLevel).FirstOrDefault();
            if (maxLevel > Levels.LastOrDefault()) return Moves.Skip(skip).ToArray();
            return Moves.Skip(skip).TakeWhile((m, i) => Levels[i + skip] <= maxLevel).ToArray();
        }
        public IEnumerable<int> getEncounterMoves(int level)
        {
            return getEncounterMoves(level, 4);
        }
        // Return the default moves a pokemon learn at the moment of its encounter level, 
        // the last 4 moves of its learned, or less if there is any special move
        // for generation 1 is not possible to learn any moves bellow this encounter moves
        public IEnumerable<int> getEncounterMoves(int level, int moveslots)
        {
            if (moveslots == 0 || !Levels.Any())
                return new int[0];
            var num = Math.Min(moveslots, 4);
            if (level < Levels.Last())
            {
                for (int i = 0; i < Levels.Length; i++)
                    if (Levels[i] > level)
                        return i <= num ? Moves.Take(i) : Moves.Skip(i - num).Take(num);
            }
            return Moves.Length <= num ? Moves: Moves.Skip(Moves.Length - num);
        }
        // Return for a pokemon with a given level encounter, the min level of the moves in the getEncounterMoves array
        // For generation 1 it should be used this level to compare to generation 2 encounter level to choose the lower encounter
        public int getMinMoveLevel(int level)
        {
            if (!Levels.Any())
                return 1;
            if (level < Levels.Last())
            {
                for (int i = 0; i < Levels.Length; i++)
                    if (Levels[i] > level)
                        return i < 4 ? 1 : Levels.Skip(i - 4).First();
            }
            return Levels.Length < 4 ? 1 : Levels.Skip(Levels.Length - 4).First();
        }
        public int getLevelLearnMove(int move)
        {
            return Moves.Select((m, i) => new { move = m, level = Levels[i] }).Where(m => m.move == move).Select(l => l.level).FirstOrDefault();
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
