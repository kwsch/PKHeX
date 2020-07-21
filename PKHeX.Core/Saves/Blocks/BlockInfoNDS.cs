using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Gen5 Block Info
    /// </summary>
    public sealed class BlockInfoNDS : BlockInfo
    {
        private readonly int ChecksumOffset;
        private readonly int ChecksumMirror;

        public BlockInfoNDS(int offset, int length, int chkOffset, int chkMirror)
        {
            Offset = offset;
            Length = length;
            ChecksumOffset = chkOffset;
            ChecksumMirror = chkMirror;
        }

        private ushort GetChecksum(byte[] data) => Checksums.CRC16_CCITT(data, Offset, Length);

        protected override bool ChecksumValid(byte[] data)
        {
            ushort chk = GetChecksum(data);
            if (chk != BitConverter.ToUInt16(data, ChecksumOffset))
                return false;
            if (chk != BitConverter.ToUInt16(data, ChecksumMirror))
                return false;
            return true;
        }

        protected override void SetChecksum(byte[] data)
        {
            ushort chk = GetChecksum(data);
            var bytes = BitConverter.GetBytes(chk);
            bytes.CopyTo(data, ChecksumOffset);
            bytes.CopyTo(data, ChecksumMirror);
        }
    }
}