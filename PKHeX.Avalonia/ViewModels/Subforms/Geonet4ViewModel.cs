using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a Geonet location row.
/// </summary>
public partial class GeonetEntryModel : ObservableObject
{
    public int Country { get; }
    public string CountryName { get; }
    public int Subregion { get; }
    public string SubregionName { get; }

    [ObservableProperty]
    private int _point;

    public GeonetEntryModel(int country, string countryName, int subregion, string subregionName, int point)
    {
        Country = country;
        CountryName = countryName;
        Subregion = subregion;
        SubregionName = subregionName;
        _point = point;
    }
}

/// <summary>
/// ViewModel for the Geonet editor (Gen 4).
/// Edits Geonet globe location data.
/// </summary>
public partial class Geonet4ViewModel : SaveEditorViewModelBase
{
    private readonly SAV4 _origin;
    private readonly SAV4 SAV4;
    private readonly Geonet4 Geonet;

    [ObservableProperty]
    private bool _globalFlag;

    public ObservableCollection<GeonetEntryModel> Entries { get; } = [];

    public string[] PointNames { get; } = ["None", "Blue", "Yellow", "Red"];

    public Geonet4ViewModel(SAV4 sav) : base(sav)
    {
        _origin = sav;
        SAV4 = (SAV4)sav.Clone();
        Geonet = new Geonet4(sav);
        _globalFlag = Geonet.GlobalFlag;
        LoadEntries();
    }

    private void LoadEntries()
    {
        Entries.Clear();

        for (int i = 1; i <= Geonet4.CountryCount; i++)
        {
            var country = i;
            var countryName = $"Country {country}";
            var subregionCount = Geonet4.GetSubregionCount((byte)country);

            if (subregionCount == 0)
            {
                var point = (int)Geonet.GetCountrySubregion((byte)country, 0);
                Entries.Add(new GeonetEntryModel(country, countryName, 0, "Default", point));
            }
            else
            {
                for (int j = 1; j <= subregionCount; j++)
                {
                    var point = (int)Geonet.GetCountrySubregion((byte)country, (byte)j);
                    Entries.Add(new GeonetEntryModel(country, countryName, j, $"Region {j}", point));
                }
            }
        }
    }

    [RelayCommand]
    private void SetAllLocations()
    {
        Geonet.SetAll();
        Geonet.Save();
        GlobalFlag = Geonet.GlobalFlag;
        LoadEntries();
    }

    [RelayCommand]
    private void SetAllLegalLocations()
    {
        Geonet.SetAllLegal();
        Geonet.Save();
        GlobalFlag = Geonet.GlobalFlag;
        LoadEntries();
    }

    [RelayCommand]
    private void ClearLocations()
    {
        Geonet.ClearAll();
        Geonet.Save();
        GlobalFlag = Geonet.GlobalFlag;
        LoadEntries();
    }

    [RelayCommand]
    private void Save()
    {
        Geonet.ClearAll();
        foreach (var entry in Entries)
        {
            if (entry.Country > 0)
                Geonet.SetCountrySubregion((byte)entry.Country, (byte)entry.Subregion, (GeonetPoint)entry.Point);
        }
        Geonet.SetSAVCountry();
        Geonet.GlobalFlag = GlobalFlag;
        Geonet.Save();

        _origin.CopyChangesFrom(SAV4);
        Modified = true;
    }
}
