namespace PKHeX.Core;

public static class EggHatchLocation6
{
    public static bool IsValidMet6XY(int location)
    {
        const int min = 6;
        const int max = 168;
        var delta = location - min;
        if ((uint)delta >= max - min)
            return false;

        if (location % 2 != 0)
            return false; // All locations are even
        return location != 80; // unused
    }

    public static bool IsValidMet6AO(int location)
    {
        const int min = 170;
        const int max = 354;
        var delta = location - min;
        if ((uint)delta >= max - min)
            return false;

        if (location % 2 != 0)
            return false; // All locations are even
        return location != 348; // unused
    }
}
