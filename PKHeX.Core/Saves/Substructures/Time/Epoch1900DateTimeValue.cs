using System;
using System.ComponentModel;

namespace PKHeX.Core;

/// <summary>
/// Stores the <see cref="Timestamp"/> to indicate the seconds since 1900 that an event occurred.
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class Epoch1900DateTimeValue(Memory<byte> Data) : EpochDateTime(Data)
{
    // Data should be 4 or 8 bytes where we only care about the first 4 or 4.5 bytes i.e. 32 or 36 bits
    // First 12 bits are year from 1900, next 4 bits are 0 indexed month, next 5 are days, next 5 are hours, next 6 bits are minutes, (optional) last 4 bits are seconds

    public Epoch1900DateTimeValue(SCBlock block) : this(block.Data) { }

    private static DateTime Epoch => new(1900, 1, 1);

    public override int Year { get => RawYear + Epoch.Year; set => RawYear = value - Epoch.Year; }
    public override int Month { get => RawMonth + 1; set => RawMonth = value - 1; }
    public bool HasSeconds => Span.Length > 4;
    public int Second {
        get => HasSeconds ? Span[4] : 0;
        set {
            if (HasSeconds) Span[4] = (byte)value;
        }
    }

    public override DateTime Timestamp
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

    public override string DisplayValue => $"{Timestamp.Year:0000}-{Timestamp.Month:00}-{Timestamp.Day:00} {Timestamp.Hour:00}ː{Timestamp.Minute:00}" + (HasSeconds ? $"ː{Timestamp.Second:00}" : ""); // not :

    /// <summary>
    /// time_t (seconds since 1900 Epoch)
    /// </summary>
    public override ulong TotalSeconds
    {
        get => (ulong)(Timestamp - Epoch).TotalSeconds;
        set => Timestamp = Epoch.AddSeconds(value);
    }
}
