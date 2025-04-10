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
    // wchar[8] creator name
    // u8 creator gender
    // u8 creator lang
    // u16 alignment
    // u32 origin seed (used as identifier)
    // u32 current seed

    public readonly Memory<byte> Raw = Raw;
    private Span<byte> Data => Raw.Span;

    private Span<byte> NameTrash => Data.Slice(0x00, 16);
    public string Name
    {
        get => StringConverter4.GetString(NameTrash);
        set => StringConverter4.SetString(NameTrash, value, 7, CreatorLang, StringConverterOption.None);
    }

    private Span<byte> CreatorTrash => Data.Slice(0x10, 16);
    public string Creator
    {
        get => StringConverter4.GetString(CreatorTrash);
        set => StringConverter4.SetString(CreatorTrash, value, 7, CreatorLang, StringConverterOption.None);
    }
    
    public byte CreatorGender { get => Data[0x20]; set => Data[0x20] = value; }
    public byte CreatorLang   { get => Data[0x21]; set => Data[0x21] = value; }

    public uint Id   { get => ReadUInt32LittleEndian(Data[0x24..]); set => WriteUInt32LittleEndian(Data[0x24..], value); }
    public uint Seed { get => ReadUInt32LittleEndian(Data[0x28..]); set => WriteUInt32LittleEndian(Data[0x28..], value); }
}
