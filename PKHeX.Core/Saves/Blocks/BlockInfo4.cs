using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Gen4 Extra Block Info
/// </summary>
public sealed class BlockInfo4 : BlockInfo
{
    private const int SIZE_FOOTER = 0x10;
    private readonly int FooterOffset;

    public BlockInfo4(uint id, int offset, int length)
    {
        ID = id;
        Offset = offset;
        Length = length;
        FooterOffset = offset + length - SIZE_FOOTER;
    }

    public uint GetKey(ReadOnlySpan<byte> data) => ReadUInt32LittleEndian(data[Offset..]);
    public uint GetMagic(ReadOnlySpan<byte> data) => ReadUInt32LittleEndian(data[FooterOffset..]);
    public uint GetRevision(ReadOnlySpan<byte> data) => ReadUInt32LittleEndian(data[(FooterOffset + 0x4)..]);
    public int GetSize(ReadOnlySpan<byte> data) => ReadInt32LittleEndian(data[(FooterOffset + 0x8)..]);
    public ushort GetID(ReadOnlySpan<byte> data) => ReadUInt16LittleEndian(data[(FooterOffset + 0xC)..]);
    private ushort GetChecksum(ReadOnlySpan<byte> data) => Checksums.CRC16_CCITT(data.Slice(Offset, Length - 2));

    private bool IsInitialized(ReadOnlySpan<byte> data)
    {
        return (ID == 0 && GetRevision(data) != 0xFFFFFFFF) || (ID != 0 && GetKey(data) != 0xFFFFFFFF);
    }

    public bool SizeValid(ReadOnlySpan<byte> data)
    {
        return GetSize(data) == Length;
    }

    protected override bool ChecksumValid(ReadOnlySpan<byte> data)
    {
        if (!IsInitialized(data))
            return true;

        ushort chk = GetChecksum(data);
        if (chk != ReadUInt16LittleEndian(data[(FooterOffset + 14)..]))
            return false;
        return true;
    }

    public bool IsValid(ReadOnlySpan<byte> data)
    {
        return IsInitialized(data) && SizeValid(data) && ChecksumValid(data);
    }

    protected override void SetChecksum(Span<byte> data)
    {
        if (!IsInitialized(data))
            return;
        ushort chk = GetChecksum(data);
        WriteUInt16LittleEndian(data[(FooterOffset + 14)..], chk);
    }

    private void SetMagic(Span<byte> data, uint magic)
    {
        if (!IsInitialized(data))
            return;
        WriteUInt32LittleEndian(data[FooterOffset..], magic);
    }

    public static void SetMagics(IEnumerable<BlockInfo4> blocks, Span<byte> data, uint magic)
    {
        foreach (var b in blocks)
            b.SetMagic(data, magic);
    }
}

public static partial class Extensions
{
    public static void SetMagics(this IEnumerable<BlockInfo4> blocks, Span<byte> data, uint magic) => BlockInfo4.SetMagics(blocks, data, magic);
}
