using System;

namespace PKHeX.Core
{
    public sealed class MysteryEvent3 : Gen3MysteryData
    {
        public const int SIZE = sizeof(uint) + 1000; // total 0x3EC

        public MysteryEvent3(byte[] data) : base(data)
        {
            if (data.Length != SIZE)
                throw new ArgumentException("Invalid size.", nameof(data));
        }

        public byte Magic { get => Data[4]; set => Data[4] = value; }
        public byte MapGroup { get => Data[5]; set => Data[5] = value; }
        public byte MapNumber { get => Data[6]; set => Data[6] = value; }
        public byte ObjectID { get => Data[7]; set => Data[7] = value; }
    }
}
