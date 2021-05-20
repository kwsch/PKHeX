namespace PKHeX.Core
{
    /// <summary>
    /// Tracks Geolocation history of a <see cref="PKM"/>
    /// </summary>
    public interface IGeoTrack : IRegionOrigin
    {
        int Geo1_Region { get; set; }
        int Geo2_Region { get; set; }
        int Geo3_Region { get; set; }
        int Geo4_Region { get; set; }
        int Geo5_Region { get; set; }
        int Geo1_Country { get; set; }
        int Geo2_Country { get; set; }
        int Geo3_Country { get; set; }
        int Geo4_Country { get; set; }
        int Geo5_Country { get; set; }
    }

    public static partial class Extensions
    {
        public static void ClearGeoLocationData(this IGeoTrack g)
        {
            g.Geo1_Country = g.Geo2_Country = g.Geo3_Country = g.Geo4_Country = g.Geo5_Country = 0;
            g.Geo1_Region = g.Geo2_Region = g.Geo3_Region = g.Geo4_Region = g.Geo5_Region = 0;
        }

        public static void TradeGeoLocation(this IGeoTrack g, int GeoCountry, int GeoRegion)
        {
            // abort if the values are invalid
            if (GeoCountry < 0 || GeoRegion < 0)
                return;

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

        public static bool GetIsValid(this IGeoTrack g) => g.GetValidity() == GeoValid.Valid;

        internal static GeoValid GetValidity(this IGeoTrack g)
        {
            bool end = false;
            GeoValid result;
            if ((result = update(g.Geo1_Country, g.Geo1_Region)) != GeoValid.Valid)
                return result;
            if ((result = update(g.Geo2_Country, g.Geo2_Region)) != GeoValid.Valid)
                return result;
            if ((result = update(g.Geo3_Country, g.Geo3_Region)) != GeoValid.Valid)
                return result;
            if ((result = update(g.Geo4_Country, g.Geo4_Region)) != GeoValid.Valid)
                return result;
            if ((result = update(g.Geo5_Country, g.Geo5_Region)) != GeoValid.Valid)
                return result;

            return result;

            GeoValid update(int c, int r)
            {
                if (end && c != 0)
                    return GeoValid.CountryAfterPreviousEmpty;
                if (c != 0)
                    return GeoValid.Valid;
                if (r != 0) // c == 0
                    return GeoValid.RegionWithoutCountry;
                end = true;
                return GeoValid.Valid;
            }
        }
    }

    internal enum GeoValid
    {
        Valid,
        CountryAfterPreviousEmpty,
        RegionWithoutCountry
    }
}
