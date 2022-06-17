using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Underground player metadata counts.
/// </summary>
/// <remarks>size 0x20, struct_name UgCountRecord</remarks>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class UgCountRecord8b : SaveBlock<SAV8BS>
{
    public UgCountRecord8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

    public short DigFossilPlayCount
    {
        get => ReadInt16LittleEndian(Data.AsSpan(Offset + 0x00));
        set => WriteInt16LittleEndian(Data.AsSpan(Offset + 0x00), value);
    }
    public short NumStatueBroadcastOnTV
    {
        get => ReadInt16LittleEndian(Data.AsSpan(Offset + 0x02));
        set => WriteInt16LittleEndian(Data.AsSpan(Offset + 0x02), value);
    }
    public int NumTimesSecretBaseBroadcastOnTVWereLiked
    {
        get => ReadInt32LittleEndian(Data.AsSpan(Offset + 0x04));
        set => WriteInt32LittleEndian(Data.AsSpan(Offset + 0x04), value);
    }
    public int SomeoneSecretBaseLikeCount
    {
        get => ReadInt32LittleEndian(Data.AsSpan(Offset + 0x08));
        set => WriteInt32LittleEndian(Data.AsSpan(Offset + 0x08), value);
    }
    public int NumSuccessfulLightStoneSearches
    {
        get => ReadInt32LittleEndian(Data.AsSpan(Offset + 0x0C));
        set => WriteInt32LittleEndian(Data.AsSpan(Offset + 0x0C), value);
    }
    public long Reserved1
    {
        get => ReadInt64LittleEndian(Data.AsSpan(Offset + 0x10));
        set => WriteInt64LittleEndian(Data.AsSpan(Offset + 0x10), value);
    }
    public long Reserved2
    {
        get => ReadInt64LittleEndian(Data.AsSpan(Offset + 0x18));
        set => WriteInt64LittleEndian(Data.AsSpan(Offset + 0x18), value);
    }
}
