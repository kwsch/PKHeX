using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class Group4(Memory<byte> Raw)
{
    public const int SIZE = 0x2C;

    // Structure:
    // wchar[8] group name
    // wchar[8] leader name
    // u8 leader gender
    // u8 leader lang
    // u16 alignment
    // u32 origin seed (used as identifier)
    // u32 current seed

    public readonly Memory<byte> Raw = Raw;
    private Span<byte> Data => Raw.Span;

    private Span<byte> NameTrash => Data.Slice(0x00, 16);
    public string Name
    {
        get => StringConverter4.GetString(NameTrash);
        set => StringConverter4.SetString(NameTrash, value, 7, LeaderLang, StringConverterOption.None);
    }

    private Span<byte> LeaderTrash => Data.Slice(0x10, 16);
    public string Leader
    {
        get => StringConverter4.GetString(LeaderTrash);
        set => StringConverter4.SetString(LeaderTrash, value, 7, LeaderLang, StringConverterOption.None);
    }
    
    public byte LeaderGender { get => Data[0x20]; set => Data[0x20] = value; }
    public byte LeaderLang   { get => Data[0x21]; set => Data[0x21] = value; }

    public uint Id   { get => ReadUInt32LittleEndian(Data[0x24..]); set => WriteUInt32LittleEndian(Data[0x24..], value); }
    public uint Seed { get => ReadUInt32LittleEndian(Data[0x28..]); set => WriteUInt32LittleEndian(Data[0x28..], value); }
}
