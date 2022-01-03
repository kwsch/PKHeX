using System;
using static System.Buffers.Binary.BinaryPrimitives;

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

        public BlockInfoRSBOX(ReadOnlySpan<byte> data, int offset)
        {
            Offset = offset;
            Length = 4 + ChecksumRegionSize;

            // Values stored in Big Endian format
            OriginalChecksum = ReadUInt32BigEndian(data[Offset..]);
            ID = ReadUInt32BigEndian(data[(Offset + 4)..]);
            SaveCount = ReadUInt32BigEndian(data[(Offset + 8)..]);
        }

        protected override bool ChecksumValid(Span<byte> data)
        {
            var chk = GetChecksum(data);
            var old = ReadUInt32BigEndian(data[Offset..]);
            return chk == old;
        }

        protected override void SetChecksum(Span<byte> data)
        {
            var chk = GetChecksum(data);
            var span = data[Offset..];
            WriteUInt32BigEndian(span, chk);
        }

        private uint GetChecksum(Span<byte> data)
        {
            var span = data.Slice(Offset + 4, ChecksumRegionSize);
            return Checksums.CheckSum16BigInvert(span);
        }
    }
}
