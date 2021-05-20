namespace PKHeX.Core
{
    public sealed class ShadowInfoEntryXD
    {
        public readonly byte[] Data;
        internal const int SIZE_ENTRY = 72;

        public ShadowInfoEntryXD() => Data = new byte[SIZE_ENTRY];
        public ShadowInfoEntryXD(byte[] data) => Data = data;

        public bool IsSnagged => Data[0] >> 6 != 0;
        public bool IsPurified { get => Data[0] >> 7 == 1; set { Data[0] &= 0x7F; if (value) Data[0] |= 0x80; } }

        public int IV_HP  { get => Data[0x0B]; set => Data[0x0B] = (byte)value; }
        public int IV_ATK { get => Data[0x0C]; set => Data[0x0C] = (byte)value; }
        public int IV_DEF { get => Data[0x0D]; set => Data[0x0D] = (byte)value; }
        public int IV_SPA { get => Data[0x0E]; set => Data[0x0E] = (byte)value; }
        public int IV_SPD { get => Data[0x0F]; set => Data[0x0F] = (byte)value; }
        public int IV_SPE { get => Data[0x10]; set => Data[0x10] = (byte)value; }

        public int Species { get => BigEndian.ToUInt16(Data, 0x1A); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x1A); }
        public uint PID { get => BigEndian.ToUInt32(Data, 0x1C); set => BigEndian.GetBytes(value).CopyTo(Data, 0x1C); }
        public int Purification { get => BigEndian.ToInt32(Data, 0x24); set => BigEndian.GetBytes(value).CopyTo(Data, 0x24); }

        public uint EXP { get => BigEndian.ToUInt32(Data, 0x04) >> 12; set => BigEndian.GetBytes((BigEndian.ToUInt32(Data, 0x04) & 0xFFF) | (value << 12)).CopyTo(Data, 0x04); }
        public bool IsEmpty => Species == 0;

        public int Index { get => Data[0x3F]; set => Data[0x3F] = (byte)value; }

        public override string ToString() => $"{(Species) Species} 0x{PID:X8} {Purification}";
    }
}
