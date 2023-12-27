namespace PKHeX.Core;

/// <summary>
/// Tracks Geolocation history of a <see cref="PKM"/>
/// </summary>
public interface IGeoTrack : IRegionOrigin
{
    byte Geo1_Region { get; set; }
    byte Geo2_Region { get; set; }
    byte Geo3_Region { get; set; }
    byte Geo4_Region { get; set; }
    byte Geo5_Region { get; set; }
    byte Geo1_Country { get; set; }
    byte Geo2_Country { get; set; }
    byte Geo3_Country { get; set; }
    byte Geo4_Country { get; set; }
    byte Geo5_Country { get; set; }
}

public static partial class Extensions
{
    /// <summary>
    /// Clears all Geolocation history.
    /// </summary>
    public static void ClearGeoLocationData(this IGeoTrack g)
    {
        g.Geo1_Country = g.Geo2_Country = g.Geo3_Country = g.Geo4_Country = g.Geo5_Country = 0;
        g.Geo1_Region = g.Geo2_Region = g.Geo3_Region = g.Geo4_Region = g.Geo5_Region = 0;
    }

    /// <summary>
    /// Inserts a new Geolocation tuple to the <see cref="IGeoTrack"/> values.
    /// </summary>
    /// <param name="g">Object tracking the geolocation history</param>
    /// <param name="GeoCountry">Newly arrived country</param>
    /// <param name="GeoRegion">Newly arrived region</param>
    public static void TradeGeoLocation(this IGeoTrack g, byte GeoCountry, byte GeoRegion)
    {
        // Trickle existing values up one slot
        g.Geo5_Country = g.Geo4_Country;
        g.Geo5_Region = g.Geo4_Region;

        g.Geo4_Country = g.Geo3_Country;
        g.Geo4_Region = g.Geo3_Region;

        g.Geo3_Country = g.Geo2_Country;
        g.Geo3_Region = g.Geo2_Region;

        g.Geo2_Country = g.Geo1_Country;
        g.Geo2_Region = g.Geo1_Region;

        g.Geo1_Country = GeoCountry;
        g.Geo1_Region = GeoRegion;
    }

    public static void SanitizeGeoLocationData(this IGeoTrack g)
    {
        if (g.Geo1_Country == 0) g.Geo1_Region = 0;
        if (g.Geo2_Country == 0) g.Geo2_Region = 0;
        if (g.Geo3_Country == 0) g.Geo3_Region = 0;
        if (g.Geo4_Country == 0) g.Geo4_Region = 0;
        if (g.Geo5_Country == 0) g.Geo5_Region = 0;

        // trickle down empty slots
        while (true)
        {
            if (g.Geo5_Country != 0 && g.Geo4_Country == 0)
            {
                g.Geo4_Country = g.Geo5_Country;
                g.Geo4_Region = g.Geo5_Region;
                g.Geo5_Country = g.Geo5_Region = 0;
            }
            if (g.Geo4_Country != 0 && g.Geo3_Country == 0)
            {
                g.Geo3_Country = g.Geo4_Country;
                g.Geo3_Region = g.Geo4_Region;
                g.Geo4_Country = g.Geo4_Region = 0;
                continue;
            }
            if (g.Geo3_Country != 0 && g.Geo2_Country == 0)
            {
                g.Geo2_Country = g.Geo3_Country;
                g.Geo2_Region = g.Geo3_Region;
                g.Geo3_Country = g.Geo3_Region = 0;
                continue;
            }
            if (g.Geo2_Country != 0 && g.Geo1_Country == 0)
            {
                g.Geo1_Country = g.Geo2_Country;
                g.Geo1_Region = g.Geo2_Region;
                g.Geo2_Country = g.Geo2_Region = 0;
                continue;
            }
            break;
        }
    }

    /// <summary>
    /// Checks if all Geolocation tuples are valid.
    /// </summary>
    public static bool GetIsValid(this IGeoTrack g) => g.GetValidity() == GeoValid.Valid;

    /// <summary>
    /// Checks if all Geolocation tuples are valid.
    /// </summary>
    internal static GeoValid GetValidity(this IGeoTrack g)
    {
        bool end = false;
        GeoValid result;
        if ((result = UpdateCheck(g.Geo1_Country, g.Geo1_Region, ref end)) != GeoValid.Valid)
            return result;
        if ((result = UpdateCheck(g.Geo2_Country, g.Geo2_Region, ref end)) != GeoValid.Valid)
            return result;
        if ((result = UpdateCheck(g.Geo3_Country, g.Geo3_Region, ref end)) != GeoValid.Valid)
            return result;
        if ((result = UpdateCheck(g.Geo4_Country, g.Geo4_Region, ref end)) != GeoValid.Valid)
            return result;
        if ((result = UpdateCheck(g.Geo5_Country, g.Geo5_Region, ref end)) != GeoValid.Valid)
            return result;

        return result;

        static GeoValid UpdateCheck(byte country, byte region, ref bool end)
        {
            if (country != 0)
                return end ? GeoValid.CountryAfterPreviousEmpty : GeoValid.Valid;
            if (region != 0) // c == 0
                return GeoValid.RegionWithoutCountry;
            end = true;
            return GeoValid.Valid;
        }
    }
}

/// <summary>
/// Geolocation Country-Region tuple validity tagging.
/// </summary>
internal enum GeoValid
{
    /// <summary> Tuple is valid. </summary>
    Valid,

    /// <summary>
    /// Lower-index country is empty, but higher has data (invalid).
    /// </summary>
    CountryAfterPreviousEmpty,

    /// <summary>
    /// Zero-value country (None) with a non-zero Region (invalid).
    /// </summary>
    RegionWithoutCountry,
}
