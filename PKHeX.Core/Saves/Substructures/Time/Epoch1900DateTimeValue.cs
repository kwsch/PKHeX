using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Stores the <see cref="Timestamp"/> to indicate the seconds since 1900 that an event occurred.
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class Epoch1900DateTimeValue(Memory<byte> Data)
{
    // Data should be 4 or 8 bytes where we only care about the first 4 or 4.5 bytes i.e. 32 or 36 bits
    // First 12 bits are year from 1900, next 4 bits are 0 indexed month, next 5 are days, next 5 are hours, next 6 bits are minutes, (optional) last 4 bits are seconds
    private Span<byte> Span => Data.Span;

    public Epoch1900DateTimeValue(SCBlock block) : this(block.Data) { }

    private static DateTime Epoch => new(1900, 1, 1);

    private uint RawDate { get => ReadUInt32LittleEndian(Span); set => WriteUInt32LittleEndian(Span, value); }
    public int Year { get => (int)(RawDate & 0xFFF) + Epoch.Year; set => RawDate = (RawDate & 0xFFFFF000) | (uint)(value - Epoch.Year); }
    public int Month { get => (int)((RawDate >> 12) & 0xF) + 1; set => RawDate = (RawDate & 0xFFFF0FFF) | (((uint)(value - 1) & 0xF) << 12); }
    public int Day { get => (int)((RawDate >> 16) & 0x1F); set => RawDate = (RawDate & 0xFFE0FFFF) | (((uint)value & 0x1F) << 16); }
    public int Hour { get => (int)((RawDate >> 21) & 0x1F); set => RawDate = (RawDate & 0xFC1FFFFF) | (((uint)value & 0x1F) << 21); }
    public int Minute { get => (int)((RawDate >> 26) & 0x3F); set => RawDate = (RawDate & 0x03FFFFFF) | (((uint)value & 0x3F) << 26); }
    public bool HasSeconds => Span.Length > 4;
    public int Second {
        get => HasSeconds ? Span[4] : 0;
        set {
            if (HasSeconds) Span[4] = (byte)value;
        }
    }

    public DateTime Timestamp
    {
        get => new(Year, Month, Day, Hour, Minute, Second);
        set
        {
            Year = value.Year;
            Month = value.Month;
            Day = value.Day;
            Hour = value.Hour;
            Minute = value.Minute;
            Second = value.Second;
        }
    }

    public string DisplayValue => $"{Timestamp.Year:0000}-{Timestamp.Month:00}-{Timestamp.Day:00} {Timestamp.Hour:00}ː{Timestamp.Minute:00}ː{Timestamp.Second:00}"; // not :

    /// <summary>
    /// time_t (seconds since 1900 Epoch)
    /// </summary>
    public ulong TotalSeconds
    {
        get => (ulong)(Timestamp - Epoch).TotalSeconds;
        set => Timestamp = Epoch.AddSeconds(value);
    }
}
