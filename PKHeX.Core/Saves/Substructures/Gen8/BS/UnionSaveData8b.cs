using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Details about the Player's union room unlock/penalty.
/// </summary>
/// <remarks>UnionSaveData size: 0xC</remarks>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class UnionSaveData8b(SAV8BS sav, Memory<byte> raw) : SaveBlock<SAV8BS>(sav, raw)
{
    public bool IsInitTalk { get => ReadUInt32LittleEndian(Data) == 1;  set => WriteUInt32LittleEndian(Data, value ? 1u : 0u); }
    public int PenaltyCount { get => ReadInt32LittleEndian(Data[0x4..]); set => WriteInt32LittleEndian(Data[0x4..], value); }
    public float PenaltyTime { get => ReadSingleLittleEndian(Data[0x8..]); set => WriteSingleLittleEndian(Data[0x8..], value); }
}
