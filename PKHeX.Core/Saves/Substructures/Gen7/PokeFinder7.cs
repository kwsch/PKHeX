using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core
{
    public sealed class PokeFinder7 : SaveBlock
    {
        public PokeFinder7(SAV7SM sav, int offset) : base(sav) => Offset = offset;
        public PokeFinder7(SAV7USUM sav, int offset) : base(sav) => Offset = offset;

        public ushort CameraVersion
        {
            get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x00));
            set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x00), value);
        }

        public bool GyroFlag
        {
            get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x02)) == 1;
            set => BitConverter.GetBytes((ushort)(value ? 1 : 0)).CopyTo(Data, Offset + 0x02);
        }

        public uint SnapCount
        {
            get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x04));
            set
            {
                if (value > 9999999) // Top bound is unchecked, check anyway
                    value = 9999999;
                WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x04), value);
            }
        }

        public uint ThumbsTotalValue
        {
            get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x0C));
            set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x0C), value);
        }

        public uint ThumbsHighValue
        {
            get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x10));
            set
            {
                if (value > 9_999_999)
                    value = 9_999_999;
                WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x10), value);

                if (value > ThumbsTotalValue)
                    ThumbsTotalValue = value;
            }
        }

        public ushort TutorialFlags
        {
            get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x14));
            set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x14), value);
        }
    }
}