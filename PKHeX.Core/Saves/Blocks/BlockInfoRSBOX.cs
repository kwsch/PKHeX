namespace PKHeX.Core
{
    /// <summary>
    /// Gen3 <see cref="GameVersion.RSBOX"/> Block Info
    /// </summary>
    public sealed class BlockInfoRSBOX : BlockInfo
    {
        public readonly uint SaveCount;
        public readonly uint Checksum;

        public BlockInfoRSBOX(byte[] data, int offset)
        {
            Offset = offset;
            Length = 0x1FFC;

            // Values stored in Big Endian format
            Checksum = BigEndian.ToUInt32(data, Offset + 0);
            ID = BigEndian.ToUInt32(data, Offset + 4);
            SaveCount = BigEndian.ToUInt32(data, Offset + 8);
        }

        protected override bool ChecksumValid(byte[] data)
        {
            var chk = GetChecksum(data);
            var old = BigEndian.ToUInt32(data, Offset);
            return chk == old;
        }

        protected override void SetChecksum(byte[] data)
        {
            var chk = GetChecksum(data);
            data[Offset + 0] = (byte)(chk >> 24);
            data[Offset + 1] = (byte)(chk >> 16);
            data[Offset + 2] = (byte)(chk >> 8);
            data[Offset + 3] = (byte)(chk >> 0);
        }

        private uint GetChecksum(byte[] data)
        {
            int start = Offset + 4;
            int end = start + Length - 4;

            return GetChecksum(data, start, end);
        }

        private static uint GetChecksum(byte[] data, int start, int end)
        {
            ushort chk = 0; // initial value
            for (int j = start; j < end; j += 2)
                chk += BigEndian.ToUInt16(data, j);
            return (uint)(chk << 16 | (ushort)(0xF004 - chk));
        }
    }
}