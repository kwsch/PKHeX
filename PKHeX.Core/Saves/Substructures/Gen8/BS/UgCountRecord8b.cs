using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Underground player metadata counts.
/// </summary>
/// <remarks>size 0x20, struct_name UgCountRecord</remarks>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class UgCountRecord8b(SAV8BS sav, Memory<byte> raw) : SaveBlock<SAV8BS>(sav, raw)
{
    public short DigFossilPlayCount     { get => ReadInt16LittleEndian(Data); set => WriteInt16LittleEndian(Data, value); }
    public short NumStatueBroadcastOnTV { get => ReadInt16LittleEndian(Data[0x02..]); set => WriteInt16LittleEndian(Data[0x02..], value); }
    public int NumTimesSecretBaseBroadcastOnTVWereLiked { get => ReadInt32LittleEndian(Data[0x04..]); set => WriteInt32LittleEndian(Data[0x04..], value); }
    public int SomeoneSecretBaseLikeCount               { get => ReadInt32LittleEndian(Data[0x08..]); set => WriteInt32LittleEndian(Data[0x08..], value); }
    public int NumSuccessfulLightStoneSearches          { get => ReadInt32LittleEndian(Data[0x0C..]); set => WriteInt32LittleEndian(Data[0x0C..], value); }
    public long Reserved1 { get => ReadInt64LittleEndian(Data[0x10..]); set => WriteInt64LittleEndian(Data[0x10..], value); }
    public long Reserved2 { get => ReadInt64LittleEndian(Data[0x18..]); set => WriteInt64LittleEndian(Data[0x18..], value); }
}
