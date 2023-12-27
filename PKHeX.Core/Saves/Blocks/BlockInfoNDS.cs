using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

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

    private ushort GetChecksum(ReadOnlySpan<byte> data) => Checksums.CRC16_CCITT(data.Slice(Offset, Length));

    protected override bool ChecksumValid(ReadOnlySpan<byte> data)
    {
        ushort chk = GetChecksum(data);
        if (chk != ReadUInt16LittleEndian(data[ChecksumOffset..]))
            return false;
        if (chk != ReadUInt16LittleEndian(data[ChecksumMirror..]))
            return false;
        return true;
    }

    protected override void SetChecksum(Span<byte> data)
    {
        ushort chk = GetChecksum(data);
        WriteUInt16LittleEndian(data[ChecksumOffset..], chk);
        WriteUInt16LittleEndian(data[ChecksumMirror..], chk);
    }
}
