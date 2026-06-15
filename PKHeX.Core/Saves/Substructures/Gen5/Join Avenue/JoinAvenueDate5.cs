using System;

namespace PKHeX.Core;

public record struct JoinAvenueDate5(ushort RawValue)
{
    private const int EpochYear = 2000;

    public int Year
    {
        readonly get => ((RawValue >> 9) & 0x7F) + EpochYear;
        set => RawValue = (ushort)((RawValue & 0x01FF) | (((value - EpochYear) & 0x7F) << 9));
    }

    public int Month
    {
        readonly get => (RawValue >> 5) & 0x0F;
        set => RawValue = (ushort)((RawValue & 0xFE1F) | ((value & 0x0F) << 5));
    }

    public int Day
    {
        readonly get => RawValue & 0x1F;
        set => RawValue = (ushort)((RawValue & ~0x1F) | (value & 0x1F));
    }

    public readonly bool HasValue => RawValue != 0;

    public DateOnly? Date
    {
        readonly get => HasValue && Month != 0 && Day != 0 && DateUtil.IsValidDate(Year, Month, Day) ? new DateOnly(Year, Month, Day) : null;
        set
        {
            if (value is not { } date)
            {
                RawValue = 0;
                return;
            }

            Year = date.Year;
            Month = date.Month;
            Day = date.Day;
        }
    }
}
