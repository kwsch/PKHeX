using System;

namespace PKHeX.Core
{
    public class EntreeSlot
    {
        public int Species // bits 0-10
        {
            get => (int)(RawValue & 0x3FF) >> 0;
            set => RawValue = (RawValue & 0xFFFF_F800) | ((uint)(value & 0x3FF) << 0);
        }
        public int Move // bits 11-20
        {
            get => (int)(RawValue & 0x001F_F800) >> 11;
            set => RawValue = (RawValue & 0xFFF0_02FF) | ((uint)(value & 0x3FF) << 11);
        }
        public int Gender // bits 21-22
        {
            get => (int)(RawValue & 0x0060_0000) >> 21;
            set => RawValue = (RawValue & 0xFF9F_FFFF) | ((uint)(value & 0x3) << 21);
        }
        public int Form // bits 23-27
        {
            get => (int)(RawValue & 0x0F80_0000) >> 23;
            set => RawValue = (RawValue & 0xF07F_FFFF) | ((uint)(value & 0x1F) << 23);
        }
        public bool Invisible // bit 28
        {
            get => ((RawValue >> 28) & 1) == 1;
            set => RawValue = (RawValue & 0xEFFFFFFF) | (value ? 0 : 1u << 28);
        }
        public int Animation // bits 29-31
        {
            get => (int)(RawValue >> 29);
            set => RawValue = (RawValue << 3) >> 3 | (uint)((value & 0x7) << 29);
        }

        private readonly byte[] Data;
        private readonly int Offset;

        public uint RawValue
        {
            get => BitConverter.ToUInt32(Data, Offset);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset);
        }

        public void Delete() => RawValue = 0;

        public EntreeForestArea Area { get; internal set; }

        public EntreeSlot(byte[] data, int ofs)
        {
            Data = data;
            Offset = ofs;
        }
    }
}