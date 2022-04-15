using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Details about the Player's union room unlock/penalty.
/// </summary>
/// <remarks>UnionSaveData size: 0xC</remarks>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class UnionSaveData8b : SaveBlock<SAV8BS>
{
    public UnionSaveData8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

    public bool IsInitTalk { get => ReadUInt32LittleEndian(Data.AsSpan(Offset)) == 1;  set => WriteUInt32LittleEndian(Data.AsSpan(Offset), value ? 1u : 0u); }
    public int PenaltyCount { get => ReadInt32LittleEndian(Data.AsSpan(Offset + 0x4)); set => WriteInt32LittleEndian(Data.AsSpan(Offset + 0x4), value); }
    public float PenaltyTime { get => ReadSingleLittleEndian(Data.AsSpan(Offset + 0x8)); set => WriteSingleLittleEndian(Data.AsSpan(Offset + 0x8), value); }
}
