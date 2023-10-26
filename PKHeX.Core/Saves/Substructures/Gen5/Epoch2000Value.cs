using System;
using System.ComponentModel;

namespace PKHeX.Core;

/// <summary>
/// Stores the <see cref="Timestamp"/> to indicate the seconds since 2000 (rounded to days) that an event occurred.
/// </summary>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class Epoch2000Value
{
    // Data should be 2 bytes where first 7 bits are year from 2000, next 4 bits are the month and last 5 bits are the day
    private readonly Memory<byte> Data;
    private Span<byte> Span => Data.Span;

    public Epoch2000Value(SCBlock block) : this(block.Data) { }
    public Epoch2000Value(Memory<byte> data) => Data = data;

    private static DateTime Epoch => new(2000, 1, 1);

    public DateTime Timestamp
    {
        get
        {
            ushort dateBytes = BitConverter.ToUInt16(Span);
            int year = Epoch.Year + (dateBytes & 0x007F);
            int month = (dateBytes & 0x0780) >> 7;
            int day = dateBytes >> 11;
            return new DateTime(year, month, day);
        }
        set
        {
            int day = value.Day;
            int month = value.Month;
            int year = value.Year - Epoch.Year;
            ushort dateBytes = (ushort)(((day & 0x1F) << 11) | ((month & 0x0F) << 7) | (year & 0x7F));
            BitConverter.GetBytes(dateBytes).CopyTo(Span);            
        }
    }

    public string DisplayValue => $"{Timestamp.Year:0000}-{Timestamp.Month:00}-{Timestamp.Day:00} {Timestamp.Hour:00}ː{Timestamp.Minute:00}ː{Timestamp.Second:00}"; // not :

    /// <summary>
    /// time_t (seconds since 2000 Epoch rounded to days)
    /// </summary>
    public ulong Seconds
    {
        get => (ulong)(Timestamp - Epoch).TotalSeconds;
        set => Timestamp = Epoch.AddSeconds(value);
    }
}
