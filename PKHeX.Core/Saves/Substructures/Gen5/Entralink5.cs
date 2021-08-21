using System;

namespace PKHeX.Core
{
    public abstract class Entralink5 : SaveBlock
    {
        protected Entralink5(SAV5 SAV, int offset) : base(SAV) => Offset = offset;

        public ushort WhiteForestLevel
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x0C);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x0C);
        }

        public ushort BlackCityLevel
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x0E);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x0E);
        }
    }

    public sealed class Entralink5BW : Entralink5
    {
        public Entralink5BW(SAV5BW SAV, int offset) : base(SAV, offset) { }
    }

    public sealed class Entralink5B2W2 : Entralink5
    {
        public Entralink5B2W2(SAV5B2W2 SAV, int offset) : base(SAV, offset) { }

        public byte PassPower1
        {
            get => Data[Offset + 0x1A0];
            set => Data[Offset + 0x1A0] = value;
        }

        public byte PassPower2
        {
            get => Data[Offset + 0x1A1];
            set => Data[Offset + 0x1A1] = value;
        }

        public byte PassPower3
        {
            get => Data[Offset + 0x1A2];
            set => Data[Offset + 0x1A2] = value;
        }
    }
}
