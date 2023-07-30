using System;

namespace PKHeX.Core;

public static class EncounterDate
{
    private static DateTime Now => DateTime.Now;
    public static DateOnly GetDateNDS() => DateOnly.FromDateTime(Now);
    public static DateOnly GetDate3DS() => DateOnly.FromDateTime(Now);
    public static DateOnly GetDateSwitch() => DateOnly.FromDateTime(Now);

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
