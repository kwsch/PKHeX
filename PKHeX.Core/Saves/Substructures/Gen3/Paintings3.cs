using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core
{
public sealed class Paintings3
{
    private byte[] data;
    private int language;
    private bool enabled;

    private static Dictionary<byte, string> PaintingStats = new Dictionary<byte, string>
    {
        [0] = "Coolness",
        [3] = "Beauty",
        [8] = "Cuteness",
        [9] = "Cleverness",
        [14] = "Toughness"
    };

    private static Dictionary<string, byte> PaintingStatsReversed = new Dictionary<string, byte>
    {
        ["Coolness"] = 0x00,
        ["Beauty"] = 0x03,
        ["Cuteness"] = 0x08,
        ["Cleverness"] = 0x09,
        ["Toughness"] = 0x0E
    };

    private static Dictionary<string, Int32> PaintAdresses = new Dictionary<string, Int32>
    {
        ["Coolness"] = 0xA0,
        ["Beauty"] = 0xA1,
        ["Cuteness"] = 0xA2,
        ["Cleverness"] = 0xA3,
        ["Toughness"] = 0xA4
    };

    public Paintings3(byte[] data, int language)
    {
        this.data = data;
        this.language = language;
        this.enabled = false;
    }

    public Paintings3(int language)
    {
        this.data = new byte[0x20];
        for (int i = 0; i < 0x20; i++)
        {
            this.data[i] = 0;
        }
        this.language = language;
        this.enabled = false;
        this.data[0x0C] = 0xFF;
        this.data[0x17] = 0xFF;
    }

    public uint PID { get => ReadUInt32LittleEndian(data.AsSpan(0x00)); set => WriteUInt32LittleEndian(data.AsSpan(0x00), value); }
    public ushort TID { get => ReadUInt16LittleEndian(data.AsSpan(0x04)); set => WriteUInt16LittleEndian(data.AsSpan(0x04), value); }
    public ushort SID { get => ReadUInt16LittleEndian(data.AsSpan(0x06)); set => WriteUInt16LittleEndian(data.AsSpan(0x06), value); }
    public ushort Species { get => SpeciesConverter.GetNational3(ReadUInt16LittleEndian(data.AsSpan(0x08)));
            set => WriteUInt16LittleEndian(data.AsSpan(0x08), SpeciesConverter.GetInternal3(value)); }
    public string Nickname { get => StringConverter3.GetString(data.AsSpan(0x0B, 10), language);
                             set => StringConverter3.SetString(data.AsSpan(0x0B, 10), value, 10, language, StringConverterOption.None); }
    public string OT { get => StringConverter3.GetString(data.AsSpan(0x16, 7), language);
                       set => StringConverter3.SetString(data.AsSpan(0x16, 7), value, 7, language, StringConverterOption.None); }
    public string Category { get => PaintingStats[data[0x0A]]; set => data[0x0A] = PaintingStatsReversed[value]; }
    public Int32 Address { get => PaintAdresses[Category]; }
    public bool Enabled { get => enabled; set => enabled = value; }
    public byte[] Data { get => data; }

    }

}
