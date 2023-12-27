using System;

namespace PKHeX.Core;

/// <summary>
/// Logic for console specific date validity.
/// </summary>
public static class EncounterDate
{
    /// <summary>
    /// Time provider to use for date fetching.
    /// </summary>
    public static ITimeProvider TimeProvider { get; set; } = DefaultTimeProvider.Instance;

    private static DateTime Now => TimeProvider.Now;

    /// <summary>
    /// Fetches a valid date for the Nintendo DS.
    /// </summary>
    public static DateOnly GetDateNDS() => DateOnly.FromDateTime(Now);

    /// <summary>
    /// Fetches a valid date for the Nintendo 3DS.
    /// </summary>
    public static DateOnly GetDate3DS() => DateOnly.FromDateTime(Now);

    /// <summary>
    /// Fetches a valid date for the Nintendo Switch.
    /// </summary>
    public static DateOnly GetDateSwitch() => DateOnly.FromDateTime(Now);

    /// <summary>
    /// Fetches a valid date for the specified <see cref="GameConsole"/>.
    /// </summary>
    public static DateOnly GetDate(GameConsole console) => console switch
    {
        GameConsole.NDS => GetDateNDS(),
        GameConsole._3DS => GetDate3DS(),
        GameConsole.NX => GetDateSwitch(),
        _ => throw new ArgumentOutOfRangeException(nameof(console), console, null),
    };

    public static bool IsValidDateNDS(DateOnly date)
    {
        if (date.Year is < 2000 or > 2099)
            return false;
        return true;
    }

    public static bool IsValidDate3DS(DateOnly date)
    {
        if (date.Year is < 2000 or > 2050)
            return false;
        return true;
    }

    public static bool IsValidDateSwitch(DateOnly date)
    {
        if (date.Year is < 2000 or > 2050)
            return false;
        return true;
    }
}

/// <summary>
/// Default time provider that uses <see cref="DateTime.Now"/>.
/// </summary>
public sealed class DefaultTimeProvider : ITimeProvider
{
    /// <summary>
    /// Singleton instance of the default time provider.
    /// </summary>
    public static readonly DefaultTimeProvider Instance = new();

    public DateTime Now => DateTime.Now;
}

/// <summary>
/// Interface for fetching the current time.
/// </summary>
public interface ITimeProvider
{
    /// <summary>
    /// Fetches the current time.
    /// </summary>
    DateTime Now { get; }
}
