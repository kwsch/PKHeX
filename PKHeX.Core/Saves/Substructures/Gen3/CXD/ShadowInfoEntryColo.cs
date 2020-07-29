namespace PKHeX.Core
{
    public sealed class ShadowInfoEntryColo
    {
        public readonly byte[] Data;
        private const int SIZE_ENTRY = 12;

        public ShadowInfoEntryColo() => Data = new byte[SIZE_ENTRY];
        public ShadowInfoEntryColo(byte[] data) => Data = data;

        public uint PID { get => BigEndian.ToUInt32(Data, 0x00); set => BigEndian.GetBytes(value).CopyTo(Data, 0x00); }
        public int Met_Location { get => BigEndian.ToUInt16(Data, 0x06); set => BigEndian.GetBytes((ushort)value).CopyTo(Data, 0x06); }
        public uint Unk08 { get => BigEndian.ToUInt32(Data, 0x08); set => BigEndian.GetBytes(value).CopyTo(Data, 0x08); }
    }
}
