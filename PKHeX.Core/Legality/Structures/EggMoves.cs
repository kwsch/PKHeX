using System;
using System.Linq;

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
        private EggMoves2(byte[] data) : base(data.Select(i => (int)i).ToArray()) { }

        public static EggMoves[] GetArray(byte[] data, int count)
        {
            int[] ptrs = new int[count+1];
            int baseOffset = (data[1] << 8 | data[0]) - (count * 2);
            for (int i = 1; i < ptrs.Length; i++)
            {
                var ofs = (i - 1) * 2;
                ptrs[i] = (data[ofs + 1] << 8 | data[ofs]) - baseOffset;
            }

            EggMoves[] entries = new EggMoves[count + 1];
            entries[0] = new EggMoves2(Array.Empty<byte>());
            for (int i = 1; i < entries.Length; i++)
                entries[i] = new EggMoves2(data.Skip(ptrs[i]).TakeWhile(b => b != 0xFF).ToArray());

            return entries;
        }
    }

    public sealed class EggMoves6 : EggMoves
    {
        private static readonly EggMoves6 None = new EggMoves6(Array.Empty<int>());

        private EggMoves6(int[] moves) : base(moves) { }

        private static EggMoves6 Get(byte[] data)
        {
            if (data.Length < 2 || data.Length % 2 != 0)
                return None;

            int count = BitConverter.ToInt16(data, 0);
            var moves = new int[count];
            for (int i = 0; i < moves.Length; i++)
                moves[i] = BitConverter.ToInt16(data, 2 + (i * 2));
            return new EggMoves6(moves);
        }

        public static EggMoves6[] GetArray(byte[][] entries)
        {
            EggMoves6[] data = new EggMoves6[entries.Length];
            for (int i = 0; i < data.Length; i++)
                data[i] = Get(entries[i]);
            return data;
        }
    }

    public sealed class EggMoves7 : EggMoves
    {
        private static readonly EggMoves7 None = new EggMoves7(Array.Empty<int>());
        public readonly int FormTableIndex;

        private EggMoves7(int[] moves, int formIndex = 0) : base(moves) => FormTableIndex = formIndex;

        private static EggMoves7 Get(byte[] data)
        {
            if (data.Length < 2 || data.Length % 2 != 0)
                return None;

            int formIndex = BitConverter.ToInt16(data, 0);
            int count = BitConverter.ToInt16(data, 2);
            var moves = new int[count];
            for (int i = 0; i < moves.Length; i++)
                moves[i] = BitConverter.ToInt16(data, 4 + (i * 2));
            return new EggMoves7(moves, formIndex);
        }

        public static EggMoves7[] GetArray(byte[][] entries)
        {
            EggMoves7[] data = new EggMoves7[entries.Length];
            for (int i = 0; i < data.Length; i++)
                data[i] = Get(entries[i]);
            return data;
        }
    }
}
