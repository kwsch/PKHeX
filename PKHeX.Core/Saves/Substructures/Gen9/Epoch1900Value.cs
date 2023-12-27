using System;
using System.ComponentModel;

namespace PKHeX.Core;

/// <summary>
/// Stores the <see cref="Timestamp"/> to indicate the seconds since 1900 (rounded to days) that an event occurred.
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class Epoch1900Value(Memory<byte> Data)
{
    // Data should be 4 bytes where we only care about the first 3 bytes i.e. 24 bits
    // First 6 bits are day, next 6 bits are 0 indexed month, last 12 bits are year from 1900
    private Span<byte> Span => Data.Span;

    public Epoch1900Value(SCBlock block) : this(block.Data) { }

    private static DateTime Epoch => new(1900, 1, 1);

    public DateTime Timestamp
    {
        get => Epoch
                .AddDays(Span[2] >> 2)
                .AddDays(-1)
                .AddMonths(((Span[2] & 0b0000_0011) << 2) | ((Span[1] & 0b1111_0000) >> 4))
                .AddYears(((Span[1] & 0b0000_1111) << 4) | Span[0]);
        set {
            int day = value.Day;
            int month = value.Month - Epoch.Month;
            int year = value.Year - Epoch.Year;
            Span[2] = (byte)(((day & 0b0011_1111) << 2) | ((month & 0b0011_0000) >> 4));
            Span[1] = (byte)(((month & 0b0000_1111) << 4) | ((year & 0b1111_0000_00000) >> 8));
            Span[0] = (byte)(year & 0b1111_1111);
        }
    }

    public string DisplayValue => $"{Timestamp.Year:0000}-{Timestamp.Month:00}-{Timestamp.Day:00} {Timestamp.Hour:00}ː{Timestamp.Minute:00}ː{Timestamp.Second:00}"; // not :

    /// <summary>
    /// time_t (seconds since 1900 Epoch rounded to days)
    /// </summary>
    public ulong Seconds
    {
        get => (ulong)(Timestamp - Epoch).TotalSeconds;
        set => Timestamp = Epoch.AddSeconds(value);
    }
}
