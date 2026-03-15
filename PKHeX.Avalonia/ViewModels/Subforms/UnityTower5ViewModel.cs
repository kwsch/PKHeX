using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a Geonet location row (Gen 5).
/// </summary>
public partial class GeonetEntry5Model : ObservableObject
{
    public int Country { get; }
    public string CountryName { get; }
    public int Subregion { get; }
    public string SubregionName { get; }

    [ObservableProperty]
    private int _point;

    public GeonetEntry5Model(int country, string countryName, int subregion, string subregionName, int point)
    {
        Country = country;
        CountryName = countryName;
        Subregion = subregion;
        SubregionName = subregionName;
        _point = point;
    }
}

/// <summary>
/// Model for a Unity Tower floor entry.
/// </summary>
public partial class UnityTowerFloorModel : ObservableObject
{
    public int Country { get; }
    public string CountryName { get; }

    [ObservableProperty]
    private bool _unlocked;

    public UnityTowerFloorModel(int country, string countryName, bool unlocked)
    {
        Country = country;
        CountryName = countryName;
        _unlocked = unlocked;
    }
}

/// <summary>
/// ViewModel for the Unity Tower / Geonet editor (Gen 5).
/// Edits Geonet globe data and Unity Tower floor unlock status.
/// </summary>
public partial class UnityTower5ViewModel : SaveEditorViewModelBase
{
    private readonly SAV5 SAV5;
    private readonly UnityTower5 Tower;

    [ObservableProperty] private bool _globalFlag;
    [ObservableProperty] private bool _unityTowerFlag;

    public ObservableCollection<GeonetEntry5Model> GeonetEntries { get; } = [];
    public ObservableCollection<UnityTowerFloorModel> FloorEntries { get; } = [];

    public string[] PointNames { get; } = ["None", "Blue", "Yellow", "Red"];

    public UnityTower5ViewModel(SAV5 sav) : base(sav)
    {
        SAV5 = sav;
        Tower = sav.UnityTower;

        _globalFlag = Tower.GlobalFlag;
        _unityTowerFlag = Tower.UnityTowerFlag;

        LoadGeonet();
        LoadFloors();
    }

    private void LoadGeonet()
    {
        GeonetEntries.Clear();
        for (int i = 1; i <= UnityTower5.CountryCount; i++)
        {
            var country = i;
            var countryName = $"Country {country}";
            var subregionCount = UnityTower5.GetSubregionCount((byte)country);

            if (subregionCount == 0)
            {
                var point = (int)Tower.GetCountrySubregion((byte)country, 0);
                GeonetEntries.Add(new GeonetEntry5Model(country, countryName, 0, "Default", point));
            }
            else
            {
                for (int j = 1; j <= subregionCount; j++)
                {
                    var point = (int)Tower.GetCountrySubregion((byte)country, (byte)j);
                    GeonetEntries.Add(new GeonetEntry5Model(country, countryName, j, $"Region {j}", point));
                }
            }
        }
    }

    private void LoadFloors()
    {
        FloorEntries.Clear();
        for (int i = 1; i <= UnityTower5.CountryCount; i++)
        {
            var countryName = $"Country {i}";
            var unlocked = Tower.GetUnityTowerFloor((byte)i);
            FloorEntries.Add(new UnityTowerFloorModel(i, countryName, unlocked));
        }
    }

    [RelayCommand]
    private void SetAllLocations()
    {
        Tower.SetAll();
        GlobalFlag = Tower.GlobalFlag;
        UnityTowerFlag = Tower.UnityTowerFlag;
        LoadGeonet();
        LoadFloors();
    }

    [RelayCommand]
    private void SetAllLegalLocations()
    {
        Tower.SetAllLegal();
        GlobalFlag = Tower.GlobalFlag;
        UnityTowerFlag = Tower.UnityTowerFlag;
        LoadGeonet();
        LoadFloors();
    }

    [RelayCommand]
    private void ClearLocations()
    {
        Tower.ClearAll();
        GlobalFlag = Tower.GlobalFlag;
        UnityTowerFlag = Tower.UnityTowerFlag;
        LoadGeonet();
        LoadFloors();
    }

    [RelayCommand]
    private void Save()
    {
        Tower.ClearAll();

        foreach (var entry in GeonetEntries)
        {
            if (entry.Country > 0)
                Tower.SetCountrySubregion((byte)entry.Country, (byte)entry.Subregion, (GeonetPoint)entry.Point);
        }

        foreach (var floor in FloorEntries)
            Tower.SetUnityTowerFloor((byte)floor.Country, floor.Unlocked);

        Tower.SetSAVCountry();
        Tower.GlobalFlag = GlobalFlag;
        Tower.UnityTowerFlag = UnityTowerFlag;

        SAV.State.Edited = true;
        Modified = true;
    }
}
