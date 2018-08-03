using System;
using System.IO;
using System.Linq;

namespace PKHeX.Core
{
    public abstract class EggMoves
    {
        protected int Count;
        public int[] Moves;
        public int FormTableIndex;

        public bool GetHasEggMove(int move) => Moves.Contains(move);
    }

    public sealed class EggMoves2 : EggMoves
    {
        private EggMoves2(byte[] data)
        {
            Count = data.Length;
            Moves = data.Select(i => (int) i).ToArray();
        }

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
        private EggMoves6(byte[] data)
        {
            if (data.Length < 2 || data.Length % 2 != 0)
            { Count = 0; Moves = Array.Empty<int>(); return; }
            using (BinaryReader br = new BinaryReader(new MemoryStream(data)))
            {
                Moves = new int[Count = br.ReadUInt16()];
                for (int i = 0; i < Count; i++)
                    Moves[i] = br.ReadUInt16();
            }
        }

        public static EggMoves[] GetArray(byte[][] entries)
        {
            EggMoves[] data = new EggMoves[entries.Length];
            for (int i = 0; i < data.Length; i++)
                data[i] = new EggMoves6(entries[i]);
            return data;
        }
    }

    public sealed class EggMoves7 : EggMoves
    {
        private EggMoves7(byte[] data)
        {
            if (data.Length < 2 || data.Length % 2 != 0)
            { Count = 0; Moves = Array.Empty<int>(); return; }
            using (BinaryReader br = new BinaryReader(new MemoryStream(data)))
            {
                FormTableIndex = br.ReadUInt16();
                Count = br.ReadUInt16();
                Moves = new int[Count];
                for (int i = 0; i < Count; i++)
                    Moves[i] = br.ReadUInt16();
            }
        }

        public static EggMoves[] GetArray(byte[][] entries)
        {
            EggMoves[] data = new EggMoves[entries.Length];
            for (int i = 0; i < data.Length; i++)
                data[i] = new EggMoves7(entries[i]);
            return data;
        }
    }
}
