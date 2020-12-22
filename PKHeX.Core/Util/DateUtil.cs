using System;

namespace PKHeX.Core
{
    public static partial class Util
    {
        /// <summary>
        /// Determines whether or not the given date components are valid.
        /// </summary>
        /// <param name="year">The year of the date of which to check the validity.</param>
        /// <param name="month">The month of the date of which to check the validity.</param>
        /// <param name="day">The day of the date of which to check the validity.</param>
        /// <returns>A boolean indicating whether or not the date is valid.</returns>
        public static bool IsDateValid(int year, int month, int day)
        {
            return !(year <= 0 || year > DateTime.MaxValue.Year || month < 1 || month > 12 || day < 1 || day > DateTime.DaysInMonth(year, month));
        }

        /// <summary>
        /// Determines whether or not the given date components are valid.
        /// </summary>
        /// <param name="year">The year of the date of which to check the validity.</param>
        /// <param name="month">The month of the date of which to check the validity.</param>
        /// <param name="day">The day of the date of which to check the validity.</param>
        /// <returns>A boolean indicating whether or not the date is valid.</returns>
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
            string tip = string.Empty;
            if (value >= SecondsPerDay)
                tip += (value / SecondsPerDay) + "d ";
            tip += new DateTime(0).AddSeconds(value).ToString("HH:mm:ss");
            if (secondsBias >= 0)
                tip += Environment.NewLine + $"Date: {Epoch2000.AddSeconds(value + secondsBias)}";
            return tip;
        }
    }
}
