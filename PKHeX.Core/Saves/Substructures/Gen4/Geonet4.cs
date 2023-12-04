using System;

namespace PKHeX.Core;

public sealed class Geonet4
{
    private readonly SAV4 SAV;

    /* Lets the globe be panned outside Japan and zoomed out in Japanese games. Has no effect in non-Japanese games.
     * Set once you register a location outside of Japan, even if it's just your own location in a non-Japanese game. */
    public bool GlobalFlag { get; set; }

    private readonly byte[] Data;
    private readonly int Offset;
    private const int CountryCount = 233;

    private static ReadOnlySpan<byte> LegalCountries =>
    [
        001, 002, 003, 006, 008, 009, 012, 013, 015, 016, 017, 018, 020, 021, 022, 023,
        025, 027, 028, 029, 031, 033, 034, 035, 036, 040, 042, 043, 045, 048, 049, 050,
        052, 054, 055, 056, 058, 059, 060, 061, 062, 069, 070, 071, 072, 074, 077, 078,
        079, 080, 081, 082, 083, 085, 086, 088, 089, 090, 091, 092, 093, 094, 095, 097,
        098, 100, 101, 102, 103, 104, 107, 111, 115, 117, 118, 121, 122, 126, 129, 131,
        133, 135, 140, 142, 146, 148, 149, 150, 151, 152, 156, 157, 158, 160, 161, 163,
        164, 166, 167, 110, 171, 172, 179, 183, 186, 187, 188, 189, 192, 193, 194, 196,
        198, 199, 200, 202, 205, 207, 211, 212, 216, 218, 219, 204, 221, 220, 222, 224,
        226, 227,
    ];

    public static byte GetSubregionCount(byte country) => country switch
    {
        009 => 24,
        012 => 7,
        028 => 27,
        036 => 13,
        043 => 31,
        070 => 6,
        071 => 22,
        077 => 16,
        094 => 35,
        101 => 20,
        103 => 50,
        156 => 20,
        166 => 16,
        172 => 7,
        193 => 17,
        199 => 24,
        219 => 12,
        220 => 51,
        _ => 0,
    };

    public enum Point
    {
        None = 0, // never communicated with
        Blue = 1, // first communicated with today
        Yellow = 2, // already communicated with
        Red = 3, // own registered location
    }

    public Geonet4(SAV4 sav)
    {
        SAV = sav;
        Offset = SAV.Geonet + 3;
        GlobalFlag = SAV.GeonetGlobalFlag;
        Data = SAV.General.Slice(Offset, CountryCount * 16).ToArray();
    }

    public void Save()
    {
        SAV.GeonetGlobalFlag = GlobalFlag;
        SAV.SetData(SAV.General.Slice(Offset, CountryCount * 16), Data);
    }

    public void SetCountrySubregion(byte country, byte subregion, Point point)
    {
        int index = ((country - 1) * 16) + (subregion / 4);
        int shift = 2 * (subregion % 4);
        Data[index] = (byte)((Data[index] & ~(0b11 << shift)) | ((int)point << shift));
    }

    private void SetAllSubregions(byte country, Point type)
    {
        var subregionCount = GetSubregionCount(country);
        if (subregionCount == 0)
        {
            SetCountrySubregion(country, 0, type);
            return;
        }

        for (byte subregion = 1; subregion <= subregionCount; subregion++)
            SetCountrySubregion(country, subregion, type);
    }

    public void SetAll()
    {
        for (byte country = 1; country <= CountryCount; country++)
            SetAllSubregions(country, Point.Yellow);

        if (SAV.Country > 0)
            SetCountrySubregion((byte)SAV.Country, (byte)SAV.Region, Point.Red);

        GlobalFlag = true;
    }

    public void SetAllLegal()
    {
        foreach (var country in LegalCountries)
            SetAllSubregions(country, Point.Yellow);

        if (SAV.Country > 0)
            SetCountrySubregion((byte)SAV.Country, (byte)SAV.Region, Point.Red);

        GlobalFlag = true;
    }

    public void ClearAll()
    {
        for (byte country = 1; country <= CountryCount; country++)
            SetAllSubregions(country, Point.None);

        if (SAV.Country > 0)
            SetCountrySubregion((byte)SAV.Country, (byte)SAV.Region, Point.Red);

        GlobalFlag = (SAV.Country > 0 && SAV.Country != 103);
    }
}
