using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Footer checksum block layout <see cref="SaveFile"/>, marked with "BEEF" magic number.
/// </summary>
/// <remarks>Shared logic is used by Gen6 and Gen7 save files.</remarks>
public abstract class SAV_BEEF : SaveFile, ISecureValueStorage
{
    protected SAV_BEEF(byte[] data, [ConstantExpected] int biOffset) : base(data)
    {
        BlockInfoOffset = biOffset;
    }

    protected SAV_BEEF([ConstantExpected] int size, [ConstantExpected] int biOffset) : base(size)
    {
        BlockInfoOffset = biOffset;
    }

    public abstract IReadOnlyList<BlockInfo> AllBlocks { get; }
    protected override void SetChecksums() => AllBlocks.SetChecksums(Data);
    public override bool ChecksumsValid => AllBlocks.GetChecksumsValid(Data);
    public override string ChecksumInfo => AllBlocks.GetChecksumInfo(Data);
    public override string MiscSaveInfo() => string.Join(Environment.NewLine, AllBlocks.Select(b => b.Summary));

    protected readonly int BlockInfoOffset;

    /// <summary>
    /// Timestamp that the save file was last saved at (Secure Value)
    /// </summary>
    public ulong TimeStampCurrent
    {
        get => ReadUInt64LittleEndian(Data.AsSpan(BlockInfoOffset));
        set => WriteUInt64LittleEndian(Data.AsSpan(BlockInfoOffset), value);
    }

    /// <summary>
    /// Timestamp that the save file was saved at prior to the <see cref="TimeStampCurrent"/> (Secure Value)
    /// </summary>
    public ulong TimeStampPrevious
    {
        get => ReadUInt64LittleEndian(Data.AsSpan(BlockInfoOffset));
        set => WriteUInt64LittleEndian(Data.AsSpan(BlockInfoOffset), value);
    }
}
