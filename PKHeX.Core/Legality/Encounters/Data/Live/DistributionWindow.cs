using System;

namespace PKHeX.Core;

/// <summary>
/// Record to store the date range availability for a distribution.
/// </summary>
/// <param name="Start">Absolute earliest date the distribution can be received.</param>
/// <param name="End">Absolute latest date the distribution can be received.</param>
/// <param name="GenerateDaysAfterStart">Days after the earliest start date that a distribution would normally be generated with.</param>
public readonly record struct DistributionWindow(DateOnly Start, DateOnly? End = null, byte GenerateDaysAfterStart = 0)
{
    public DistributionWindow(int startYear, int startMonth, int startDay, byte generateDaysAfterStart = 0)
        : this(new DateOnly(startYear, startMonth, startDay), null, generateDaysAfterStart) { }

    public DistributionWindow(int startYear, int startMonth, int startDay, int endYear, int endMonth, int endDay, byte generateDaysAfterStart = 0)
        : this(new DateOnly(startYear, startMonth, startDay), new DateOnly(endYear, endMonth, endDay), generateDaysAfterStart) { }

    /// <summary>
    /// Checks if the date obtained is within the date of availability for the given range.
    /// </summary>
    /// <param name="obtained">Date obtained.</param>
    /// <returns>True if the date obtained is within the date of availability for the given range.</returns>
    public bool Contains(DateOnly obtained) => (Start <= obtained && End is null) || obtained <= End;

    /// <summary>
    /// Get a valid date within the generation range.
    /// </summary>
    public DateOnly GetGenerateDate() => Start.AddDays(GenerateDaysAfterStart);

    /// <summary>
    /// Checks if the distribution was acquired earlier than generally available.
    /// </summary>
    /// <param name="obtained">Date obtained.</param>
    /// <remarks>Only relevant when <see cref="GenerateDaysAfterStart"/> is non-zero.</remarks>
    public bool IsEarlyAcquisition(DateOnly obtained) => obtained < Start.AddDays(GenerateDaysAfterStart);

    /// <summary>
    /// Checks if the distribution can be acquired earlier than generally available.
    /// </summary>
    public bool CanAcquireEarly => GenerateDaysAfterStart > 0;
}
