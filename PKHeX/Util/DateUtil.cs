using System;

namespace PKHeX
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
    }
}
