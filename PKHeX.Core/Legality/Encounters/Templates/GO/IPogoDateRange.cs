using System;

namespace PKHeX.Core;

/// <summary>
/// Represents a date range for an encounter.
/// </summary>
public interface IPogoDateRange
{
    /// <summary> Start date the encounter became available. If zero, no date specified (unbounded start). </summary>
    int StartDate { get; }

    /// <summary> Last day the encounter was available. If zero, no date specified (unbounded finish). </summary>
    /// <remarks> If there is no end date (yet), we'll try to clamp to a date in the near-future to prevent it from being open-ended. </remarks>
    int EndDate { get; }
}

public static class PogoDateRangeExtensions
{
    public static string GetDateString(int time) => time == 0 ? "X" : $"{GetDate(time):yyyy.MM.dd}";

    private static DateOnly GetDate(int time)
    {
        var d = time & 0xFF;
        var m = (time >> 8) & 0xFF;
        var y = time >> 16;
        return new DateOnly(y, m, d);
    }

    public static bool IsWithinStartEnd(this IPogoDateRange time, int stamp)
    {
        if (time.EndDate == 0)
            return time.StartDate <= stamp && GetDate(stamp) <= GetMaxDate();
        if (time.StartDate == 0)
            return stamp <= time.EndDate;
        return time.StartDate <= stamp && stamp <= time.EndDate;
    }

    /// <summary>
    /// Converts a split timestamp into a single integer.
    /// </summary>
    public static int GetTimeStamp(int year, int month, int day) => (year << 16) | (month << 8) | day;

    private static DateOnly GetMaxDate() => DateOnly.FromDateTime(DateTime.UtcNow.AddHours(12)); // UTC+12 for Kiribati, no daylight savings

    /// <summary>
    /// Gets a random date within the availability range.
    /// </summary>
    public static DateOnly GetRandomValidDate(this IPogoDateRange time)
    {
        if (time.StartDate == 0)
            return time.EndDate == 0 ? GetMaxDate() : GetDate(time.EndDate);

        var start = GetDate(time.StartDate);
        if (time.EndDate == 0)
            return start;
        var end = GetDate(time.EndDate);
        return DateUtil.GetRandomDateWithin(start, end);
    }
}
