using System;
using static System.Buffers.Binary.BinaryPrimitives;
using static PKHeX.Core.StringConverter5;

namespace PKHeX.Core;

public sealed class JoinAvenueFan5(Memory<byte> data) : IJoinAvenueEntity5
{
    public const int SIZE = 0x60;
    private Span<byte> Data => data.Span;
    public string FileExtension => "jah5";
    public ReadOnlySpan<byte> Write() => Data;
    public void CopyFrom(JoinAvenueFan5 other) => other.Data.CopyTo(Data);

    public string Name
    {
        get => GetString(Data[..0xE]); // no terminator
        set => SetString(Data[..0xE], value, 7, Language);
    }

    public byte Country { get => Data[0x0E]; set => Data[0x0E] = value; }
    public byte Subregion { get => Data[0x0F]; set => Data[0x0F] = value; }

    public string Shout
    {
        get => GetString(Data[0x10..0x20]);
        set => SetString(Data[0x10..0x20], value, 8, Language);
    }

    public byte Version { get => Data[0x20]; set => Data[0x20] = value; }
    public byte Language { get => Data[0x21]; set => Data[0x21] = value; }

    public byte Unknown22
    {
        get => (byte)(Data[0x22] & 0x0F);
        set => Data[0x22] = (byte)((Data[0x22] & 0xF0) | (value & 0x0F));
    }

    public byte Gender
    {
        get => (byte)(Data[0x22] >> 4);
        set => Data[0x22] = (byte)((Data[0x22] & 0x0F) | ((value & 0x0F) << 4));
    }

    public byte Unused23 { get => Data[0x23]; set => Data[0x23] = value; }

    public ushort TID16 // strangely not ID32 (no SID16)
    {
        get => ReadUInt16LittleEndian(Data[0x24..]);
        set => WriteUInt16LittleEndian(Data[0x24..], value);
    }

    public byte Unknown26 { get => Data[0x26]; set => Data[0x26] = value; }
    public byte Unknown27 { get => Data[0x27]; set => Data[0x27] = value; }

    public ushort PlayedTime
    {
        get => ReadUInt16LittleEndian(Data[0x28..]);
        set => WriteUInt16LittleEndian(Data[0x28..], value);
    }

    public ushort PlayedHours
    {
        get => (ushort)(PlayedTime & 0x03FF);
        set => PlayedTime = (ushort)((PlayedTime & ~0x03FF) | (value & 0x03FF));
    }

    public byte PlayedMinutes
    {
        get => (byte)(PlayedTime >> 10);
        set => PlayedTime = (ushort)((PlayedTime & 0x03FF) | ((value & 0x3F) << 10));
    }

    /// <summary>
    /// Overworld Sprite ID
    /// </summary>
    public ushort Sprite
    {
        get => ReadUInt16LittleEndian(Data[0x2A..]);
        set => WriteUInt16LittleEndian(Data[0x2A..], value);
    }

    public string Greeting
    {
        get => GetString(Data[0x2C..0x3C]);
        set => SetString(Data[0x2C..0x3C], value, 8, Language);
    }

    public string Farewell
    {
        get => GetString(Data[0x3C..0x4C]);
        set => SetString(Data[0x3C..0x4C], value, 8, Language);
    }

    public byte Unknown4C { get => Data[0x4C]; set => Data[0x4C] = value; }
    public byte Unknown4D { get => Data[0x4D]; set => Data[0x4D] = value; }
    public byte Unknown4F { get => Data[0x4E]; set => Data[0x4E] = value; }
    public bool IsInteractedToday { get => Data[0x4F] != 0; set => Data[0x4F] = (byte)(value ? 1 : 0); }

    public ushort Species { get => ReadUInt16LittleEndian(Data[0x50..]); set => WriteUInt16LittleEndian(Data[0x50..], value); }
    public ushort Unknown52 { get => ReadUInt16LittleEndian(Data[0x52..]); set => WriteUInt16LittleEndian(Data[0x52..], value); }

    public byte Unknown54 { get => Data[0x54]; set => Data[0x54] = value; }
    public byte BubbleTarget { get => Data[0x55]; set => Data[0x55] = value; }
    public byte Unknown56 { get => Data[0x56]; set => Data[0x56] = value; }
    public byte MetYear { get => Data[0x57]; set => Data[0x57] = value; }
    public byte MetMonth { get => Data[0x58]; set => Data[0x58] = value; }
    public byte MetDay { get => Data[0x59]; set => Data[0x59] = value; }

    public ushort Unknown5A { get => ReadUInt16LittleEndian(Data[0x5A..]); set => WriteUInt16LittleEndian(Data[0x5A..], value); }
    public uint Seed { get => ReadUInt32LittleEndian(Data[0x5C..]); set => WriteUInt32LittleEndian(Data[0x5C..], value); }
}
