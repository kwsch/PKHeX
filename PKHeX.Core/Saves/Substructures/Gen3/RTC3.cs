using System;

namespace PKHeX.Core
{
    public sealed class RTC3
    {
        public readonly byte[] Data;
        public const int Size = 8;

        public RTC3(byte[] data) => Data = data;

        public int Day { get => BitConverter.ToUInt16(Data, 0x00); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x00); }
        public int Hour { get => Data[2]; set => Data[2] = (byte)value; }
        public int Minute { get => Data[3]; set => Data[3] = (byte)value; }
        public int Second { get => Data[4]; set => Data[4] = (byte)value; }
    }
}