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

    public sealed class Musical5 : SaveBlock
    {
        public Musical5(SAV5BW SAV, int offset) : base(SAV) => Offset = offset;
        public Musical5(SAV5B2W2 SAV, int offset) : base(SAV) => Offset = offset;

        private const int PropOffset = 0x258;

        public void UnlockAllMusicalProps()
        {
            // 101 props, which is 12.X bytes of bitflags.
            var bitFieldOffset = Offset + PropOffset;
            for (int i = 0; i < 0xC; i++)
                Data[bitFieldOffset + i] = 0xFF;
            Data[bitFieldOffset + 0xC] = 0x1F; // top 3 bits unset, to complete multiple of 8 (101=>104 bits).
        }

        public bool GetHasProp(int prop)
        {
            var bitFieldOffset = Offset + PropOffset;
            var bitOffset = prop >> 3;
            return SAV.GetFlag(bitFieldOffset + bitOffset, prop & 7);
        }

        public void SetHasProp(int prop, bool value = true)
        {
            var bitFieldOffset = Offset + PropOffset;
            var bitOffset = prop >> 3;
            SAV.SetFlag(bitFieldOffset + bitOffset, prop & 7, value);
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
