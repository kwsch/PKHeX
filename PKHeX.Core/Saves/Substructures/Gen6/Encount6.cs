using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Swarm and other overworld info
/// </summary>
public sealed class Encount6 : SaveBlock<SAV6>
{
    public Encount6(SAV6XY SAV, Memory<byte> raw) : base(SAV, raw) { }
    public Encount6(SAV6AO SAV, Memory<byte> raw) : base(SAV, raw) { }

    public ushort RepelItemUsed { get => ReadUInt16LittleEndian(Data); set => WriteUInt16LittleEndian(Data, value); }
    public byte RepelSteps { get => Data[0x02]; set => Data[0x02] = value; }

    // 0x04

    public PokeRadar6 Radar
    {
        get => new(Raw.Slice(0x04, PokeRadar6.SIZE));
        set => value.Data.Span.CopyTo(Data[0x04..]);
    }

    // 0x1C

    public Roamer6 Roamer
    {
        get => new(Raw.Slice(0x1C, Roamer6.SIZE));
        set => value.Data.Span.CopyTo(Data.Slice(0x1C, Roamer6.SIZE));
    }

    // 0x44

    // 4 bytes at end??
}

[TypeConverter(typeof(ValueTypeTypeConverter))]
public sealed class PokeRadar6(Memory<byte> Data)
{
    public const int SIZE = 2 + (RecordCount * PokeRadarRecord.SIZE); // 0x18

    private const int MaxCharge = 50;
    private const int RecordCount = 5;

    public readonly Memory<byte> Data = Data;
    private Span<byte> Span => Data.Span;

    public override string ToString() => ((Species)PokeRadarSpecies).ToString();

    public ushort PokeRadarSpecies { get => ReadUInt16LittleEndian(Span[..2]); set => WriteUInt16LittleEndian(Span[..2], value); }
    private ushort PokeRadarPacked { get => ReadUInt16LittleEndian(Span[2..4]); set => WriteUInt16LittleEndian(Span[2..4], value); }

    public int PokeRadarCharge { get => PokeRadarPacked & 0x3FFF; set => PokeRadarPacked = (ushort)((PokeRadarPacked & ~0x3FFF) | Math.Min(MaxCharge, value)); }
    public bool PokeRadarFlag1 { get => PokeRadarPacked >> 14 != 0; set => PokeRadarPacked = (ushort)((PokeRadarPacked & ~(1 << 14)) | (value ? (1 << 14) : 0)); }
    public bool PokeRadarFlag2 { get => PokeRadarPacked >> 15 != 0; set => PokeRadarPacked = (ushort)((PokeRadarPacked & ~(1 << 15)) | (value ? (1 << 15) : 0)); }

    public PokeRadarRecord GetRecord(int index) => PokeRadarRecord.ReadRecord(Span[GetRecordOffset(index)..]);
    public void SetRecord(PokeRadarRecord record, int index) => record.WriteRecord(Span[GetRecordOffset(index)..]);

    private static int GetRecordOffset(int index)
    {
        if ((uint) index >= RecordCount)
            throw new ArgumentOutOfRangeException(nameof(index));

        return 6 + (index * 2);
    }

    public PokeRadarRecord Record1 { get => GetRecord(0); set => SetRecord(value, 0); }
    public PokeRadarRecord Record2 { get => GetRecord(1); set => SetRecord(value, 1); }
    public PokeRadarRecord Record3 { get => GetRecord(2); set => SetRecord(value, 2); }
    public PokeRadarRecord Record4 { get => GetRecord(3); set => SetRecord(value, 3); }
    public PokeRadarRecord Record5 { get => GetRecord(4); set => SetRecord(value, 4); }
}

[TypeConverter(typeof(ValueTypeTypeConverter))]
public sealed class PokeRadarRecord
{
    public const int SIZE = 4;
    public override string ToString() => ((Species)Species).ToString();

    public ushort Species { get; set; }
    public ushort Count { get; set; }

    private PokeRadarRecord(ushort species, ushort count)
    {
        Species = species;
        Count = count;
    }

    public static PokeRadarRecord ReadRecord(ReadOnlySpan<byte> data)
    {
        var species = ReadUInt16LittleEndian(data);
        var count = ReadUInt16LittleEndian(data[2..]);
        return new PokeRadarRecord(species, count);
    }

    public void WriteRecord(Span<byte> data)
    {
        WriteUInt16LittleEndian(data, Species);
        WriteUInt16LittleEndian(data[2..], Count);
    }
}

[TypeConverter(typeof(ValueTypeTypeConverter))]
public sealed class Roamer6(Memory<byte> Data)
{
    public const int SIZE = 0x28;

    public readonly Memory<byte> Data = Data;
    private Span<byte> Span => Data.Span;

    public override string ToString() => ((Species)Species).ToString();

    private ushort SpecForm { get => ReadUInt16LittleEndian(Span[..2]); set => WriteUInt16LittleEndian(Span[..2], value); }
    public ushort Species { get => (ushort)(SpecForm & 0x3FF); set => SpecForm = (ushort)((SpecForm & ~0x3FF) | (value & 0x3FF)); }
    public bool Flag1 { get => SpecForm >> 14 != 0; set => SpecForm = (ushort)((SpecForm & 0xBFFF) | (value ? (1 << 14) : 0)); }
    public bool Flag2 { get => SpecForm >> 15 != 0; set => SpecForm = (ushort)((SpecForm & 0x7FFF) | (value ? (1 << 15) : 0)); }

    public byte CurrentLevel { get => Span[4]; set => Span[4] = value; }
    private int Status { get => Span[7]; set => Span[7] = (byte)value; }
    public Roamer6State RoamStatus { get => (Roamer6State)((Status >> 4) & 0xF); set => Status = (Status & 0x0F) | (((int)value << 4) & 0xF0); }

    public uint TimesEncountered { get => ReadUInt32LittleEndian(Span[36..40]); set => WriteUInt32LittleEndian(Span[36..40], value); }
}

public enum Roamer6State
{
    Inactive,
    Roaming,
    Stationary,
    Defeated,
    Captured,
}
