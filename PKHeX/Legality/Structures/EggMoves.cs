using System.IO;

namespace PKHeX
{
    public abstract class EggMoves
    {
        protected int Count;
        public int[] Moves;
        public int FormTableIndex;
    }
    public class EggMoves6 : EggMoves
    {
        private EggMoves6(byte[] data)
        {
            if (data.Length < 2 || data.Length % 2 != 0)
            { Count = 0; Moves = new int[0]; return; }
            using (BinaryReader br = new BinaryReader(new MemoryStream(data)))
            {
                Moves = new int[Count = br.ReadUInt16()];
                for (int i = 0; i < Count; i++)
                    Moves[i] = br.ReadUInt16();
            }
        }
        public static EggMoves[] getArray(byte[][] entries)
        {
            EggMoves6[] data = new EggMoves6[entries.Length];
            for (int i = 0; i < data.Length; i++)
                data[i] = new EggMoves6(entries[i]);
            return data;
        }
    }
    public class EggMoves7 : EggMoves
    {
        private EggMoves7(byte[] data)
        {
            if (data.Length < 2 || data.Length % 2 != 0)
            { Count = 0; Moves = new int[0]; return; }
            using (BinaryReader br = new BinaryReader(new MemoryStream(data)))
            {
                FormTableIndex = br.ReadUInt16();
                Count = br.ReadUInt16();
                Moves = new int[Count];
                for (int i = 0; i < Count; i++)
                    Moves[i] = br.ReadUInt16();
            }
        }
        public static EggMoves[] getArray(byte[][] entries)
        {
            EggMoves7[] data = new EggMoves7[entries.Length];
            for (int i = 0; i < data.Length; i++)
                data[i] = new EggMoves7(entries[i]);
            return data;
        }
    }
}
