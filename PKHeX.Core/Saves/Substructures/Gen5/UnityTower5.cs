using System;
using static PKHeX.Core.LocaleNDS5;

namespace PKHeX.Core;

public sealed class UnityTower5(SAV5 SAV, Memory<byte> raw) : SaveBlock<SAV5>(SAV, raw), IGeonet
{
    private const int UnityTowerOffset = 0x320;
    private const int GeonetGlobalFlagOffset = 0x344;
    private const int UnityTowerFlagOffset = 0x345;
    private const int GeonetOffset = 0x348;

    public static byte GetSubregionCount(byte country) => LocaleNDS5.GetSubregionCount(country);

    public bool GlobalFlag { get => Data[GeonetGlobalFlagOffset] != 0; set => Data[GeonetGlobalFlagOffset] = (byte)(value ? 1 : 0); }
    public bool UnityTowerFlag { get => Data[UnityTowerFlagOffset] != 0; set => Data[UnityTowerFlagOffset] = (byte)(value ? 1 : 0); }

    public GeonetPoint GetCountrySubregion(byte country, byte subregion)
    {
        int index = GeonetOffset + ((country - 1) * 16) + (subregion / 4);
        int shift = 2 * (subregion % 4);
        return (GeonetPoint)((Data[index] & 0b11 << shift) >> shift);
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
