using System;
using System.ComponentModel;

namespace PKHeX.Core;

/// <summary>
/// Stores the <see cref="Timestamp"/> to indicate the seconds since 1900 that an event occurred.
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class Epoch1900DateTimeValue(Memory<byte> Data)
{
    // Data should be 8 bytes where we only care about the first 4.5 bytes i.e. 36 bits
    // First 12 bits are year from 1900, next 4 bits are 0 indexed month, next 5 are days, next 5 are hours, next 6 bits are minutes, last 4 bits are seconds
    private Span<byte> Span => Data.Span;

    public Epoch1900DateTimeValue(SCBlock block) : this(block.Data) { }

    private static DateTime Epoch => new(1900, 1, 1);

    public DateTime Timestamp
    {
        get => Epoch
                .AddSeconds(Span[4])
                .AddMinutes(((Span[3] & 0b1111_1100) >> 2))
                .AddHours((Span[3] & 0b0000_0011) << 3 | (Span[2] & 0b1110_0000) >> 5)
                .AddDays((Span[2] & 0b0001_1111) - 1)
                .AddMonths((Span[1] & 0b1111_0000) >> 4)
                .AddYears(((Span[1] & 0b0000_1111) << 8) | Span[0]);
        set
        {
            int day = value.Day;
            int month = value.Month - Epoch.Month;
            int year = value.Year - Epoch.Year;
            int hour = value.Hour;
            int minute = value.Minute;
            int second = value.Second;
            Span[4] = (byte)second;
            Span[3] = (byte)(((minute & 0b0011_1111) << 2) | ((hour & 0b0001_1000) >> 3));
            Span[2] = (byte)(((hour & 0b0000_0111) << 5) | (day & 0b0001_1111));
            Span[1] = (byte)(((month & 0b0000_1111) << 4) | ((year & 0b1111_0000_00000) >> 8));
            Span[0] = (byte)(year & 0b1111_1111);
        }
    }

    public string DisplayValue => $"{Timestamp.Year:0000}-{Timestamp.Month:00}-{Timestamp.Day:00} {Timestamp.Hour:00}ː{Timestamp.Minute:00}ː{Timestamp.Second:00}"; // not :

    /// <summary>
    /// time_t (seconds since 1900 Epoch)
    /// </summary>
    public ulong Seconds
    {
        get => (ulong)(Timestamp - Epoch).TotalSeconds;
        set => Timestamp = Epoch.AddSeconds(value);
    }
}
