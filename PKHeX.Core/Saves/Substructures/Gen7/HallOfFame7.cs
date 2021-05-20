using System;

namespace PKHeX.Core
{
    public sealed class HallOfFame7 : SaveBlock
    {
        public HallOfFame7(SAV7SM sav, int offset) : base(sav) => Offset = offset;
        public HallOfFame7(SAV7USUM sav, int offset) : base(sav) => Offset = offset;

        // this HoF region is immediately after the Event Flags
        private const int MaxCount = 12;
        public int First1 { get => BitConverter.ToUInt16(SAV.Data, Offset + 0x00); set => SAV.SetData(BitConverter.GetBytes((ushort)value), Offset + 0x00); }
        public int First2 { get => BitConverter.ToUInt16(SAV.Data, Offset + 0x02); set => SAV.SetData(BitConverter.GetBytes((ushort)value), Offset + 0x02); }
        public int First3 { get => BitConverter.ToUInt16(SAV.Data, Offset + 0x04); set => SAV.SetData(BitConverter.GetBytes((ushort)value), Offset + 0x04); }
        public int First4 { get => BitConverter.ToUInt16(SAV.Data, Offset + 0x04); set => SAV.SetData(BitConverter.GetBytes((ushort)value), Offset + 0x04); }
        public int First5 { get => BitConverter.ToUInt16(SAV.Data, Offset + 0x06); set => SAV.SetData(BitConverter.GetBytes((ushort)value), Offset + 0x06); }
        public int First6 { get => BitConverter.ToUInt16(SAV.Data, Offset + 0x08); set => SAV.SetData(BitConverter.GetBytes((ushort)value), Offset + 0x08); }

        public int Current1 { get => BitConverter.ToUInt16(SAV.Data, Offset + 0x0A); set => SAV.SetData(BitConverter.GetBytes((ushort)value), Offset + 0x0A); }
        public int Current2 { get => BitConverter.ToUInt16(SAV.Data, Offset + 0x0C); set => SAV.SetData(BitConverter.GetBytes((ushort)value), Offset + 0x0C); }
        public int Current3 { get => BitConverter.ToUInt16(SAV.Data, Offset + 0x0E); set => SAV.SetData(BitConverter.GetBytes((ushort)value), Offset + 0x0E); }
        public int Current4 { get => BitConverter.ToUInt16(SAV.Data, Offset + 0x10); set => SAV.SetData(BitConverter.GetBytes((ushort)value), Offset + 0x10); }
        public int Current5 { get => BitConverter.ToUInt16(SAV.Data, Offset + 0x12); set => SAV.SetData(BitConverter.GetBytes((ushort)value), Offset + 0x12); }
        public int Current6 { get => BitConverter.ToUInt16(SAV.Data, Offset + 0x14); set => SAV.SetData(BitConverter.GetBytes((ushort)value), Offset + 0x14); }

        public int GetEntry(int index)
        {
            if ((uint)index >= MaxCount)
                throw new ArgumentOutOfRangeException(nameof(index));
            return BitConverter.ToUInt16(SAV.Data, Offset + (index * 2));
        }

        public void SetEntry(int index, ushort value)
        {
            if ((uint)index >= MaxCount)
                throw new ArgumentOutOfRangeException(nameof(index));
            SAV.SetData(BitConverter.GetBytes(value), Offset + (index * 2));
        }
    }
}
