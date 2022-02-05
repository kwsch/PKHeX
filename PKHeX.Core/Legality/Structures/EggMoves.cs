using System;
using System.Linq;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core
{
    public abstract class EggMoves
    {
        public readonly int[] Moves;
        protected EggMoves(int[] moves) => Moves = moves;
        public bool GetHasEggMove(int move) => Moves.Contains(move);
    }

    public sealed class EggMoves2 : EggMoves
    {
        private EggMoves2(int[] moves) : base(moves) { }

        public static EggMoves2[] GetArray(ReadOnlySpan<byte> data, int count)
        {
            var entries = new EggMoves2[count + 1];
            var empty = entries[0] = new EggMoves2(Array.Empty<int>());

            int baseOffset = ReadInt16LittleEndian(data) - (count * 2);
            for (int i = 1; i < entries.Length; i++)
            {
                int start = ReadInt16LittleEndian(data[((i - 1) * 2)..]) - baseOffset;
                int end = data[start..].IndexOf((byte)0xFF) + start;
                if (start == end)
                {
                    entries[i] = empty;
                    continue;
                }

                int[] moves = new int[end - start];
                for (int m = start; m < end; m++)
                    moves[m - start] = data[m];

                entries[i] = new EggMoves2(moves);
            }

            return entries;
        }
    }

    public sealed class EggMoves6 : EggMoves
    {
        private static readonly EggMoves6 None = new(Array.Empty<int>());

        private EggMoves6(int[] moves) : base(moves) { }

        private static EggMoves6 Get(ReadOnlySpan<byte> data)
        {
            if (data.Length == 0)
                return None;

            int count = ReadInt16LittleEndian(data);
            var moves = new int[count];
            var span = data[2..];
            for (int i = 0; i < moves.Length; i++)
                moves[i] = ReadInt16LittleEndian(span[(i * 2)..]);
            return new EggMoves6(moves);
        }

        public static EggMoves6[] GetArray(BinLinkerAccessor entries)
        {
            EggMoves6[] data = new EggMoves6[entries.Length];
            for (int i = 0; i < data.Length; i++)
                data[i] = Get(entries[i]);
            return data;
        }
    }

    public sealed class EggMoves7 : EggMoves
    {
        private static readonly EggMoves7 None = new(Array.Empty<int>());
        public readonly int FormTableIndex;

        private EggMoves7(int[] moves, int formIndex = 0) : base(moves) => FormTableIndex = formIndex;

        private static EggMoves7 Get(ReadOnlySpan<byte> data)
        {
            if (data.Length == 0)
                return None;

            int formIndex = ReadInt16LittleEndian(data);
            int count = ReadInt16LittleEndian(data[2..]);
            var moves = new int[count];

            var moveSpan = data[4..];
            for (int i = 0; i < moves.Length; i++)
                moves[i] = ReadInt16LittleEndian(moveSpan[(i * 2)..]);
            return new EggMoves7(moves, formIndex);
        }

        public static EggMoves7[] GetArray(BinLinkerAccessor entries)
        {
            EggMoves7[] data = new EggMoves7[entries.Length];
            for (int i = 0; i < data.Length; i++)
                data[i] = Get(entries[i]);
            return data;
        }
    }
}
