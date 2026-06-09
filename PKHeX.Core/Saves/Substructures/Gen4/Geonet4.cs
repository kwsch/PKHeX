using static PKHeX.Core.LocaleNDS4;

namespace PKHeX.Core;

public sealed class Geonet4 : IGeonet
{
    private readonly SAV4 SAV;
    private readonly byte[] Data;
    private readonly int Offset;

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

    public static byte GetSubregionCount(byte country) => LocaleNDS4.GetSubregionCount(country);

    public bool GlobalFlag { get => SAV.GeonetGlobalFlag; set => SAV.GeonetGlobalFlag = value; }

    public GeonetPoint GetCountrySubregion(byte country, byte subregion)
    {
        int index = ((country - 1) * 16) + (subregion / 4);
        int shift = 2 * (subregion % 4);
        return (GeonetPoint)((Data[index] & 0b11 << shift) >> shift);
    }

    public void SetCountrySubregion(byte country, byte subregion, GeonetPoint point)
    {
        int index = ((country - 1) * 16) + (subregion / 4);
        int shift = 2 * (subregion % 4);
        Data[index] = (byte)((Data[index] & ~(0b11 << shift)) | ((int)point << shift));
    }

    private void SetAllSubregions(byte country, GeonetPoint type)
    {
        var subregionCount = LocaleNDS4.GetSubregionCount(country);
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
            SetAllSubregions(country, GeonetPoint.Yellow);

        SetSAVCountry();
        GlobalFlag = true;
    }

    public void SetAllLegal()
    {
        foreach (var country in LegalCountries)
            SetAllSubregions(country, GeonetPoint.Yellow);

        SetSAVCountry();
        GlobalFlag = true;
    }

    public void ClearAll()
    {
        for (byte country = 1; country <= CountryCount; country++)
            SetAllSubregions(country, GeonetPoint.None);

        SetSAVCountry();
        GlobalFlag = (SAV.Country > 0 && SAV.Country != Japan);
    }

    public void SetSAVCountry()
    {
        if (SAV.Country > 0)
            SetCountrySubregion((byte)SAV.Country, (byte)SAV.Region, GeonetPoint.Red);
    }
}
