using System;
using static System.Buffers.Binary.BinaryPrimitives;
using static PKHeX.Core.StringConverter5;

namespace PKHeX.Core;

public sealed class JoinAvenueAssistant5(Memory<byte> data) : IJoinAvenueEntity5
{
    public const int SIZE = 0x58;
    private Span<byte> Data => data.Span;
    public string FileExtension => "jaa5";
    public ReadOnlySpan<byte> Write() => Data;
    public void CopyFrom(JoinAvenueAssistant5 other) => other.Data.CopyTo(Data);

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

    public ushort TID16
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

    public byte Position0 { get => Data[0x2C]; set => Data[0x2C] = value; }
    public byte Position1 { get => Data[0x2D]; set => Data[0x2D] = value; }
    public byte Position2 { get => Data[0x2E]; set => Data[0x2E] = value; }
    public byte PositionUnused { get => Data[0x2F]; set => Data[0x2F] = value; }
    public byte MetYear { get => Data[0x30]; set => Data[0x30] = value; }
    public byte MetMonth { get => Data[0x31]; set => Data[0x31] = value; }
    public byte MetDay { get => Data[0x32]; set => Data[0x32] = value; }
    public bool IsInteractedToday { get => (Data[0x33] & 1) != 0; set => Data[0x33] = (byte)(value ? 1 : 0); }

    public string Greeting
    {
        get => GetString(Data[0x34..0x44]);
        set => SetString(Data[0x34..0x44], value, 8, Language);
    }

    public string Farewell
    {
        get => GetString(Data[0x44..0x54]);
        set => SetString(Data[0x44..0x54], value, 8, Language);
    }

    public uint Seed
    {
        get => ReadUInt32LittleEndian(Data[0x54..]);
        set => WriteUInt32LittleEndian(Data[0x54..], value);
    }
}
