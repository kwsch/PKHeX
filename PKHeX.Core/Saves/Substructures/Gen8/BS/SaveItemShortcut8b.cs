using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Tracks the 4 select bound item slots. Size: 0x8 (4 * u16)
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class SaveItemShortcut8b(SAV8BS sav, Memory<byte> raw) : SaveBlock<SAV8BS>(sav, raw)
{
    public int Item0 { get => ReadInt32LittleEndian(Data); set => WriteInt32LittleEndian(Data, value); }
    public int Item1 { get => ReadInt32LittleEndian(Data[0x02..]); set => WriteInt32LittleEndian(Data[0x02..], value); }
    public int Item2 { get => ReadInt32LittleEndian(Data[0x04..]); set => WriteInt32LittleEndian(Data[0x04..], value); }
    public int Item3 { get => ReadInt32LittleEndian(Data[0x06..]); set => WriteInt32LittleEndian(Data[0x06..], value); }
}
