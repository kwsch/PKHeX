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
    public static void SetDefaultRegionOrigins(this IRegionOrigin o)
    {
        o.ConsoleRegion = 1; // North America
        o.Region = 7; // California
        o.Country = 49; // USA
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
