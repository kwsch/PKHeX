using System;

namespace PKHeX.Core
{
    public sealed class WonderCard3 : Gen3MysteryData
    {
        /// <summary>
        /// 0x150: Total Size of this object
        /// </summary>
        public const int SIZE = sizeof(uint) + 332;

        public WonderCard3(byte[] data) : base(data)
        {
            if (data.Length != SIZE)
                throw new ArgumentException("Invalid size.", nameof(data));
        }

        public ushort CardID { get => BitConverter.ToUInt16(Data, 4); set => BitConverter.GetBytes(value).CopyTo(Data, 4); }
        public ushort Icon { get => BitConverter.ToUInt16(Data, 6); set => BitConverter.GetBytes(value).CopyTo(Data, 6); }
        public uint Count { get => BitConverter.ToUInt16(Data, 8); set => BitConverter.GetBytes(value).CopyTo(Data, 8); }

        public byte Type { get => (byte)(Data[0xC] & 0b11); set => Data[0xC] = (byte)((Data[0xC] & ~0b11) | (value & 0b11)); }
        public byte Color { get => (byte)(Data[0xC] & 0b00111100); set => Data[0xC] = (byte)((Data[0xC] & ~0b00111100) | (value & 0b00111100)); }
        public byte ShareState { get => (byte)(Data[0xC] & 0b11000000); set => Data[0xC] = (byte)((Data[0xC] & ~0b11000000) | (value & 0b11000000)); }

        public byte Count2 { get => Data[0xD]; set => Data[0xD] = value; }
    }
}
