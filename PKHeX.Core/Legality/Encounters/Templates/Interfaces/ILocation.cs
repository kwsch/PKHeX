namespace PKHeX.Core;

/// <summary>
/// Exposes details about an encounter with a specific location ID required.
/// </summary>
public interface ILocation
{
    /// <summary>
    /// Met Location ID the encounter is found at.
    /// </summary>
    int Location { get; }

    /// <summary>
    /// Egg Location ID the encounter is obtained with.
    /// </summary>
    int EggLocation { get; }
}

public static partial class Extensions
{
    public static int GetLocation(this ILocation enc)
    {
        return enc.Location != 0
            ? enc.Location
            : enc.EggLocation;
    }

    public static string? GetEncounterLocation(this ILocation enc, int gen, int version = -1)
    {
        int loc = enc.GetLocation();
        if (loc < 0)
            return null;

        bool egg = loc != enc.Location;
        return GameInfo.GetLocationName(egg, loc, gen, gen, (GameVersion)version);
    }
}
