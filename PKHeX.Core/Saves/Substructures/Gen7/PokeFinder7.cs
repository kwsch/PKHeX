using System;

namespace PKHeX.Core
{
    public sealed class PokeFinder7 : SaveBlock
    {
        public PokeFinder7(SAV7SM sav, int offset) : base(sav) => Offset = offset;
        public PokeFinder7(SAV7USUM sav, int offset) : base(sav) => Offset = offset;

        public ushort CameraVersion
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x00);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x00);
        }

        public bool GyroFlag
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x02) == 1;
            set => BitConverter.GetBytes((ushort)(value ? 1 : 0)).CopyTo(Data, Offset + 0x02);
        }

        public uint SnapCount
        {
            get => BitConverter.ToUInt32(Data, Offset + 0x04);
            set
            {
                if (value > 9999999) // Top bound is unchecked, check anyway
                    value = 9999999;
                BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x04);
            }
        }

        public uint ThumbsTotalValue
        {
            get => BitConverter.ToUInt32(Data, Offset + 0x0C);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x0C);
        }

        public uint ThumbsHighValue
        {
            get => BitConverter.ToUInt32(Data, Offset + 0x10);
            set
            {
                if (value > 9_999_999)
                    value = 9_999_999;
                BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x10);

                if (value > ThumbsTotalValue)
                    ThumbsTotalValue = value;
            }
        }

        public ushort TutorialFlags
        {
            get => BitConverter.ToUInt16(Data, Offset + 0x14);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x14);
        }
    }
}