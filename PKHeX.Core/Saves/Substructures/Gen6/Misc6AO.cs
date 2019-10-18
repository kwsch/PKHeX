using System;

namespace PKHeX.Core
{
    public interface IMisc6
    {
        uint Money { get; set; }
    }

    public sealed class Misc6AO : SaveBlock, IMisc6
    {
        public Misc6AO(SAV6AO sav, int offset) : base(sav) => Offset = offset;
        public Misc6AO(SAV6AODemo sav, int offset) : base(sav) => Offset = offset;

        public uint Money
        {
            get => BitConverter.ToUInt32(Data, Offset + 0x8);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x8);
        }

        public int Badges
        {
            get => Data[Offset + 0xC];
            set => Data[Offset + 0xC] = (byte)value;
        }

        public int BP
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x30);
            set => BitConverter.GetBytes((ushort)value).CopyTo(Data, Offset + 0x30);
        }

        public int Vivillon
        {
            get => Data[Offset + 0x44];
            set => Data[Offset + 0x44] = (byte)value;
        }
    }
}