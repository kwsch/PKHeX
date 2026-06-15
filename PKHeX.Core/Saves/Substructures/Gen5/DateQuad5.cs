using System;

namespace PKHeX.Core;

/// <summary>
/// Exposes a date stored as 4 bytes, with the format: [DayOfWeek, Day, Month, YearSince2000].
/// </summary>
/// <param name="Raw">Backing memory for the date. Must be at least 4 bytes long.</param>
public struct DateQuad5(Memory<byte> Raw)
{
    public const int SIZE = 4;
    public DateQuad5() : this(new byte[SIZE]) { }

    private const int Epoch = 2000;
    public Span<byte> Data => Raw.Span;
    public byte DayOfWeek { get => Data[0x00]; set => Data[0x00] = value; }
    public byte Day { get => Data[0x01]; set => Data[0x01] = value; }
    public byte Month { get => Data[0x02]; set => Data[0x02] = value; }
    public byte Year { get => Data[0x03]; set => Data[0x03] = value; }

    public DateOnly ToDateOnly()
    {
        int year = Year + 2000;
        return new DateOnly(year, Month, Day);
    }

    public bool IsEmpty => Year == 0 && Month == 0 && Day == 0 && DayOfWeek == 0;

    public bool IsValid => Year <= 99 && Month is (>= 1 and <= 12)
                                      && Day >= 1 && Day <= DateTime.DaysInMonth(Year + Epoch, Month)
                                      && ((byte)ToDateOnly().DayOfWeek == DayOfWeek);

    public void FromDateOnly(DateOnly date)
    {
        Year = (byte)(date.Year - Epoch);
        Month = (byte)date.Month;
        Day = (byte)date.Day;
        DayOfWeek = (byte)date.DayOfWeek;
    }

    public void SetEmpty()
    {
        Year = 0;
        Month = 0;
        Day = 0;
        DayOfWeek = 0;
    }
}
