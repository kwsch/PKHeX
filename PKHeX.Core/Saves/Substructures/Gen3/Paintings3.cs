using System;
using System.Collections.Generic;
using System.Linq;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core
{
public sealed class Paintings3
{
    private byte[] data;
    private int language;
    private string category;
    private bool enabled;
    private Random rnd = Util.Rand;

    private static Dictionary<byte, string> PaintingStats = new Dictionary<byte, string>
    {
        [0] = "Coolness",
        [1] = "Coolness",
        [2] = "Coolness",
        [3] = "Beauty",
        [4] = "Beauty",
        [5] = "Beauty",
        [6] = "Cuteness",
        [7] = "Cuteness",
        [8] = "Cuteness",
        [9] = "Cleverness",
        [10] = "Cleverness",
        [11] = "Cleverness",
        [12] = "Toughness",
        [13] = "Toughness",
        [14] = "Toughness"
    };

    private static string[] Categories = new string[] { "Coolness", "Beauty", "Cuteness", "Cleverness", "Toughness" };

    private static Dictionary<string, List<byte>> PaintingStatsReversed = new Dictionary<string, List<byte>>
    {
        ["Coolness"] = new List<byte> { 0x00, 0x01, 0x02 },
        ["Beauty"] = new List<byte> { 0x03, 0x04, 0x05 },
        ["Cuteness"] = new List<byte> { 0x06, 0x07, 0x08 },
        ["Cleverness"] = new List<byte> { 0x09, 0x0A, 0x0B },
        ["Toughness"] = new List<byte> { 0x0C, 0x0D, 0x0E }
    };

    private static Dictionary<string, Int32> PaintAdresses = new Dictionary<string, Int32>
    {
        ["Coolness"] = 0xA0,
        ["Beauty"] = 0xA1,
        ["Cuteness"] = 0xA2,
        ["Cleverness"] = 0xA3,
        ["Toughness"] = 0xA4
    };

    public Paintings3(byte[] data, int language, int category_id)
    {
            this.data = data;
            this.language = language;
            this.enabled = false;
            this.category = Categories[category_id];
    }

    public Paintings3(int language, string category)
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
        this.category = category;
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
    public string Category { get => category; set => data[0x0A] = PaintingStatsReversed[value].ElementAt(rnd.Next(PaintingStatsReversed[value].Count)); }
    public Int32 Address { get => PaintAdresses[Category]; }
    public bool Enabled { get => enabled; set => enabled = value; }
    public byte[] Data { get => data; }
    public int Language { get => language; }
    public bool XORShiny => ((uint)(TID ^ SID ^ (PID >> 16) ^ (PID & 0xFFFF))) < 8 ? true : false;

        public override string ToString()
        {
            return category;
        }

}
}
