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

    private Span<byte> Data => Raw.Span;

    private Span<byte> GroupNameTrash => Data[..16];
    public string GroupName
    {
        get => StringConverter4.GetString(GroupNameTrash);
        set => StringConverter4.SetString(GroupNameTrash, value, 7, LeaderLanguage, StringConverterOption.None);
    }

    private Span<byte> LeaderTrash => Data.Slice(0x10, 16);
    public string LeaderName
    {
        get => StringConverter4.GetString(LeaderTrash);
        set => StringConverter4.SetString(LeaderTrash, value, 7, LeaderLanguage, StringConverterOption.None);
    }

    public byte LeaderGender { get => Data[0x20]; set => Data[0x20] = value; }
    public byte LeaderLanguage { get => Data[0x21]; set => Data[0x21] = value; }

    // 0x22,0x23 unused alignment

    public uint Id   { get => ReadUInt32LittleEndian(Data[0x24..]); set => WriteUInt32LittleEndian(Data[0x24..], value); }
    public uint Seed { get => ReadUInt32LittleEndian(Data[0x28..]); set => WriteUInt32LittleEndian(Data[0x28..], value); }

    public override string ToString() => $"{Seed:X8} {GroupName} {LeaderName} {Id:X8}";
}
