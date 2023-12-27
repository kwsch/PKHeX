namespace PKHeX.Core;

/// <summary>
/// Exposes details about the 3DS Console geolocation settings the trainer has set.
/// </summary>
public interface IRegionOrigin
{
    /// <summary> Console hardware region. </summary>
    /// <see cref="Region3DSIndex"/>
    byte ConsoleRegion { get; set; }
    /// <summary> Console's configured Country via System Settings. </summary>
    byte Country { get; set; }
    /// <summary> Console's configured Region within <see cref="Country"/> via System Settings. </summary>
    byte Region { get; set; }
}

public static partial class Extensions
{
    public static void SetDefaultRegionOrigins(this IRegionOrigin o, int language)
    {
        if (language == 1)
        {
            o.ConsoleRegion = 0; // Japan
            o.Country = 1; // Japan
            o.Region = 0;
        }
        else
        {
            o.ConsoleRegion = 1; // North America
            o.Country = 49; // USA
            o.Region = 7; // California
        }
    }

    public static void CopyRegionOrigin(this IRegionOrigin source, IRegionOrigin dest)
    {
        dest.ConsoleRegion = source.ConsoleRegion;
        dest.Country = source.Country;
        dest.Region = source.Region;
    }

    public static void ClearRegionOrigin(this IRegionOrigin o)
    {
        o.ConsoleRegion = o.Region = o.Country = 0;
    }
}
