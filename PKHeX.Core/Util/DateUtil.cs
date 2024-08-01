using System;

namespace PKHeX.Core;

public static class DateUtil
{
    /// <summary>
    /// Determines whether the given date components are valid.
    /// </summary>
    /// <param name="year">The year of the date of which to check the validity.</param>
    /// <param name="month">The month of the date of which to check the validity.</param>
    /// <param name="day">The day of the date of which to check the validity.</param>
    /// <returns>A boolean indicating if the date is valid.</returns>
    public static bool IsDateValid(int year, int month, int day)
    {
        if (year is <= 0 or > 9999)
            return false;
        if (month is < 1 or > 12)
            return false;
        if (day < 1 || day > DateTime.DaysInMonth(year, month))
            return false;

        return true;
    }

    /// <summary>
    /// Determines whether the given date components are valid.
    /// </summary>
    /// <param name="year">The year of the date of which to check the validity.</param>
    /// <param name="month">The month of the date of which to check the validity.</param>
    /// <param name="day">The day of the date of which to check the validity.</param>
    /// <returns>A boolean indicating if the date is valid.</returns>
    public static bool IsDateValid(uint year, uint month, uint day)
    {
        return year < int.MaxValue && month < int.MaxValue && day < int.MaxValue && IsDateValid((int)year, (int)month, (int)day);
    }

    private static readonly DateTime Epoch2000 = new(2000, 1, 1);
    private const int SecondsPerDay = 60*60*24; // 86400

    public static int GetSecondsFrom2000(DateTime date, DateTime time)
    {
        int seconds = (int)(date - Epoch2000).TotalSeconds;
        seconds -= seconds % SecondsPerDay;
        seconds += (int)(time - Epoch2000).TotalSeconds;
        return seconds;
    }

    public static void GetDateTime2000(uint seconds, out DateTime date, out DateTime time)
    {
        date = Epoch2000.AddSeconds(seconds);
        time = Epoch2000.AddSeconds(seconds % SecondsPerDay);
    }

    public static string ConvertDateValueToString(int value, int secondsBias = -1)
    {
        var sb = new System.Text.StringBuilder();
        if (value >= SecondsPerDay)
            sb.Append(value / SecondsPerDay).Append("d ");
        value %= SecondsPerDay;
        sb.Append(new TimeOnly(ticks: value * TimeSpan.TicksPerSecond).ToString("HH:mm:ss"));
        if (secondsBias >= 0)
            sb.Append(Environment.NewLine).Append($"Date: {Epoch2000.AddSeconds(value + secondsBias)}");
        return sb.ToString();
    }

    /// <summary>
    /// Gets a random day within the random range of <see cref="start"/> and <see cref="end"/> days, inclusive.
    /// </summary>
    /// <param name="start">First valid date</param>
    /// <param name="end">Last valid date</param>
    /// <param name="r">Random to use</param>
    /// <returns>Date within the specified range, inclusive.</returns>
    public static DateOnly GetRandomDateWithin(DateOnly start, DateOnly end, Random r)
    {
        var delta = end.DayNumber - start.DayNumber;
        var bias = r.Next(delta + 1);
        return start.AddDays(bias);
    }

    /// <inheritdoc cref="GetRandomDateWithin(DateOnly,DateOnly,Random)"/>
    public static DateOnly GetRandomDateWithin(DateOnly start, DateOnly end) => GetRandomDateWithin(start, end, Util.Rand);
}
