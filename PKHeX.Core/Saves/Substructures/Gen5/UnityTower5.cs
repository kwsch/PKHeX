using System;

namespace PKHeX.Core;

public sealed class UnityTower5(SAV5 SAV, Memory<byte> raw) : SaveBlock<SAV5>(SAV, raw), IGeonet
{
    private const int UnityTowerOffset = 0x320;
    private const int GeonetGlobalFlagOffset = 0x344;
    private const int UnityTowerFlagOffset = 0x345;
    private const int GeonetOffset = 0x348;

    public const int CountryCount = 232;
    private const int Japan = 105;

    private static ReadOnlySpan<byte> LegalCountries =>
    [
        001, 002, 003, 006, 008, 009, 012, 013, 015, 016, 017, 018, 020, 021, 022, 023,
        025, 027, 028, 029, 031, 033, 034, 035, 036, 040, 042, 043, 045, 047, 048, 049,
        051, 053, 054, 058, 060, 061, 062, 063, 064, 071, 072, 073, 074, 076, 079, 080,
        081, 082, 083, 084, 085, 087, 088, 090, 091, 092, 093, 094, 095, 096, 098, 099,
        101, 102, 103, 105, 106, 109, 111, 115, 117, 118, 121, 125, 128, 130, 132, 134,
        138, 139, 141, 145, 147, 148, 149, 150, 151, 155, 156, 157, 160, 161, 163, 164,
        166, 167, 170, 173, 174, 181, 185, 186, 188, 189, 190, 191, 194, 195, 196, 198,
        199, 200, 201, 203, 205, 206, 210, 211, 215, 217, 218, 219, 220, 221, 222, 224,
        226, 227,
    ];

    public static byte GetSubregionCount(byte country) => country switch
    {
        009 => 24, // Argentina
        012 =>  8, // Australia
        028 => 27, // Brazil
        036 => 13, // Canada
        043 => 33, // China
        072 =>  6, // Finland
        073 => 22, // France
        079 => 16, // Germany
        095 => 35, // India
        102 => 20, // Italy
        105 => 50, // Japan
        155 => 22, // Norway
        166 => 16, // Poland
        174 =>  8, // Russian Federation
        195 => 17, // Spain
        200 => 22, // Sweden
        218 => 12, // United Kingdom
        220 => 51, // United States of America
        _ => 0,
    };

    public bool GlobalFlag { get => Data[GeonetGlobalFlagOffset] != 0; set => Data[GeonetGlobalFlagOffset] = (byte)(value ? 1 : 0); }
    public bool UnityTowerFlag { get => Data[UnityTowerFlagOffset] != 0; set => Data[UnityTowerFlagOffset] = (byte)(value ? 1 : 0); }

    public GeonetPoint GetCountrySubregion(byte country, byte subregion)
    {
        int index = GeonetOffset + ((country - 1) * 16) + (subregion / 4);
        int shift = 2 * (subregion % 4);
        return (GeonetPoint)(((Data[index] & 0b11 << shift) >> shift));
    }

    public void SetCountrySubregion(byte country, byte subregion, GeonetPoint point)
    {
        int index = GeonetOffset + ((country - 1) * 16) + (subregion / 4);
        int shift = 2 * (subregion % 4);
        Data[index] = (byte)((Data[index] & ~(0b11 << shift)) | ((int)point << shift));
    }

    /// <summary>
    /// Gets whether the floor is unlocked for the specified country.
    /// </summary>
    /// <param name="country">Country index</param>
    /// <returns>Floor status.</returns>
    public bool GetUnityTowerFloor(byte country)
    {
        int index = UnityTowerOffset + (country / 8);
        int shift = country % 8;
        return ((Data[index] & 0b1 << shift) >> shift) != 0b0;
    }

    /// <summary>
    /// Sets whether the floor is unlocked for the specified country.
    /// </summary>
    /// <param name="country">Country index</param>
    /// <param name="isUnlocked">Floor status</param>
    public void SetUnityTowerFloor(byte country, bool isUnlocked)
    {
        int index = UnityTowerOffset + (country / 8);
        int shift = country % 8;
        Data[index] = (byte)((Data[index] & ~(0b1 << shift)) | (isUnlocked ? 0b1 : 0b0) << shift);
    }

    private void SetAllSubregions(byte country, GeonetPoint type, bool floor)
    {
        SetUnityTowerFloor(country, floor);

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
            SetAllSubregions(country, GeonetPoint.Yellow, true);

        SetSAVCountry();
        GlobalFlag = true;
        UnityTowerFlag = true;
    }

    public void SetAllLegal()
    {
        foreach (var country in LegalCountries)
            SetAllSubregions(country, GeonetPoint.Yellow, true);

        SetSAVCountry();
        GlobalFlag = true;
        UnityTowerFlag = true;
    }

    public void ClearAll()
    {
        for (byte country = 1; country <= CountryCount; country++)
            SetAllSubregions(country, GeonetPoint.None, false);

        SetSAVCountry();
        GlobalFlag = (SAV.Country > 0 && SAV.Country != Japan);
        UnityTowerFlag = false;
    }

    public void SetSAVCountry()
    {
        if (SAV.Country > 0)
            SetCountrySubregion((byte)SAV.Country, (byte)SAV.Region, GeonetPoint.Red);
    }
}
