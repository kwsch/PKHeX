namespace PKHeX.Core;

/// <summary>
/// Hatch Location validity for <see cref="GameVersion.Gen6"/>.
/// </summary>
public static class EggHatchLocation6
{
    /// <summary>
    /// Returns true if the hatch location is valid for X and Y.
    /// </summary>
    public static bool IsValidMet6XY(ushort location)
    {
        const int min = 6;
        const int max = 168;
        var delta = location - min;
        if ((uint)delta > max - min)
            return false;

        if (location % 2 != 0)
            return false; // All locations are even
        return location != 80; // unused
    }

    /// <summary>
    /// Returns true if the hatch location is valid for Omega Ruby and Alpha Sapphire.
    /// </summary>
    public static bool IsValidMet6AO(ushort location)
    {
        const int min = 170;
        const int max = 354;
        var delta = location - min;
        if ((uint)delta > max - min)
            return false;

        if (location % 2 != 0)
            return false; // All locations are even
        return location != 348; // unused
    }
}
