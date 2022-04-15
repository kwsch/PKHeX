using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core
{
    public interface IMisc6
    {
        uint Money { get; set; }
    }

    public sealed class Misc6AO : SaveBlock<SAV6>, IMisc6
    {
        public Misc6AO(SAV6AO sav, int offset) : base(sav) => Offset = offset;
        public Misc6AO(SAV6AODemo sav, int offset) : base(sav) => Offset = offset;

        public uint Money
        {
            get => ReadUInt32LittleEndian(Data.AsSpan(Offset + 0x8));
            set => WriteUInt32LittleEndian(Data.AsSpan(Offset + 0x8), value);
        }

        public int Badges
        {
            get => Data[Offset + 0xC];
            set => Data[Offset + 0xC] = (byte)value;
        }

        public int BP
        {
            get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x30));
            set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x30), (ushort)value);
        }

        public int Vivillon
        {
            get => Data[Offset + 0x44];
            set => Data[Offset + 0x44] = (byte)value;
        }
    }
}