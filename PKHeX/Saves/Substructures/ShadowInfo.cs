namespace PKHeX
{
    public class ShadowInfoXD
    {
        private readonly byte[] Data;
        private const int SIZE_ENTRY = 72;
        public ShadowInfoXD(byte[] data)
        {
            Data = (byte[])(data?.Clone() ?? new byte[SIZE_ENTRY]);
        }

        public bool IsSnagged => Data[0] >> 6 != 0;
        public bool IsPurified { get { return Data[0] >> 7 == 1; } set { Data[0] &= 0x7F; if (value) Data[0] |= 0x80; } }
        public ushort Species { get { return BigEndian.ToUInt16(Data, 0x1A); } set { BigEndian.GetBytes(value).CopyTo(Data, 0x1A); } }
        public uint PID { get { return BigEndian.ToUInt32(Data, 0x1C); }set { BigEndian.GetBytes(value).CopyTo(Data, 0x1C); } }
        public int Purification { get { return BigEndian.ToInt32(Data, 0x24); } set { BigEndian.GetBytes(value).CopyTo(Data, 0x24); } }

        public uint EXP { get { return BigEndian.ToUInt32(Data, 0x04) >> 12; } set { BigEndian.GetBytes((BigEndian.ToUInt32(Data, 0x04) & 0xFFF) | (value << 12)).CopyTo(Data, 0x04); } }
    }

    public class ShadowInfoColo
    {
        private readonly byte[] Data;
        private const int SIZE_ENTRY = 12;
        public ShadowInfoColo(byte[] data = null)
        {
            Data = (byte[])(data?.Clone() ?? new byte[SIZE_ENTRY]);
        }
        public uint PID { get { return BigEndian.ToUInt32(Data, 0x00); } set { BigEndian.GetBytes(value).CopyTo(Data, 0x00); } }
        public int Met_Location { get { return BigEndian.ToUInt16(Data, 0x06); } set { BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x06); } }
        public uint _0x08 { get { return BigEndian.ToUInt32(Data, 0x08); } set { BigEndian.GetBytes(value).CopyTo(Data, 0x08); } }
    }
}