using System;

namespace PKHeX.Core
{
    public sealed class SecretBase6GoodPlacement
    {
        public const int SIZE = 12;

        public ushort Good { get; set; }
        public ushort X { get; set; }
        public ushort Y { get; set; }
        public byte Rotation { get; set; }
        // byte unused

        public ushort Param1 { get; set; }
        public ushort Param2 { get; set; }

        public SecretBase6GoodPlacement(byte[] data, int offset)
        {
            Good = BitConverter.ToUInt16(data, offset);
            X = BitConverter.ToUInt16(data, offset + 2);
            Y = BitConverter.ToUInt16(data, offset + 4);
            Rotation = data[offset + 6];

            Param1 = BitConverter.ToUInt16(data, offset + 8);
            Param2 = BitConverter.ToUInt16(data, offset + 10);
        }

        public void Write(byte[] data, int offset)
        {
            BitConverter.GetBytes(Good).CopyTo(data, offset);
            BitConverter.GetBytes(X).CopyTo(data, offset + 2);
            BitConverter.GetBytes(Y).CopyTo(data, offset + 4);
            data[6] = Rotation;

            BitConverter.GetBytes(Param1).CopyTo(data, offset + 8);
            BitConverter.GetBytes(Param2).CopyTo(data, offset + 10);
        }
    }
}