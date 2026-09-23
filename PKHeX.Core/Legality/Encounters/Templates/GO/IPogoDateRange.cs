using System;

namespace PKHeX.Core;

/// <summary>
/// Represents a date range for an encounter, relative to the first day of GO.
/// </summary>
public interface IPogoDateRange
{
    /// <summary> Start date the encounter became available. If zero, no date specified (unbounded start). </summary>
    ushort DayStart { get; }

    /// <summary> Last day the encounter was available. If zero, no date specified (unbounded finish). </summary>
    /// <remarks> If there is no end date (yet), we'll try to clamp to a date in the near-future to prevent it from being open-ended. </remarks>
    ushort DayEnd { get; }

    bool IsLocalDayStart { get; }
    bool IsLocalDayEnd { get; }

    internal const int FirstDay = 736150 - 1; // 2016-07-06 (Launch Day), -1 for time zones.
    internal const ushort DateNone = 0;
    internal bool IsNoDateEnd => DayEnd == DateNone;
    internal bool IsNoDateStart => DayStart == DateNone;
    internal bool IsNoDateEither => IsNoDateStart && IsNoDateEnd;
}

public static class PogoDateRangeExtensions
{
    public static string GetDateString(ushort day, int shift, bool always = false) => day == IPogoDateRange.DateNone && !always ? "X" : $"{GetDate((ushort)(day + shift)):yyyy.MM.dd}";
    private static DateOnly GetDate(ushort day) => DateOnly.FromDayNumber(GetDayNumber(day));
    private static int GetDayNumber(ushort day) => IPogoDateRange.FirstDay + day;
    private static ushort GetDayRelative(in DateOnly date) => (ushort)(date.DayNumber - IPogoDateRange.FirstDay);

    public static bool IsWithinStartEnd(this IPogoDateRange time, DateOnly date)
    {
        var day = GetDayRelative(date);
        if (time.IsNoDateEnd)
            return time.DayStart <= day && date <= GetMaxDate();
        if (time.IsNoDateStart)
            return day <= time.DayEnd;
        return time.DayStart <= day && day <= time.DayEnd;
    }

    private static DateOnly GetMaxDate() => DateOnly.FromDateTime(DateTime.UtcNow.AddHours(12)); // UTC+12 for Kiribati, no daylight savings
    private static DateOnly GetCurrentDateLocal() => DateOnly.FromDateTime(DateTime.Now);

    /// <summary>
    /// Gets a random date within the availability range.
    /// </summary>
    public static DateOnly GetRandomValidDate(this IPogoDateRange time)
    {
        if (time.IsNoDateStart)
            return time.IsNoDateEnd ? GetCurrentDateLocal() : GetDate((ushort)(time.DayEnd - (time.IsLocalDayEnd ? 1 : 0)));

        var start = time.DayStart + (time.IsLocalDayStart ? 1 : 0);
        if (time.IsNoDateEnd)
            return GetDate((ushort)start);

        // Both bounds present: apply local-day adjustments and pick a random day in the inclusive range.
        var end = time.DayEnd - (time.IsLocalDayEnd ? 1 : 0);
        var delta = end - start + 1;
        var day = (ushort)(Util.Rand.Next(delta) + start);
        return GetDate(day);
    }
}
