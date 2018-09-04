using System;

namespace PKHeX.WinForms
{
    public abstract class PokeDex
    {
        public bool[] Owned { get; protected set; }

        protected static bool[] SetBits(byte[] data, int offset, int length)
        {
            byte[] d = new byte[length];
            Array.Copy(data, offset, d, 0, length);
            bool[] b = new bool[8 * d.Length];
            for (int i = 0; i < b.Length; i++)
                b[i] = (d[i / 8] & 1 << (i & 7)) != 0;
            return b;
        }

        protected static byte[] SetBits(bool[] b)
        {
            byte[] data = new byte[b.Length / 8];
            for (int i = 0; i < b.Length; i++)
                data[i / 8] |= (byte)(b[i] ? 1 << (i & 7) : 0);
            return data;
        }
    }
}