using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 6 Secret Base Decoration Good Inventory stock for a given good-index.
    /// </summary>
    public class SecretBase6GoodStock
    {
        public const int SIZE = 4;

        public ushort Count { get; set; }
        public bool IsNew { get; set; }

        public SecretBase6GoodStock(byte[] data, int offset)
        {
            Count = BitConverter.ToUInt16(data, offset);
            IsNew = data[offset + 2] != 0;
        }

        public void Write(byte[] data, int offset)
        {
            BitConverter.GetBytes(Count).CopyTo(data, offset);
            data[offset + 2] = (byte)(IsNew ? 1 : 0);
            data[offset + 3] = 0;
        }
    }
}
