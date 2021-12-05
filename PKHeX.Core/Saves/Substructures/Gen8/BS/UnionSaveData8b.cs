using System;
using System.ComponentModel;

namespace PKHeX.Core;

/// <summary>
/// Details about the Player's union room unlock/penalty.
/// </summary>
/// <remarks>UnionSaveData size: 0xC</remarks>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class UnionSaveData8b : SaveBlock
{
    public UnionSaveData8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

    public bool IsInitTalk { get => BitConverter.ToUInt32(Data, Offset) == 1;  set => BitConverter.GetBytes(value ? 1u : 0u).CopyTo(Data, Offset); }
    public int PenaltyCount { get => BitConverter.ToInt32(Data, Offset + 0x4); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x4); }
    public float PenaltyTime { get => BitConverter.ToSingle(Data, Offset + 0x8); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x8); }
}
