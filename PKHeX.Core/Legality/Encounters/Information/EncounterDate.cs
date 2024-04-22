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
    public static TimeProvider TimeProvider { get; set; } = TimeProvider.System;

    private static DateTime Now => TimeProvider.GetLocalNow().DateTime;

    /// <summary>
    /// Fetches a valid date for the Nintendo GameCube.
    /// </summary>
    public static DateTime GetDateTimeGC() => Now;

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

    /// <summary>
    /// Checks if the date is valid for the Nintendo DS.
    /// </summary>
    public static bool IsValidDateNDS(DateOnly date)
    {
        if (date.Year is < 2000 or > 2099)
            return false;
        return true;
    }

    /// <summary>
    /// Checks if the date is valid for the Nintendo 3DS.
    /// </summary>
    public static bool IsValidDate3DS(DateOnly date)
    {
        if (date.Year is < 2000 or > 2050)
            return false;
        return true;
    }

    /// <summary>
    /// Checks if the date is valid for the Nintendo Switch.
    /// </summary>
    public static bool IsValidDateSwitch(DateOnly date)
    {
        if (date.Year is < 2000 or > 2060)
            return false;
        return true;
    }
}
