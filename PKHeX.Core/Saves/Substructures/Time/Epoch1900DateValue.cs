using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Stores the <see cref="Timestamp"/> to indicate the seconds since 1900 (rounded to days) that an event occurred.
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class Epoch1900DateValue(Memory<byte> Data)
{
    // Data should be 4 bytes where we only care about the first 3 bytes i.e. 24 bits
    // First 12 bits are year from 1900, next 6 bits are 0 indexed month, next 6 are days
    private Span<byte> Span => Data.Span;

    public Epoch1900DateValue(SCBlock block) : this(block.Data) { }

    private static DateTime Epoch => new(1900, 1, 1);

    private uint RawDate { get => ReadUInt32LittleEndian(Span); set => WriteUInt32LittleEndian(Span, value); }
    public int Year { get => (int)(RawDate & 0xFFF) + Epoch.Year; set => RawDate = (RawDate & 0xFFFFF000) | (uint)(value - Epoch.Year); }
    public int Month { get => (int)((RawDate >> 12) & 0x3F) + 1; set => RawDate = (RawDate & 0xFFFC0FFF) | (((uint)(value - 1) & 0x3F) << 12); }
    public int Day { get => (int)((RawDate >> 18) & 0x3F); set => RawDate = (RawDate & 0xFF03FFFF) | (((uint)value & 0x3F) << 18); }

    public DateTime Timestamp
    {
        get => new(Year, Month, Day);
        set
        {
            Year = value.Year;
            Month = value.Month;
            Day = value.Day;
        }
    }

    public string DisplayValue => $"{Timestamp.Year:0000}-{Timestamp.Month:00}-{Timestamp.Day:00} {Timestamp.Hour:00}ː{Timestamp.Minute:00}ː{Timestamp.Second:00}"; // not :

    /// <summary>
    /// time_t (seconds since 1900 Epoch rounded to days)
    /// </summary>
    public ulong TotalSeconds
    {
        get => (ulong)(Timestamp - Epoch).TotalSeconds;
        set => Timestamp = Epoch.AddSeconds(value);
    }
}
