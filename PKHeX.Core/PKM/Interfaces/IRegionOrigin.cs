namespace PKHeX.Core;

/// <summary>
/// Exposes details about the 3DS Console geolocation settings the trainer has set.
/// </summary>
public interface IRegionOrigin : IRegionOriginReadOnly
{
    /// <summary> Console hardware region. </summary>
    /// <see cref="Region3DSIndex"/>
    new byte ConsoleRegion { get; set; }
    /// <summary> Console's configured Country via System Settings. </summary>
    new byte Country { get; set; }
    /// <summary> Console's configured Region within <see cref="Country"/> via System Settings. </summary>
    new byte Region { get; set; }
}

public interface IRegionOriginReadOnly
{
    /// <summary> Console hardware region. </summary>
    /// <see cref="Region3DSIndex"/>
    byte ConsoleRegion { get; }
    /// <summary> Console's configured Country via System Settings. </summary>
    byte Country { get; }
    /// <summary> Console's configured Region within <see cref="Country"/> via System Settings. </summary>
    byte Region { get; }
}

public readonly record struct GeoRegion3DS(byte ConsoleRegion, byte Country, byte Region);

public static class RegionOriginExtensions
{
    public static GeoRegion3DS GetRegionOrigin(this IRegionOriginReadOnly o) => new(o.ConsoleRegion, o.Country, o.Region);
    public static void SetRegionOrigin(this IRegionOrigin o, GeoRegion3DS r)
    {
        o.ConsoleRegion = r.ConsoleRegion;
        o.Country = r.Country;
        o.Region = r.Region;
    }

    public static GeoRegion3DS GetRegionOrigin(this ITrainerInfo tr, int language)
    {
        if (tr is IRegionOriginReadOnly r)
            return r.GetRegionOrigin();
        if (language == 1) // Japanese
            return new GeoRegion3DS(0, 1, 0); // Japan
        return new GeoRegion3DS(1, 49, 7); // North America, USA, California
    }

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

    public static void CopyRegionOrigin(this IRegionOriginReadOnly source, IRegionOrigin dest)
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
