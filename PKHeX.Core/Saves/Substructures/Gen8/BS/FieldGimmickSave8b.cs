using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// size: 0xC
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class FieldGimmickSave8b(SAV8BS sav, Memory<byte> raw) : SaveBlock<SAV8BS>(sav, raw)
{
    public int Value0 { get => ReadInt32LittleEndian(Data); set => WriteInt32LittleEndian(Data, value); }
    public int Value1 { get => ReadInt32LittleEndian(Data[0x04..]); set => WriteInt32LittleEndian(Data[0x04..], value); }
    public int Value2 { get => ReadInt32LittleEndian(Data[0x08..]); set => WriteInt32LittleEndian(Data[0x08..], value); }
}
