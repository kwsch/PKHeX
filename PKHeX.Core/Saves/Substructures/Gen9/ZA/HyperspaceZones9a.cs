using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class HyperspaceZones9a(SAV9ZA sav, SCBlock block) : SaveBlock<SAV9ZA>(sav, block.Raw)
{
    public const int MaxCount = 50;

    public HyperspaceZoneEntry9a GetEntry(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)index, MaxCount);
        var slice = Raw.Slice(index * HyperspaceZoneEntry9a.Size, HyperspaceZoneEntry9a.Size);
        return new HyperspaceZoneEntry9a(slice);
    }

    private const int FooterStart = MaxCount * HyperspaceZoneEntry9a.Size;

    public int Count
    {
        get => ReadInt32LittleEndian(Data[FooterStart..]);
        set
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)value, MaxCount);
            WriteInt32LittleEndian(Data[FooterStart..], value);
        }
    }

    [TypeConverter(typeof(TypeConverterU64))]
    public ulong SpecialScanNextSeed
    {
        get => ReadUInt64LittleEndian(Data[0x28B0..]);
        set => WriteUInt64LittleEndian(Data[0x28B0..], value);
    }
}

public sealed record HyperspaceZoneEntry9a(Memory<byte> Raw)
{
    public const int Size = 0xC8;
    private Span<byte> Data => Raw.Span;

    /*
     *Guessing first 0x20 bytes is the string for which map it's in
       Next 0x20 bytes might be string for where you start in the map
       Next 0x20 bytes are some string
       0x60-0x66 look like byte values of some kind.
       0x6A is another 0x20 bytes of some string
       0x8A is some bool?
       0x8C, 0x90, 0x94 are 3 floats that are probably coordinates
       0x9C is some uint
       0xA4 is some uint
       0xA8 to the end is 0x20 bytes of some string.
     */

    public string MapName
    {
        get => GetString(Data[..0x20]);
        set => SetString(Data[..0x20], value);
    }

    public string StartLocation
    {
        get => GetString(Data[0x20..0x40]);
        set => SetString(Data[0x20..0x40], value);
    }

    // 0x60-0x66 look like byte values of some kind.
    public byte _0x60 { get => Data[0x60]; set => Data[0x60] = value; }
    public byte _0x61 { get => Data[0x61]; set => Data[0x61] = value; }
    public byte _0x62 { get => Data[0x62]; set => Data[0x62] = value; }
    public byte _0x63 { get => Data[0x63]; set => Data[0x63] = value; }
    public byte _0x64 { get => Data[0x64]; set => Data[0x64] = value; }
    public byte _0x65 { get => Data[0x65]; set => Data[0x65] = value; }
    public byte _0x66 { get => Data[0x66]; set => Data[0x66] = value; }

    // 0x6A
    public string EndLocation
    {
        get => GetString(Data[0x6A..0x8A]);
        set => SetString(Data[0x6A..0x8A], value);
    }

    public bool SomeFlag
    {
        get => Data[0x8A] != 0;
        set => Data[0x8A] = (byte)(value ? 1 : 0);
    }

    public float X
    {
        get => ReadSingleLittleEndian(Data[0x8C..0x90]);
        set => WriteSingleLittleEndian(Data[0x8C..0x90], value);
    }

    public float Y
    {
        get => ReadSingleLittleEndian(Data[0x90..0x94]);
        set => WriteSingleLittleEndian(Data[0x90..0x94], value);
    }

    public float Z
    {
        get => ReadSingleLittleEndian(Data[0x94..0x98]);
        set => WriteSingleLittleEndian(Data[0x94..0x98], value);
    }

    public uint Rotation
    {
        get => ReadUInt32LittleEndian(Data[0x98..0x9C]);
        set => WriteUInt32LittleEndian(Data[0x98..0x9C], value);
    }

    public uint SomeValue
    {
        get => ReadUInt32LittleEndian(Data[0x9C..0xA0]);
        set => WriteUInt32LittleEndian(Data[0x9C..0xA0], value);
    }

    public uint AnotherValue
    {
        get => ReadUInt32LittleEndian(Data[0xA0..0xA4]);
        set => WriteUInt32LittleEndian(Data[0xA0..0xA4], value);
    }

    public uint YetAnotherValue
    {
        get => ReadUInt32LittleEndian(Data[0xA4..0xA8]);
        set => WriteUInt32LittleEndian(Data[0xA4..0xA8], value);
    }

    public string SomeString
    {
        get => GetString(Data[0xA8..]);
        set => SetString(Data[0xA8..], value);
    }

    private string GetString(ReadOnlySpan<byte> data) => StringConverter8.GetString(data);
    private void SetString(Span<byte> data, ReadOnlySpan<char> value, int maxLength = 15) => StringConverter8.SetString(data, value, maxLength);
}
