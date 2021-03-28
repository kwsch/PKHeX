using System;

namespace PKHeX.Core
{
    public sealed class WonderCard3Extra : Gen3MysteryData
    {
        /// <summary>
        /// 0x28: Total Size of this object
        /// </summary>
        public const int SIZE = sizeof(uint) + 36;

        public WonderCard3Extra(byte[] data) : base(data)
        {
            if (data.Length != SIZE)
                throw new ArgumentException("Invalid size.", nameof(data));
        }

        public ushort Wins   { get => BitConverter.ToUInt16(Data, 0x4); set => BitConverter.GetBytes(value).CopyTo(Data, 0x4); }
        public ushort Losses { get => BitConverter.ToUInt16(Data, 0x6); set => BitConverter.GetBytes(value).CopyTo(Data, 0x6); }
        public ushort Trades { get => BitConverter.ToUInt16(Data, 0x8); set => BitConverter.GetBytes(value).CopyTo(Data, 0x8); }
        public ushort Unk    { get => BitConverter.ToUInt16(Data, 0xA); set => BitConverter.GetBytes(value).CopyTo(Data, 0xA); }

        // u16 distributedMons[2][7]
    }
}
