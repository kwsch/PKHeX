using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class AdventureInfo5(SAV5 sav, Memory<byte> raw) : SaveBlock<SAV5>(sav, raw)
{
    public ulong RTC { get => ReadUInt64LittleEndian(Data); set => WriteUInt64LittleEndian(Data, value); }
    public Span<byte> ConsoleMACAddress => Data[0x8..0xE];
    public byte BirthMonth { get => Data[0x0E]; set => Data[0x0E] = value; }
    public byte BirthDay   { get => Data[0x0F]; set => Data[0x0F] = value; }

    public bool Flag { get => Data[0x10] != 0; set => Data[0x10] = (byte)(value ? 1 : 0); }

    public Date5 Date => new(Raw[0x14..0x24]);
    public Time5 Time => new(Raw[0x24..0x30]);

    public DateTime? Moment
    {
        get
        {
            if (!Date.IsValid || !Time.IsValid)
                return null;
            var date = Date.ToDateOnly();
            var time = Time.ToTimeOnly();
            return new DateTime(date, time);
        }
        set
        {
            if (value is not { } dt)
                return;
            Date.FromDateOnly(DateOnly.FromDateTime(dt));
            Time.FromTimeOnly(TimeOnly.FromDateTime(dt));
        }
    }

    public uint DaysElapsed     { get => ReadUInt32LittleEndian(Data[0x30..]); set => WriteUInt32LittleEndian(Data[0x30..], value); }
    public ulong SecondsToStart { get => ReadUInt64LittleEndian(Data[0x34..]); set => WriteUInt64LittleEndian(Data[0x34..], value); }
    public ulong SecondsToFame  { get => ReadUInt64LittleEndian(Data[0x3C..]); set => WriteUInt64LittleEndian(Data[0x3C..], value); }
    public uint PenaltyTime     { get => ReadUInt32LittleEndian(Data[0x44..]); set => WriteUInt32LittleEndian(Data[0x44..], value); }

    public byte Unknown48 { get => Data[0x48]; set => Data[0x48] = value; }
    public byte Unknown49 { get => Data[0x49]; set => Data[0x49] = value; }
    // 2 bytes alignment

    public uint Unknown4C { get => ReadUInt32LittleEndian(Data[0x4C..]); set => WriteUInt32LittleEndian(Data[0x4C..], value); }
    public uint Unknown50 { get => ReadUInt32LittleEndian(Data[0x50..]); set => WriteUInt32LittleEndian(Data[0x50..], value); }
    public uint Unknown54 { get => ReadUInt32LittleEndian(Data[0x54..]); set => WriteUInt32LittleEndian(Data[0x54..], value); }
    public uint Unknown58 { get => ReadUInt32LittleEndian(Data[0x58..]); set => WriteUInt32LittleEndian(Data[0x58..], value); }
}

public struct Date5(Memory<byte> Raw)
{
    public Span<byte> Data => Raw.Span;

    public uint Year { get => ReadUInt32LittleEndian(Data); set => WriteUInt32LittleEndian(Data, value); }
    public uint Month { get => ReadUInt32LittleEndian(Data[4..]); set => WriteUInt32LittleEndian(Data[4..], value); }
    public uint Day { get => ReadUInt32LittleEndian(Data[8..]); set => WriteUInt32LittleEndian(Data[8..], value); }
    public uint DayOfWeek { get => ReadUInt32LittleEndian(Data[12..]); set => WriteUInt32LittleEndian(Data[12..], value); }

    public uint Epoch = 2000;
    public DateOnly ToDateOnly()
    {
        int year = (int)Year + (int)Epoch;
        return new DateOnly(year, (int)Month, (int)Day);
    }

    public bool IsEmpty => Year == 0 && Month == 0 && Day == 0 && DayOfWeek == 0;
    public bool IsValid => Year <= 99 && Month is (>= 1 and <= 12)
                                        && Day >= 1 && Day <= DateTime.DaysInMonth((int)Year + (int)Epoch, (int)Month)
                                        && ((byte)ToDateOnly().DayOfWeek == DayOfWeek);
    public void FromDateOnly(DateOnly date)
    {
        Year = (uint)date.Year - Epoch;
        Month = (uint)date.Month;
        Day = (uint)date.Day;
        DayOfWeek = (uint)date.DayOfWeek;
    }
}

public struct Time5(Memory<byte> Raw)
{
    public Span<byte> Data => Raw.Span;
    public uint Hour { get => ReadUInt32LittleEndian(Data); set => WriteUInt32LittleEndian(Data, value); }
    public uint Minute { get => ReadUInt32LittleEndian(Data[4..]); set => WriteUInt32LittleEndian(Data[4..], value); }
    public uint Second { get => ReadUInt32LittleEndian(Data[8..]); set => WriteUInt32LittleEndian(Data[8..], value); }
    public bool IsEmpty => Hour == 0 && Minute == 0 && Second == 0;
    public bool IsValid => Hour < 24 && Minute < 60 && Second < 60;
    public void FromTimeOnly(TimeOnly time)
    {
        Hour = (uint)time.Hour;
        Minute = (uint)time.Minute;
        Second = (uint)time.Second;
    }
    public TimeOnly ToTimeOnly() => new((int)Hour, (int)Minute, (int)Second);
}
