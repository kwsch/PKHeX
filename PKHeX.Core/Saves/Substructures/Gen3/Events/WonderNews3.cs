using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core
{
    public sealed class WonderNews3 : Gen3MysteryData
    {
        /// <summary>
        /// 0x1C0: Total Size of this object
        /// </summary>
        public const int SIZE = sizeof(uint) + 444;

        public WonderNews3(byte[] data) : base(data)
        {
            if (data.Length != SIZE)
                throw new ArgumentException("Invalid size.", nameof(data));
        }

        public ushort NewsID   { get => ReadUInt16LittleEndian(Data.AsSpan(4)); set => WriteUInt16LittleEndian(Data.AsSpan(4), value); }
        public byte Flag { get => Data[6]; set => Data[6] = value; }
        public byte Color { get => Data[7]; set => Data[7] = value; }
    }
}
