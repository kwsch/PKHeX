using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core
{
    public sealed class HallOfFame7 : SaveBlock
    {
        public HallOfFame7(SAV7SM sav, int offset) : base(sav) => Offset = offset;
        public HallOfFame7(SAV7USUM sav, int offset) : base(sav) => Offset = offset;

        // this HoF region is immediately after the Event Flags
        private const int MaxCount = 12;
        public int First1 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x00)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x00), (ushort)value); }
        public int First2 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x02)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x02), (ushort)value); }
        public int First3 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x04)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x04), (ushort)value); }
        public int First4 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x04)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x04), (ushort)value); }
        public int First5 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x06)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x06), (ushort)value); }
        public int First6 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x08)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x08), (ushort)value); }

        public int Current1 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x0A)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x0A), (ushort)value); }
        public int Current2 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x0C)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x0C), (ushort)value); }
        public int Current3 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x0E)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x0E), (ushort)value); }
        public int Current4 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x10)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x10), (ushort)value); }
        public int Current5 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x12)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x12), (ushort)value); }
        public int Current6 { get => ReadUInt16LittleEndian(Data.AsSpan(Offset + 0x14)); set => WriteUInt16LittleEndian(Data.AsSpan(Offset + 0x14), (ushort)value); }

        public int GetEntry(int index)
        {
            if ((uint)index >= MaxCount)
                throw new ArgumentOutOfRangeException(nameof(index));
            return ReadUInt16LittleEndian(SAV.Data.AsSpan(Offset + (index * 2)));
        }

        public void SetEntry(int index, ushort value)
        {
            if ((uint)index >= MaxCount)
                throw new ArgumentOutOfRangeException(nameof(index));
            WriteUInt16LittleEndian(SAV.Data.AsSpan(Offset + (index * 2)), value);
        }
    }
}
