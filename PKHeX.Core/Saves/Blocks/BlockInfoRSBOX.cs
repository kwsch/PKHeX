using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Gen3 <see cref="GameVersion.RSBOX"/> Block Info
    /// </summary>
    public sealed class BlockInfoRSBOX : BlockInfo
    {
        public readonly uint SaveCount;
        public readonly uint OriginalChecksum;
        private const int ChecksumRegionSize = 0x1FF8;

        public BlockInfoRSBOX(byte[] data, int offset)
        {
            Offset = offset;
            Length = 4 + ChecksumRegionSize;

            // Values stored in Big Endian format
            OriginalChecksum = BigEndian.ToUInt32(data, Offset);
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
            var span = new ReadOnlySpan<byte>(data, start, ChecksumRegionSize);
            return Checksums.CheckSum16BigInvert(span);
        }
    }
}
