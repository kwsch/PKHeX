using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class FashionEditorViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly FashionUnlock8? _fashion8;

    private static readonly string[] RegionNames =
    [
        "Eyewear", "Headwear", "Outerwear", "Tops", "Bags",
        "Gloves", "Bottoms", "Legwear", "Footwear"
    ];

    public FashionEditorViewModel(SaveFile sav)
    {
        _sav = sav;

        if (sav is SAV8SWSH swsh)
        {
            _fashion8 = swsh.Fashion;
            IsSupported = true;
            LoadData();
        }
    }

    public bool IsSupported { get; }
    public string GameInfo => $"{_sav.Version} - Fashion Unlocks";

    [ObservableProperty]
    private ObservableCollection<FashionRegionViewModel> _regions = [];

    private void LoadData()
    {
        if (_fashion8 is null) return;

        Regions.Clear();

        // Fashion regions start at index 6 (Eyewear = 6)
        for (int i = 0; i < RegionNames.Length; i++)
        {
            int regionIndex = FashionUnlock8.REGION_EYEWEAR + i;
            var owned = _fashion8.GetIndexesOwnedFlag(regionIndex);
            Regions.Add(new FashionRegionViewModel(RegionNames[i], regionIndex, owned.Length));
        }
    }

    [RelayCommand]
    private void UnlockAll()
    {
        _fashion8?.UnlockAll();
        LoadData();
    }

    [RelayCommand]
    private void UnlockAllLegal()
    {
        _fashion8?.UnlockAllLegal();
        LoadData();
    }

    [RelayCommand]
    private void Reset()
    {
        _fashion8?.Reset();
        LoadData();
    }

    [RelayCommand]
    private void Clear()
    {
        _fashion8?.Clear();
        LoadData();
    }

    [RelayCommand]
    private void Refresh() => LoadData();
}

public partial class FashionRegionViewModel : ViewModelBase
{
    public FashionRegionViewModel(string name, int regionIndex, int itemCount)
    {
        Name = name;
        RegionIndex = regionIndex;
        ItemCount = itemCount;
    }

    public string Name { get; }
    public int RegionIndex { get; }
    public int ItemCount { get; }
    public string Summary => $"{ItemCount} items";
}
