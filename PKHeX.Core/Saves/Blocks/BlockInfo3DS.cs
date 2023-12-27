using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Gen6+ Block Info (inside BEEF chunk)
/// </summary>
public abstract class BlockInfo3DS : BlockInfo
{
    private readonly int BlockInfoOffset;

    // ==chunk def== @ BlockInfoOffset
    // u64 timestamp1
    // u64 timestamp2
    // u8[4] BEEF magic
    // n*{blockInfo}, where n varies per sav type

    // ==block info def==
    // u32 length
    // u16 id
    // u16 checksum

    // when stored, each block size is rounded up to nearest 0x200, and the next chunk is immediately after

    protected BlockInfo3DS(int bo, uint id, int ofs, int len)
    {
        BlockInfoOffset = bo;
        ID = id;
        Offset = ofs;
        Length = len;
    }

    private int ChecksumOffset => BlockInfoOffset + 0x14 + ((int)ID * 8) + 6;
    protected abstract ushort GetChecksum(ReadOnlySpan<byte> data);

    protected override bool ChecksumValid(ReadOnlySpan<byte> data)
    {
        ushort chk = GetChecksum(data);
        var old = ReadUInt16LittleEndian(data[ChecksumOffset..]);
        return chk == old;
    }

    protected override void SetChecksum(Span<byte> data)
    {
        ushort chk = GetChecksum(data);
        WriteUInt16LittleEndian(data[ChecksumOffset..], chk);
    }
}

public sealed class BlockInfo6(int bo, uint id, int ofs, int len) : BlockInfo3DS(bo, id, ofs, len)
{
    protected override ushort GetChecksum(ReadOnlySpan<byte> data) => Checksums.CRC16_CCITT(data.Slice(Offset, Length));
}

public sealed class BlockInfo7(int bo, uint id, int ofs, int len) : BlockInfo3DS(bo, id, ofs, len)
{
    protected override ushort GetChecksum(ReadOnlySpan<byte> data) => Checksums.CRC16Invert(data.Slice(Offset, Length));
}

public sealed class BlockInfo7b(int bo, uint id, int ofs, int len) : BlockInfo3DS(bo, id, ofs, len)
{
    protected override ushort GetChecksum(ReadOnlySpan<byte> data) => Checksums.CRC16NoInvert(data.Slice(Offset, Length));
}
