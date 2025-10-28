namespace PKHeX.Core;

/// <summary>
/// Exposes details about an encounter with a specific location ID required.
/// </summary>
public interface ILocation
{
    /// <summary>
    /// Met Location ID the encounter is found at.
    /// </summary>
    ushort Location { get; }

    /// <summary>
    /// Egg Location ID the encounter is obtained with.
    /// </summary>
    ushort EggLocation { get; }
}

public static partial class Extensions
{
    public static ushort GetLocation(this ILocation enc)
    {
        return enc.Location != 0
            ? enc.Location
            : enc.EggLocation;
    }

    public static string? GetEncounterLocation(this ILocation enc, byte generation, GameVersion version = 0)
    {
        ushort loc = enc.GetLocation();
        if (loc == 0)
            return null;

        bool egg = loc != enc.Location;
        return GameInfo.GetLocationName(egg, loc, generation, generation, version);
    }
}
