using System;
using System.ComponentModel;

namespace PKHeX.Core;

[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class Epoch0000DateTime(Memory<byte> Data): EpochDateTime(Data)
{
    // Data should be 4 or 8 bytes where we only care about the first 4 bytes i.e. 32  bits
    // First 12 bits are year from 0000, next 4 bits are month, next 5 are days, next 5 are hours, next 6 bits are minutes

    private static DateTime Epoch => new(0, 1, 1);

    public override int Year { get => (int)(RawDate & 0xFFF); set => RawDate = (RawDate & 0xFFFFF000) | (uint)(value); }
    public override int Month { get => (int)((RawDate >> 12) & 0xF); set => RawDate = (RawDate & 0xFFFF0FFF) | (((uint)value & 0xF) << 12); }
    
    public override DateTime Timestamp
    {
        get => new(Year, Month, Day, Hour, Minute, 0);
        set
        {
            Year = value.Year;
            Month = value.Month;
            Day = value.Day;
            Hour = value.Hour;
            Minute = value.Minute;
        }
    }

    public override string DisplayValue => $"{Timestamp.Year:0000}-{Timestamp.Month:00}-{Timestamp.Day:00} {Timestamp.Hour:00}ː{Timestamp.Minute:00}ː{Timestamp.Second:00}"; // not :

    /// <summary>
    /// time_t
    /// </summary>
    public override ulong TotalSeconds
    {
        get => (ulong)(Timestamp - Epoch).TotalSeconds;
        set => Timestamp = Epoch.AddSeconds(value);
    }
}
